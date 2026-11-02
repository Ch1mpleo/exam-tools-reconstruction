using System;
using System.Collections;
using NHibernate;
using QuestionLib.Entity;

namespace QuestionLib.Business;

public class BOEssayQuestion : BOBase
{
	public BOEssayQuestion(ISessionFactory sessionFactory)
		: base(sessionFactory)
	{
	}

	// [behavior 0x060001bd] Queries by EQID, closes, and returns the first EssayQuestion or null.
	public EssayQuestion Load(int eqid)
	{
		IList list = null;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from EssayQuestion q Where q.EQID=:eqid";
			IQuery query = session.CreateQuery(text);
			query.SetParameter("eqid", eqid);
			list = query.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return (list.Count > 0) ? ((EssayQuestion)list[0]) : null;
	}

	// [behavior 0x060001be] Queries EssayQuestion by parameterized CourseId and returns the list.
	public IList LoadByCourse(string courseId)
	{
		IList list = null;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from EssayQuestion q Where CourseId=:courseId";
			IQuery query = session.CreateQuery(text);
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

	// [behavior 0x060001bf] Queries EssayQuestion by parameterized ChapterId and returns the list.
	public IList LoadByChapter(int chapterId)
	{
		IList list = null;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from EssayQuestion q Where ChapterId=:chapterId";
			IQuery query = session.CreateQuery(text);
			query.SetParameter("chapterId", chapterId);
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

	// [behavior 0x060001c0] Deletes the selected essay ID inside an NHibernate transaction; commit/close on success,
	// [behavior 0x060001c0] rollback/close/rethrow on error.
	public void Delete(int eqid)
	{
		session = sessionFactory.OpenSession();
		ITransaction transaction = session.BeginTransaction();
		try
		{
			string query = "from EssayQuestion q Where q.EQID=" + eqid;
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

	// [behavior 0x060001c1] Saves each essay in one transaction; commit returns true, catch rolls back and returns false. No
	// [behavior 0x060001c1] explicit session close appears in this body.
	public bool SaveList(IList list)
	{
		ISession session = sessionFactory.OpenSession();
		ITransaction transaction = session.BeginTransaction();
		try
		{
			foreach (EssayQuestion item in list)
			{
				session.Save(item);
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

	// [behavior 0x060001c2] Deletes EssayQuestion rows for a chapter in one transaction and closes; errors roll back, close
	// [behavior 0x060001c2] and rethrow.
	public void DeleteQuestionInChapter(int chapterId)
	{
		session = sessionFactory.OpenSession();
		ITransaction transaction = session.BeginTransaction();
		try
		{
			string query = "from EssayQuestion q Where q.ChapterId=" + chapterId;
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
