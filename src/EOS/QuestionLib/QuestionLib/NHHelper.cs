using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace QuestionLib;

public class NHHelper
{
	public static string ConnectionString = "";

	public ISessionFactory SessionFactory;

	// [behavior 0x06000004] Reads NHibernate configuration, adds the QuestionLib mapping assembly, sets both supported
	// [behavior 0x06000004] connection-string property keys from the static ConnectionString, then builds a session factory.
	// [behavior 0x06000004] No connection string is supplied by this method itself.
	public void Configure()
	{
		Configuration configuration = new Configuration().Configure();
		configuration.AddAssembly("QuestionLib");
		configuration.Properties["hibernate.connection.connection_string"] = ConnectionString;
		configuration.Properties["connection.connection_string"] = ConnectionString;
		SessionFactory = configuration.BuildSessionFactory();
	}

	// [behavior 0x06000005] Reads NHibernate configuration and QuestionLib mappings, then invokes SchemaExport.Create with
	// [behavior 0x06000005] script and export enabled. This is database/schema-writing support, not client registration; not
	// [behavior 0x06000005] executed.
	public void ExportTables()
	{
		Configuration configuration = new Configuration().Configure();
		configuration.AddAssembly("QuestionLib");
		new SchemaExport(configuration).Create(script: true, export: true);
	}

	// [behavior 0x06000006] Creates a fresh NHHelper, calls Configure and returns that newly built session factory; it is
	// [behavior 0x06000006] not a cached singleton.
	public static ISessionFactory GetSessionFactory()
	{
		NHHelper nHHelper = new NHHelper();
		nHHelper.Configure();
		return nHHelper.SessionFactory;
	}
}
