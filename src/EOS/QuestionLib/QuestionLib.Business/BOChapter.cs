using System;
using System.Collections;
using NHibernate;
using QuestionLib.Entity;

namespace QuestionLib.Business;

public class BOChapter : BOBase
{
	public BOChapter(ISessionFactory sessionFactory)
		: base(sessionFactory)
	{
	}

	// [behavior 0x060001b1] Runs parameterized HQL for a chapter and all three fill-blank enum values; materializes a list
	// [behavior 0x060001b1] and closes on success/error. The chapter parameter is passed as text here.
	public IList LoadFillBlankQuestionByChapter(int chapterId)
	{
		QuestionType questionType = QuestionType.FILL_BLANK_ALL;
		QuestionType questionType2 = QuestionType.FILL_BLANK_GROUP;
		QuestionType questionType3 = QuestionType.FILL_BLANK_EMPTY;
		session = sessionFactory.OpenSession();
		IList result;
		try
		{
			string text = null;
			text = "from Question q Where (q.QType=:type1 OR q.QType=:type2 OR q.QType=:type3)  AND ChapterId=:chapterId";
			IQuery query = session.CreateQuery(text);
			query.SetParameter("type1", questionType);
			query.SetParameter("type2", questionType2);
			query.SetParameter("type3", questionType3);
			query.SetParameter("chapterId", chapterId.ToString());
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

	// [behavior 0x060001b2] Runs HQL filtered by supplied QuestionType and chapter, returning the materialized list.
	// [behavior 0x060001b2] ChapterId is passed as a string parameter.
	public IList LoadQuestionByChapter(QuestionType qt, int chapterId)
	{
		session = sessionFactory.OpenSession();
		IList result;
		try
		{
			string text = null;
			text = "from Question q Where q.QType=:type and ChapterId=:chapterId";
			IQuery query = session.CreateQuery(text);
			query.SetParameter("type", qt);
			query.SetParameter("chapterId", chapterId.ToString());
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

	// [behavior 0x060001b3] Queries Passage by parameterized chapter ID; closes and returns the list.
	public IList LoadPassageByChapter(int chapterId)
	{
		session = sessionFactory.OpenSession();
		IList result;
		try
		{
			string text = null;
			text = "from Passage p Where ChapterId=:chapterId";
			IQuery query = session.CreateQuery(text);
			query.SetParameter("chapterId", chapterId);
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

	// [behavior 0x060001b4] Queries MatchQuestion by parameterized chapter ID; closes and returns the list.
	public IList LoadMatchQuestionByChapter(int chapterId)
	{
		session = sessionFactory.OpenSession();
		IList result;
		try
		{
			string text = null;
			text = "from MatchQuestion m Where ChapterId=:chapterId";
			IQuery query = session.CreateQuery(text);
			query.SetParameter("chapterId", chapterId);
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
}
