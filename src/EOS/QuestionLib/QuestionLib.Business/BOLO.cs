using System;
using System.Collections;
using NHibernate;
using QuestionLib.Entity;

namespace QuestionLib.Business;

public class BOLO : BOBase
{
	public BOLO(ISessionFactory sessionFactory)
		: base(sessionFactory)
	{
	}

	// [behavior 0x060001c6] Queries LO by LOID, closes and returns the first LO or null.
	public LO Load(int loid)
	{
		IList list = null;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from LO lo Where lo.LOID=:loid";
			IQuery query = session.CreateQuery(text);
			query.SetParameter("loid", loid);
			list = query.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return (list.Count > 0) ? ((LO)list[0]) : null;
	}

	// [behavior 0x060001c7] Queries LO by trimmed name and CID, then directly reads list[0].LOID. An empty result is not
	// [behavior 0x060001c7] guarded and will throw.
	public int GetLOID(string lo_name, string CID)
	{
		IList list = null;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from LO lo Where lo.LO_Name=:lo_name And lo.CID=:CID";
			IQuery query = session.CreateQuery(text);
			query.SetParameter("lo_name", lo_name.Trim());
			query.SetParameter("CID", CID);
			list = query.List();
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		LO lO = (LO)list[0];
		return lO.LOID;
	}

	// [behavior 0x060001c8] Queries LO by parameterized course CID; closes and returns the list.
	public IList LoadLOByCourse(string courseId)
	{
		session = sessionFactory.OpenSession();
		IList result;
		try
		{
			string text = null;
			text = "from LO lo Where lo.CID=:courseId";
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

	// [behavior 0x060001c9] Queries LO by CID and LO_Name; returns whether the materialized list is nonempty.
	public bool IsLOExists(string CID, string lo_name)
	{
		session = sessionFactory.OpenSession();
		IList list;
		try
		{
			string text = null;
			text = "from LO lo Where lo.CID=:CID And lo.LO_Name=:lo_name";
			IQuery query = session.CreateQuery(text);
			query.SetParameter("lo_name", lo_name);
			query.SetParameter("CID", CID);
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

	// [behavior 0x060001ca] Queries LO by CID and LO_Desc; returns whether the list is nonempty.
	public bool IsLODescriptionExists(string CID, string lo_desc)
	{
		session = sessionFactory.OpenSession();
		IList list;
		try
		{
			string text = null;
			text = "from LO lo Where lo.CID=:CID And lo.LO_Desc=:lo_desc";
			IQuery query = session.CreateQuery(text);
			query.SetParameter("lo_desc", lo_desc);
			query.SetParameter("CID", CID);
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

	// [behavior 0x060001cb] Keeps the first QuestionLO with each LOID, preserving order. Uses a nested linear scan and
	// [behavior 0x060001cb] object references, so time grows quadratically.
	public static ArrayList RemoveDupLO(ArrayList listLO)
	{
		ArrayList arrayList = new ArrayList();
		foreach (QuestionLO item in listLO)
		{
			bool flag = false;
			foreach (QuestionLO item2 in arrayList)
			{
				if (item.LOID == item2.LOID)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				arrayList.Add(item);
			}
		}
		return arrayList;
	}
}
