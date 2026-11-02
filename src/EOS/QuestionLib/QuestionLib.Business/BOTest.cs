using System;
using System.Collections;
using NHibernate;
using QuestionLib.Entity;

namespace QuestionLib.Business;

public class BOTest : BOBase
{
	public BOTest(ISessionFactory sessionFactory)
		: base(sessionFactory)
	{
	}

	// [behavior 0x060001e7] Queries Test by CourseId and returns the list.
	public IList LoadTest(string courseId)
	{
		session = sessionFactory.OpenSession();
		IList result;
		try
		{
			string text = null;
			text = "from Test t Where CourseId=:courseId";
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

	// [behavior 0x060001e8] Queries Test by TestId and returns the first model or null.
	public Test LoadTestByTestId(string testId)
	{
		session = sessionFactory.OpenSession();
		IList list;
		try
		{
			string text = null;
			text = "from Test t Where TestId=:testId";
			IQuery query = session.CreateQuery(text);
			query.SetParameter("testId", testId);
			list = query.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		if (list.Count > 0)
		{
			return (Test)list[0];
		}
		return null;
	}

	// [behavior 0x060001e9] Queries Test by CourseId and returns the list; it duplicates LoadTest behavior.
	public IList LoadTestByCourse(string courseId)
	{
		session = sessionFactory.OpenSession();
		IList result;
		try
		{
			string text = null;
			text = "from Test t Where CourseId=:courseId";
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

	// [behavior 0x060001ea] Queries Test by TestId and returns whether any row exists.
	public bool IsTestExists(string testId)
	{
		session = sessionFactory.OpenSession();
		IList list;
		try
		{
			string text = null;
			text = "from Test t Where TestId=:testId";
			IQuery query = session.CreateQuery(text);
			query.SetParameter("testId", testId);
			list = query.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		if (list.Count == 0)
		{
			return false;
		}
		return true;
	}
}
