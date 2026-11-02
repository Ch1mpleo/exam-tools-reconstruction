using System;
using System.Collections;
using NHibernate;
using QuestionLib.Entity;

namespace QuestionLib.Business;

public class BOQuestionLO : BOBase
{
	public BOQuestionLO(ISessionFactory sessionFactory)
		: base(sessionFactory)
	{
	}

	// [behavior 0x060001e4] Queries question/type-specific QuestionLO links, then creates another session factory through
	// [behavior 0x060001e4] NHHelper and loads each linked LO individually. Returns LO objects, not the original link
	// [behavior 0x060001e4] records.
	public IList LoadLO(QuestionType qType, int qid)
	{
		IList list = null;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from QuestionLO qlo Where qlo.QType=:qType and qlo.QID=:qid";
			IQuery query = session.CreateQuery(text);
			query.SetParameter("qType", qType);
			query.SetParameter("qid", qid);
			list = query.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		ArrayList arrayList = new ArrayList();
		BOLO bOLO = new BOLO(NHHelper.GetSessionFactory());
		foreach (QuestionLO item in list)
		{
			LO value = bOLO.Load(item.LOID);
			arrayList.Add(value);
		}
		return arrayList;
	}

	// [behavior 0x060001e5] Transactionally deletes links for both QID and QType; commit/close or rollback/close/rethrow.
	public void DeleteQuestionLO(int qid, QuestionType qType)
	{
		session = sessionFactory.OpenSession();
		ITransaction transaction = session.BeginTransaction();
		try
		{
			string query = "from QuestionLO qlo Where qlo.QType=" + (int)qType + " and qlo.QID=" + qid;
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
