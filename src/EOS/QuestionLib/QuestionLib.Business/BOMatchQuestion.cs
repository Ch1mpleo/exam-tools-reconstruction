using System;
using System.Collections;
using System.Data.SqlClient;
using NHibernate;
using QuestionLib.Entity;

namespace QuestionLib.Business;

public class BOMatchQuestion : BOBase
{
	public BOMatchQuestion(ISessionFactory sessionFactory)
		: base(sessionFactory)
	{
	}

	// [behavior 0x060001cd] Queries MatchQuestion by MID and returns the first model or null after closing.
	public MatchQuestion LoadMatch(int mid)
	{
		session = sessionFactory.OpenSession();
		IList list;
		try
		{
			string text = null;
			text = "from MatchQuestion mq Where  mq.MID=:mid";
			IQuery query = session.CreateQuery(text);
			query.SetParameter("mid", mid);
			list = query.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return (list.Count > 0) ? ((MatchQuestion)list[0]) : null;
	}

	// [behavior 0x060001ce] Queries MatchQuestion by CourseId and returns the list.
	public IList LoadMatchOfCourse(string courseId)
	{
		session = sessionFactory.OpenSession();
		IList result;
		try
		{
			string text = null;
			text = "from MatchQuestion mq Where  mq.CourseId=:courseId";
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

	// [behavior 0x060001cf] Saves each match model, deduplicates its QuestionLOs by LOID, assigns saved MID/MATCH type to
	// [behavior 0x060001cf] links and saves them in one transaction. Commit returns true; catch rolls back/returns false; no
	// [behavior 0x060001cf] explicit session close.
	public bool SaveList(IList list)
	{
		ISession session = sessionFactory.OpenSession();
		ITransaction transaction = session.BeginTransaction();
		try
		{
			foreach (MatchQuestion item in list)
			{
				session.Save(item);
				item.QuestionLOs = BOLO.RemoveDupLO(item.QuestionLOs);
				foreach (QuestionLO questionLO in item.QuestionLOs)
				{
					questionLO.QID = item.MID;
					questionLO.QType = QuestionType.MATCH;
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

	// [behavior 0x060001d0] The chapter/connection overload transactionally deletes matching QuestionLO links then
	// [behavior 0x060001d0] MatchQuestion rows. The model overload transactionally deletes that MID and its MATCH links.
	// [behavior 0x060001d0] Errors roll back, close and rethrow.
	public bool Delete(int chapterID, string conStr)
	{
		int num = 3;
		SqlConnection sqlConnection = new SqlConnection(conStr);
		sqlConnection.Open();
		SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
		string cmdText = "DELETE FROM QuestionLO WHERE QType = " + num + " AND qid IN (SELECT mid FROM MatchQuestion WHERE chapterID=" + chapterID + ")";
		string cmdText2 = "DELETE FROM MatchQuestion WHERE chapterID=" + chapterID;
		SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
		sqlCommand.Transaction = sqlTransaction;
		SqlCommand sqlCommand2 = new SqlCommand(cmdText2, sqlConnection);
		sqlCommand2.Transaction = sqlTransaction;
		try
		{
			sqlCommand.ExecuteNonQuery();
			sqlCommand2.ExecuteNonQuery();
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

	// [behavior 0x060001d0] The chapter/connection overload transactionally deletes matching QuestionLO links then
	// [behavior 0x060001d0] MatchQuestion rows. The model overload transactionally deletes that MID and its MATCH links.
	// [behavior 0x060001d0] Errors roll back, close and rethrow.
	public void Delete(MatchQuestion m)
	{
		int mID = m.MID;
		session = sessionFactory.OpenSession();
		ITransaction transaction = session.BeginTransaction();
		try
		{
			string query = "from MatchQuestion mq Where mq.MID=" + mID;
			session.Delete(query);
			int num = 3;
			query = "from QuestionLO qlo Where qlo.QType=" + num + " and qlo.QID=" + mID;
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
}
