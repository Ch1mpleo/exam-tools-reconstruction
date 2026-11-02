// Recovered C#. Descriptive aliases are analyst-assigned; _M suffix is original MethodDef RID.
using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Windows.Forms;

public class RulesForm : Form
{
	private IContainer components;

	private TextBox txtNoiQuy;

	private CheckBox chbRead;

	private Button btnNext;

	private PictureBox PictureBox_F0065;

	private Label Label_F0066;

	private Label Label_F0067;

	private Panel Panel_F0068;

	private Panel Panel_F0069;

	public RulesForm()
	{
		InitializeComponent_M0128();
	}

	// [behavior 0x0600011f] Shows a new login form, then hides the rules form.
	private void NextClick_M011f(object P_0, EventArgs P_1)
	{
		Control_Show_M0129((Control)new AuthenticateForm());
		Control_Hide_M012a((Control)this);
	}

	// [behavior 0x06000120] Enables Next exactly when consent is checked, and changes button colors.
	private void RulesConsentChanged_M0120(object P_0, EventArgs P_1)
	{
		Control_set_Enabled_M012c((Control)btnNext, CheckBox_get_Checked_M012b(chbRead));
		Control_set_BackColor_M012d((Control)btnNext, CheckBox_get_Checked_M012b(chbRead) ? ClientTheme.Color_F005b : ClientTheme.Color_F005a);
		Control_set_ForeColor_M012e((Control)btnNext, CheckBox_get_Checked_M012b(chbRead) ? Color.White : ClientTheme.Color_F0059);
	}

	// [behavior 0x06000121] Delegate the rules-form brand paint event to the shared glyph painter.
	private void PaintBrandGlyph_M0121(object P_0, PaintEventArgs P_1)
	{
		ClientTheme.DrawBrandGlyph_M00f4(PaintEventArgs_get_Graphics_M012f(P_1));
	}

	// [behavior 0x06000122] Apply the Next hover color only if that button is enabled.
	private void NextMouseEnter_M0122(object P_0, EventArgs P_1)
	{
		if (Control_get_Enabled_M0130((Control)btnNext))
		{
			Control_set_BackColor_M012d((Control)btnNext, ClientTheme.Color_F005c);
		}
	}

	// [behavior 0x06000123] Restore the Next normal color only if enabled.
	private void NextMouseLeave_M0123(object P_0, EventArgs P_1)
	{
		if (Control_get_Enabled_M0130((Control)btnNext))
		{
			Control_set_BackColor_M012d((Control)btnNext, ClientTheme.Color_F005b);
		}
	}

	// [behavior 0x06000124] Checks current Windows Administrator-role membership.
	private bool IsAdministrator_M0124()
	{
		if (WindowsPrincipal_IsInRole_M0133(WindowsPrincipal_Create_M0132(WindowsIdentity_GetCurrent_M0131()), WindowsBuiltInRole.Administrator))
		{
			return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	// [behavior 0x06000125] Checks path indicators for running from a temporary/archive location; displays an error and
	// [behavior 0x06000125] requests exit when unsuitable. Also checks administrator membership.
	private void ValidateStartupEnvironment_M0125(object P_0, EventArgs P_1)
	{
		string text = String_ToLower_M0135(Application_get_ExecutablePath_M0134());
		if (String_Contains_M0136(text, "\\temp\\") || String_Contains_M0136(text, "\\appdata\\local\\temp\\") || String_EndsWith_M0137(text, ".zip") || String_EndsWith_M0137(text, ".rar"))
		{
			MessageBox_Show_M0138("Please extract the software to a folder before running it.", "Runtime environment error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			Application_Exit_M0139();
			Environment_Exit_M013a(0);
		}
		if (!IsAdministrator_M0124())
		{
			MessageBox_Show_M0138("You must login Windows as System Administrator or Run [EOS Client] as Administrator.", "Run as Administrator!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			Application_Exit_M0139();
		}
	}

	// [behavior 0x06000126] Empty handler: no instructions with application effects.
	private void EmptyHandler_M0126(object P_0, EventArgs P_1)
	{
	}

	// [behavior 0x06000127] Dispose rules-form components when requested, then base.Dispose.
	protected override void Dispose(bool P_0)
	{
		if (P_0 && components != null)
		{
			IDisposable_Dispose_M013b((IDisposable)components);
		}
		base.Dispose(P_0);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	// [behavior 0x06000128] Constructs the rules UI and loads the Vietnamese rules text from resources; connects
	// [behavior 0x06000128] consent/Next/load handlers.
	private void InitializeComponent_M0128()
	{
		ComponentResourceManager componentResourceManager = ComponentResourceManager_Create_M013d(Type_GetTypeFromHandle_M013c(typeof(RulesForm).TypeHandle));
		txtNoiQuy = TextBox_Create_M013e();
		chbRead = CheckBox_Create_M013f();
		btnNext = Button_Create_M0140();
		PictureBox_F0065 = PictureBox_Create_M0141();
		Label_F0066 = Label_Create_M0142();
		Label_F0067 = Label_Create_M0142();
		Panel_F0068 = Panel_Create_M0143();
		Panel_F0069 = Panel_Create_M0143();
		Control_SuspendLayout_M0144((Control)this);
		Control_set_BackColor_M012d((Control)Panel_F0068, ClientTheme.Color_F005b);
		Control_set_Location_M0145((Control)Panel_F0068, new Point(0, 0));
		Control_set_Size_M0146((Control)Panel_F0068, new Size(1040, 3));
		Control_set_BackColor_M012d((Control)PictureBox_F0065, ClientTheme.Color_F0055);
		Control_set_Location_M0145((Control)PictureBox_F0065, new Point(40, 26));
		Control_set_Size_M0146((Control)PictureBox_F0065, new Size(22, 26));
		Control_add_Paint_M0147((Control)PictureBox_F0065, (PaintEventHandler)PaintBrandGlyph_M0121);
		Control_set_AutoSize_M0148((Control)Label_F0066, true);
		Control_set_BackColor_M012d((Control)Label_F0066, Color.Transparent);
		Control_set_Font_M014a((Control)Label_F0066, Font_Create_M0149("Georgia", 17f, FontStyle.Regular));
		Control_set_ForeColor_M012e((Control)Label_F0066, ClientTheme.Color_F0057);
		Control_set_Location_M0145((Control)Label_F0066, new Point(68, 21));
		Control_set_Text_M014b((Control)Label_F0066, "EOS");
		Control_set_AutoSize_M0148((Control)Label_F0067, true);
		Control_set_BackColor_M012d((Control)Label_F0067, Color.Transparent);
		Control_set_Font_M014a((Control)Label_F0067, Font_Create_M0149("Segoe UI", 8f, FontStyle.Bold));
		Control_set_ForeColor_M012e((Control)Label_F0067, ClientTheme.Color_F005b);
		Control_set_Location_M0145((Control)Label_F0067, new Point(41, 57));
		Control_set_Text_M014b((Control)Label_F0067, ClientTheme.LetterSpaceUppercase_M00f3("N\u1ed9i quy k\u1ef3 thi"));
		Control_set_BackColor_M012d((Control)Panel_F0069, ClientTheme.Color_F005a);
		Control_set_Location_M0145((Control)Panel_F0069, new Point(40, 84));
		Control_set_Size_M0146((Control)Panel_F0069, new Size(960, 1));
		Control_set_BackColor_M012d((Control)txtNoiQuy, ClientTheme.Color_F0056);
		TextBoxBase_set_BorderStyle_M014c((TextBoxBase)txtNoiQuy, BorderStyle.None);
		Control_set_Font_M014a((Control)txtNoiQuy, Font_Create_M014d("Segoe UI", 11f, FontStyle.Regular, GraphicsUnit.Point, (byte)0));
		Control_set_ForeColor_M012e((Control)txtNoiQuy, ClientTheme.Color_F0057);
		TextBoxBase_set_Multiline_M014e((TextBoxBase)txtNoiQuy, true);
		Control_set_Name_M014f((Control)txtNoiQuy, "txtNoiQuy");
		TextBoxBase_set_ReadOnly_M0150((TextBoxBase)txtNoiQuy, true);
		TextBox_set_ScrollBars_M0151(txtNoiQuy, ScrollBars.Vertical);
		Control_set_TabIndex_M0152((Control)txtNoiQuy, 2);
		Control_set_Text_M014b((Control)txtNoiQuy, ResourceManager_GetString_M0153((ResourceManager)componentResourceManager, "txtNoiQuy.Text"));
		Control_add_TextChanged_M0154((Control)txtNoiQuy, (EventHandler)EmptyHandler_M0126);
		Panel panel = ClientTheme.WrapContent_M00f6(txtNoiQuy, 18);
		Control_set_Location_M0145((Control)panel, new Point(40, 96));
		Control_set_Size_M0146((Control)panel, new Size(960, 436));
		Control_set_Anchor_M0155((Control)panel, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
		Control_set_Anchor_M0155((Control)chbRead, AnchorStyles.Bottom | AnchorStyles.Left);
		Control_set_AutoSize_M0148((Control)chbRead, true);
		Control_set_BackColor_M012d((Control)chbRead, Color.Transparent);
		Control_set_Font_M014a((Control)chbRead, Font_Create_M0149("Segoe UI", 9.5f, FontStyle.Regular));
		Control_set_ForeColor_M012e((Control)chbRead, ClientTheme.Color_F0057);
		Control_set_Location_M0145((Control)chbRead, new Point(40, 550));
		Control_set_Name_M014f((Control)chbRead, "chbRead");
		Control_set_TabIndex_M0152((Control)chbRead, 1);
		Control_set_Text_M014b((Control)chbRead, "T\u00f4i \u0111\u00e3 \u0111\u1ecdc, hi\u1ec3u r\u00f5 v\u00e0 cam k\u1ebft tu\u00e2n th\u1ee7 N\u1ed9i quy k\u1ef3 thi");
		ButtonBase_set_UseVisualStyleBackColor_M0156((ButtonBase)chbRead, false);
		CheckBox_add_CheckedChanged_M0157(chbRead, (EventHandler)RulesConsentChanged_M0120);
		Control_set_Anchor_M0155((Control)btnNext, AnchorStyles.Bottom | AnchorStyles.Right);
		Control_set_Enabled_M012c((Control)btnNext, false);
		ButtonBase_set_FlatStyle_M0158((ButtonBase)btnNext, FlatStyle.Flat);
		FlatButtonAppearance_set_BorderSize_M015a(ButtonBase_get_FlatAppearance_M0159((ButtonBase)btnNext), 0);
		Control_set_Font_M014a((Control)btnNext, Font_Create_M0149("Segoe UI", 9.75f, FontStyle.Bold));
		Control_set_BackColor_M012d((Control)btnNext, ClientTheme.Color_F005a);
		Control_set_ForeColor_M012e((Control)btnNext, ClientTheme.Color_F0059);
		Control_set_Location_M0145((Control)btnNext, new Point(860, 544));
		Control_set_Name_M014f((Control)btnNext, "btnNext");
		Control_set_Size_M0146((Control)btnNext, new Size(140, 38));
		Control_set_TabIndex_M0152((Control)btnNext, 3);
		Control_set_Text_M014b((Control)btnNext, "Next");
		ButtonBase_set_UseVisualStyleBackColor_M0156((ButtonBase)btnNext, false);
		Control_add_MouseEnter_M015b((Control)btnNext, (EventHandler)NextMouseEnter_M0122);
		Control_add_MouseLeave_M015c((Control)btnNext, (EventHandler)NextMouseLeave_M0123);
		Control_add_Click_M015d((Control)btnNext, (EventHandler)NextClick_M011f);
		ContainerControl_set_AutoScaleDimensions_M015e((ContainerControl)this, new SizeF(6f, 13f));
		ContainerControl_set_AutoScaleMode_M015f((ContainerControl)this, AutoScaleMode.Font);
		Control_set_BackColor_M012d((Control)this, ClientTheme.Color_F0055);
		Form_set_ClientSize_M0160((Form)this, new Size(1040, 620));
		ControlCollection_Add_M0162(Control_get_Controls_M0161((Control)this), (Control)Panel_F0068);
		ControlCollection_Add_M0162(Control_get_Controls_M0161((Control)this), (Control)PictureBox_F0065);
		ControlCollection_Add_M0162(Control_get_Controls_M0161((Control)this), (Control)Label_F0066);
		ControlCollection_Add_M0162(Control_get_Controls_M0161((Control)this), (Control)Label_F0067);
		ControlCollection_Add_M0162(Control_get_Controls_M0161((Control)this), (Control)Panel_F0069);
		ControlCollection_Add_M0162(Control_get_Controls_M0161((Control)this), (Control)panel);
		ControlCollection_Add_M0162(Control_get_Controls_M0161((Control)this), (Control)chbRead);
		ControlCollection_Add_M0162(Control_get_Controls_M0161((Control)this), (Control)btnNext);
		Form_set_FormBorderStyle_M0163((Form)this, FormBorderStyle.FixedSingle);
		Form_set_MaximizeBox_M0164((Form)this, false);
		Form_set_MinimizeBox_M0165((Form)this, false);
		Control_set_Name_M0166((Control)this, "frmAnnoucement");
		Form_set_StartPosition_M0167((Form)this, FormStartPosition.CenterScreen);
		Control_set_Text_M014b((Control)this, "EOS \u2014 Exam Rules");
		Form_add_Load_M0168((Form)this, (EventHandler)ValidateStartupEnvironment_M0125);
		Control_ResumeLayout_M0169((Control)this, false);
		Control_PerformLayout_M016a((Control)this);
	}

	static void Control_Show_M0129(Control P_0)
	{
		P_0.Show();
	}

	static void Control_Hide_M012a(Control P_0)
	{
		P_0.Hide();
	}

	static bool CheckBox_get_Checked_M012b(CheckBox P_0)
	{
		return P_0.Checked;
	}

	static void Control_set_Enabled_M012c(Control P_0, bool P_1)
	{
		P_0.Enabled = P_1;
	}

	static void Control_set_BackColor_M012d(Control P_0, Color P_1)
	{
		P_0.BackColor = P_1;
	}

	static void Control_set_ForeColor_M012e(Control P_0, Color P_1)
	{
		P_0.ForeColor = P_1;
	}

	static Graphics PaintEventArgs_get_Graphics_M012f(PaintEventArgs P_0)
	{
		return P_0.Graphics;
	}

	static bool Control_get_Enabled_M0130(Control P_0)
	{
		return P_0.Enabled;
	}

	static WindowsIdentity WindowsIdentity_GetCurrent_M0131()
	{
		return WindowsIdentity.GetCurrent();
	}

	static WindowsPrincipal WindowsPrincipal_Create_M0132(WindowsIdentity P_0)
	{
		return new WindowsPrincipal(P_0);
	}

	static bool WindowsPrincipal_IsInRole_M0133(WindowsPrincipal P_0, WindowsBuiltInRole P_1)
	{
		return P_0.IsInRole(P_1);
	}

	static string Application_get_ExecutablePath_M0134()
	{
		return Application.ExecutablePath;
	}

	static string String_ToLower_M0135(string P_0)
	{
		return P_0.ToLower();
	}

	static bool String_Contains_M0136(string P_0, string P_1)
	{
		return P_0.Contains(P_1);
	}

	static bool String_EndsWith_M0137(string P_0, string P_1)
	{
		return P_0.EndsWith(P_1);
	}

	static DialogResult MessageBox_Show_M0138(string P_0, string P_1, MessageBoxButtons P_2, MessageBoxIcon P_3)
	{
		return MessageBox.Show(P_0, P_1, P_2, P_3);
	}

	static void Application_Exit_M0139()
	{
		Application.Exit();
	}

	static void Environment_Exit_M013a(int P_0)
	{
		Environment.Exit(P_0);
	}

	static void IDisposable_Dispose_M013b(IDisposable P_0)
	{
		P_0.Dispose();
	}

	static Type Type_GetTypeFromHandle_M013c(RuntimeTypeHandle P_0)
	{
		return Type.GetTypeFromHandle(P_0);
	}

	static ComponentResourceManager ComponentResourceManager_Create_M013d(Type P_0)
	{
		return new ComponentResourceManager(P_0);
	}

	static TextBox TextBox_Create_M013e()
	{
		return new TextBox();
	}

	static CheckBox CheckBox_Create_M013f()
	{
		return new CheckBox();
	}

	static Button Button_Create_M0140()
	{
		return new Button();
	}

	static PictureBox PictureBox_Create_M0141()
	{
		return new PictureBox();
	}

	static Label Label_Create_M0142()
	{
		return new Label();
	}

	static Panel Panel_Create_M0143()
	{
		return new Panel();
	}

	static void Control_SuspendLayout_M0144(Control P_0)
	{
		P_0.SuspendLayout();
	}

	static void Control_set_Location_M0145(Control P_0, Point P_1)
	{
		P_0.Location = P_1;
	}

	static void Control_set_Size_M0146(Control P_0, Size P_1)
	{
		P_0.Size = P_1;
	}

	static void Control_add_Paint_M0147(Control P_0, PaintEventHandler P_1)
	{
		P_0.Paint += P_1;
	}

	static void Control_set_AutoSize_M0148(Control P_0, bool P_1)
	{
		P_0.AutoSize = P_1;
	}

	static Font Font_Create_M0149(string P_0, float P_1, FontStyle P_2)
	{
		return new Font(P_0, P_1, P_2);
	}

	static void Control_set_Font_M014a(Control P_0, Font P_1)
	{
		P_0.Font = P_1;
	}

	static void Control_set_Text_M014b(Control P_0, string P_1)
	{
		P_0.Text = P_1;
	}

	static void TextBoxBase_set_BorderStyle_M014c(TextBoxBase P_0, BorderStyle P_1)
	{
		P_0.BorderStyle = P_1;
	}

	static Font Font_Create_M014d(string P_0, float P_1, FontStyle P_2, GraphicsUnit P_3, byte P_4)
	{
		return new Font(P_0, P_1, P_2, P_3, P_4);
	}

	static void TextBoxBase_set_Multiline_M014e(TextBoxBase P_0, bool P_1)
	{
		P_0.Multiline = P_1;
	}

	static void Control_set_Name_M014f(Control P_0, string P_1)
	{
		P_0.Name = P_1;
	}

	static void TextBoxBase_set_ReadOnly_M0150(TextBoxBase P_0, bool P_1)
	{
		P_0.ReadOnly = P_1;
	}

	static void TextBox_set_ScrollBars_M0151(TextBox P_0, ScrollBars P_1)
	{
		P_0.ScrollBars = P_1;
	}

	static void Control_set_TabIndex_M0152(Control P_0, int P_1)
	{
		P_0.TabIndex = P_1;
	}

	static string ResourceManager_GetString_M0153(ResourceManager P_0, string P_1)
	{
		return P_0.GetString(P_1);
	}

	static void Control_add_TextChanged_M0154(Control P_0, EventHandler P_1)
	{
		P_0.TextChanged += P_1;
	}

	static void Control_set_Anchor_M0155(Control P_0, AnchorStyles P_1)
	{
		P_0.Anchor = P_1;
	}

	static void ButtonBase_set_UseVisualStyleBackColor_M0156(ButtonBase P_0, bool P_1)
	{
		P_0.UseVisualStyleBackColor = P_1;
	}

	static void CheckBox_add_CheckedChanged_M0157(CheckBox P_0, EventHandler P_1)
	{
		P_0.CheckedChanged += P_1;
	}

	static void ButtonBase_set_FlatStyle_M0158(ButtonBase P_0, FlatStyle P_1)
	{
		P_0.FlatStyle = P_1;
	}

	static FlatButtonAppearance ButtonBase_get_FlatAppearance_M0159(ButtonBase P_0)
	{
		return P_0.FlatAppearance;
	}

	static void FlatButtonAppearance_set_BorderSize_M015a(FlatButtonAppearance P_0, int P_1)
	{
		P_0.BorderSize = P_1;
	}

	static void Control_add_MouseEnter_M015b(Control P_0, EventHandler P_1)
	{
		P_0.MouseEnter += P_1;
	}

	static void Control_add_MouseLeave_M015c(Control P_0, EventHandler P_1)
	{
		P_0.MouseLeave += P_1;
	}

	static void Control_add_Click_M015d(Control P_0, EventHandler P_1)
	{
		P_0.Click += P_1;
	}

	static void ContainerControl_set_AutoScaleDimensions_M015e(ContainerControl P_0, SizeF P_1)
	{
		P_0.AutoScaleDimensions = P_1;
	}

	static void ContainerControl_set_AutoScaleMode_M015f(ContainerControl P_0, AutoScaleMode P_1)
	{
		P_0.AutoScaleMode = P_1;
	}

	static void Form_set_ClientSize_M0160(Form P_0, Size P_1)
	{
		P_0.ClientSize = P_1;
	}

	static Control.ControlCollection Control_get_Controls_M0161(Control P_0)
	{
		return P_0.Controls;
	}

	static void ControlCollection_Add_M0162(Control.ControlCollection P_0, Control P_1)
	{
		P_0.Add(P_1);
	}

	static void Form_set_FormBorderStyle_M0163(Form P_0, FormBorderStyle P_1)
	{
		P_0.FormBorderStyle = P_1;
	}

	static void Form_set_MaximizeBox_M0164(Form P_0, bool P_1)
	{
		P_0.MaximizeBox = P_1;
	}

	static void Form_set_MinimizeBox_M0165(Form P_0, bool P_1)
	{
		P_0.MinimizeBox = P_1;
	}

	static void Control_set_Name_M0166(Control P_0, string P_1)
	{
		P_0.Name = P_1;
	}

	static void Form_set_StartPosition_M0167(Form P_0, FormStartPosition P_1)
	{
		P_0.StartPosition = P_1;
	}

	static void Form_add_Load_M0168(Form P_0, EventHandler P_1)
	{
		P_0.Load += P_1;
	}

	static void Control_ResumeLayout_M0169(Control P_0, bool P_1)
	{
		P_0.ResumeLayout(P_1);
	}

	static void Control_PerformLayout_M016a(Control P_0)
	{
		P_0.PerformLayout();
	}
}
