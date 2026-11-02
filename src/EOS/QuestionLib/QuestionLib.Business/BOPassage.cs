using System;
using System.Collections;
using System.Data.SqlClient;
using NHibernate;
using QuestionLib.Entity;

namespace QuestionLib.Business;

public class BOPassage : BOBase
{
	public BOPassage(ISessionFactory sessionFactory)
		: base(sessionFactory)
	{
	}

	// [behavior 0x060001d3] Queries Passage by PID and returns the first model or null.
	public Passage LoadPassage(int pid)
	{
		session = sessionFactory.OpenSession();
		IList list;
		try
		{
			string text = null;
			text = "from Passage p Where p.PID=:pid";
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
		return (list.Count > 0) ? ((Passage)list[0]) : null;
	}

	// [behavior 0x060001d4] Queries Passage by CourseId and returns the materialized list.
	public IList LoadPassageByCourse(string courseId)
	{
		session = sessionFactory.OpenSession();
		IList result;
		try
		{
			string text = null;
			text = "from Passage p Where CourseId=:courseId";
			IQuery query = session.CreateQuery(text);
			query.SetParameter("courseId", courseId);
			result = query.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return result;
	}

	// [behavior 0x060001d5] Transactionally deletes passage/question/answer/LO data. The PID overload relies on caller-
	// [behavior 0x060001d5] supplied child QIDs for answer/link deletion; the chapter SQL overload derives child IDs through
	// [behavior 0x060001d5] subqueries. Errors roll back and rethrow.
	public void Delete(int pid, int[] qid_list)
	{
		session = sessionFactory.OpenSession();
		ITransaction transaction = session.BeginTransaction();
		try
		{
			string query = "from Passage p Where p.PID=" + pid;
			session.Delete(query);
			query = "from Question q Where q.PID=" + pid;
			session.Delete(query);
			for (int i = 0; i < qid_list.Length; i++)
			{
				int num = qid_list[i];
				query = "from QuestionAnswer qa Where qa.QID=" + num;
				session.Delete(query);
				int num2 = 0;
				query = "from QuestionLO qLO Where Qtype=" + num2 + " and qLO.QID=" + num.ToString();
				session.Delete(query);
			}
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

	// [behavior 0x060001d5] Transactionally deletes passage/question/answer/LO data. The PID overload relies on caller-
	// [behavior 0x060001d5] supplied child QIDs for answer/link deletion; the chapter SQL overload derives child IDs through
	// [behavior 0x060001d5] subqueries. Errors roll back and rethrow.
	public bool Delete(int chapterID, string conStr)
	{
		int num = 0;
		SqlConnection sqlConnection = new SqlConnection(conStr);
		sqlConnection.Open();
		SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
		string cmdText = "DELETE FROM QuestionLO WHERE QType = " + num + " AND qid IN (SELECT qid FROM Question WHERE pid IN (SELECT pid FROM Passage WHERE chapterID=" + chapterID + "))";
		string cmdText2 = "DELETE FROM QuestionAnswer WHERE qid IN (SELECT qid FROM Question WHERE pid IN (SELECT pid FROM Passage WHERE chapterID=" + chapterID + "))";
		string cmdText3 = "DELETE FROM Question WHERE pid IN (SELECT pid FROM Passage WHERE chapterID=" + chapterID + ")";
		string cmdText4 = "DELETE FROM Passage WHERE chapterID=" + chapterID;
		SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
		sqlCommand.Transaction = sqlTransaction;
		SqlCommand sqlCommand2 = new SqlCommand(cmdText2, sqlConnection);
		sqlCommand2.Transaction = sqlTransaction;
		SqlCommand sqlCommand3 = new SqlCommand(cmdText3, sqlConnection);
		sqlCommand3.Transaction = sqlTransaction;
		SqlCommand sqlCommand4 = new SqlCommand(cmdText4, sqlConnection);
		sqlCommand4.Transaction = sqlTransaction;
		try
		{
			sqlCommand.ExecuteNonQuery();
			sqlCommand2.ExecuteNonQuery();
			sqlCommand3.ExecuteNonQuery();
			sqlCommand4.ExecuteNonQuery();
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

	// [behavior 0x060001d7] Saves passages, then questions with assigned PID, then answers with assigned QID, then
	// [behavior 0x060001d7] deduplicated LO links with QID/QType, in one transaction. Commit/true or rollback/false; no
	// [behavior 0x060001d7] explicit session close.
	public bool SaveList(IList list)
	{
		ISession session = sessionFactory.OpenSession();
		ITransaction transaction = session.BeginTransaction();
		try
		{
			foreach (Passage item in list)
			{
				session.Save(item);
				foreach (Question passageQuestion in item.PassageQuestions)
				{
					passageQuestion.PID = item.PID;
					session.Save(passageQuestion);
					foreach (QuestionAnswer questionAnswer in passageQuestion.QuestionAnswers)
					{
						questionAnswer.QID = passageQuestion.QID;
						session.Save(questionAnswer);
					}
					passageQuestion.QuestionLOs = BOLO.RemoveDupLO(passageQuestion.QuestionLOs);
					foreach (QuestionLO questionLO in passageQuestion.QuestionLOs)
					{
						questionLO.QID = passageQuestion.QID;
						questionLO.QType = passageQuestion.QType;
						session.Save(questionLO);
					}
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
}
