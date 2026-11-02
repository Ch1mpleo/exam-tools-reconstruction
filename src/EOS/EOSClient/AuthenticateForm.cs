// Recovered C#. Descriptive aliases are analyst-assigned; _M suffix is original MethodDef RID.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Windows.Forms;
using EOSClient;
using EncryptData;
using IRemote;
using NAudio.Wave;
using QuestionLib;

public class AuthenticateForm : Form
{
	[Serializable]
	[CompilerGenerated]
	private sealed class ExamCloseHandlers
	{
		public static readonly ExamCloseHandlers Type_000e_F004f = new ExamCloseHandlers();

		public static FormClosedEventHandler FormClosedEventHandler_F0050;

		internal void StopSessionLocks_M00e7(object P_0, FormClosedEventArgs P_1)
		{
			ExamNetworkLock_Stop_M00e8();
			ExamAppLock_Stop_M00e9();
		}

		internal static void ExamNetworkLock_Stop_M00e8()
		{
			ExamNetworkLock.Stop();
		}

		internal static void ExamAppLock_Stop_M00e9()
		{
			ExamAppLock.Stop();
		}
	}

	public const string ClientVersion = "2.0.2.6.0.7.2.9";

	private Button btnLogin;

	private TextBox txtUser;

	private TextBox txtPassword;

	private string domain;

	private Label lblUser;

	private Label lblPass;

	private Button btnCancel;

	private Label lblDomain;

	private Label lblDomainValue;

	private Container components;

	private Label lblExamCode;

	private Label lblMessage;

	private LinkLabel linkLBLCheckFont;

	private LinkLabel lblLinkCheckSound;

	private Label lblServerLabel;

	private TextBox txtExamCode;

	private ComboBox cbServer;

	private PictureBox pictureFlame;

	private Label lblWordmark;

	private Label lblTagline;

	private Panel panelAccentBar;

	private Panel panelNotice;

	private Label lblNoticeText;

	private Panel panelDivider1;

	private Panel panelDivider2;

	private Label lblFooter;

	private Panel examCodeBox;

	private Panel userBox;

	private Panel passBox;

	private Panel serverBox;

	private ServerInfo serverInfo;

	private FontCheckForm fontCheckForm;

	public AuthenticateForm()
	{
		InitializeComponent_M003d();
	}

	// [behavior 0x0600003c] Dispose component container when requested, then base.Dispose.
	protected override void Dispose(bool P_0)
	{
		if (P_0 && components != null)
		{
			Container_Dispose_M0051(components);
		}
		base.Dispose(P_0);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	// [behavior 0x0600003d] Creates the login UI, styles it and wires events; no remote calls occur here.
	private void InitializeComponent_M003d()
	{
		AppSettingsReader appSettingsReader = AppSettingsReader_Create_M0052();
		btnLogin = Button_Create_M0053();
		txtUser = TextBox_Create_M0054();
		txtPassword = TextBox_Create_M0054();
		lblUser = Label_Create_M0055();
		lblPass = Label_Create_M0055();
		btnCancel = Button_Create_M0053();
		lblDomain = Label_Create_M0055();
		lblDomainValue = Label_Create_M0055();
		lblExamCode = Label_Create_M0055();
		txtExamCode = TextBox_Create_M0054();
		lblMessage = Label_Create_M0055();
		linkLBLCheckFont = LinkLabel_Create_M0056();
		lblLinkCheckSound = LinkLabel_Create_M0056();
		lblServerLabel = Label_Create_M0055();
		cbServer = ComboBox_Create_M0057();
		pictureFlame = PictureBox_Create_M0058();
		lblWordmark = Label_Create_M0055();
		lblTagline = Label_Create_M0055();
		panelAccentBar = Panel_Create_M0059();
		panelNotice = Panel_Create_M0059();
		lblNoticeText = Label_Create_M0055();
		panelDivider1 = Panel_Create_M0059();
		panelDivider2 = Panel_Create_M0059();
		lblFooter = Label_Create_M0055();
		examCodeBox = Panel_Create_M0059();
		userBox = Panel_Create_M0059();
		passBox = Panel_Create_M0059();
		serverBox = Panel_Create_M0059();
		ISupportInitialize_BeginInit_M005a((ISupportInitialize)pictureFlame);
		Control_SuspendLayout_M005b((Control)panelNotice);
		Control_SuspendLayout_M005b((Control)examCodeBox);
		Control_SuspendLayout_M005b((Control)userBox);
		Control_SuspendLayout_M005b((Control)passBox);
		Control_SuspendLayout_M005b((Control)serverBox);
		Control_SuspendLayout_M005c((Control)this);
		Control_set_BackColor_M005d((Control)btnLogin, Color.FromArgb(233, 30, 99));
		Button_set_DialogResult_M005e(btnLogin, DialogResult.OK);
		FlatButtonAppearance_set_BorderSize_M0060(ButtonBase_get_FlatAppearance_M005f((ButtonBase)btnLogin), 0);
		ButtonBase_set_FlatStyle_M0061((ButtonBase)btnLogin, FlatStyle.Flat);
		Control_set_Font_M0063((Control)btnLogin, Font_Create_M0062("Segoe UI", 9.75f, FontStyle.Bold));
		Control_set_ForeColor_M0064((Control)btnLogin, Color.White);
		Control_set_Location_M0065((Control)btnLogin, new Point(32, 444));
		Control_set_Name_M0066((Control)btnLogin, "btnLogin");
		Control_set_Size_M0067((Control)btnLogin, new Size(270, 38));
		Control_set_TabIndex_M0068((Control)btnLogin, 4);
		Control_set_Text_M0069((Control)btnLogin, "Login");
		ButtonBase_set_UseVisualStyleBackColor_M006a((ButtonBase)btnLogin, false);
		Control_add_Click_M006b((Control)btnLogin, (EventHandler)LoginClick_M0046);
		Control_add_MouseEnter_M006c((Control)btnLogin, (EventHandler)LoginMouseEnter_M0042);
		Control_add_MouseLeave_M006d((Control)btnLogin, (EventHandler)LoginMouseLeave_M0043);
		Control_set_BackColor_M005d((Control)txtUser, Color.FromArgb(248, 249, 245));
		TextBoxBase_set_BorderStyle_M006e((TextBoxBase)txtUser, BorderStyle.None);
		Control_set_Font_M0063((Control)txtUser, Font_Create_M006f("Segoe UI", 9.75f));
		Control_set_Location_M0065((Control)txtUser, new Point(10, 9));
		Control_set_Name_M0066((Control)txtUser, "txtUser");
		Control_set_Size_M0067((Control)txtUser, new Size(356, 18));
		Control_set_TabIndex_M0068((Control)txtUser, 0);
		Control_add_Enter_M0070((Control)txtUser, (EventHandler)FieldEnter_M0040);
		Control_add_Leave_M0071((Control)txtUser, (EventHandler)FieldLeave_M0041);
		Control_set_BackColor_M005d((Control)txtPassword, Color.FromArgb(248, 249, 245));
		TextBoxBase_set_BorderStyle_M006e((TextBoxBase)txtPassword, BorderStyle.None);
		Control_set_Font_M0063((Control)txtPassword, Font_Create_M006f("Segoe UI", 9.75f));
		Control_set_Location_M0065((Control)txtPassword, new Point(10, 9));
		Control_set_Name_M0066((Control)txtPassword, "txtPassword");
		TextBox_set_PasswordChar_M0072(txtPassword, '*');
		Control_set_Size_M0067((Control)txtPassword, new Size(356, 18));
		Control_set_TabIndex_M0068((Control)txtPassword, 0);
		Control_add_Enter_M0070((Control)txtPassword, (EventHandler)FieldEnter_M0040);
		Control_add_Leave_M0071((Control)txtPassword, (EventHandler)FieldLeave_M0041);
		Control_set_AutoSize_M0073((Control)lblUser, true);
		Control_set_BackColor_M005d((Control)lblUser, Color.Transparent);
		Control_set_Font_M0063((Control)lblUser, Font_Create_M0062("Segoe UI", 8f, FontStyle.Bold));
		Control_set_ForeColor_M0064((Control)lblUser, Color.FromArgb(87, 98, 109));
		Control_set_Location_M0065((Control)lblUser, new Point(33, 220));
		Control_set_Name_M0066((Control)lblUser, "lblUser");
		Control_set_Size_M0067((Control)lblUser, new Size(89, 13));
		Control_set_TabIndex_M0068((Control)lblUser, 7);
		Control_set_Text_M0069((Control)lblUser, "U S E R N A M E");
		Control_set_AutoSize_M0073((Control)lblPass, true);
		Control_set_BackColor_M005d((Control)lblPass, Color.Transparent);
		Control_set_Font_M0063((Control)lblPass, Font_Create_M0062("Segoe UI", 8f, FontStyle.Bold));
		Control_set_ForeColor_M0064((Control)lblPass, Color.FromArgb(87, 98, 109));
		Control_set_Location_M0065((Control)lblPass, new Point(33, 286));
		Control_set_Name_M0066((Control)lblPass, "lblPass");
		Control_set_Size_M0067((Control)lblPass, new Size(89, 13));
		Control_set_TabIndex_M0068((Control)lblPass, 8);
		Control_set_Text_M0069((Control)lblPass, "P A S S W O R D");
		Control_set_BackColor_M005d((Control)btnCancel, Color.FromArgb(248, 249, 245));
		Button_set_DialogResult_M005e(btnCancel, DialogResult.Cancel);
		FlatButtonAppearance_set_BorderColor_M0074(ButtonBase_get_FlatAppearance_M005f((ButtonBase)btnCancel), Color.FromArgb(215, 217, 210));
		ButtonBase_set_FlatStyle_M0061((ButtonBase)btnCancel, FlatStyle.Flat);
		Control_set_Font_M0063((Control)btnCancel, Font_Create_M006f("Segoe UI", 9.75f));
		Control_set_ForeColor_M0064((Control)btnCancel, Color.FromArgb(87, 98, 109));
		Control_set_Location_M0065((Control)btnCancel, new Point(318, 444));
		Control_set_Name_M0066((Control)btnCancel, "btnCancel");
		Control_set_Size_M0067((Control)btnCancel, new Size(90, 38));
		Control_set_TabIndex_M0068((Control)btnCancel, 5);
		Control_set_Text_M0069((Control)btnCancel, "Exit");
		ButtonBase_set_UseVisualStyleBackColor_M006a((ButtonBase)btnCancel, false);
		Control_add_Click_M006b((Control)btnCancel, (EventHandler)ExitClick_M0049);
		Control_add_MouseEnter_M006c((Control)btnCancel, (EventHandler)ExitMouseEnter_M0044);
		Control_add_MouseLeave_M006d((Control)btnCancel, (EventHandler)ExitMouseLeave_M0045);
		Control_set_AutoSize_M0073((Control)lblDomain, true);
		Control_set_BackColor_M005d((Control)lblDomain, Color.Transparent);
		Control_set_Font_M0063((Control)lblDomain, Font_Create_M006f("Segoe UI", 8.5f));
		Control_set_ForeColor_M0064((Control)lblDomain, Color.FromArgb(87, 98, 109));
		Control_set_Location_M0065((Control)lblDomain, new Point(33, 415));
		Control_set_Name_M0066((Control)lblDomain, "lblDomain");
		Control_set_Size_M0067((Control)lblDomain, new Size(49, 15));
		Control_set_TabIndex_M0068((Control)lblDomain, 10);
		Control_set_Text_M0069((Control)lblDomain, "Domain");
		Control_set_BackColor_M005d((Control)lblDomainValue, Color.Transparent);
		Control_set_Font_M0063((Control)lblDomainValue, Font_Create_M0062("Segoe UI", 8.5f, FontStyle.Bold));
		Control_set_ForeColor_M0064((Control)lblDomainValue, Color.FromArgb(27, 36, 48));
		Control_set_Location_M0065((Control)lblDomainValue, new Point(208, 415));
		Control_set_Name_M0066((Control)lblDomainValue, "lblDomainValue");
		Control_set_Size_M0067((Control)lblDomainValue, new Size(200, 16));
		Control_set_TabIndex_M0068((Control)lblDomainValue, 11);
		Control_set_Text_M0069((Control)lblDomainValue, (string)AppSettingsReader_GetValue_M0076(appSettingsReader, "domain", Type_GetTypeFromHandle_M0075(typeof(string).TypeHandle)));
		Label_set_TextAlign_M0077(lblDomainValue, ContentAlignment.MiddleRight);
		Control_set_AutoSize_M0073((Control)lblExamCode, true);
		Control_set_BackColor_M005d((Control)lblExamCode, Color.Transparent);
		Control_set_Font_M0063((Control)lblExamCode, Font_Create_M0062("Segoe UI", 8f, FontStyle.Bold));
		Control_set_ForeColor_M0064((Control)lblExamCode, Color.FromArgb(87, 98, 109));
		Control_set_Location_M0065((Control)lblExamCode, new Point(33, 154));
		Control_set_Name_M0066((Control)lblExamCode, "lblExamCode");
		Control_set_Size_M0067((Control)lblExamCode, new Size(95, 13));
		Control_set_TabIndex_M0068((Control)lblExamCode, 6);
		Control_set_Text_M0069((Control)lblExamCode, "E X A M   C O D E");
		Control_set_BackColor_M005d((Control)txtExamCode, Color.FromArgb(248, 249, 245));
		TextBoxBase_set_BorderStyle_M006e((TextBoxBase)txtExamCode, BorderStyle.None);
		Control_set_Font_M0063((Control)txtExamCode, Font_Create_M006f("Segoe UI", 9.75f));
		Control_set_Location_M0065((Control)txtExamCode, new Point(10, 9));
		Control_set_Name_M0066((Control)txtExamCode, "txtExamCode");
		Control_set_Size_M0067((Control)txtExamCode, new Size(356, 18));
		Control_set_TabIndex_M0068((Control)txtExamCode, 0);
		Control_add_Enter_M0070((Control)txtExamCode, (EventHandler)FieldEnter_M0040);
		Control_add_Leave_M0071((Control)txtExamCode, (EventHandler)FieldLeave_M0041);
		Control_set_BackColor_M005d((Control)lblMessage, Color.Transparent);
		Control_set_Font_M0063((Control)lblMessage, Font_Create_M006f("Segoe UI", 8.25f));
		Control_set_ForeColor_M0064((Control)lblMessage, Color.FromArgb(87, 98, 109));
		Control_set_Location_M0065((Control)lblMessage, new Point(32, 490));
		Control_set_Name_M0066((Control)lblMessage, "lblMessage");
		Control_set_Size_M0067((Control)lblMessage, new Size(376, 16));
		Control_set_TabIndex_M0068((Control)lblMessage, 12);
		Control_set_Text_M0069((Control)lblMessage, "Registering the exam may take a moment, please wait.");
		Label_set_TextAlign_M0077(lblMessage, ContentAlignment.MiddleCenter);
		LinkLabel_set_ActiveLinkColor_M0078(linkLBLCheckFont, Color.FromArgb(233, 30, 99));
		Control_set_AutoSize_M0073((Control)linkLBLCheckFont, true);
		Control_set_Font_M0063((Control)linkLBLCheckFont, Font_Create_M006f("Segoe UI", 8.25f));
		LinkLabel_set_LinkBehavior_M0079(linkLBLCheckFont, LinkBehavior.HoverUnderline);
		LinkLabel_set_LinkColor_M007a(linkLBLCheckFont, Color.FromArgb(87, 98, 109));
		Control_set_Location_M0065((Control)linkLBLCheckFont, new Point(255, 534));
		Control_set_Name_M0066((Control)linkLBLCheckFont, "linkLBLCheckFont");
		Control_set_Size_M0067((Control)linkLBLCheckFont, new Size(63, 13));
		Control_set_TabIndex_M0068((Control)linkLBLCheckFont, 7);
		Label_set_TabStop_M007b((Label)linkLBLCheckFont, true);
		Control_set_Text_M0069((Control)linkLBLCheckFont, "Check font");
		LinkLabel_set_VisitedLinkColor_M007c(linkLBLCheckFont, Color.FromArgb(87, 98, 109));
		LinkLabel_add_LinkClicked_M007d(linkLBLCheckFont, (LinkLabelLinkClickedEventHandler)ShowFontCheck_M004d);
		LinkLabel_set_ActiveLinkColor_M0078(lblLinkCheckSound, Color.FromArgb(233, 30, 99));
		Control_set_AutoSize_M0073((Control)lblLinkCheckSound, true);
		Control_set_Font_M0063((Control)lblLinkCheckSound, Font_Create_M006f("Segoe UI", 8.25f));
		LinkLabel_set_LinkBehavior_M0079(lblLinkCheckSound, LinkBehavior.HoverUnderline);
		LinkLabel_set_LinkColor_M007a(lblLinkCheckSound, Color.FromArgb(87, 98, 109));
		Control_set_Location_M0065((Control)lblLinkCheckSound, new Point(127, 534));
		Control_set_Name_M0066((Control)lblLinkCheckSound, "lblLinkCheckSound");
		Control_set_Size_M0067((Control)lblLinkCheckSound, new Size(113, 13));
		Control_set_TabIndex_M0068((Control)lblLinkCheckSound, 6);
		Label_set_TabStop_M007b((Label)lblLinkCheckSound, true);
		Control_set_Text_M0069((Control)lblLinkCheckSound, "Check sound (7 secs)");
		LinkLabel_set_VisitedLinkColor_M007c(lblLinkCheckSound, Color.FromArgb(87, 98, 109));
		LinkLabel_add_LinkClicked_M007d(lblLinkCheckSound, (LinkLabelLinkClickedEventHandler)CheckSound_M004e);
		Control_set_AutoSize_M0073((Control)lblServerLabel, true);
		Control_set_BackColor_M005d((Control)lblServerLabel, Color.Transparent);
		Control_set_Font_M0063((Control)lblServerLabel, Font_Create_M0062("Segoe UI", 8f, FontStyle.Bold));
		Control_set_ForeColor_M0064((Control)lblServerLabel, Color.FromArgb(87, 98, 109));
		Control_set_Location_M0065((Control)lblServerLabel, new Point(33, 352));
		Control_set_Name_M0066((Control)lblServerLabel, "lblServerLabel");
		Control_set_Size_M0067((Control)lblServerLabel, new Size(61, 13));
		Control_set_TabIndex_M0068((Control)lblServerLabel, 9);
		Control_set_Text_M0069((Control)lblServerLabel, "S E R V E R");
		Control_set_BackColor_M005d((Control)cbServer, Color.FromArgb(248, 249, 245));
		ComboBox_set_FlatStyle_M007e(cbServer, FlatStyle.Flat);
		Control_set_Font_M0063((Control)cbServer, Font_Create_M006f("Segoe UI", 9.75f));
		Control_set_ForeColor_M0064((Control)cbServer, Color.FromArgb(27, 36, 48));
		ListControl_set_FormattingEnabled_M007f((ListControl)cbServer, true);
		Control_set_Location_M0065((Control)cbServer, new Point(10, 7));
		Control_set_Name_M0066((Control)cbServer, "cbServer");
		Control_set_Size_M0067((Control)cbServer, new Size(356, 25));
		Control_set_TabIndex_M0068((Control)cbServer, 0);
		Control_add_Enter_M0070((Control)cbServer, (EventHandler)FieldEnter_M0040);
		Control_add_Leave_M0071((Control)cbServer, (EventHandler)FieldLeave_M0041);
		Control_set_BackColor_M005d((Control)pictureFlame, Color.FromArgb(237, 238, 233));
		Control_set_Location_M0065((Control)pictureFlame, new Point(32, 24));
		Control_set_Name_M0066((Control)pictureFlame, "pictureFlame");
		Control_set_Size_M0067((Control)pictureFlame, new Size(22, 26));
		PictureBox_set_TabIndex_M0080(pictureFlame, 1);
		PictureBox_set_TabStop_M0081(pictureFlame, false);
		Control_add_Paint_M0082((Control)pictureFlame, (PaintEventHandler)PaintBrandGlyph_M003e);
		Control_set_AutoSize_M0073((Control)lblWordmark, true);
		Control_set_BackColor_M005d((Control)lblWordmark, Color.Transparent);
		Control_set_Font_M0063((Control)lblWordmark, Font_Create_M006f("Georgia", 17f));
		Control_set_ForeColor_M0064((Control)lblWordmark, Color.FromArgb(27, 36, 48));
		Control_set_Location_M0065((Control)lblWordmark, new Point(60, 19));
		Control_set_Name_M0066((Control)lblWordmark, "lblWordmark");
		Control_set_Size_M0067((Control)lblWordmark, new Size(57, 27));
		Control_set_TabIndex_M0068((Control)lblWordmark, 2);
		Control_set_Text_M0069((Control)lblWordmark, "EOS");
		Control_set_AutoSize_M0073((Control)lblTagline, true);
		Control_set_BackColor_M005d((Control)lblTagline, Color.Transparent);
		Control_set_Font_M0063((Control)lblTagline, Font_Create_M0062("Segoe UI", 8f, FontStyle.Bold));
		Control_set_ForeColor_M0064((Control)lblTagline, Color.FromArgb(233, 30, 99));
		Control_set_Location_M0065((Control)lblTagline, new Point(33, 55));
		Control_set_Name_M0066((Control)lblTagline, "lblTagline");
		Control_set_Size_M0067((Control)lblTagline, new Size(191, 13));
		Control_set_TabIndex_M0068((Control)lblTagline, 3);
		Control_set_Text_M0069((Control)lblTagline, "R E K I N D L I N G   T H E   S P I R I T");
		Control_set_BackColor_M005d((Control)panelAccentBar, Color.FromArgb(233, 30, 99));
		Control_set_Location_M0065((Control)panelAccentBar, new Point(0, 0));
		Control_set_Name_M0066((Control)panelAccentBar, "panelAccentBar");
		Control_set_Size_M0067((Control)panelAccentBar, new Size(440, 3));
		Control_set_TabIndex_M0068((Control)panelAccentBar, 0);
		Control_set_BackColor_M005d((Control)panelNotice, Color.FromArgb(225, 232, 240));
		ControlCollection_Add_M0084(Control_get_Controls_M0083((Control)panelNotice), (Control)lblNoticeText);
		Control_set_Location_M0065((Control)panelNotice, new Point(32, 80));
		Control_set_Name_M0066((Control)panelNotice, "panelNotice");
		Control_set_Size_M0067((Control)panelNotice, new Size(376, 52));
		Control_set_TabIndex_M0068((Control)panelNotice, 4);
		Control_add_Paint_M0082((Control)panelNotice, (PaintEventHandler)PaintPanelBorder_M003f);
		Control_set_BackColor_M005d((Control)lblNoticeText, Color.Transparent);
		Control_set_Font_M0063((Control)lblNoticeText, Font_Create_M006f("Segoe UI", 8.5f));
		Control_set_ForeColor_M0064((Control)lblNoticeText, Color.FromArgb(27, 77, 139));
		Control_set_Location_M0065((Control)lblNoticeText, new Point(12, 8));
		Control_set_Name_M0066((Control)lblNoticeText, "lblNoticeText");
		Control_set_Size_M0067((Control)lblNoticeText, new Size(352, 36));
		Control_set_TabIndex_M0068((Control)lblNoticeText, 0);
		Control_set_Text_M0069((Control)lblNoticeText, "This exam cannot be taken inside a virtual machine. Please run EOS on a physical device.");
		Control_set_BackColor_M005d((Control)panelDivider1, Color.FromArgb(215, 217, 210));
		Control_set_Location_M0065((Control)panelDivider1, new Point(32, 142));
		Control_set_Name_M0066((Control)panelDivider1, "panelDivider1");
		Control_set_Size_M0067((Control)panelDivider1, new Size(376, 1));
		Control_set_TabIndex_M0068((Control)panelDivider1, 5);
		Control_set_BackColor_M005d((Control)panelDivider2, Color.FromArgb(215, 217, 210));
		Control_set_Location_M0065((Control)panelDivider2, new Point(32, 520));
		Control_set_Name_M0066((Control)panelDivider2, "panelDivider2");
		Control_set_Size_M0067((Control)panelDivider2, new Size(376, 1));
		Control_set_TabIndex_M0068((Control)panelDivider2, 13);
		Control_set_BackColor_M005d((Control)lblFooter, Color.Transparent);
		Control_set_Font_M0063((Control)lblFooter, Font_Create_M006f("Segoe UI", 8f));
		Control_set_ForeColor_M0064((Control)lblFooter, Color.FromArgb(125, 133, 140));
		Control_set_Location_M0065((Control)lblFooter, new Point(0, 566));
		Control_set_Name_M0066((Control)lblFooter, "lblFooter");
		Control_set_Size_M0067((Control)lblFooter, new Size(440, 16));
		Control_set_TabIndex_M0068((Control)lblFooter, 14);
		Control_set_Text_M0069((Control)lblFooter, "EOS Exam Client - v29.07.20.26");
		Label_set_TextAlign_M0077(lblFooter, ContentAlignment.MiddleCenter);
		Control_set_BackColor_M005d((Control)examCodeBox, Color.FromArgb(215, 217, 210));
		ControlCollection_Add_M0084(Control_get_Controls_M0083((Control)examCodeBox), (Control)txtExamCode);
		Control_set_Location_M0065((Control)examCodeBox, new Point(32, 172));
		Control_set_Name_M0066((Control)examCodeBox, "examCodeBox");
		Control_set_Size_M0067((Control)examCodeBox, new Size(376, 36));
		Control_set_TabIndex_M0068((Control)examCodeBox, 0);
		Control_set_BackColor_M005d((Control)userBox, Color.FromArgb(215, 217, 210));
		ControlCollection_Add_M0084(Control_get_Controls_M0083((Control)userBox), (Control)txtUser);
		Control_set_Location_M0065((Control)userBox, new Point(32, 238));
		Control_set_Name_M0066((Control)userBox, "userBox");
		Control_set_Size_M0067((Control)userBox, new Size(376, 36));
		Control_set_TabIndex_M0068((Control)userBox, 1);
		Control_set_BackColor_M005d((Control)passBox, Color.FromArgb(215, 217, 210));
		ControlCollection_Add_M0084(Control_get_Controls_M0083((Control)passBox), (Control)txtPassword);
		Control_set_Location_M0065((Control)passBox, new Point(32, 304));
		Control_set_Name_M0066((Control)passBox, "passBox");
		Control_set_Size_M0067((Control)passBox, new Size(376, 36));
		Control_set_TabIndex_M0068((Control)passBox, 2);
		Control_set_BackColor_M005d((Control)serverBox, Color.FromArgb(215, 217, 210));
		ControlCollection_Add_M0084(Control_get_Controls_M0083((Control)serverBox), (Control)cbServer);
		Control_set_Location_M0065((Control)serverBox, new Point(32, 370));
		Control_set_Name_M0066((Control)serverBox, "serverBox");
		Control_set_Size_M0067((Control)serverBox, new Size(376, 36));
		Control_set_TabIndex_M0068((Control)serverBox, 3);
		Form_set_AcceptButton_M0085((Form)this, (IButtonControl)btnLogin);
		Form_set_AutoScaleBaseSize_M0086((Form)this, new Size(5, 13));
		Control_set_BackColor_M005d((Control)this, Color.FromArgb(237, 238, 233));
		Form_set_ClientSize_M0087((Form)this, new Size(440, 604));
		ControlCollection_Add_M0084(Control_get_Controls_M0088((Control)this), (Control)panelAccentBar);
		ControlCollection_Add_M0084(Control_get_Controls_M0088((Control)this), (Control)pictureFlame);
		ControlCollection_Add_M0084(Control_get_Controls_M0088((Control)this), (Control)lblWordmark);
		ControlCollection_Add_M0084(Control_get_Controls_M0088((Control)this), (Control)lblTagline);
		ControlCollection_Add_M0084(Control_get_Controls_M0088((Control)this), (Control)panelNotice);
		ControlCollection_Add_M0084(Control_get_Controls_M0088((Control)this), (Control)panelDivider1);
		ControlCollection_Add_M0084(Control_get_Controls_M0088((Control)this), (Control)lblExamCode);
		ControlCollection_Add_M0084(Control_get_Controls_M0088((Control)this), (Control)examCodeBox);
		ControlCollection_Add_M0084(Control_get_Controls_M0088((Control)this), (Control)lblUser);
		ControlCollection_Add_M0084(Control_get_Controls_M0088((Control)this), (Control)userBox);
		ControlCollection_Add_M0084(Control_get_Controls_M0088((Control)this), (Control)lblPass);
		ControlCollection_Add_M0084(Control_get_Controls_M0088((Control)this), (Control)passBox);
		ControlCollection_Add_M0084(Control_get_Controls_M0088((Control)this), (Control)lblServerLabel);
		ControlCollection_Add_M0084(Control_get_Controls_M0088((Control)this), (Control)serverBox);
		ControlCollection_Add_M0084(Control_get_Controls_M0088((Control)this), (Control)lblDomain);
		ControlCollection_Add_M0084(Control_get_Controls_M0088((Control)this), (Control)lblDomainValue);
		ControlCollection_Add_M0084(Control_get_Controls_M0088((Control)this), (Control)btnLogin);
		ControlCollection_Add_M0084(Control_get_Controls_M0088((Control)this), (Control)btnCancel);
		ControlCollection_Add_M0084(Control_get_Controls_M0088((Control)this), (Control)lblMessage);
		ControlCollection_Add_M0084(Control_get_Controls_M0088((Control)this), (Control)panelDivider2);
		ControlCollection_Add_M0084(Control_get_Controls_M0088((Control)this), (Control)lblLinkCheckSound);
		ControlCollection_Add_M0084(Control_get_Controls_M0088((Control)this), (Control)linkLBLCheckFont);
		ControlCollection_Add_M0084(Control_get_Controls_M0088((Control)this), (Control)lblFooter);
		Form_set_FormBorderStyle_M0089((Form)this, FormBorderStyle.FixedSingle);
		Form_set_MaximizeBox_M008a((Form)this, false);
		Form_set_MinimizeBox_M008b((Form)this, false);
		Control_set_Name_M008c((Control)this, "AuthenticateForm");
		Form_set_StartPosition_M008d((Form)this, FormStartPosition.CenterScreen);
		Control_set_Text_M0069((Control)this, "EOS \u2014 Exam Login");
		Form_add_FormClosing_M008e((Form)this, (FormClosingEventHandler)Closing_M0050);
		Form_add_Load_M008f((Form)this, (EventHandler)LoadServerConfiguration_M004a);
		ISupportInitialize_EndInit_M0090((ISupportInitialize)pictureFlame);
		Control_ResumeLayout_M0091((Control)panelNotice, false);
		Control_ResumeLayout_M0091((Control)examCodeBox, false);
		Control_PerformLayout_M0092((Control)examCodeBox);
		Control_ResumeLayout_M0091((Control)userBox, false);
		Control_PerformLayout_M0092((Control)userBox);
		Control_ResumeLayout_M0091((Control)passBox, false);
		Control_PerformLayout_M0092((Control)passBox);
		Control_ResumeLayout_M0091((Control)serverBox, false);
		Control_ResumeLayout_M0093((Control)this, false);
		Control_PerformLayout_M0094((Control)this);
	}

	// [behavior 0x0600003e] Pass the paint event Graphics into the shared brand-glyph painter.
	private void PaintBrandGlyph_M003e(object P_0, PaintEventArgs P_1)
	{
		ClientTheme.DrawBrandGlyph_M00f4(PaintEventArgs_get_Graphics_M0095(P_1));
	}

	private void PaintPanelBorder_M003f(object P_0, PaintEventArgs P_1)
	{
		ClientTheme.DrawPanelBorder_M00f7(P_1, (Panel)P_0, ClientTheme.Color_F005f);
	}

	// [behavior 0x06000040] On input focus, change its parent-panel background to the accent if a parent exists.
	private void FieldEnter_M0040(object P_0, EventArgs P_1)
	{
		if (P_0 is Control control && Control_get_Parent_M0096(control) != null)
		{
			Control_set_BackColor_M005d(Control_get_Parent_M0096(control), ClientTheme.Color_F005b);
		}
	}

	// [behavior 0x06000041] On input blur, restore its parent-panel border/background color if a parent exists.
	private void FieldLeave_M0041(object P_0, EventArgs P_1)
	{
		if (P_0 is Control control && Control_get_Parent_M0096(control) != null)
		{
			Control_set_BackColor_M005d(Control_get_Parent_M0096(control), ClientTheme.Color_F005a);
		}
	}

	private void LoginMouseEnter_M0042(object P_0, EventArgs P_1)
	{
		Control_set_BackColor_M005d((Control)btnLogin, ClientTheme.Color_F005c);
	}

	private void LoginMouseLeave_M0043(object P_0, EventArgs P_1)
	{
		Control_set_BackColor_M005d((Control)btnLogin, ClientTheme.Color_F005b);
	}

	// [behavior 0x06000044] On Exit hover, darken its border and text.
	private void ExitMouseEnter_M0044(object P_0, EventArgs P_1)
	{
		FlatButtonAppearance_set_BorderColor_M0074(ButtonBase_get_FlatAppearance_M005f((ButtonBase)btnCancel), ClientTheme.Color_F0058);
		Control_set_ForeColor_M0064((Control)btnCancel, ClientTheme.Color_F0057);
	}

	// [behavior 0x06000045] On Exit mouse leave, restore its border and text colors.
	private void ExitMouseLeave_M0045(object P_0, EventArgs P_1)
	{
		FlatButtonAppearance_set_BorderColor_M0074(ButtonBase_get_FlatAppearance_M005f((ButtonBase)btnCancel), ClientTheme.Color_F005a);
		Control_set_ForeColor_M0064((Control)btnCancel, ClientTheme.Color_F0058);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	// [behavior 0x06000046] EOS login orchestration: validate inputs, choose endpoint/public address, connect and Ping,
	// [behavior 0x06000046] compare required version, ConductExam, handle status, load GUI bytes, optionally run camera
	// [behavior 0x06000046] check-in, instantiate exam form and attach cleanup. See README.md for the ordered branch
	// [behavior 0x06000046] explanation.
	private void LoginClick_M0046(object P_0, EventArgs P_1)
	{
		Control_set_Enabled_M0097((Control)btnLogin, false);
		Control_set_Enabled_M0097((Control)btnCancel, false);
		if (!String_Equals_M009a(String_Trim_M0099(Control_get_Text_M0098((Control)txtExamCode)), ""))
		{
			if (!String_Equals_M009a(String_Trim_M0099(Control_get_Text_M0098((Control)txtUser)), ""))
			{
				if (String_Equals_M009a(String_Trim_M0099(Control_get_Text_M0098((Control)txtPassword)), ""))
				{
					MessageBox_Show_M009b("Please provide a password", "Login");
					Control_set_Enabled_M0097((Control)btnLogin, true);
					Control_set_Enabled_M0097((Control)btnCancel, true);
					return;
				}
				if (String_IsNullOrEmpty_M009c(domain) || String_Equals_M009a(String_Trim_M0099(domain), ""))
				{
					MessageBox_Show_M009b("Please provide a domain address", "Login");
					Control_set_Enabled_M0097((Control)btnLogin, true);
					Control_set_Enabled_M0097((Control)btnCancel, true);
					return;
				}
				if (!String_IsNullOrEmpty_M009c(Control_get_Text_M0098((Control)cbServer)))
				{
					try
					{
						string text = "0.0.0.0";
						int num = -1;
						string text2 = "0.0.0.0";
						if (ComboBox_get_SelectedItem_M009d(cbServer) is EndpointItem endpointItem)
						{
							text = endpointItem.IP;
							num = endpointItem.Port;
							text2 = endpointItem.Public_IP;
						}
						ServerInfo_set_IP_M009e(serverInfo, text);
						ServerInfo_set_Port_M009f(serverInfo, num);
						if (!String_Equals_M009a(String_Trim_M0099(text2), ""))
						{
							ServerInfo_set_IP_M009e(serverInfo, text2);
						}
						IRemoteServer remoteServer = ConnectAndPing_M0047(ServerInfo_get_IP_M00a0(serverInfo), ServerInfo_get_Port_M00a1(serverInfo));
						if (remoteServer != null)
						{
							if (HasVersionMismatch_M0048(remoteServer))
							{
								MessageBox_Show_M00a2("Your EOSClient version does not match the version required by the exam server.\nPlease install the required EOSClient version before taking the exam.", "Update required", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
								Control_set_Enabled_M0097((Control)btnLogin, true);
								Control_set_Enabled_M0097((Control)btnCancel, true);
							}
							else
							{
								RegisterData registerData = RegisterData_Create_M00a3();
								RegisterData_set_Login_M00a5(registerData, String_Trim_M0099(String_ToLower_M00a4(Control_get_Text_M0098((Control)txtUser))));
								RegisterData_set_Password_M00a6(registerData, Control_get_Text_M0098((Control)txtPassword));
								RegisterData_set_ExamCode_M00a7(registerData, Control_get_Text_M0098((Control)txtExamCode));
								RegisterData_set_Machine_M00aa(registerData, String_ToUpper_M00a9(Environment_get_MachineName_M00a8()));
								RegisterData_set_ClientVersion_M00ab(registerData, "2.0.2.6.0.7.2.9");
								EOSData eOSData = IRemoteServer_ConductExam_M00ac(remoteServer, registerData);
								if (eOSData.Status == RegisterStatus.EXAM_CODE_NOT_EXISTS)
								{
									MessageBox_Show_M00a2("Exam code is not available!", "Start exam", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
									Control_set_Enabled_M0097((Control)btnLogin, true);
									Control_set_Enabled_M0097((Control)btnCancel, true);
								}
								else if (eOSData.Status != RegisterStatus.FINISHED)
								{
									if (eOSData.Status != RegisterStatus.REGISTERED)
									{
										if (eOSData.Status == RegisterStatus.REGISTER_ERROR)
										{
											MessageBox_Show_M00a2("Register ERROR, try again", "Exam Registering", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
											Control_set_Enabled_M0097((Control)btnLogin, true);
											Control_set_Enabled_M0097((Control)btnCancel, true);
										}
										else if (eOSData.Status != RegisterStatus.NOT_ALLOW_MACHINE)
										{
											if (eOSData.Status != RegisterStatus.NOT_ALLOW_STUDENT)
											{
												if (eOSData.Status == RegisterStatus.LOGIN_FAILED)
												{
													MessageBox_Show_M009b("Sorry, unable to verify your information. Check [User Name] and [Password]!", "Login failed");
													Control_set_Enabled_M0097((Control)btnLogin, true);
													Control_set_Enabled_M0097((Control)btnCancel, true);
												}
												else if (eOSData.Status == RegisterStatus.CLIENT_VERSION_NOT_SUPPORTED)
												{
													MessageBox_Show_M00a2("Your EOSClient version does not match the version required by the exam server.\nPlease install the required EOSClient version before taking the exam.", "Update required", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
													Control_set_Enabled_M0097((Control)btnLogin, true);
													Control_set_Enabled_M0097((Control)btnCancel, true);
												}
											}
											else
											{
												MessageBox_Show_M00a2("The account is NOT allowed to take the exam!", "Exam Registering", MessageBoxButtons.OK, MessageBoxIcon.Hand);
												Control_set_Enabled_M0097((Control)btnLogin, true);
												Control_set_Enabled_M0097((Control)btnCancel, true);
											}
										}
										else
										{
											MessageBox_Show_M00a2("Your machine is not allowed to take the exam!", "Exam Registering", MessageBoxButtons.OK, MessageBoxIcon.Hand);
											Control_set_Enabled_M0097((Control)btnLogin, true);
											Control_set_Enabled_M0097((Control)btnCancel, true);
										}
									}
									else
									{
										MessageBox_Show_M00a2(String_Concat_M00ad("This user [", Control_get_Text_M0098((Control)txtUser), "] is already registered. Need re-assign to continue the exam."), "Exam Registering", MessageBoxButtons.OK, MessageBoxIcon.Hand);
										Control_set_Enabled_M0097((Control)btnLogin, true);
										Control_set_Enabled_M0097((Control)btnCancel, true);
									}
								}
								else
								{
									MessageBox_Show_M00a2("The exam is finished!", "Start exam", MessageBoxButtons.OK, MessageBoxIcon.Hand);
									Control_set_Enabled_M0097((Control)btnLogin, true);
									Control_set_Enabled_M0097((Control)btnCancel, true);
								}
								if (eOSData.Status == RegisterStatus.NEW || eOSData.Status == RegisterStatus.RE_ASSIGN)
								{
									Control_Hide_M00ae((Control)this);
									eOSData.GUI = GZipHelper_DeCompress_M00af(eOSData.GUI, eOSData.OriginSize);
									Assembly assembly = Assembly_Load_M00b0(eOSData.GUI);
									eOSData.GUI = null;
									eOSData.ServerInfomation = serverInfo;
									eOSData.RegData = registerData;
									byte[] array = ((!eOSData.SkipCameraCheckIn) ? ((byte[])MethodBase_Invoke_M00b3((MethodBase)Type_GetMethod_M00b2(Assembly_GetType_M00b1(assembly, "ExamClient.PreExamEntry"), "RunCheckIn"), (object)null, new object[1] { eOSData })) : new byte[0]);
									if (array == null)
									{
										Application_set_UseWaitCursor_M00b4(false);
										Cursor_set_Current_M00b6(Cursors_get_Default_M00b5());
										MessageBox_Show_M00a2("Face check-in could not be completed. Please try logging in again.", "Face check-in", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
										Control_Show_M00b7((Control)this);
										Control_set_Enabled_M0097((Control)btnLogin, true);
										Control_set_Enabled_M0097((Control)btnCancel, true);
									}
									else
									{
										Type type = Assembly_GetType_M00b1(assembly, "ExamClient.frmEnglishExamClient");
										Form form = (Form)Activator_CreateInstance_M00b8(type);
										IExamclient_SetExamData_M00b9((IExamclient)form, eOSData);
										if (array.Length != 0)
										{
											MethodBase_Invoke_M00b3((MethodBase)Type_GetMethod_M00b2(type, "SetCheckInPhoto"), (object)form, new object[1] { array });
										}
										Form_add_FormClosed_M00ba(form, (FormClosedEventHandler)delegate
										{
											ExamCloseHandlers.ExamNetworkLock_Stop_M00e8();
											ExamCloseHandlers.ExamAppLock_Stop_M00e9();
										});
										Control_Show_M00bb((Control)form);
										Application_set_UseWaitCursor_M00b4(false);
										Cursor_set_Current_M00b6(Cursors_get_Default_M00b5());
									}
								}
							}
						}
						else
						{
							MessageBox_Show_M00a2("Unable to connect to the exam server. Please check your network.", "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
							Control_set_Enabled_M0097((Control)btnLogin, true);
							Control_set_Enabled_M0097((Control)btnCancel, true);
						}
						return;
					}
					catch (Exception ex)
					{
						Application_set_UseWaitCursor_M00b4(false);
						Cursor_set_Current_M00b6(Cursors_get_Default_M00b5());
						MessageBox_Show_M00bd(Object_ToString_M00bc((object)ex));
						MessageBox_Show_M00a2("Start Exam Error:\nCannot connect to the EOS server!\n", "Connecting...", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						Control_set_Enabled_M0097((Control)btnLogin, true);
						Control_set_Enabled_M0097((Control)btnCancel, true);
						return;
					}
				}
				MessageBox_Show_M009b("Please provide a server", "Login");
				Control_set_Enabled_M0097((Control)btnLogin, true);
				Control_set_Enabled_M0097((Control)btnCancel, true);
			}
			else
			{
				MessageBox_Show_M009b("Please provide an username", "Login");
				Control_set_Enabled_M0097((Control)btnLogin, true);
				Control_set_Enabled_M0097((Control)btnCancel, true);
			}
		}
		else
		{
			MessageBox_Show_M009b("Please provide an Exam code", "Login");
			Control_set_Enabled_M0097((Control)btnLogin, true);
			Control_set_Enabled_M0097((Control)btnCancel, true);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	// [behavior 0x06000047] Builds tcp://IP:port/Server, obtains an IRemoteServer proxy and calls Ping. Returns the proxy
	// [behavior 0x06000047] only on success; exceptions return null.
	private IRemoteServer ConnectAndPing_M0047(string P_0, int P_1)
	{
		try
		{
			string text = String_Concat_M00be(new object[5]
			{
				"tcp://",
				P_0,
				":",
				P_1,
				"/Server"
			});
			IRemoteServer remoteServer = (IRemoteServer)Activator_GetObject_M00bf(Type_GetTypeFromHandle_M0075(typeof(IRemoteServer).TypeHandle), text);
			if (remoteServer != null && IRemoteServer_Ping_M00c0(remoteServer))
			{
				return remoteServer;
			}
		}
		catch
		{
		}
		return null;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	// [behavior 0x06000048] Returns true only for a nonblank server-required version that differs from the local string
	// [behavior 0x06000048] under OrdinalIgnoreCase. Empty/null versions and exceptions return false. This is equality, not
	// [behavior 0x06000048] numeric ordering.
	private static bool HasVersionMismatch_M0048(IRemoteServer P_0)
	{
		bool result;
		try
		{
			string text = IRemoteServer_GetMinRequiredClientVersion_M00c1(P_0);
			result = text != null && String_get_Length_M00c2(String_Trim_M0099(text)) != 0 && !String_Equals_M00c4(String_Trim_M00c3("2.0.2.6.0.7.2.9"), String_Trim_M0099(text), StringComparison.OrdinalIgnoreCase);
		}
		catch
		{
			result = false;
		}
		return result;
	}

	// [behavior 0x06000049] Requests application exit and terminates the process with Environment.Exit(0).
	private void ExitClick_M0049(object P_0, EventArgs P_1)
	{
		Application_Exit_M00c5();
		Environment_Exit_M00c6(0);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	// [behavior 0x0600004a] Decrypts/deserializes the local EOS server file, configures the domain and validates/adds each
	// [behavior 0x0600004a] endpoint. Missing/invalid configuration requests exit.
	private void LoadServerConfiguration_M004a(object P_0, EventArgs P_1)
	{
		string text = "EOS_Server_Info.dat";
		if (File_Exists_M00c8(String_Concat_M00ad(Application_get_StartupPath_M00c7(), "\\", text)))
		{
			try
			{
				string text2 = "10901886";
				byte[] array = EncryptSupport_DecryptQuestions_FromFile_M00c9(String_Concat_M00ad(Application_get_StartupPath_M00c7(), "\\", text), text2);
				serverInfo = (ServerInfo)EncryptSupport_ByteArrayToObject_M00ca(array);
				ObjectCollection_Clear_M00cc(ComboBox_get_Items_M00cb(cbServer));
				ListControl_set_DisplayMember_M00cd((ListControl)cbServer, "Display");
				domain = String_Trim_M0099(ServerInfo_get_Domain_M00ce(serverInfo) ?? string.Empty);
				Control_set_Text_M0069((Control)lblDomainValue, domain);
				if (ServerInfo_get_Servers_M00cf(serverInfo) == null || ServerInfo_get_Servers_M00cf(serverInfo).Count == 0)
				{
					throw InvalidDataException_Create_M00d0("The server list is empty.");
				}
				foreach (ServerEndpointInfo item in ServerInfo_get_Servers_M00cf(serverInfo))
				{
					AddEndpoint_M004b(ServerEndpointInfo_get_ServerName_M00d1(item), ServerEndpointInfo_get_IP_M00d2(item), ServerEndpointInfo_get_Port_M00d3(item), ServerEndpointInfo_get_PublicIP_M00d4(item));
				}
				return;
			}
			catch
			{
				MessageBox_Show_M00a2(String_Concat_M00ad("Invalid [", text, "] file format."), "Server information error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				Application_Exit_M00c5();
				return;
			}
		}
		MessageBox_Show_M00a2(String_Concat_M00ad("File [", text, "] not found."), "Server information error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		Application_Exit_M00c5();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	// [behavior 0x0600004b] Adds a valid address/positive-port endpoint, choosing a label and preserving optional Public_IP.
	private void AddEndpoint_M004b(string P_0, string P_1, int P_2, string P_3)
	{
		if (!String_IsNullOrEmpty_M009c(P_1) && P_2 > 0)
		{
			ObjectCollection_Add_M00d6(ComboBox_get_Items_M00cb(cbServer), (object)new EndpointItem
			{
				Display = (String_IsNullOrEmpty_M009c(P_0) ? String_Concat_M00d5((object)P_1, (object)":", (object)P_2) : P_0),
				IP = P_1,
				Port = P_2,
				Public_IP = (P_3 ?? "")
			});
		}
	}

	// [behavior 0x0600004c] Checks the current Windows principal for Administrator membership.
	private bool IsAdministrator_M004c()
	{
		if (WindowsPrincipal_IsInRole_M00d9(WindowsPrincipal_Create_M00d8(WindowsIdentity_GetCurrent_M00d7()), WindowsBuiltInRole.Administrator))
		{
			return true;
		}
		return false;
	}

	// [behavior 0x0600004d] Shows an existing undisposed font-check form, creating it when necessary.
	private void ShowFontCheck_M004d(object P_0, LinkLabelLinkClickedEventArgs P_1)
	{
		if (fontCheckForm == null || Control_get_IsDisposed_M00da((Control)fontCheckForm))
		{
			fontCheckForm = new FontCheckForm();
		}
		Control_Show_M00bb((Control)fontCheckForm);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	// [behavior 0x0600004e] Checks ghosts.mp3 exists and plays it; otherwise shows an audio-file error.
	private void CheckSound_M004e(object P_0, LinkLabelLinkClickedEventArgs P_1)
	{
		if (!File_Exists_M00c8("ghosts.mp3"))
		{
			MessageBox_Show_M009b("Audio file ghosts.mp3 cannot be found!", "Check sound");
		}
		else
		{
			PlayMp3_M004f("ghosts.mp3");
		}
	}

	// [behavior 0x0600004f] Reads the MP3 into memory, constructs an NAudio reader and WaveOut, initializes playback, sets
	// [behavior 0x0600004f] volume to 1 and calls Play. Playback disposal is not visible here.
	public void PlayMp3_M004f(string P_0)
	{
		FileStream fileStream = File_OpenRead_M00db(P_0);
		byte[] array = new byte[Stream_get_Length_M00dc((Stream)fileStream)];
		Stream_Read_M00dd((Stream)fileStream, array, 0, (int)Stream_get_Length_M00dc((Stream)fileStream));
		Stream_Close_M00de((Stream)fileStream);
		Mp3FileReader mp3FileReader = Mp3FileReader_Create_M00e0((Stream)MemoryStream_Create_M00df(array));
		WaveOut waveOut = WaveOut_Create_M00e1();
		WaveOut_Init_M00e2(waveOut, (IWaveProvider)mp3FileReader);
		WaveOut_set_Volume_M00e3(waveOut, 1f);
		WaveOut_Play_M00e4(waveOut);
	}

	// [behavior 0x06000050] Requests application exit and terminates the process with Environment.Exit(0).
	private void Closing_M0050(object P_0, FormClosingEventArgs P_1)
	{
		Application_Exit_M00c5();
		Environment_Exit_M00c6(0);
	}

	static void Container_Dispose_M0051(Container P_0)
	{
		P_0.Dispose();
	}

	static AppSettingsReader AppSettingsReader_Create_M0052()
	{
		return new AppSettingsReader();
	}

	static Button Button_Create_M0053()
	{
		return new Button();
	}

	static TextBox TextBox_Create_M0054()
	{
		return new TextBox();
	}

	static Label Label_Create_M0055()
	{
		return new Label();
	}

	static LinkLabel LinkLabel_Create_M0056()
	{
		return new LinkLabel();
	}

	static ComboBox ComboBox_Create_M0057()
	{
		return new ComboBox();
	}

	static PictureBox PictureBox_Create_M0058()
	{
		return new PictureBox();
	}

	static Panel Panel_Create_M0059()
	{
		return new Panel();
	}

	static void ISupportInitialize_BeginInit_M005a(ISupportInitialize P_0)
	{
		P_0.BeginInit();
	}

	static void Control_SuspendLayout_M005b(Control P_0)
	{
		P_0.SuspendLayout();
	}

	static void Control_SuspendLayout_M005c(Control P_0)
	{
		P_0.SuspendLayout();
	}

	static void Control_set_BackColor_M005d(Control P_0, Color P_1)
	{
		P_0.BackColor = P_1;
	}

	static void Button_set_DialogResult_M005e(Button P_0, DialogResult P_1)
	{
		P_0.DialogResult = P_1;
	}

	static FlatButtonAppearance ButtonBase_get_FlatAppearance_M005f(ButtonBase P_0)
	{
		return P_0.FlatAppearance;
	}

	static void FlatButtonAppearance_set_BorderSize_M0060(FlatButtonAppearance P_0, int P_1)
	{
		P_0.BorderSize = P_1;
	}

	static void ButtonBase_set_FlatStyle_M0061(ButtonBase P_0, FlatStyle P_1)
	{
		P_0.FlatStyle = P_1;
	}

	static Font Font_Create_M0062(string P_0, float P_1, FontStyle P_2)
	{
		return new Font(P_0, P_1, P_2);
	}

	static void Control_set_Font_M0063(Control P_0, Font P_1)
	{
		P_0.Font = P_1;
	}

	static void Control_set_ForeColor_M0064(Control P_0, Color P_1)
	{
		P_0.ForeColor = P_1;
	}

	static void Control_set_Location_M0065(Control P_0, Point P_1)
	{
		P_0.Location = P_1;
	}

	static void Control_set_Name_M0066(Control P_0, string P_1)
	{
		P_0.Name = P_1;
	}

	static void Control_set_Size_M0067(Control P_0, Size P_1)
	{
		P_0.Size = P_1;
	}

	static void Control_set_TabIndex_M0068(Control P_0, int P_1)
	{
		P_0.TabIndex = P_1;
	}

	static void Control_set_Text_M0069(Control P_0, string P_1)
	{
		P_0.Text = P_1;
	}

	static void ButtonBase_set_UseVisualStyleBackColor_M006a(ButtonBase P_0, bool P_1)
	{
		P_0.UseVisualStyleBackColor = P_1;
	}

	static void Control_add_Click_M006b(Control P_0, EventHandler P_1)
	{
		P_0.Click += P_1;
	}

	static void Control_add_MouseEnter_M006c(Control P_0, EventHandler P_1)
	{
		P_0.MouseEnter += P_1;
	}

	static void Control_add_MouseLeave_M006d(Control P_0, EventHandler P_1)
	{
		P_0.MouseLeave += P_1;
	}

	static void TextBoxBase_set_BorderStyle_M006e(TextBoxBase P_0, BorderStyle P_1)
	{
		P_0.BorderStyle = P_1;
	}

	static Font Font_Create_M006f(string P_0, float P_1)
	{
		return new Font(P_0, P_1);
	}

	static void Control_add_Enter_M0070(Control P_0, EventHandler P_1)
	{
		P_0.Enter += P_1;
	}

	static void Control_add_Leave_M0071(Control P_0, EventHandler P_1)
	{
		P_0.Leave += P_1;
	}

	static void TextBox_set_PasswordChar_M0072(TextBox P_0, char P_1)
	{
		P_0.PasswordChar = P_1;
	}

	static void Control_set_AutoSize_M0073(Control P_0, bool P_1)
	{
		P_0.AutoSize = P_1;
	}

	static void FlatButtonAppearance_set_BorderColor_M0074(FlatButtonAppearance P_0, Color P_1)
	{
		P_0.BorderColor = P_1;
	}

	static Type Type_GetTypeFromHandle_M0075(RuntimeTypeHandle P_0)
	{
		return Type.GetTypeFromHandle(P_0);
	}

	static object AppSettingsReader_GetValue_M0076(AppSettingsReader P_0, string P_1, Type P_2)
	{
		return P_0.GetValue(P_1, P_2);
	}

	static void Label_set_TextAlign_M0077(Label P_0, ContentAlignment P_1)
	{
		P_0.TextAlign = P_1;
	}

	static void LinkLabel_set_ActiveLinkColor_M0078(LinkLabel P_0, Color P_1)
	{
		P_0.ActiveLinkColor = P_1;
	}

	static void LinkLabel_set_LinkBehavior_M0079(LinkLabel P_0, LinkBehavior P_1)
	{
		P_0.LinkBehavior = P_1;
	}

	static void LinkLabel_set_LinkColor_M007a(LinkLabel P_0, Color P_1)
	{
		P_0.LinkColor = P_1;
	}

	static void Label_set_TabStop_M007b(Label P_0, bool P_1)
	{
		P_0.TabStop = P_1;
	}

	static void LinkLabel_set_VisitedLinkColor_M007c(LinkLabel P_0, Color P_1)
	{
		P_0.VisitedLinkColor = P_1;
	}

	static void LinkLabel_add_LinkClicked_M007d(LinkLabel P_0, LinkLabelLinkClickedEventHandler P_1)
	{
		P_0.LinkClicked += P_1;
	}

	static void ComboBox_set_FlatStyle_M007e(ComboBox P_0, FlatStyle P_1)
	{
		P_0.FlatStyle = P_1;
	}

	static void ListControl_set_FormattingEnabled_M007f(ListControl P_0, bool P_1)
	{
		P_0.FormattingEnabled = P_1;
	}

	static void PictureBox_set_TabIndex_M0080(PictureBox P_0, int P_1)
	{
		P_0.TabIndex = P_1;
	}

	static void PictureBox_set_TabStop_M0081(PictureBox P_0, bool P_1)
	{
		P_0.TabStop = P_1;
	}

	static void Control_add_Paint_M0082(Control P_0, PaintEventHandler P_1)
	{
		P_0.Paint += P_1;
	}

	static Control.ControlCollection Control_get_Controls_M0083(Control P_0)
	{
		return P_0.Controls;
	}

	static void ControlCollection_Add_M0084(Control.ControlCollection P_0, Control P_1)
	{
		P_0.Add(P_1);
	}

	static void Form_set_AcceptButton_M0085(Form P_0, IButtonControl P_1)
	{
		P_0.AcceptButton = P_1;
	}

	static void Form_set_AutoScaleBaseSize_M0086(Form P_0, Size P_1)
	{
		P_0.AutoScaleBaseSize = P_1;
	}

	static void Form_set_ClientSize_M0087(Form P_0, Size P_1)
	{
		P_0.ClientSize = P_1;
	}

	static Control.ControlCollection Control_get_Controls_M0088(Control P_0)
	{
		return P_0.Controls;
	}

	static void Form_set_FormBorderStyle_M0089(Form P_0, FormBorderStyle P_1)
	{
		P_0.FormBorderStyle = P_1;
	}

	static void Form_set_MaximizeBox_M008a(Form P_0, bool P_1)
	{
		P_0.MaximizeBox = P_1;
	}

	static void Form_set_MinimizeBox_M008b(Form P_0, bool P_1)
	{
		P_0.MinimizeBox = P_1;
	}

	static void Control_set_Name_M008c(Control P_0, string P_1)
	{
		P_0.Name = P_1;
	}

	static void Form_set_StartPosition_M008d(Form P_0, FormStartPosition P_1)
	{
		P_0.StartPosition = P_1;
	}

	static void Form_add_FormClosing_M008e(Form P_0, FormClosingEventHandler P_1)
	{
		P_0.FormClosing += P_1;
	}

	static void Form_add_Load_M008f(Form P_0, EventHandler P_1)
	{
		P_0.Load += P_1;
	}

	static void ISupportInitialize_EndInit_M0090(ISupportInitialize P_0)
	{
		P_0.EndInit();
	}

	static void Control_ResumeLayout_M0091(Control P_0, bool P_1)
	{
		P_0.ResumeLayout(P_1);
	}

	static void Control_PerformLayout_M0092(Control P_0)
	{
		P_0.PerformLayout();
	}

	static void Control_ResumeLayout_M0093(Control P_0, bool P_1)
	{
		P_0.ResumeLayout(P_1);
	}

	static void Control_PerformLayout_M0094(Control P_0)
	{
		P_0.PerformLayout();
	}

	static Graphics PaintEventArgs_get_Graphics_M0095(PaintEventArgs P_0)
	{
		return P_0.Graphics;
	}

	static Control Control_get_Parent_M0096(Control P_0)
	{
		return P_0.Parent;
	}

	static void Control_set_Enabled_M0097(Control P_0, bool P_1)
	{
		P_0.Enabled = P_1;
	}

	static string Control_get_Text_M0098(Control P_0)
	{
		return P_0.Text;
	}

	static string String_Trim_M0099(string P_0)
	{
		return P_0.Trim();
	}

	static bool String_Equals_M009a(string P_0, string P_1)
	{
		return P_0.Equals(P_1);
	}

	static DialogResult MessageBox_Show_M009b(string P_0, string P_1)
	{
		return MessageBox.Show(P_0, P_1);
	}

	static bool String_IsNullOrEmpty_M009c(string P_0)
	{
		return string.IsNullOrEmpty(P_0);
	}

	static object ComboBox_get_SelectedItem_M009d(ComboBox P_0)
	{
		return P_0.SelectedItem;
	}

	static void ServerInfo_set_IP_M009e(ServerInfo P_0, string P_1)
	{
		P_0.IP = P_1;
	}

	static void ServerInfo_set_Port_M009f(ServerInfo P_0, int P_1)
	{
		P_0.Port = P_1;
	}

	static string ServerInfo_get_IP_M00a0(ServerInfo P_0)
	{
		return P_0.IP;
	}

	static int ServerInfo_get_Port_M00a1(ServerInfo P_0)
	{
		return P_0.Port;
	}

	static DialogResult MessageBox_Show_M00a2(string P_0, string P_1, MessageBoxButtons P_2, MessageBoxIcon P_3)
	{
		return MessageBox.Show(P_0, P_1, P_2, P_3);
	}

	static RegisterData RegisterData_Create_M00a3()
	{
		return new RegisterData();
	}

	static string String_ToLower_M00a4(string P_0)
	{
		return P_0.ToLower();
	}

	static void RegisterData_set_Login_M00a5(RegisterData P_0, string P_1)
	{
		P_0.Login = P_1;
	}

	static void RegisterData_set_Password_M00a6(RegisterData P_0, string P_1)
	{
		P_0.Password = P_1;
	}

	static void RegisterData_set_ExamCode_M00a7(RegisterData P_0, string P_1)
	{
		P_0.ExamCode = P_1;
	}

	static string Environment_get_MachineName_M00a8()
	{
		return Environment.MachineName;
	}

	static string String_ToUpper_M00a9(string P_0)
	{
		return P_0.ToUpper();
	}

	static void RegisterData_set_Machine_M00aa(RegisterData P_0, string P_1)
	{
		P_0.Machine = P_1;
	}

	static void RegisterData_set_ClientVersion_M00ab(RegisterData P_0, string P_1)
	{
		P_0.ClientVersion = P_1;
	}

	static EOSData IRemoteServer_ConductExam_M00ac(IRemoteServer P_0, RegisterData P_1)
	{
		return P_0.ConductExam(P_1);
	}

	static string String_Concat_M00ad(string P_0, string P_1, string P_2)
	{
		return P_0 + P_1 + P_2;
	}

	static void Control_Hide_M00ae(Control P_0)
	{
		P_0.Hide();
	}

	static byte[] GZipHelper_DeCompress_M00af(byte[] P_0, int P_1)
	{
		return GZipHelper.DeCompress(P_0, P_1);
	}

	static Assembly Assembly_Load_M00b0(byte[] P_0)
	{
		return Assembly.Load(P_0);
	}

	static Type Assembly_GetType_M00b1(Assembly P_0, string P_1)
	{
		return P_0.GetType(P_1);
	}

	static MethodInfo Type_GetMethod_M00b2(Type P_0, string P_1)
	{
		return P_0.GetMethod(P_1);
	}

	static object MethodBase_Invoke_M00b3(MethodBase P_0, object P_1, object[] P_2)
	{
		return P_0.Invoke(P_1, P_2);
	}

	static void Application_set_UseWaitCursor_M00b4(bool P_0)
	{
		Application.UseWaitCursor = P_0;
	}

	static Cursor Cursors_get_Default_M00b5()
	{
		return Cursors.Default;
	}

	static void Cursor_set_Current_M00b6(Cursor P_0)
	{
		Cursor.Current = P_0;
	}

	static void Control_Show_M00b7(Control P_0)
	{
		P_0.Show();
	}

	static object Activator_CreateInstance_M00b8(Type P_0)
	{
		return Activator.CreateInstance(P_0);
	}

	static void IExamclient_SetExamData_M00b9(IExamclient P_0, EOSData P_1)
	{
		P_0.SetExamData(P_1);
	}

	static void Form_add_FormClosed_M00ba(Form P_0, FormClosedEventHandler P_1)
	{
		P_0.FormClosed += P_1;
	}

	static void Control_Show_M00bb(Control P_0)
	{
		P_0.Show();
	}

	static string Object_ToString_M00bc(object P_0)
	{
		return P_0.ToString();
	}

	static DialogResult MessageBox_Show_M00bd(string P_0)
	{
		return MessageBox.Show(P_0);
	}

	static string String_Concat_M00be(object[] P_0)
	{
		return string.Concat(P_0);
	}

	static object Activator_GetObject_M00bf(Type P_0, string P_1)
	{
		return Activator.GetObject(P_0, P_1);
	}

	static bool IRemoteServer_Ping_M00c0(IRemoteServer P_0)
	{
		return P_0.Ping();
	}

	static string IRemoteServer_GetMinRequiredClientVersion_M00c1(IRemoteServer P_0)
	{
		return P_0.GetMinRequiredClientVersion();
	}

	static int String_get_Length_M00c2(string P_0)
	{
		return P_0.Length;
	}

	static string String_Trim_M00c3(string P_0)
	{
		return P_0.Trim();
	}

	static bool String_Equals_M00c4(string P_0, string P_1, StringComparison P_2)
	{
		return string.Equals(P_0, P_1, P_2);
	}

	static void Application_Exit_M00c5()
	{
		Application.Exit();
	}

	static void Environment_Exit_M00c6(int P_0)
	{
		Environment.Exit(P_0);
	}

	static string Application_get_StartupPath_M00c7()
	{
		return Application.StartupPath;
	}

	static bool File_Exists_M00c8(string P_0)
	{
		return File.Exists(P_0);
	}

	static byte[] EncryptSupport_DecryptQuestions_FromFile_M00c9(string P_0, string P_1)
	{
		return EncryptSupport.DecryptQuestions_FromFile(P_0, P_1);
	}

	static object EncryptSupport_ByteArrayToObject_M00ca(byte[] P_0)
	{
		return EncryptSupport.ByteArrayToObject(P_0);
	}

	static ComboBox.ObjectCollection ComboBox_get_Items_M00cb(ComboBox P_0)
	{
		return P_0.Items;
	}

	static void ObjectCollection_Clear_M00cc(ComboBox.ObjectCollection P_0)
	{
		P_0.Clear();
	}

	static void ListControl_set_DisplayMember_M00cd(ListControl P_0, string P_1)
	{
		P_0.DisplayMember = P_1;
	}

	static string ServerInfo_get_Domain_M00ce(ServerInfo P_0)
	{
		return P_0.Domain;
	}

	static List<ServerEndpointInfo> ServerInfo_get_Servers_M00cf(ServerInfo P_0)
	{
		return P_0.Servers;
	}

	static InvalidDataException InvalidDataException_Create_M00d0(string P_0)
	{
		return new InvalidDataException(P_0);
	}

	static string ServerEndpointInfo_get_ServerName_M00d1(ServerEndpointInfo P_0)
	{
		return P_0.ServerName;
	}

	static string ServerEndpointInfo_get_IP_M00d2(ServerEndpointInfo P_0)
	{
		return P_0.IP;
	}

	static int ServerEndpointInfo_get_Port_M00d3(ServerEndpointInfo P_0)
	{
		return P_0.Port;
	}

	static string ServerEndpointInfo_get_PublicIP_M00d4(ServerEndpointInfo P_0)
	{
		return P_0.PublicIP;
	}

	static string String_Concat_M00d5(object P_0, object P_1, object P_2)
	{
		return string.Concat(P_0, P_1, P_2);
	}

	static int ObjectCollection_Add_M00d6(ComboBox.ObjectCollection P_0, object P_1)
	{
		return P_0.Add(P_1);
	}

	static WindowsIdentity WindowsIdentity_GetCurrent_M00d7()
	{
		return WindowsIdentity.GetCurrent();
	}

	static WindowsPrincipal WindowsPrincipal_Create_M00d8(WindowsIdentity P_0)
	{
		return new WindowsPrincipal(P_0);
	}

	static bool WindowsPrincipal_IsInRole_M00d9(WindowsPrincipal P_0, WindowsBuiltInRole P_1)
	{
		return P_0.IsInRole(P_1);
	}

	static bool Control_get_IsDisposed_M00da(Control P_0)
	{
		return P_0.IsDisposed;
	}

	static FileStream File_OpenRead_M00db(string P_0)
	{
		return File.OpenRead(P_0);
	}

	static long Stream_get_Length_M00dc(Stream P_0)
	{
		return P_0.Length;
	}

	static int Stream_Read_M00dd(Stream P_0, byte[] P_1, int P_2, int P_3)
	{
		return P_0.Read(P_1, P_2, P_3);
	}

	static void Stream_Close_M00de(Stream P_0)
	{
		P_0.Close();
	}

	static MemoryStream MemoryStream_Create_M00df(byte[] P_0)
	{
		return new MemoryStream(P_0);
	}

	static Mp3FileReader Mp3FileReader_Create_M00e0(Stream P_0)
	{
		return new Mp3FileReader(P_0);
	}

	static WaveOut WaveOut_Create_M00e1()
	{
		return new WaveOut();
	}

	static void WaveOut_Init_M00e2(WaveOut P_0, IWaveProvider P_1)
	{
		P_0.Init(P_1);
	}

	static void WaveOut_set_Volume_M00e3(WaveOut P_0, float P_1)
	{
		P_0.Volume = P_1;
	}

	static void WaveOut_Play_M00e4(WaveOut P_0)
	{
		P_0.Play();
	}
}
