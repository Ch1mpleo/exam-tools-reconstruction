using System;
using System.Collections;
using System.Globalization;
using NHibernate;

namespace QuestionLib.Business;

public class BOEOSLog : BOBase
{
	public BOEOSLog(ISessionFactory sessionFactory)
		: base(sessionFactory)
	{
	}

	// [behavior 0x060001c4] Builds optional account/date HQL filters, adds parameter values, parses dates as dd/MM/yyyy with
	// [behavior 0x060001c4] inclusive 00:00:00 / 23:59:59 endpoints, materializes the list and closes.
	public IList LoadLog(string username, string fromdate, string todate)
	{
		session = sessionFactory.OpenSession();
		IList result;
		try
		{
			string text = "";
			string text2 = null;
			text2 = "from EOSLog t Where 1=1 ";
			if (username != "")
			{
				text += " and Log_Account like :username ";
			}
			if (fromdate != "")
			{
				text += " and CreatedDate >= :fromdate ";
			}
			if (todate != "")
			{
				text += " and CreatedDate <= :todate ";
			}
			text2 += text;
			IQuery query = session.CreateQuery(text2);
			if (username != "")
			{
				query.SetParameter("username", "%" + username + "%");
			}
			if (fromdate != "")
			{
				query.SetParameter("fromdate", DateTime.ParseExact(fromdate + " 00:00:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture));
			}
			if (todate != "")
			{
				query.SetParameter("todate", DateTime.ParseExact(todate + " 23:59:59", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture));
			}
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
