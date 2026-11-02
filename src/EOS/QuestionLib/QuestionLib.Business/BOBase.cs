using System;
using System.Collections;
using System.Data.SqlClient;
using NHibernate;
using QuestionLib.Entity;

namespace QuestionLib.Business;

public class BOBase
{
	protected ISessionFactory sessionFactory;

	protected ISession session;

	public BOBase()
	{
	}

	public BOBase(ISessionFactory sessionFactory)
	{
		this.sessionFactory = sessionFactory;
	}

	// [behavior 0x060001a6] Opens an NHibernate session/transaction; SaveOrUpdate, Flush, Commit, Close, then returns the
	// [behavior 0x060001a6] input object. On failure: Rollback, Close, throw ex. The transaction is disposed by the using
	// [behavior 0x060001a6] scope.
	public object SaveOrUpdate(object obj)
	{
		session = sessionFactory.OpenSession();
		using ITransaction transaction = session.BeginTransaction();
		try
		{
			session.SaveOrUpdate(obj);
			session.Flush();
			transaction.Commit();
			session.Close();
			return obj;
		}
		catch (Exception ex)
		{
			transaction.Rollback();
			session.Close();
			throw ex;
		}
	}

	// [behavior 0x060001a7] Calls Save and Flush, returning the input object. The single-argument overload owns a
	// [behavior 0x060001a7] session/transaction and commits/closes or rolls back/closes on error. The supplied-session
	// [behavior 0x060001a7] overload leaves transaction/session ownership with its caller.
	public object Save(object obj)
	{
		session = sessionFactory.OpenSession();
		using ITransaction transaction = session.BeginTransaction();
		try
		{
			session.Save(obj);
			session.Flush();
			transaction.Commit();
			session.Close();
			return obj;
		}
		catch (Exception ex)
		{
			transaction.Rollback();
			session.Close();
			throw ex;
		}
	}

	// [behavior 0x060001a7] Calls Save and Flush, returning the input object. The single-argument overload owns a
	// [behavior 0x060001a7] session/transaction and commits/closes or rolls back/closes on error. The supplied-session
	// [behavior 0x060001a7] overload leaves transaction/session ownership with its caller.
	public object Save(object obj, ISession mySession)
	{
		try
		{
			mySession.Save(obj);
			mySession.Flush();
			return obj;
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	// [behavior 0x060001a9] Calls Update and Flush, returning the input object. The single-argument overload owns and
	// [behavior 0x060001a9] finishes the session/transaction; the supplied-session overload does not commit or close that
	// [behavior 0x060001a9] session.
	public object Update(object obj)
	{
		session = sessionFactory.OpenSession();
		using ITransaction transaction = session.BeginTransaction();
		try
		{
			session.Update(obj);
			session.Flush();
			transaction.Commit();
			session.Close();
			return obj;
		}
		catch (Exception ex)
		{
			transaction.Rollback();
			session.Close();
			throw ex;
		}
	}

	// [behavior 0x060001a9] Calls Update and Flush, returning the input object. The single-argument overload owns and
	// [behavior 0x060001a9] finishes the session/transaction; the supplied-session overload does not commit or close that
	// [behavior 0x060001a9] session.
	public object Update(object obj, ISession mySession)
	{
		try
		{
			mySession.Update(obj);
			mySession.Flush();
			return obj;
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	// [behavior 0x060001ab] Opens a session and calls Load(existingObject, id), mutating the caller-provided object; closes
	// [behavior 0x060001ab] on success/error and rethrows errors.
	public void Load(object obj, object id)
	{
		session = sessionFactory.OpenSession();
		try
		{
			session.Load(obj, id);
			session.Close();
		}
		catch (Exception ex)
		{
			session.Close();
			throw ex;
		}
	}

	// [behavior 0x060001ac] Opens a session/transaction, deletes the object, flushes, commits and closes. Failure rolls
	// [behavior 0x060001ac] back, closes and rethrows.
	public void Delete(object obj)
	{
		session = sessionFactory.OpenSession();
		using ITransaction transaction = session.BeginTransaction();
		try
		{
			session.Delete(obj);
			session.Flush();
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

	// [behavior 0x060001ad] Runs the concatenated HQL "from " + typeName, materializes its list, commits the read
	// [behavior 0x060001ad] transaction and closes the session. There is no explicit catch/rollback path in this method.
	public IList List(string typeName)
	{
		IList result = null;
		session = sessionFactory.OpenSession();
		using (ITransaction transaction = session.BeginTransaction())
		{
			IQuery query = session.CreateQuery("from " + typeName);
			result = query.List();
			transaction.Commit();
			session.Close();
		}
		return result;
	}

	// [behavior 0x060001ae] The chapter overload builds a SELECT string and obtains a provider connection, but never
	// [behavior 0x060001ae] executes the query and returns its initial null result. The course overload immediately returns
	// [behavior 0x060001ae] null. These are incomplete shipped implementations, not missing decompiler logic.
	public IList ListID(string typeName, QuestionType qt, int chapterID)
	{
		IList result = null;
		string text = "";
		string text2 = "";
		switch (qt)
		{
		case QuestionType.READING:
			text = "pid";
			text2 = "=0";
			break;
		case QuestionType.MULTIPLE_CHOICE:
			text = "qid";
			text2 = "=1";
			break;
		case QuestionType.INDICATE_MISTAKE:
			text = "qid";
			text2 = "=2";
			break;
		case QuestionType.MATCH:
			text = "mid";
			text2 = "=3";
			break;
		default:
			text = "qid";
			text2 = ">3";
			break;
		}
		string text3 = "SELECT " + text + " FROM " + typeName + " WHERE chapterId=" + chapterID + " AND qType=" + text2;
		SqlConnection sqlConnection = (SqlConnection)sessionFactory.ConnectionProvider.GetConnection();
		return result;
	}

	// [behavior 0x060001ae] The chapter overload builds a SELECT string and obtains a provider connection, but never
	// [behavior 0x060001ae] executes the query and returns its initial null result. The course overload immediately returns
	// [behavior 0x060001ae] null. These are incomplete shipped implementations, not missing decompiler logic.
	public IList ListID(string typeName, QuestionType qt, string courseID)
	{
		return null;
	}
}
