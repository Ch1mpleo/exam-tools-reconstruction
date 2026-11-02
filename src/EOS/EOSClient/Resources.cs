// Recovered C#. Descriptive aliases are analyst-assigned; _M suffix is original MethodDef RID.
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Resources
{
	private static ResourceManager ResourceManager_F006e;

	private static CultureInfo CultureInfo_F006f;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager Property_0005
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			if (ResourceManager_F006e == null)
			{
				ResourceManager_F006e = ResourceManager_Create_M01b7("GsIWD(XU58$7Yn4=\\&BpxD<k%'", Type_get_Assembly_M01b6(Type_GetTypeFromHandle_M01b5(typeof(Resources).TypeHandle)));
			}
			return ResourceManager_F006e;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static CultureInfo Property_0006
	{
		get
		{
			return CultureInfo_F006f;
		}
		set
		{
			CultureInfo_F006f = value;
		}
	}

	internal Resources()
	{
	}

	static Type Type_GetTypeFromHandle_M01b5(RuntimeTypeHandle P_0)
	{
		return Type.GetTypeFromHandle(P_0);
	}

	static Assembly Type_get_Assembly_M01b6(Type P_0)
	{
		return P_0.Assembly;
	}

	static ResourceManager ResourceManager_Create_M01b7(string P_0, Assembly P_1)
	{
		return new ResourceManager(P_0, P_1);
	}
}
