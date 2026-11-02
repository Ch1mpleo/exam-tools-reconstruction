using System;
using System.Collections;
using NHibernate;

namespace QuestionLib.Business;

public class BOQuestionAnswer : BOBase
{
	public BOQuestionAnswer(ISessionFactory sessionFactory)
		: base(sessionFactory)
	{
	}

	// [behavior 0x060001e2] Queries QuestionAnswer by parameterized QID and returns the list after closing.
	public IList LoadAnswer(int qid)
	{
		session = sessionFactory.OpenSession();
		IList result;
		try
		{
			string text = null;
			text = "from QuestionAnswer qa Where qa.QID=:qid";
			IQuery query = session.CreateQuery(text);
			query.SetParameter("qid", qid);
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
