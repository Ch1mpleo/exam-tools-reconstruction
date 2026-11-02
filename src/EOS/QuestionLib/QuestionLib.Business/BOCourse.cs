using System;
using System.Collections;
using NHibernate;

namespace QuestionLib.Business;

public class BOCourse : BOBase
{
	public BOCourse(ISessionFactory sessionFactory)
		: base(sessionFactory)
	{
	}

	// [behavior 0x060001b6] Queries Chapter rows by parameterized CID; closes and returns the list.
	public IList LoadChapterByCourse(string courseId)
	{
		session = sessionFactory.OpenSession();
		IList result;
		try
		{
			string text = null;
			text = "from Chapter ch Where CID=:courseId";
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

	// [behavior 0x060001b7] Queries Course by CID and returns list.Count > 0 after closing the session.
	public bool IsCourseExists(string courseId)
	{
		bool result = false;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from Course c Where CID=:courseId";
			IQuery query = session.CreateQuery(text);
			query.SetParameter("courseId", courseId);
			IList list = query.List();
			session.Close();
			if (list.Count > 0)
			{
				result = true;
			}
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return result;
	}

	// [behavior 0x060001b8] Queries Course by CID and ImageSize != 0, returning whether any row exists.
	public bool IsCourseImage(string courseId)
	{
		bool result = false;
		session = sessionFactory.OpenSession();
		try
		{
			string text = null;
			text = "from Course c Where CID=:courseId and ImageSize != 0";
			IQuery query = session.CreateQuery(text);
			query.SetParameter("courseId", courseId);
			IList list = query.List();
			session.Close();
			if (list.Count > 0)
			{
				result = true;
			}
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return result;
	}

	// [behavior 0x060001b9] Projects Course.ImageData with UniqueResult and returns it as byte[] or null; closes the session
	// [behavior 0x060001b9] on success/error.
	public byte[] GetImageDataByCourse(string courseId)
	{
		byte[] result = null;
		session = sessionFactory.OpenSession();
		try
		{
			string queryString = "select c.ImageData from Course c Where c.CID = :courseId";
			IQuery query = session.CreateQuery(queryString);
			query.SetParameter("courseId", courseId);
			object obj = query.UniqueResult();
			if (obj != null)
			{
				result = obj as byte[];
			}
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return result;
	}

	// [behavior 0x060001ba] Projects Course.ImageSize with UniqueResult; returns the integer or zero when absent.
	public int GetImageSizeByCourse(string courseId)
	{
		int result = 0;
		session = sessionFactory.OpenSession();
		try
		{
			string queryString = "select c.ImageSize from Course c Where c.CID = :courseId";
			IQuery query = session.CreateQuery(queryString);
			query.SetParameter("courseId", courseId);
			object obj = query.UniqueResult();
			if (obj != null)
			{
				result = (int)obj;
			}
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
		return result;
	}

	// [behavior 0x060001bb] Projects Course.NumberOfPage with UniqueResult; returns the integer or zero when absent.
	public int GetImageNumberOfPageByCourse(string courseId)
	{
		int result = 0;
		session = sessionFactory.OpenSession();
		try
		{
			string queryString = "select c.NumberOfPage from Course c Where c.CID = :courseId";
			IQuery query = session.CreateQuery(queryString);
			query.SetParameter("courseId", courseId);
			object obj = query.UniqueResult();
			if (obj != null)
			{
				result = (int)obj;
			}
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
