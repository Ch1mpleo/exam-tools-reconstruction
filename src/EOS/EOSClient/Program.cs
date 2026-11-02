// Recovered C#. Descriptive aliases are analyst-assigned; _M suffix is original MethodDef RID.
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;

internal static class Program
{
	[STAThread]
	// [behavior 0x0600019b] STA entry point: installs UI/AppDomain exception handling, enables styles and runs RulesForm.
	private static void Main()
	{
		Application_SetUnhandledExceptionMode_M019f(UnhandledExceptionMode.CatchException);
		Application_add_ThreadException_M01a0((ThreadExceptionEventHandler)HandleUiException_M019c);
		AppDomain_add_UnhandledException_M01a2(AppDomain_get_CurrentDomain_M01a1(), (UnhandledExceptionEventHandler)HandleUnhandledException_M019d);
		Application_EnableVisualStyles_M01a3();
		Application_SetCompatibleTextRenderingDefault_M01a4(false);
		Application_Run_M01a5((Form)new RulesForm());
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	// [behavior 0x0600019c] Logs a WinForms thread exception and displays a recovery message.
	private static void HandleUiException_M019c(object P_0, ThreadExceptionEventArgs P_1)
	{
		AppendErrorLog_M019e(ThreadExceptionEventArgs_get_Exception_M01a6(P_1));
		MessageBox_Show_M01a9(String_Concat_M01a8("An unexpected error occurred: ", Exception_get_Message_M01a7(ThreadExceptionEventArgs_get_Exception_M01a6(P_1)), "\nThe application will keep running \u2014 if you were submitting your exam, please try again."), "Unexpected error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
	}

	// [behavior 0x0600019d] Logs the AppDomain exception object when it is an Exception.
	private static void HandleUnhandledException_M019d(object P_0, UnhandledExceptionEventArgs P_1)
	{
		if (UnhandledExceptionEventArgs_get_ExceptionObject_M01aa(P_1) is Exception ex)
		{
			AppendErrorLog_M019e(ex);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	// [behavior 0x0600019e] Appends timestamp and exception text to eos_unhandled_errors.log; failures while logging are
	// [behavior 0x0600019e] swallowed.
	private static void AppendErrorLog_M019e(Exception P_0)
	{
		try
		{
			string text = String_Concat_M01ad(new string[6]
			{
				DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
				" | ",
				Object_ToString_M01ab((object)P_0),
				Environment_get_NewLine_M01ac(),
				"----",
				Environment_get_NewLine_M01ac()
			});
			File_AppendAllText_M01b0(Path_Combine_M01af(Application_get_StartupPath_M01ae(), "eos_unhandled_errors.log"), text);
		}
		catch
		{
		}
	}

	static void Application_SetUnhandledExceptionMode_M019f(UnhandledExceptionMode P_0)
	{
		Application.SetUnhandledExceptionMode(P_0);
	}

	static void Application_add_ThreadException_M01a0(ThreadExceptionEventHandler P_0)
	{
		Application.ThreadException += P_0;
	}

	static AppDomain AppDomain_get_CurrentDomain_M01a1()
	{
		return AppDomain.CurrentDomain;
	}

	static void AppDomain_add_UnhandledException_M01a2(AppDomain P_0, UnhandledExceptionEventHandler P_1)
	{
		P_0.UnhandledException += P_1;
	}

	static void Application_EnableVisualStyles_M01a3()
	{
		Application.EnableVisualStyles();
	}

	static void Application_SetCompatibleTextRenderingDefault_M01a4(bool P_0)
	{
		Application.SetCompatibleTextRenderingDefault(P_0);
	}

	static void Application_Run_M01a5(Form P_0)
	{
		Application.Run(P_0);
	}

	static Exception ThreadExceptionEventArgs_get_Exception_M01a6(ThreadExceptionEventArgs P_0)
	{
		return P_0.Exception;
	}

	static string Exception_get_Message_M01a7(Exception P_0)
	{
		return P_0.Message;
	}

	static string String_Concat_M01a8(string P_0, string P_1, string P_2)
	{
		return P_0 + P_1 + P_2;
	}

	static DialogResult MessageBox_Show_M01a9(string P_0, string P_1, MessageBoxButtons P_2, MessageBoxIcon P_3)
	{
		return MessageBox.Show(P_0, P_1, P_2, P_3);
	}

	static object UnhandledExceptionEventArgs_get_ExceptionObject_M01aa(UnhandledExceptionEventArgs P_0)
	{
		return P_0.ExceptionObject;
	}

	static string Object_ToString_M01ab(object P_0)
	{
		return P_0.ToString();
	}

	static string Environment_get_NewLine_M01ac()
	{
		return Environment.NewLine;
	}

	static string String_Concat_M01ad(string[] P_0)
	{
		return string.Concat(P_0);
	}

	static string Application_get_StartupPath_M01ae()
	{
		return Application.StartupPath;
	}

	static string Path_Combine_M01af(string P_0, string P_1)
	{
		return Path.Combine(P_0, P_1);
	}

	static void File_AppendAllText_M01b0(string P_0, string P_1)
	{
		File.AppendAllText(P_0, P_1);
	}
}
