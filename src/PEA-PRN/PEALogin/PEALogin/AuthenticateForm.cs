using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Windows.Forms;
using EncryptData;
using GradeLib;
using IRemote;

namespace PEALogin;

public class AuthenticateForm : Form
{
	private Button btnLogin;

	private TextBox txtUser;

	private TextBox txtPassword;

	private TextBox txtDomain;

	private Label lblUser;

	private Label lblPass;

	private Button btnCancel;

	private Label lblDomain;

	private Container components = null;

	private Label lblExamCode;

	private Label lblMessage;

	private Label label1;

	private TextBox txtTestName;

	private string version = "41750C47-4395-4E5D-AD36-32CE826F5F59";

	private ServerInfo si = null;

	private PEAData pd = null;

	public AuthenticateForm()
	{
		InitializeComponent();
	}

	// [behavior 0x06000002] Disposes owned components/resources when requested and delegates lifetime cleanup as shown
	// [behavior 0x06000002] below.
	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	// [behavior 0x06000003] Constructs WinForms controls, assigns layout/style properties and wires event callbacks. Runtime
	// [behavior 0x06000003] business logic resides in those handlers.
	private void InitializeComponent()
	{
		this.btnLogin = new System.Windows.Forms.Button();
		this.txtUser = new System.Windows.Forms.TextBox();
		this.txtPassword = new System.Windows.Forms.TextBox();
		this.txtDomain = new System.Windows.Forms.TextBox();
		this.lblUser = new System.Windows.Forms.Label();
		this.lblPass = new System.Windows.Forms.Label();
		this.btnCancel = new System.Windows.Forms.Button();
		this.lblDomain = new System.Windows.Forms.Label();
		this.lblExamCode = new System.Windows.Forms.Label();
		this.txtTestName = new System.Windows.Forms.TextBox();
		this.lblMessage = new System.Windows.Forms.Label();
		this.label1 = new System.Windows.Forms.Label();
		base.SuspendLayout();
		this.btnLogin.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.btnLogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnLogin.Location = new System.Drawing.Point(96, 157);
		this.btnLogin.Name = "btnLogin";
		this.btnLogin.Size = new System.Drawing.Size(86, 30);
		this.btnLogin.TabIndex = 3;
		this.btnLogin.Text = "Login";
		this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
		this.txtUser.Location = new System.Drawing.Point(96, 54);
		this.txtUser.Name = "txtUser";
		this.txtUser.Size = new System.Drawing.Size(180, 20);
		this.txtUser.TabIndex = 1;
		this.txtPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtPassword.Location = new System.Drawing.Point(96, 89);
		this.txtPassword.Name = "txtPassword";
		this.txtPassword.PasswordChar = '*';
		this.txtPassword.Size = new System.Drawing.Size(180, 22);
		this.txtPassword.TabIndex = 2;
		this.txtPassword.Tag = "";
		this.txtDomain.Enabled = false;
		this.txtDomain.Location = new System.Drawing.Point(96, 126);
		this.txtDomain.Name = "txtDomain";
		this.txtDomain.Size = new System.Drawing.Size(180, 20);
		this.txtDomain.TabIndex = 9;
		this.lblUser.AutoSize = true;
		this.lblUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblUser.Location = new System.Drawing.Point(16, 55);
		this.lblUser.Name = "lblUser";
		this.lblUser.Size = new System.Drawing.Size(77, 16);
		this.lblUser.TabIndex = 6;
		this.lblUser.Text = "User name:";
		this.lblPass.AutoSize = true;
		this.lblPass.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblPass.Location = new System.Drawing.Point(22, 92);
		this.lblPass.Name = "lblPass";
		this.lblPass.Size = new System.Drawing.Size(71, 16);
		this.lblPass.TabIndex = 7;
		this.lblPass.Text = "Password:";
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btnCancel.Location = new System.Drawing.Point(282, 157);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(86, 30);
		this.btnCancel.TabIndex = 4;
		this.btnCancel.Text = "Exit";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.lblDomain.AutoSize = true;
		this.lblDomain.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblDomain.Location = new System.Drawing.Point(35, 127);
		this.lblDomain.Name = "lblDomain";
		this.lblDomain.Size = new System.Drawing.Size(58, 16);
		this.lblDomain.TabIndex = 5;
		this.lblDomain.Text = "Domain:";
		this.lblExamCode.AutoSize = true;
		this.lblExamCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblExamCode.Location = new System.Drawing.Point(18, 20);
		this.lblExamCode.Name = "lblExamCode";
		this.lblExamCode.Size = new System.Drawing.Size(75, 16);
		this.lblExamCode.TabIndex = 10;
		this.lblExamCode.Text = "Test name:";
		this.txtTestName.Location = new System.Drawing.Point(96, 19);
		this.txtTestName.Name = "txtTestName";
		this.txtTestName.Size = new System.Drawing.Size(272, 20);
		this.txtTestName.TabIndex = 0;
		this.lblMessage.AutoSize = true;
		this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.lblMessage.ForeColor = System.Drawing.Color.Red;
		this.lblMessage.Location = new System.Drawing.Point(9, 191);
		this.lblMessage.Name = "lblMessage";
		this.lblMessage.Size = new System.Drawing.Size(391, 16);
		this.lblMessage.TabIndex = 11;
		this.lblMessage.Text = "Register the exam may take some minutes! Please wait!";
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.Location = new System.Drawing.Point(9, 215);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(157, 16);
		this.label1.TabIndex = 12;
		this.label1.Text = "Ver 5.0 (build 25.06.20.26)";
		base.AcceptButton = this.btnLogin;
		this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
		base.ClientSize = new System.Drawing.Size(409, 240);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.lblMessage);
		base.Controls.Add(this.lblExamCode);
		base.Controls.Add(this.txtTestName);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.lblPass);
		base.Controls.Add(this.lblUser);
		base.Controls.Add(this.txtDomain);
		base.Controls.Add(this.txtPassword);
		base.Controls.Add(this.txtUser);
		base.Controls.Add(this.lblDomain);
		base.Controls.Add(this.btnLogin);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "AuthenticateForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "PEA Login";
		base.Load += new System.EventHandler(this.AuthenticateForm_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	// [behavior 0x06000004] Validates four textboxes, calls ConductTest through a TCP .NET Remoting proxy, handles
	// [behavior 0x06000004] registration statuses, then decompresses/loads the returned GUI and supplies its session and
	// [behavior 0x06000004] authentication form. Only NEW/REASSIGN enter that success path.
	private void btnLogin_Click(object sender, EventArgs e)
	{
		if (txtTestName.Text.Trim().Equals(""))
		{
			MessageBox.Show("Please provide a [Test name]", "Exam Registering");
			return;
		}
		if (txtUser.Text.Trim().Equals(""))
		{
			MessageBox.Show("Please provide an [User name]", "Exam Registering");
			return;
		}
		if (txtPassword.Text.Trim().Equals(""))
		{
			MessageBox.Show("Please provide a [Password]", "Exam Registering");
			return;
		}
		if (txtDomain.Text.Trim().Equals(""))
		{
			MessageBox.Show("Please provide a [Domain] address", "Exam Registering");
			return;
		}
		try
		{
			string remotingURL = "tcp://" + si.IP + ":" + si.Port + "/Server";
			IRemoteServer s = (IRemoteServer)Activator.GetObject(typeof(IRemoteServer), remotingURL);
			RegisterData rd = new RegisterData();
			rd.Login = txtUser.Text;
			rd.Password = txtPassword.Text;
			rd.TestName = txtTestName.Text;
			rd.PaperNo = -1;
			rd.Machine = Environment.MachineName.ToUpper();
			pd = s.ConductTest(rd);
			if (pd.Status == RegisterStatus.TEST_NAME_NOT_EXISTS)
			{
				MessageBox.Show("The test name is not available!", "Exam Registering", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else if (pd.Status == RegisterStatus.PAPER_NO_NOT_EXISTS)
			{
				MessageBox.Show("The [Paper no] does not exists!", "Exam Registering", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			else if (pd.Status == RegisterStatus.FINISHED)
			{
				MessageBox.Show("The test is finished!", "Exam Registering", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			else if (pd.Status == RegisterStatus.REGISTERED)
			{
				MessageBox.Show("This user [" + txtUser.Text + "] is already registered. Need re-assign to continue the exam.", "Exam Registering", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			else if (pd.Status == RegisterStatus.REGISTER_ERROR)
			{
				MessageBox.Show("Register ERROR, try again", "Exam Registering", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else if (pd.Status == RegisterStatus.NOT_ALLOW_STUDENT)
			{
				MessageBox.Show("The account is NOT allow to take the exam!", "Exam Registering", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			else if (pd.Status == RegisterStatus.LOGIN_FAILED)
			{
				MessageBox.Show("Sorry, unable to verify your information. Check [User Name] and [Password]!", "Login failed");
			}
			if (pd.Status == RegisterStatus.NEW || pd.Status == RegisterStatus.REASSIGN)
			{
				pd.GUI = GZipHelper.DeCompress(pd.GUI, pd.OriginSize);
				Assembly a = Assembly.Load(pd.GUI);
				Type fType = a.GetType("PEAClient.FrmPEAClient");
				Form f = (Form)Activator.CreateInstance(fType);
				IExamclient iec = (IExamclient)f;
				pd.GUI = null;
				pd.ServerInfomation = si;
				iec.SetExamData(pd);
				iec.SetAuthenForm(this);
				f.Show();
				Hide();
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "Login");
		}
	}

	// [behavior 0x06000005] Requests WinForms application exit. There is no remote cancellation or submission call.
	private void btnCancel_Click(object sender, EventArgs e)
	{
		Application.Exit();
	}

	// [behavior 0x06000006] Checks administrator membership, decrypts/deserializes PEA_Server_Info.dat, checks the client
	// [behavior 0x06000006] version identifier and displays the configured domain. Errors request application exit. File
	// [behavior 0x06000006] existence uses StartupPath; the decrypt call receives a relative filename.
	private void AuthenticateForm_Load(object sender, EventArgs e)
	{
		if (!IsAdministrator())
		{
			MessageBox.Show("You must login Windows as System Administrator or Run the application as administrator.", "PEA Login", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			Application.Exit();
		}
		string serverInfoFile = "PEA_Server_Info.dat";
		if (File.Exists(Application.StartupPath + "\\" + serverInfoFile))
		{
			try
			{
				string key = "04021976";
				byte[] buf = EncryptSupport.Decrypt_FromFile(serverInfoFile, key);
				si = (ServerInfo)EncryptSupport.ByteArrayToObject(buf);
				if (!version.Equals(si.Version))
				{
					MessageBox.Show("Wrong PEA client version! Please copy the right PEA client version.", "Version Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					Application.Exit();
				}
				txtDomain.Text = si.Domain;
				return;
			}
			catch
			{
				MessageBox.Show("Wrong [" + serverInfoFile + "] file format! Please copy the right EOS client version.", "Version Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				Application.Exit();
				return;
			}
		}
		MessageBox.Show("File [" + serverInfoFile + "] not found! Please copy the right EOS client version.", "Version Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		Application.Exit();
	}

	// [behavior 0x06000007] Checks the current WindowsPrincipal for WindowsBuiltInRole.Administrator. This is an operating-
	// [behavior 0x06000007] system role check, separate from the remote exam-account login.
	private bool IsAdministrator()
	{
		WindowsIdentity identity = WindowsIdentity.GetCurrent();
		WindowsPrincipal principle = new WindowsPrincipal(identity);
		if (principle.IsInRole(WindowsBuiltInRole.Administrator))
		{
			return true;
		}
		return false;
	}
}
