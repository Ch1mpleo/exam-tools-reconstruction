// Recovered C#. Descriptive aliases are analyst-assigned; _M suffix is original MethodDef RID.
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;

internal static class ClientTheme
{
	[CompilerGenerated]
	private sealed class FieldFocusHandlers
	{
		public Panel Panel_F0060;

		internal void Type_0011___________________________________________M011b(object P_0, EventArgs P_1)
		{
			Control_set_BackColor_M011d((Control)Panel_F0060, Color_F005b);
		}

		internal void Type_0011___________________________________________M011c(object P_0, EventArgs P_1)
		{
			Control_set_BackColor_M011d((Control)Panel_F0060, Color_F005a);
		}

		internal static void Control_set_BackColor_M011d(Control P_0, Color P_1)
		{
			P_0.BackColor = P_1;
		}
	}

	public static readonly Color Color_F0055 = Color.FromArgb(237, 238, 233);

	public static readonly Color Color_F0056 = Color.FromArgb(248, 249, 245);

	public static readonly Color Color_F0057 = Color.FromArgb(27, 36, 48);

	public static readonly Color Color_F0058 = Color.FromArgb(87, 98, 109);

	public static readonly Color Color_F0059 = Color.FromArgb(125, 133, 140);

	public static readonly Color Color_F005a = Color.FromArgb(215, 217, 210);

	public static readonly Color Color_F005b = Color.FromArgb(233, 30, 99);

	public static readonly Color Color_F005c = Color.FromArgb(173, 20, 87);

	public static readonly Color Color_F005d = Color.FromArgb(27, 77, 139);

	public static readonly Color Color_F005e = Color.FromArgb(225, 232, 240);

	public static readonly Color Color_F005f = Color.FromArgb(185, 198, 214);

	// [behavior 0x060000f3] Uppercases the string and inserts one space between successive characters.
	public static string LetterSpaceUppercase_M00f3(string P_0)
	{
		string text = String_ToUpperInvariant_M00f9(P_0);
		StringBuilder stringBuilder = StringBuilder_Create_M00fa();
		for (int i = 0; i < String_get_Length_M00fd(text); i++)
		{
			if (i > 0)
			{
				StringBuilder_Append_M00fb(stringBuilder, ' ');
			}
			StringBuilder_Append_M00fb(stringBuilder, String_get_Chars_M00fc(text, i));
		}
		return Object_ToString_M00fe((object)stringBuilder);
	}

	// [behavior 0x060000f4] Draws the brand mark using Bezier paths, a pen and a brush, disposing drawing objects.
	public static void DrawBrandGlyph_M00f4(Graphics P_0)
	{
		Graphics_set_SmoothingMode_M00ff(P_0, SmoothingMode.AntiAlias);
		GraphicsPath graphicsPath = GraphicsPath_Create_M0100();
		try
		{
			GraphicsPath_AddBezier_M0101(graphicsPath, new PointF(11f, 1f), new PointF(11f, 1f), new PointF(4f, 8.5f), new PointF(4f, 15.5f));
			GraphicsPath_AddBezier_M0101(graphicsPath, new PointF(4f, 15.5f), new PointF(4f, 20.5f), new PointF(7.2f, 24f), new PointF(11f, 24f));
			GraphicsPath_AddBezier_M0101(graphicsPath, new PointF(11f, 24f), new PointF(14.8f, 24f), new PointF(18f, 20.5f), new PointF(18f, 15.5f));
			GraphicsPath_AddBezier_M0101(graphicsPath, new PointF(18f, 15.5f), new PointF(18f, 8.5f), new PointF(11f, 1f), new PointF(11f, 1f));
			GraphicsPath_CloseFigure_M0102(graphicsPath);
			Pen pen = Pen_Create_M0103(Color_F005b, 1.4f);
			try
			{
				Graphics_DrawPath_M0104(P_0, pen, graphicsPath);
			}
			finally
			{
				if (pen != null)
				{
					IDisposable_Dispose_M0105((IDisposable)pen);
				}
			}
		}
		finally
		{
			if (graphicsPath != null)
			{
				IDisposable_Dispose_M0105((IDisposable)graphicsPath);
			}
		}
		GraphicsPath graphicsPath2 = GraphicsPath_Create_M0100();
		try
		{
			GraphicsPath_AddBezier_M0101(graphicsPath2, new PointF(11f, 9f), new PointF(11f, 9f), new PointF(7.8f, 13.2f), new PointF(7.8f, 16.6f));
			GraphicsPath_AddBezier_M0101(graphicsPath2, new PointF(7.8f, 16.6f), new PointF(7.8f, 19.2f), new PointF(9.2f, 21f), new PointF(11f, 21f));
			GraphicsPath_AddBezier_M0101(graphicsPath2, new PointF(11f, 21f), new PointF(12.8f, 21f), new PointF(14.2f, 19.2f), new PointF(14.2f, 16.6f));
			GraphicsPath_AddBezier_M0101(graphicsPath2, new PointF(14.2f, 16.6f), new PointF(14.2f, 13.2f), new PointF(11f, 9f), new PointF(11f, 9f));
			GraphicsPath_CloseFigure_M0102(graphicsPath2);
			SolidBrush solidBrush = SolidBrush_Create_M0106(Color_F005b);
			try
			{
				Graphics_FillPath_M0107(P_0, (Brush)solidBrush, graphicsPath2);
			}
			finally
			{
				if (solidBrush != null)
				{
					IDisposable_Dispose_M0105((IDisposable)solidBrush);
				}
			}
		}
		finally
		{
			if (graphicsPath2 != null)
			{
				IDisposable_Dispose_M0105((IDisposable)graphicsPath2);
			}
		}
	}

	// [behavior 0x060000f5] Builds a bordered input container with a centered child and focus-color callbacks.
	public static Panel WrapInputField_M00f5(Control P_0, int P_1, int P_2, int P_3, int P_4)
	{
		Panel Panel_F0060 = Panel_Create_M0108();
		Control_set_BackColor_M0109((Control)Panel_F0060, Color_F005a);
		Control_set_Location_M010a((Control)Panel_F0060, new Point(P_1, P_2));
		Control_set_Size_M010b((Control)Panel_F0060, new Size(P_3, P_4));
		Panel panel = Panel_Create_M0108();
		Control_set_BackColor_M0109((Control)panel, Color_F0056);
		Control_set_Location_M010a((Control)panel, new Point(1, 1));
		Control_set_Size_M010b((Control)panel, new Size(P_3 - 2, P_4 - 2));
		ControlCollection_Add_M010d(Control_get_Controls_M010c((Control)Panel_F0060), (Control)panel);
		Control_set_Width_M010e(P_0, P_3 - 2 - 18);
		Control_set_Left_M010f(P_0, 9);
		Control_set_Top_M0111(P_0, (Control_get_Height_M0110((Control)panel) - Control_get_Height_M0110(P_0)) / 2);
		ControlCollection_Add_M010d(Control_get_Controls_M010c((Control)panel), P_0);
		Control_add_Enter_M0112(P_0, (EventHandler)delegate
		{
			FieldFocusHandlers.Control_set_BackColor_M011d((Control)Panel_F0060, Color_F005b);
		});
		Control_add_Leave_M0113(P_0, (EventHandler)delegate
		{
			FieldFocusHandlers.Control_set_BackColor_M011d((Control)Panel_F0060, Color_F005a);
		});
		return Panel_F0060;
	}

	// [behavior 0x060000f6] Builds nested padded panels and docks the supplied child inside them.
	public static Panel WrapContent_M00f6(Control P_0, int P_1)
	{
		Panel panel = Panel_Create_M0108();
		Control_set_BackColor_M0109((Control)panel, Color_F005a);
		Control_set_Padding_M0114((Control)panel, new Padding(1));
		Panel panel2 = Panel_Create_M0108();
		Control_set_BackColor_M0109((Control)panel2, Color_F0056);
		Control_set_Dock_M0115((Control)panel2, DockStyle.Fill);
		Control_set_Padding_M0114((Control)panel2, new Padding(P_1));
		ControlCollection_Add_M010d(Control_get_Controls_M010c((Control)panel), (Control)panel2);
		Control_set_Dock_M0115(P_0, DockStyle.Fill);
		ControlCollection_Add_M010d(Control_get_Controls_M010c((Control)panel2), P_0);
		return panel;
	}

	// [behavior 0x060000f7] Draws a rectangle at the panel border using a disposable pen.
	public static void DrawPanelBorder_M00f7(PaintEventArgs P_0, Panel P_1, Color P_2)
	{
		Pen pen = Pen_Create_M0116(P_2);
		try
		{
			Graphics_DrawRectangle_M0119(PaintEventArgs_get_Graphics_M0117(P_0), pen, 0, 0, Control_get_Width_M0118((Control)P_1) - 1, Control_get_Height_M0110((Control)P_1) - 1);
		}
		finally
		{
			if (pen != null)
			{
				IDisposable_Dispose_M0105((IDisposable)pen);
			}
		}
	}

	static string String_ToUpperInvariant_M00f9(string P_0)
	{
		return P_0.ToUpperInvariant();
	}

	static StringBuilder StringBuilder_Create_M00fa()
	{
		return new StringBuilder();
	}

	static StringBuilder StringBuilder_Append_M00fb(StringBuilder P_0, char P_1)
	{
		return P_0.Append(P_1);
	}

	static char String_get_Chars_M00fc(string P_0, int P_1)
	{
		return P_0[P_1];
	}

	static int String_get_Length_M00fd(string P_0)
	{
		return P_0.Length;
	}

	static string Object_ToString_M00fe(object P_0)
	{
		return P_0.ToString();
	}

	static void Graphics_set_SmoothingMode_M00ff(Graphics P_0, SmoothingMode P_1)
	{
		P_0.SmoothingMode = P_1;
	}

	static GraphicsPath GraphicsPath_Create_M0100()
	{
		return new GraphicsPath();
	}

	static void GraphicsPath_AddBezier_M0101(GraphicsPath P_0, PointF P_1, PointF P_2, PointF P_3, PointF P_4)
	{
		P_0.AddBezier(P_1, P_2, P_3, P_4);
	}

	static void GraphicsPath_CloseFigure_M0102(GraphicsPath P_0)
	{
		P_0.CloseFigure();
	}

	static Pen Pen_Create_M0103(Color P_0, float P_1)
	{
		return new Pen(P_0, P_1);
	}

	static void Graphics_DrawPath_M0104(Graphics P_0, Pen P_1, GraphicsPath P_2)
	{
		P_0.DrawPath(P_1, P_2);
	}

	static void IDisposable_Dispose_M0105(IDisposable P_0)
	{
		P_0.Dispose();
	}

	static SolidBrush SolidBrush_Create_M0106(Color P_0)
	{
		return new SolidBrush(P_0);
	}

	static void Graphics_FillPath_M0107(Graphics P_0, Brush P_1, GraphicsPath P_2)
	{
		P_0.FillPath(P_1, P_2);
	}

	static Panel Panel_Create_M0108()
	{
		return new Panel();
	}

	static void Control_set_BackColor_M0109(Control P_0, Color P_1)
	{
		P_0.BackColor = P_1;
	}

	static void Control_set_Location_M010a(Control P_0, Point P_1)
	{
		P_0.Location = P_1;
	}

	static void Control_set_Size_M010b(Control P_0, Size P_1)
	{
		P_0.Size = P_1;
	}

	static Control.ControlCollection Control_get_Controls_M010c(Control P_0)
	{
		return P_0.Controls;
	}

	static void ControlCollection_Add_M010d(Control.ControlCollection P_0, Control P_1)
	{
		P_0.Add(P_1);
	}

	static void Control_set_Width_M010e(Control P_0, int P_1)
	{
		P_0.Width = P_1;
	}

	static void Control_set_Left_M010f(Control P_0, int P_1)
	{
		P_0.Left = P_1;
	}

	static int Control_get_Height_M0110(Control P_0)
	{
		return P_0.Height;
	}

	static void Control_set_Top_M0111(Control P_0, int P_1)
	{
		P_0.Top = P_1;
	}

	static void Control_add_Enter_M0112(Control P_0, EventHandler P_1)
	{
		P_0.Enter += P_1;
	}

	static void Control_add_Leave_M0113(Control P_0, EventHandler P_1)
	{
		P_0.Leave += P_1;
	}

	static void Control_set_Padding_M0114(Control P_0, Padding P_1)
	{
		P_0.Padding = P_1;
	}

	static void Control_set_Dock_M0115(Control P_0, DockStyle P_1)
	{
		P_0.Dock = P_1;
	}

	static Pen Pen_Create_M0116(Color P_0)
	{
		return new Pen(P_0);
	}

	static Graphics PaintEventArgs_get_Graphics_M0117(PaintEventArgs P_0)
	{
		return P_0.Graphics;
	}

	static int Control_get_Width_M0118(Control P_0)
	{
		return P_0.Width;
	}

	static void Graphics_DrawRectangle_M0119(Graphics P_0, Pen P_1, int P_2, int P_3, int P_4, int P_5)
	{
		P_0.DrawRectangle(P_1, P_2, P_3, P_4, P_5);
	}
}
