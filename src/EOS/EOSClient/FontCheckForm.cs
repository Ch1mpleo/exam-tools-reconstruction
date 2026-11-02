// Recovered C#. Descriptive aliases are analyst-assigned; _M suffix is original MethodDef RID.
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

public class FontCheckForm : Form
{
	private IContainer components;

	private Button btnClose;

	private TextBox txtFontGuide;

	private Label lblTestDisplay;

	public FontCheckForm()
	{
		InitializeComponent_M0170();
	}

	// [behavior 0x0600016c] Creates a font by name, compares its resolved name with the requested name, and disposes it.
	private bool FontResolvesExactly_M016c(string P_0)
	{
		Font font = Font_Create_M0171(P_0, 12f, FontStyle.Regular);
		bool result;
		try
		{
			result = (String_Equals_M0173(Font_get_Name_M0172(font), P_0) ? true : false);
		}
		finally
		{
			if (font != null)
			{
				IDisposable_Dispose_M0174((IDisposable)font);
			}
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	// [behavior 0x0600016d] Enumerates installed font families, checks named Chinese/Japanese fonts and renders a diagnostic
	// [behavior 0x0600016d] report.
	private void CheckInstalledFonts_M016d(object P_0, EventArgs P_1)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		FontFamily[] array = FontCollection_get_Families_M0176((FontCollection)InstalledFontCollection_Create_M0175());
		for (int i = 0; i < array.Length; i++)
		{
			string text = String_ToUpper_M0179(String_Trim_M0178(FontFamily_GetName_M0177(array[i], 0)));
			if (String_StartsWith_M017b(text, String_ToUpper_M017a("SimSun")))
			{
				flag = true;
			}
			if (String_StartsWith_M017b(text, String_ToUpper_M017a("KaiTi")))
			{
				flag2 = true;
			}
			if (String_StartsWith_M017b(text, String_ToUpper_M017a("Ms Mincho")))
			{
				flag3 = true;
			}
			if (String_StartsWith_M017b(text, String_ToUpper_M017a("HGSeikai")))
			{
				flag4 = true;
			}
			if (String_StartsWith_M017b(text, String_ToUpper_M017a("NtMotoya")))
			{
				flag5 = true;
			}
		}
		string text2 = "CHECK FONT RESULT:\r\n\r\n";
		string text3 = "SimSun";
		text2 = ((!flag) ? String_Concat_M017c(text2, "Chinese font ('", text3, "') : NOT FOUND.\r\n\r\n") : String_Concat_M017c(text2, "Chinese font ('", text3, "') : OK.\r\n\r\n"));
		string text4 = "KaiTi";
		text2 = ((!flag2) ? String_Concat_M017c(text2, "Chinese font ('", text4, "') : NOT FOUND.\r\n\r\n") : String_Concat_M017c(text2, "Chinese font ('", text4, "') : OK.\r\n\r\n"));
		string text5 = "MS Mincho";
		text2 = (flag3 ? String_Concat_M017c(text2, "Japanese font 1 ('", text5, "') : OK.\r\n\r\n") : String_Concat_M017c(text2, "Japanese font 1 ('", text5, "') :  NOT FOUND.\r\n\r\n"));
		text5 = "HGSeikaishotaiPRO";
		text2 = ((!flag4) ? String_Concat_M017c(text2, "Japanese font 2 ('", text5, "') :  NOT FOUND.\r\n\r\n") : String_Concat_M017c(text2, "Japanese font 2 ('", text5, "') : OK.\r\n\r\n"));
		text5 = "NtMotoya Kyotai";
		text2 = (flag5 ? String_Concat_M017c(text2, "Japanese font 3 ('", text5, "') : OK.\r\n\r\n") : String_Concat_M017c(text2, "Japanese font 3 ('", text5, "') :  NOT FOUND.\r\n\r\n"));
		if (!flag || !flag3 || !flag2 || !flag4 || !flag5)
		{
			text2 = String_Concat_M017d(text2, "\r\n\r\nINSTALLING FONTS ON Windows:\r\n\r\nThere are several ways to install fonts on Windows.\r\nKeep in mind that you must be an Administrator on the target machine to install fonts.\r\n\r\n - Download the font.\r\n - Double-click on a font file to open the font preview and select 'Install'.\r\n\r\nOR\r\n\r\n - Right-click on a font file, and then select 'Install'.");
		}
		Control_set_Text_M017e((Control)txtFontGuide, text2);
	}

	// [behavior 0x0600016e] Closes the font-check form.
	private void CloseClick_M016e(object P_0, EventArgs P_1)
	{
		Form_Close_M017f((Form)this);
	}

	// [behavior 0x0600016f] Dispose font-check-form components when requested, then base.Dispose.
	protected override void Dispose(bool P_0)
	{
		if (P_0 && components != null)
		{
			IDisposable_Dispose_M0174((IDisposable)components);
		}
		base.Dispose(P_0);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	// [behavior 0x06000170] Constructs the font-check utility controls and event connections.
	private void InitializeComponent_M0170()
	{
		btnClose = Button_Create_M0180();
		txtFontGuide = TextBox_Create_M0181();
		lblTestDisplay = Label_Create_M0182();
		Control_SuspendLayout_M0183((Control)this);
		Control_set_Anchor_M0184((Control)btnClose, AnchorStyles.Bottom | AnchorStyles.Right);
		Control_set_Location_M0185((Control)btnClose, new Point(628, 480));
		Control_set_Name_M0186((Control)btnClose, "btnClose");
		Control_set_Size_M0187((Control)btnClose, new Size(75, 23));
		Control_set_TabIndex_M0188((Control)btnClose, 0);
		Control_set_Text_M017e((Control)btnClose, "Close");
		ButtonBase_set_UseVisualStyleBackColor_M0189((ButtonBase)btnClose, true);
		Control_add_Click_M018a((Control)btnClose, (EventHandler)CloseClick_M016e);
		Control_set_Anchor_M0184((Control)txtFontGuide, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
		Control_set_BackColor_M018b((Control)txtFontGuide, Color.White);
		Control_set_Font_M018d((Control)txtFontGuide, Font_Create_M018c("Microsoft Sans Serif", 9.75f, FontStyle.Regular, GraphicsUnit.Point, (byte)0));
		Control_set_Location_M0185((Control)txtFontGuide, new Point(12, 12));
		TextBoxBase_set_Multiline_M018e((TextBoxBase)txtFontGuide, true);
		Control_set_Name_M0186((Control)txtFontGuide, "txtFontGuide");
		TextBoxBase_set_ReadOnly_M018f((TextBoxBase)txtFontGuide, true);
		TextBox_set_ScrollBars_M0190(txtFontGuide, ScrollBars.Vertical);
		Control_set_Size_M0187((Control)txtFontGuide, new Size(691, 448));
		Control_set_TabIndex_M0188((Control)txtFontGuide, 1);
		Control_set_Anchor_M0184((Control)lblTestDisplay, AnchorStyles.Bottom | AnchorStyles.Left);
		Control_set_AutoSize_M0191((Control)lblTestDisplay, true);
		Control_set_Font_M018d((Control)lblTestDisplay, Font_Create_M018c("MS Mincho", 9.75f, FontStyle.Regular, GraphicsUnit.Point, (byte)0));
		Control_set_Location_M0185((Control)lblTestDisplay, new Point(9, 480));
		Control_set_Name_M0186((Control)lblTestDisplay, "lblTestDisplay");
		Control_set_Size_M0187((Control)lblTestDisplay, new Size(173, 39));
		Control_set_TabIndex_M0188((Control)lblTestDisplay, 2);
		Control_set_Text_M017e((Control)lblTestDisplay, "\u30d9\u30c8\u30ca\u30e0 (in Japanese)\r\n\u8d8a\u5357 (in Chinese)\r\nVi\u1ec7t Nam (in Vietnamese)\r\n");
		ContainerControl_set_AutoScaleDimensions_M0192((ContainerControl)this, new SizeF(6f, 13f));
		ContainerControl_set_AutoScaleMode_M0193((ContainerControl)this, AutoScaleMode.Font);
		Form_set_ClientSize_M0194((Form)this, new Size(715, 532));
		ControlCollection_Add_M0196(Control_get_Controls_M0195((Control)this), (Control)lblTestDisplay);
		ControlCollection_Add_M0196(Control_get_Controls_M0195((Control)this), (Control)txtFontGuide);
		ControlCollection_Add_M0196(Control_get_Controls_M0195((Control)this), (Control)btnClose);
		Control_set_Name_M0197((Control)this, "frmCheckFont");
		Control_set_Text_M017e((Control)this, "Check fonts for Japanese/Chinese exam.");
		Form_add_Load_M0198((Form)this, (EventHandler)CheckInstalledFonts_M016d);
		Control_ResumeLayout_M0199((Control)this, false);
		Control_PerformLayout_M019a((Control)this);
	}

	static Font Font_Create_M0171(string P_0, float P_1, FontStyle P_2)
	{
		return new Font(P_0, P_1, P_2);
	}

	static string Font_get_Name_M0172(Font P_0)
	{
		return P_0.Name;
	}

	static bool String_Equals_M0173(string P_0, string P_1)
	{
		return P_0.Equals(P_1);
	}

	static void IDisposable_Dispose_M0174(IDisposable P_0)
	{
		P_0.Dispose();
	}

	static InstalledFontCollection InstalledFontCollection_Create_M0175()
	{
		return new InstalledFontCollection();
	}

	static FontFamily[] FontCollection_get_Families_M0176(FontCollection P_0)
	{
		return P_0.Families;
	}

	static string FontFamily_GetName_M0177(FontFamily P_0, int P_1)
	{
		return P_0.GetName(P_1);
	}

	static string String_Trim_M0178(string P_0)
	{
		return P_0.Trim();
	}

	static string String_ToUpper_M0179(string P_0)
	{
		return P_0.ToUpper();
	}

	static string String_ToUpper_M017a(string P_0)
	{
		return P_0.ToUpper();
	}

	static bool String_StartsWith_M017b(string P_0, string P_1)
	{
		return P_0.StartsWith(P_1);
	}

	static string String_Concat_M017c(string P_0, string P_1, string P_2, string P_3)
	{
		return P_0 + P_1 + P_2 + P_3;
	}

	static string String_Concat_M017d(string P_0, string P_1)
	{
		return P_0 + P_1;
	}

	static void Control_set_Text_M017e(Control P_0, string P_1)
	{
		P_0.Text = P_1;
	}

	static void Form_Close_M017f(Form P_0)
	{
		P_0.Close();
	}

	static Button Button_Create_M0180()
	{
		return new Button();
	}

	static TextBox TextBox_Create_M0181()
	{
		return new TextBox();
	}

	static Label Label_Create_M0182()
	{
		return new Label();
	}

	static void Control_SuspendLayout_M0183(Control P_0)
	{
		P_0.SuspendLayout();
	}

	static void Control_set_Anchor_M0184(Control P_0, AnchorStyles P_1)
	{
		P_0.Anchor = P_1;
	}

	static void Control_set_Location_M0185(Control P_0, Point P_1)
	{
		P_0.Location = P_1;
	}

	static void Control_set_Name_M0186(Control P_0, string P_1)
	{
		P_0.Name = P_1;
	}

	static void Control_set_Size_M0187(Control P_0, Size P_1)
	{
		P_0.Size = P_1;
	}

	static void Control_set_TabIndex_M0188(Control P_0, int P_1)
	{
		P_0.TabIndex = P_1;
	}

	static void ButtonBase_set_UseVisualStyleBackColor_M0189(ButtonBase P_0, bool P_1)
	{
		P_0.UseVisualStyleBackColor = P_1;
	}

	static void Control_add_Click_M018a(Control P_0, EventHandler P_1)
	{
		P_0.Click += P_1;
	}

	static void Control_set_BackColor_M018b(Control P_0, Color P_1)
	{
		P_0.BackColor = P_1;
	}

	static Font Font_Create_M018c(string P_0, float P_1, FontStyle P_2, GraphicsUnit P_3, byte P_4)
	{
		return new Font(P_0, P_1, P_2, P_3, P_4);
	}

	static void Control_set_Font_M018d(Control P_0, Font P_1)
	{
		P_0.Font = P_1;
	}

	static void TextBoxBase_set_Multiline_M018e(TextBoxBase P_0, bool P_1)
	{
		P_0.Multiline = P_1;
	}

	static void TextBoxBase_set_ReadOnly_M018f(TextBoxBase P_0, bool P_1)
	{
		P_0.ReadOnly = P_1;
	}

	static void TextBox_set_ScrollBars_M0190(TextBox P_0, ScrollBars P_1)
	{
		P_0.ScrollBars = P_1;
	}

	static void Control_set_AutoSize_M0191(Control P_0, bool P_1)
	{
		P_0.AutoSize = P_1;
	}

	static void ContainerControl_set_AutoScaleDimensions_M0192(ContainerControl P_0, SizeF P_1)
	{
		P_0.AutoScaleDimensions = P_1;
	}

	static void ContainerControl_set_AutoScaleMode_M0193(ContainerControl P_0, AutoScaleMode P_1)
	{
		P_0.AutoScaleMode = P_1;
	}

	static void Form_set_ClientSize_M0194(Form P_0, Size P_1)
	{
		P_0.ClientSize = P_1;
	}

	static Control.ControlCollection Control_get_Controls_M0195(Control P_0)
	{
		return P_0.Controls;
	}

	static void ControlCollection_Add_M0196(Control.ControlCollection P_0, Control P_1)
	{
		P_0.Add(P_1);
	}

	static void Control_set_Name_M0197(Control P_0, string P_1)
	{
		P_0.Name = P_1;
	}

	static void Form_add_Load_M0198(Form P_0, EventHandler P_1)
	{
		P_0.Load += P_1;
	}

	static void Control_ResumeLayout_M0199(Control P_0, bool P_1)
	{
		P_0.ResumeLayout(P_1);
	}

	static void Control_PerformLayout_M019a(Control P_0)
	{
		P_0.PerformLayout();
	}
}
