using System;
using System.Collections;
using System.Data.SqlClient;
using NHibernate;
using QuestionLib.Entity;

namespace QuestionLib.Business;

public class BOQuestion : BOBase
{
	public BOQuestion(ISessionFactory sessionFactory)
		: base(sessionFactory)
	{
	}

	// [behavior 0x060001d9] Queries Question by parameterized PID, closes and returns its list.
	public IList LoadPassageQuestion(int pid)
	{
		IList list = null;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from Question q Where q.PID=:pid";
			IQuery query = session.CreateQuery(text);
			query.SetParameter("pid", pid);
			list = query.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return list;
	}

	// [behavior 0x060001da] Queries Question by QID and returns the first model or null.
	public Question Load(int qid)
	{
		IList list = null;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from Question q Where q.QID=:qid";
			IQuery query = session.CreateQuery(text);
			query.SetParameter("qid", qid);
			list = query.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return (list.Count > 0) ? ((Question)list[0]) : null;
	}

	// [behavior 0x060001db] Queries Question by QuestionType, closes and returns the list.
	public IList LoadByType(QuestionType type)
	{
		IList list = null;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from Question q Where q.QType=:type";
			IQuery query = session.CreateQuery(text);
			query.SetParameter("type", type);
			list = query.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return list;
	}

	// [behavior 0x060001dc] Queries Question by QuestionType and CourseId, closes and returns the list.
	public IList LoadByTypeOfCourse(QuestionType type, string courseId)
	{
		IList list = null;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from Question q Where q.QType=:type and CourseId=:courseId";
			IQuery query = session.CreateQuery(text);
			query.SetParameter("type", type);
			query.SetParameter("courseId", courseId);
			list = query.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return list;
	}

	// [behavior 0x060001dd] Queries the three fill-blank types for CourseId with separate parameters; closes and returns the
	// [behavior 0x060001dd] list.
	public IList LoadFillBlankByTypeOfCourse(string courseId)
	{
		IList list = null;
		QuestionType questionType = QuestionType.FILL_BLANK_ALL;
		QuestionType questionType2 = QuestionType.FILL_BLANK_EMPTY;
		QuestionType questionType3 = QuestionType.FILL_BLANK_GROUP;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from Question q Where (q.QType=:type1 OR q.QType=:type2 OR q.QType=:type3) and CourseId=:courseId";
			IQuery query = session.CreateQuery(text);
			query.SetParameter("type1", questionType);
			query.SetParameter("type2", questionType2);
			query.SetParameter("type3", questionType3);
			query.SetParameter("courseId", courseId);
			list = query.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return list;
	}

	// [behavior 0x060001de] The QID overload deletes that Question, its answers and type-qualified links. The chapter SQL
	// [behavior 0x060001de] overload deletes answers, then links, then questions through subqueries in one transaction.
	// [behavior 0x060001de] Preserve its exact predicates; errors roll back/close/rethrow.
	public void Delete(int qid, QuestionType qt)
	{
		session = sessionFactory.OpenSession();
		ITransaction transaction = session.BeginTransaction();
		try
		{
			string query = "from Question q Where q.QID=" + qid;
			session.Delete(query);
			query = "from QuestionAnswer qa Where qa.QID=" + qid;
			session.Delete(query);
			query = "from QuestionLO qlo Where qlo.QType=" + (int)qt + " and qlo.QID=" + qid;
			session.Delete(query);
			transaction.Commit();
			session.Close();
		}
		catch (Exception ex)
		{
			transaction.Rollback();
			session.Close();
			throw ex;
		}
	}

	// [behavior 0x060001df] Saves each question, assigns its QID to answers, deduplicates LO links and assigns their
	// [behavior 0x060001df] QID/QType, then commits. Errors roll back and return false; no explicit session close.
	public bool SaveList(IList list)
	{
		ISession session = sessionFactory.OpenSession();
		ITransaction transaction = session.BeginTransaction();
		try
		{
			foreach (Question item in list)
			{
				session.Save(item);
				foreach (QuestionAnswer questionAnswer in item.QuestionAnswers)
				{
					questionAnswer.QID = item.QID;
					session.Save(questionAnswer);
				}
				item.QuestionLOs = BOLO.RemoveDupLO(item.QuestionLOs);
				foreach (QuestionLO questionLO in item.QuestionLOs)
				{
					questionLO.QID = item.QID;
					questionLO.QType = item.QType;
					session.Save(questionLO);
				}
			}
			transaction.Commit();
			return true;
		}
		catch
		{
			transaction.Rollback();
			return false;
		}
	}

	// [behavior 0x060001de] The QID overload deletes that Question, its answers and type-qualified links. The chapter SQL
	// [behavior 0x060001de] overload deletes answers, then links, then questions through subqueries in one transaction.
	// [behavior 0x060001de] Preserve its exact predicates; errors roll back/close/rethrow.
	public bool Delete(int chapterID, QuestionType qt, string conStr)
	{
		SqlConnection sqlConnection = new SqlConnection(conStr);
		sqlConnection.Open();
		SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
		string cmdText = "DELETE FROM QuestionAnswer WHERE qid in (SELECT qid FROM Question WHERE QType=" + (int)qt + " AND chapterId=" + chapterID + ")";
		string cmdText2 = "DELETE FROM QuestionLO WHERE qid in (SELECT qid FROM Question WHERE QType=" + (int)qt + " AND chapterId=" + chapterID + ")";
		string cmdText3 = "DELETE FROM Question WHERE QType=" + (int)qt + " AND chapterID=" + chapterID;
		SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
		sqlCommand.Transaction = sqlTransaction;
		SqlCommand sqlCommand2 = new SqlCommand(cmdText2, sqlConnection);
		sqlCommand2.Transaction = sqlTransaction;
		SqlCommand sqlCommand3 = new SqlCommand(cmdText3, sqlConnection);
		sqlCommand3.Transaction = sqlTransaction;
		try
		{
			sqlCommand.ExecuteNonQuery();
			sqlCommand2.ExecuteNonQuery();
			sqlCommand3.ExecuteNonQuery();
			sqlTransaction.Commit();
			sqlConnection.Close();
			return true;
		}
		catch (Exception ex)
		{
			sqlTransaction.Rollback();
			sqlConnection.Close();
			throw ex;
		}
	}
}
