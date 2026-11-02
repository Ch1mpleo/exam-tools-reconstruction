using System;
using System.Windows.Forms;

namespace PEALogin;

internal static class Program
{
	[STAThread]
	// [behavior 0x06000008] STA entry point: enable visual styles, select compatible text rendering and start the message
	// [behavior 0x06000008] loop with AuthenticateForm.
	private static void Main()
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(defaultValue: false);
		Application.Run(new AuthenticateForm());
	}
}
