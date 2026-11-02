using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using NHibernate;

namespace QuestionLib.Business;

public class BOTestTemplate : BOBase
{
	public BOTestTemplate(ISessionFactory sessionFactory)
		: base(sessionFactory)
	{
	}

	// [behavior 0x060001ec] Queries TestTemplate by name and returns whether the materialized list is nonempty.
	public bool IsTestTemplateExists(string testTemplateName)
	{
		session = sessionFactory.OpenSession();
		IList list;
		try
		{
			string text = null;
			text = "from TestTemplate t Where TestTemplateName=:testTemplateName";
			IQuery query = session.CreateQuery(text);
			query.SetParameter("testTemplateName", testTemplateName);
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

	// [behavior 0x060001ed] Executes a parameterized SQL join of TestTemplate and Course for CID, filling a display
	// [behavior 0x060001ed] DataTable including template/course names, duration and distinct-history metadata.
	public static DataTable LoadTestTemplate(string CID, string conStr)
	{
		SqlConnection sqlConnection = new SqlConnection(conStr);
		sqlConnection.Open();
		string cmdText = "SELECT tt.TestTemplateID, tt.TestTemplateName AS 'Template name', tt.CID, c.Name AS 'Course name', tt.CreatedBy, tt.CreatedDate, tt.DistinctWithLastTest AS 'Distinct last tests',tt.Duration, tt.Note FROM TestTemplate AS tt INNER JOIN Course AS c ON tt.CID = c.CID WHERE tt.CID = @CID";
		SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
		sqlCommand.Parameters.Add("CID", SqlDbType.NVarChar);
		sqlCommand.Parameters["CID"].Value = CID;
		SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
		DataTable dataTable = new DataTable();
		sqlDataAdapter.Fill(dataTable);
		sqlConnection.Close();
		return dataTable;
	}

	// [behavior 0x060001ee] Transactionally deletes child TestTemplateDetails before the parent template using typed ID
	// [behavior 0x060001ee] parameters. Commit/true; failure rolls back/closes/rethrows.
	public static bool Delete(int testTemplateID, string conStr)
	{
		SqlConnection sqlConnection = new SqlConnection(conStr);
		sqlConnection.Open();
		SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
		string cmdText = "DELETE FROM TestTemplateDetails WHERE TestTemplateID = @testTemplateID";
		string cmdText2 = "DELETE FROM TestTemplate WHERE TestTemplateID = @testTemplateID";
		SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
		sqlCommand.Parameters.Add("testTemplateID", SqlDbType.Int);
		sqlCommand.Parameters["testTemplateID"].Value = testTemplateID;
		sqlCommand.Transaction = sqlTransaction;
		SqlCommand sqlCommand2 = new SqlCommand(cmdText2, sqlConnection);
		sqlCommand2.Parameters.Add("testTemplateID", SqlDbType.Int);
		sqlCommand2.Parameters["testTemplateID"].Value = testTemplateID;
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

	// [behavior 0x060001ef] Reads DistinctWithLastTest for the template, uses it as TOP N, then reads the latest TestIDs for
	// [behavior 0x060001ef] CourseID ordered by InsertOrder descending.
	public static List<string> GetDistinctTestIds(string courseID, int testTemplateID, string conStr)
	{
		List<string> list = new List<string>();
		SqlConnection sqlConnection = new SqlConnection(conStr);
		sqlConnection.Open();
		string cmdText = "SELECT DistinctWithLastTest FROM TestTemplate WHERE TestTemplateID = @testTemplateID";
		SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
		sqlCommand.Parameters.Add("testTemplateID", SqlDbType.Int);
		sqlCommand.Parameters["testTemplateID"].Value = testTemplateID;
		object obj = sqlCommand.ExecuteScalar();
		int num = Convert.ToInt32(obj.ToString());
		cmdText = "SELECT TOP " + num + " TestID FROM Test WHERE CourseID=@courseID ORDER BY InsertOrder DESC";
		sqlCommand = new SqlCommand(cmdText, sqlConnection);
		sqlCommand.Parameters.Add("courseID", SqlDbType.NVarChar);
		sqlCommand.Parameters["courseID"].Value = courseID;
		SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
		while (sqlDataReader.Read())
		{
			list.Add(sqlDataReader.GetString(0));
		}
		sqlDataReader.Close();
		sqlConnection.Close();
		return list;
	}
}
