using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

internal class _003CModule_003E
{
	internal struct Type_0002
	{
		internal uint UInt32_F0005;

		internal void Method_M000b()
		{
			UInt32_F0005 = 1024u;
		}

		internal uint Method_M000c(Type_0004 rangeDecoder)
		{
			uint num = (rangeDecoder.UInt32_F0009 >> 11) * UInt32_F0005;
			if (rangeDecoder.UInt32_F0008 < num)
			{
				rangeDecoder.UInt32_F0009 = num;
				UInt32_F0005 += 2048 - UInt32_F0005 >> 5;
				if (rangeDecoder.UInt32_F0009 < 16777216)
				{
					rangeDecoder.UInt32_F0008 = (rangeDecoder.UInt32_F0008 << 8) | (byte)rangeDecoder.Stream_F000a.ReadByte();
					rangeDecoder.UInt32_F0009 <<= 8;
				}
				return 0u;
			}
			rangeDecoder.UInt32_F0009 -= num;
			rangeDecoder.UInt32_F0008 -= num;
			UInt32_F0005 -= UInt32_F0005 >> 5;
			if (rangeDecoder.UInt32_F0009 < 16777216)
			{
				rangeDecoder.UInt32_F0008 = (rangeDecoder.UInt32_F0008 << 8) | (byte)rangeDecoder.Stream_F000a.ReadByte();
				rangeDecoder.UInt32_F0009 <<= 8;
			}
			return 1u;
		}
	}

	internal struct Type_0003
	{
		internal readonly Type_0002[] Type_0002___F0006;

		internal readonly int Int32_F0007;

		internal Type_0003(int numBitLevels)
		{
			Int32_F0007 = numBitLevels;
			Type_0002___F0006 = new Type_0002[1 << numBitLevels];
		}

		internal void Method_M000e()
		{
			for (uint num = 1u; num < 1 << Int32_F0007; num++)
			{
				Type_0002___F0006[num].Method_M000b();
			}
		}

		internal uint Method_M000f(Type_0004 rangeDecoder)
		{
			uint num = 1u;
			for (int num2 = Int32_F0007; num2 > 0; num2--)
			{
				num = (num << 1) + Type_0002___F0006[num].Method_M000c(rangeDecoder);
			}
			return num - (uint)(1 << Int32_F0007);
		}

		internal uint Method_M0010(Type_0004 rangeDecoder)
		{
			uint num = 1u;
			uint num2 = 0u;
			for (int i = 0; i < Int32_F0007; i++)
			{
				uint num3 = Type_0002___F0006[num].Method_M000c(rangeDecoder);
				num <<= 1;
				num += num3;
				num2 |= num3 << i;
			}
			return num2;
		}

		internal static uint Method_M0011(Type_0002[] Models, uint startIndex, Type_0004 rangeDecoder, int NumBitLevels)
		{
			uint num = 1u;
			uint num2 = 0u;
			for (int i = 0; i < NumBitLevels; i++)
			{
				uint num3 = Models[startIndex + num].Method_M000c(rangeDecoder);
				num <<= 1;
				num += num3;
				num2 |= num3 << i;
			}
			return num2;
		}
	}

	internal class Type_0004
	{
		internal uint UInt32_F0008;

		internal uint UInt32_F0009;

		internal Stream Stream_F000a;

		internal void Method_M0012(Stream stream)
		{
			Stream_F000a = stream;
			UInt32_F0008 = 0u;
			UInt32_F0009 = uint.MaxValue;
			for (int i = 0; i < 5; i++)
			{
				UInt32_F0008 = (UInt32_F0008 << 8) | (byte)Stream_F000a.ReadByte();
			}
		}

		internal void Method_M0013()
		{
			Stream_F000a = null;
		}

		internal void Method_M0014()
		{
			while (UInt32_F0009 < 16777216)
			{
				UInt32_F0008 = (UInt32_F0008 << 8) | (byte)Stream_F000a.ReadByte();
				UInt32_F0009 <<= 8;
			}
		}

		internal uint Method_M0015(int numTotalBits)
		{
			uint num = UInt32_F0009;
			uint num2 = UInt32_F0008;
			uint num3 = 0u;
			for (int num4 = numTotalBits; num4 > 0; num4--)
			{
				num >>= 1;
				uint num5 = num2 - num >> 31;
				num2 -= num & (num5 - 1);
				num3 = (num3 << 1) | (1 - num5);
				if (num < 16777216)
				{
					num2 = (num2 << 8) | (byte)Stream_F000a.ReadByte();
					num <<= 8;
				}
			}
			UInt32_F0009 = num;
			UInt32_F0008 = num2;
			return num3;
		}

		internal Type_0004()
		{
		}
	}

	internal class Type_0005
	{
		internal class Type_0006
		{
			internal readonly Type_0003[] Type_0003___F001d = new Type_0003[16];

			internal readonly Type_0003[] Type_0003___F001e = new Type_0003[16];

			internal Type_0002 Type_0002_F001f;

			internal Type_0002 Type_0002_F0020;

			internal Type_0003 Type_0003_F0021 = new Type_0003(8);

			internal uint UInt32_F0022;

			internal void Method_M001f(uint numPosStates)
			{
				for (uint num = UInt32_F0022; num < numPosStates; num++)
				{
					Type_0003___F001d[num] = new Type_0003(3);
					Type_0003___F001e[num] = new Type_0003(3);
				}
				UInt32_F0022 = numPosStates;
			}

			internal void Method_M0020()
			{
				Type_0002_F001f.Method_M000b();
				for (uint num = 0u; num < UInt32_F0022; num++)
				{
					Type_0003___F001d[num].Method_M000e();
					Type_0003___F001e[num].Method_M000e();
				}
				Type_0002_F0020.Method_M000b();
				Type_0003_F0021.Method_M000e();
			}

			internal uint Method_M0021(Type_0004 rangeDecoder, uint posState)
			{
				if (Type_0002_F001f.Method_M000c(rangeDecoder) == 0)
				{
					return Type_0003___F001d[posState].Method_M000f(rangeDecoder);
				}
				uint num = 8u;
				if (Type_0002_F0020.Method_M000c(rangeDecoder) != 0)
				{
					num += 8;
					num += Type_0003_F0021.Method_M000f(rangeDecoder);
				}
				else
				{
					num += Type_0003___F001e[posState].Method_M000f(rangeDecoder);
				}
				return num;
			}

			internal Type_0006()
			{
			}
		}

		internal class Type_0007
		{
			internal struct Type_0008
			{
				internal Type_0002[] Type_0002___F0027;

				internal void Method_M0029()
				{
					Type_0002___F0027 = new Type_0002[768];
				}

				internal void Method_M002a()
				{
					for (int i = 0; i < 768; i++)
					{
						Type_0002___F0027[i].Method_M000b();
					}
				}

				internal byte Method_M002b(Type_0004 rangeDecoder)
				{
					uint num = 1u;
					do
					{
						num = (num << 1) | Type_0002___F0027[num].Method_M000c(rangeDecoder);
					}
					while (num < 256);
					return (byte)num;
				}

				internal byte Method_M002c(Type_0004 rangeDecoder, byte matchByte)
				{
					uint num = 1u;
					do
					{
						uint num2 = (uint)((matchByte >> 7) & 1);
						matchByte <<= 1;
						uint num3 = Type_0002___F0027[(1 + num2 << 8) + num].Method_M000c(rangeDecoder);
						num = (num << 1) | num3;
						if (num2 != num3)
						{
							while (num < 256)
							{
								num = (num << 1) | Type_0002___F0027[num].Method_M000c(rangeDecoder);
							}
							break;
						}
					}
					while (num < 256);
					return (byte)num;
				}
			}

			internal Type_0008[] ____________________________________________F0023;

			internal int Int32_F0024;

			internal int Int32_F0025;

			internal uint UInt32_F0026;

			internal void Method_M0023(int numPosBits, int numPrevBits)
			{
				if (____________________________________________F0023 == null || Int32_F0025 != numPrevBits || Int32_F0024 != numPosBits)
				{
					Int32_F0024 = numPosBits;
					UInt32_F0026 = (uint)((1 << numPosBits) - 1);
					Int32_F0025 = numPrevBits;
					uint num = (uint)(1 << Int32_F0025 + Int32_F0024);
					____________________________________________F0023 = new Type_0008[num];
					for (uint num2 = 0u; num2 < num; num2++)
					{
						____________________________________________F0023[num2].Method_M0029();
					}
				}
			}

			internal void Method_M0024()
			{
				uint num = (uint)(1 << Int32_F0025 + Int32_F0024);
				for (uint num2 = 0u; num2 < num; num2++)
				{
					____________________________________________F0023[num2].Method_M002a();
				}
			}

			internal uint Method_M0025(uint pos, byte prevByte)
			{
				return ((pos & UInt32_F0026) << Int32_F0025) + (uint)(prevByte >> 8 - Int32_F0025);
			}

			internal byte Method_M0026(Type_0004 rangeDecoder, uint pos, byte prevByte)
			{
				return ____________________________________________F0023[Method_M0025(pos, prevByte)].Method_M002b(rangeDecoder);
			}

			internal byte Method_M0027(Type_0004 rangeDecoder, uint pos, byte prevByte, byte matchByte)
			{
				return ____________________________________________F0023[Method_M0025(pos, prevByte)].Method_M002c(rangeDecoder, matchByte);
			}

			internal Type_0007()
			{
			}
		}

		internal readonly Type_0002[] Type_0002___F000b = new Type_0002[192];

		internal readonly Type_0002[] Type_0002___F000c = new Type_0002[192];

		internal readonly Type_0002[] Type_0002___F000d = new Type_0002[12];

		internal readonly Type_0002[] Type_0002___F000e = new Type_0002[12];

		internal readonly Type_0002[] Type_0002___F000f = new Type_0002[12];

		internal readonly Type_0002[] Type_0002___F0010 = new Type_0002[12];

		internal readonly Type_0006 _________________________________________F0011 = new Type_0006();

		internal readonly Type_0007 __________________________________________F0012 = new Type_0007();

		internal readonly Type_0009 __________________________________________F0013 = new Type_0009();

		internal readonly Type_0002[] Type_0002___F0014 = new Type_0002[114];

		internal readonly Type_0003[] Type_0003___F0015 = new Type_0003[4];

		internal readonly Type_0004 Type_0004_F0016 = new Type_0004();

		internal readonly Type_0006 _________________________________________F0017 = new Type_0006();

		internal bool Boolean_F0018;

		internal uint UInt32_F0019;

		internal uint UInt32_F001a;

		internal Type_0003 Type_0003_F001b = new Type_0003(4);

		internal uint UInt32_F001c;

		internal Type_0005()
		{
			UInt32_F0019 = uint.MaxValue;
			for (int i = 0; (long)i < 4L; i++)
			{
				Type_0003___F0015[i] = new Type_0003(6);
			}
		}

		internal void Method_M0018(uint dictionarySize)
		{
			if (UInt32_F0019 != dictionarySize)
			{
				UInt32_F0019 = dictionarySize;
				UInt32_F001a = Math.Max(UInt32_F0019, 1u);
				uint windowSize = Math.Max(UInt32_F001a, 4096u);
				__________________________________________F0013.Method_M002d(windowSize);
			}
		}

		internal void ____________________________________________________________________________________M0019(int lp, int lc)
		{
			__________________________________________F0012.Method_M0023(lp, lc);
		}

		internal void Method_M001a(int pb)
		{
			uint num = (uint)(1 << pb);
			_________________________________________F0011.Method_M001f(num);
			_________________________________________F0017.Method_M001f(num);
			UInt32_F001c = num - 1;
		}

		internal void Method_M001b(Stream inStream, Stream outStream)
		{
			Type_0004_F0016.Method_M0012(inStream);
			__________________________________________F0013.Method_M002e(outStream, Boolean_F0018);
			for (uint num = 0u; num < 12; num++)
			{
				for (uint num2 = 0u; num2 <= UInt32_F001c; num2++)
				{
					uint num3 = (num << 4) + num2;
					Type_0002___F000b[num3].Method_M000b();
					Type_0002___F000c[num3].Method_M000b();
				}
				Type_0002___F000d[num].Method_M000b();
				Type_0002___F000e[num].Method_M000b();
				Type_0002___F000f[num].Method_M000b();
				Type_0002___F0010[num].Method_M000b();
			}
			__________________________________________F0012.Method_M0024();
			for (uint num = 0u; num < 4; num++)
			{
				Type_0003___F0015[num].Method_M000e();
			}
			for (uint num = 0u; num < 114; num++)
			{
				Type_0002___F0014[num].Method_M000b();
			}
			_________________________________________F0011.Method_M0020();
			_________________________________________F0017.Method_M0020();
			Type_0003_F001b.Method_M000e();
		}

		internal void Method_M001c(Stream inStream, Stream outStream, long inSize, long outSize)
		{
			Method_M001b(inStream, outStream);
			Type_000a type_000a = default;
			type_000a.Method_M0035();
			uint num = 0u;
			uint num2 = 0u;
			uint num3 = 0u;
			uint num4 = 0u;
			ulong num5 = 0uL;
			if (num5 < (ulong)outSize)
			{
				Type_0002___F000b[type_000a.UInt32_F002d << 4].Method_M000c(Type_0004_F0016);
				type_000a.Method_M0036();
				byte b = __________________________________________F0012.Method_M0026(Type_0004_F0016, 0u, 0);
				__________________________________________F0013.Method_M0032(b);
				num5++;
			}
			while (num5 < (ulong)outSize)
			{
				uint num6 = (uint)(int)num5 & UInt32_F001c;
				if (Type_0002___F000b[(type_000a.UInt32_F002d << 4) + num6].Method_M000c(Type_0004_F0016) != 0)
				{
					uint num8;
					if (Type_0002___F000d[type_000a.UInt32_F002d].Method_M000c(Type_0004_F0016) == 1)
					{
						if (Type_0002___F000e[type_000a.UInt32_F002d].Method_M000c(Type_0004_F0016) == 0)
						{
							if (Type_0002___F000c[(type_000a.UInt32_F002d << 4) + num6].Method_M000c(Type_0004_F0016) == 0)
							{
								type_000a.Method_M0039();
								__________________________________________F0013.Method_M0032(__________________________________________F0013.Method_M0033(num));
								num5++;
								continue;
							}
						}
						else
						{
							uint num7;
							if (Type_0002___F000f[type_000a.UInt32_F002d].Method_M000c(Type_0004_F0016) == 0)
							{
								num7 = num2;
							}
							else
							{
								if (Type_0002___F0010[type_000a.UInt32_F002d].Method_M000c(Type_0004_F0016) != 0)
								{
									num7 = num4;
									num4 = num3;
								}
								else
								{
									num7 = num3;
								}
								num3 = num2;
							}
							num2 = num;
							num = num7;
						}
						num8 = _________________________________________F0017.Method_M0021(Type_0004_F0016, num6) + 2;
						type_000a.Method_M0038();
					}
					else
					{
						num4 = num3;
						num3 = num2;
						num2 = num;
						num8 = 2 + _________________________________________F0011.Method_M0021(Type_0004_F0016, num6);
						type_000a.Method_M0037();
						uint num9 = Type_0003___F0015[Method_M001e(num8)].Method_M000f(Type_0004_F0016);
						if (num9 >= 4)
						{
							int num10 = (int)((num9 >> 1) - 1);
							num = (2 | (num9 & 1)) << num10;
							if (num9 >= 14)
							{
								num += Type_0004_F0016.Method_M0015(num10 - 4) << 4;
								num += Type_0003_F001b.Method_M0010(Type_0004_F0016);
							}
							else
							{
								num += Type_0003.Method_M0011(Type_0002___F0014, num - num9 - 1, Type_0004_F0016, num10);
							}
						}
						else
						{
							num = num9;
						}
					}
					if ((num >= num5 || num >= UInt32_F001a) && num == uint.MaxValue)
					{
						break;
					}
					__________________________________________F0013.Method_M0031(num, num8);
					num5 += num8;
				}
				else
				{
					byte prevByte = __________________________________________F0013.Method_M0033(0u);
					byte b2 = ((!type_000a.Method_M003a()) ? __________________________________________F0012.Method_M0027(Type_0004_F0016, (uint)num5, prevByte, __________________________________________F0013.Method_M0033(num)) : __________________________________________F0012.Method_M0026(Type_0004_F0016, (uint)num5, prevByte));
					__________________________________________F0013.Method_M0032(b2);
					type_000a.Method_M0036();
					num5++;
				}
			}
			__________________________________________F0013.Method_M0030();
			__________________________________________F0013.Method_M002f();
			Type_0004_F0016.Method_M0013();
		}

		internal void Method_M001d(byte[] properties)
		{
			int lc = properties[0] % 9;
			int num = properties[0] / 9;
			int lp = num % 5;
			int pb = num / 5;
			uint num2 = 0u;
			for (int i = 0; i < 4; i++)
			{
				num2 += (uint)(properties[1 + i] << i * 8);
			}
			Method_M0018(num2);
			____________________________________________________________________________________M0019(lp, lc);
			Method_M001a(pb);
		}

		internal static uint Method_M001e(uint len)
		{
			len -= 2;
			if (len < 4)
			{
				return len;
			}
			return 3u;
		}
	}

	internal class Type_0009
	{
		internal byte[] Byte___F0028;

		internal uint UInt32_F0029;

		internal Stream Stream_F002a;

		internal uint UInt32_F002b;

		internal uint UInt32_F002c;

		internal void Method_M002d(uint windowSize)
		{
			if (UInt32_F002c != windowSize)
			{
				Byte___F0028 = new byte[windowSize];
			}
			UInt32_F002c = windowSize;
			UInt32_F0029 = 0u;
			UInt32_F002b = 0u;
		}

		internal void Method_M002e(Stream stream, bool solid)
		{
			Method_M002f();
			Stream_F002a = stream;
			if (!solid)
			{
				UInt32_F002b = 0u;
				UInt32_F0029 = 0u;
			}
		}

		internal void Method_M002f()
		{
			Method_M0030();
			Stream_F002a = null;
			Buffer.BlockCopy(new byte[Byte___F0028.Length], 0, Byte___F0028, 0, Byte___F0028.Length);
		}

		internal void Method_M0030()
		{
			uint num = UInt32_F0029 - UInt32_F002b;
			if (num != 0)
			{
				Stream_F002a.Write(Byte___F0028, (int)UInt32_F002b, (int)num);
				if (UInt32_F0029 >= UInt32_F002c)
				{
					UInt32_F0029 = 0u;
				}
				UInt32_F002b = UInt32_F0029;
			}
		}

		internal void Method_M0031(uint distance, uint len)
		{
			uint num = UInt32_F0029 - distance - 1;
			if (num >= UInt32_F002c)
			{
				num += UInt32_F002c;
			}
			while (len != 0)
			{
				if (num >= UInt32_F002c)
				{
					num = 0u;
				}
				Byte___F0028[UInt32_F0029++] = Byte___F0028[num++];
				if (UInt32_F0029 >= UInt32_F002c)
				{
					Method_M0030();
				}
				len--;
			}
		}

		internal void Method_M0032(byte b)
		{
			Byte___F0028[UInt32_F0029++] = b;
			if (UInt32_F0029 >= UInt32_F002c)
			{
				Method_M0030();
			}
		}

		internal byte Method_M0033(uint distance)
		{
			uint num = UInt32_F0029 - distance - 1;
			if (num >= UInt32_F002c)
			{
				num += UInt32_F002c;
			}
			return Byte___F0028[num];
		}

		internal Type_0009()
		{
		}
	}

	internal struct Type_000a
	{
		internal uint UInt32_F002d;

		internal void Method_M0035()
		{
			UInt32_F002d = 0u;
		}

		internal void Method_M0036()
		{
			if (UInt32_F002d >= 4)
			{
				if (UInt32_F002d < 10)
				{
					UInt32_F002d -= 3u;
				}
				else
				{
					UInt32_F002d -= 6u;
				}
			}
			else
			{
				UInt32_F002d = 0u;
			}
		}

		internal void Method_M0037()
		{
			UInt32_F002d = ((UInt32_F002d < 7) ? 7u : 10u);
		}

		internal void Method_M0038()
		{
			UInt32_F002d = ((UInt32_F002d < 7) ? 8u : 11u);
		}

		internal void Method_M0039()
		{
			UInt32_F002d = ((UInt32_F002d < 7) ? 9u : 11u);
		}

		internal bool Method_M003a()
		{
			return UInt32_F002d < 7;
		}
	}

	[StructLayout(LayoutKind.Explicit, Size = 2048)]
	internal struct Type_000b
	{
	}

	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 1344)]
	internal struct Type_000c
	{
	}

	internal static byte[] Byte___F0001;

	internal static Type_000b __________________________________________F0002/* Not supported: data(7A 1C 6F 8B C2 12 60 A7 22 DC 6E D7 49 ED BC 58 41 B5 F9 F5 B4 56 44 32 AF 83 88 79 46 65 F4 B2 CD 7A 2C 1C 90 A6 87 D5 29 C6 A2 CC DC 4D 78 CA BD 16 76 5B 6C F7 86 10 9F 7E 32 A8 D2 E6 8B 3C AB BE 46 92 4A C3 32 50 F4 A0 12 FF 9C 6C F4 4D 51 51 5D 27 86 BA 85 F8 66 50 FA 88 18 36 B2 40 2F C7 5E 3B 54 52 9C 53 89 94 4C 2F 43 CC 88 BF 67 53 BA 22 D5 41 79 C6 4E C6 F1 13 55 1B DC 38 5A 83 18 B5 7C 91 80 CB 75 E4 2B 96 35 10 1B 74 5D 00 CE CB A0 AC 87 93 29 90 97 5F 10 AD AB 61 34 14 89 C9 39 4F 91 F0 F1 2A 89 6C D6 EC F9 E7 56 F8 07 E2 C8 4B 2C 47 B8 6A 11 C5 B1 63 7E 57 E3 1D 1D 83 91 65 D9 65 4D C9 D3 AD 10 91 A8 C6 8F B8 30 B2 94 8C E3 AF FA 6D 0D 32 7F C7 FF D5 A6 5A 9A 45 87 30 5B 66 63 B2 59 8E 5F 1D 2D 2F 3D 6D E4 F8 F2 E5 57 61 3B 5F E1 AC 51 F7 7E 90 F5 D3 FE 7B F3 3C 74 3E F7 CE F7 8C F4 51 82 CE FA 5A 53 B4 CC 10 E3 35 4D 7E 3C 07 7A A2 AF 28 83 6E DD 2D C0 45 33 EF 1B 98 22 02 AA 87 35 30 46 F5 99 31 91 45 F8 AB 37 29 3F F4 D7 FD 90 0F D8 50 37 2E ED C5 88 02 45 B6 57 E6 AA CF 7E 29 54 AA 58 40 3A E2 6D 57 14 FC 02 36 EA 18 7B B4 BF 27 B3 A4 5A D0 0F 39 CA 2C 56 48 93 AC 14 48 4E 58 A6 E3 96 9B 49 75 2C 16 3A CA 88 6C 99 2A 65 8E 5D E8 DC 8E 34 91 E7 15 C0 95 B9 A4 70 A6 A5 41 EB C2 26 58 A3 5A 00 11 7A 93 36 49 22 38 56 13 89 25 6B 52 CA 97 A5 1D DA D3 D1 3D 20 3F 7A 0C A0 63 57 96 B1 73 8A 90 BC 20 AA E3 4B B7 5A 4F 2E 3E 7A D3 D3 17 BB 82 26 C5 CE 6E C3 67 67 5B 7E D4 58 12 0E 10 97 85 F0 59 34 D1 D3 84 0C 2B CA 15 CA 35 87 7A 4B 33 CC 74 76 38 80 82 D5 0D 1E 6C 7C D0 93 B5 6C 55 A3 6E 9F F6 A5 95 A2 A1 C9 00 DF 99 F7 AD 4F 92 C6 36 24 DD F9 4F 97 21 59 0E 41 23 05 A5 F7 07 05 0F 18 88 E9 12 99 10 63 67 29 60 A8 B6 98 D9 F5 A0 0A 30 B7 B0 97 A3 F8 5C 26 C9 7E CA EE 3B BA 01 26 C5 CA 54 29 41 7E 0E 6C 6A 77 0B 22 39 BD A3 47 43 B8 12 5B E9 BE 5B 67 FC 9A 89 CB 34 E9 FF 45 A9 2F 89 32 2D 48 2F 77 DF 89 A6 6C 7B FA AD 45 75 A2 C5 22 83 8B A8 2A 85 19 A5 D3 8F 86 CF 9E BF 03 FE 23 43 D4 CE F6 12 02 BD D2 43 F5 F4 26 98 16 80 AE 13 9B 5A AC 86 34 3E 45 B5 98 8B 10 2A 2C 59 6F 5F DA 96 61 E0 E5 E1 B0 2D 3C E1 77 43 E2 2A 62 8F 53 C6 0A A5 6E 39 B8 30 22 58 92 41 DB 41 B9 9A 4C 1B 41 93 BB 41 9A 68 CB 60 BD 69 2A E4 E6 B9 D8 53 CD DB E2 29 08 55 D4 5B 11 40 F8 67 DC A9 AB 41 73 8B 67 6D D1 F0 36 0A C1 92 96 73 B2 A8 59 D7 95 2E 85 60 2F 76 05 54 E4 B8 55 FD A2 5B CA F7 E6 14 63 59 AB A5 2E 6D 3B FD 62 D0 3E FC A9 96 FC D2 A8 26 70 66 79 8E 0F 64 E4 4A 26 36 B5 60 86 CC B7 82 15 8E 08 B6 40 24 6E 35 5B E8 A4 32 4E 25 31 30 9C 45 31 79 3E FA EF 71 21 CF B8 9D 0C 83 F3 E4 F2 22 04 28 FE 4F EE 01 51 0B 91 31 AD 50 18 93 F1 4F 96 97 20 83 D2 9D 55 F2 E0 33 28 C0 A5 6A 47 C9 72 6F 6E 49 1F FE AD 2F E7 69 0D 63 8A D0 1A 5E 4A 30 8E F8 49 6D BC E6 C9 16 31 02 49 2F DF 7F AD 37 7F 8B 84 65 24 0E 04 58 21 35 7A 46 2F 9A F5 63 28 18 23 18 D4 02 22 F0 A1 7B 19 35 78 5D B2 B4 38 13 D4 0B 2C B2 E1 E5 E1 1A 0F 1F 37 02 EF A9 5F E9 AC C9 FE D5 DD 8E 20 66 C0 BC 48 77 B6 84 E9 D5 66 D6 12 3B B3 CB 01 98 98 F0 71 0E 3C 40 C5 76 37 54 70 7E A6 EE 74 E1 9D D9 84 A9 CB 0A B6 DC 06 CA 2D 06 B0 4C 5A B0 98 1D FC 2F 9A 56 E3 03 89 E9 53 CA FA 83 6A DA B3 D6 2F 54 87 0B 47 B5 91 80 58 89 3A 53 B3 3A 3F C3 AC 35 BE 4F C1 33 0D 78 13 02 49 98 6C D0 8D 01 EB 81 17 D0 9A 5A 7B C1 5B 08 8D 34 3E 83 91 2D F9 8D 51 51 61 BA A9 75 A7 BB BD 54 8B 85 36 BB 6B E5 39 71 47 4D 5E 32 A5 AA EE AA 65 2A 7F C4 DE 31 98 DD 45 F8 4A 0A 13 66 64 D1 98 F7 08 C8 9A 1D 42 DA AB F4 45 23 AA C9 6D AC BE B9 F6 B0 D1 CC DF 67 3C 88 17 0D 01 DA C0 CC 33 D5 08 49 32 B4 40 96 BD A6 EB 75 CB A9 AC 97 F0 EB 9A 63 2F DF 55 53 2C D0 DD E0 37 D1 D1 C1 53 5A 85 C5 20 05 AE 84 51 BD C8 0F EE 08 B8 74 58 E7 FD 67 5C 47 D5 6F E4 A1 5F 91 66 51 C5 BA 5C 20 A5 59 AF ED 69 5A 59 A9 27 1E 24 6F 7C 1E B6 93 B9 17 4B 71 EF 14 B3 DA E4 ED CC 7A 0A 73 E5 DD CC 64 54 7B 75 8E 7B CE 08 4E CD BA 62 5D 70 5F BB 8F 4F 18 11 8B 6B 61 53 05 0D 88 40 89 98 95 ED 33 89 04 89 92 FD 34 CB E3 3F F6 CE 95 A9 A1 DB BB 33 C1 8B 62 6D 16 7A F4 EA 96 63 92 CA 17 CC 33 A7 56 9B D6 64 FF 7E E3 1C 83 C2 20 08 D5 CD AF 31 D1 83 AB 10 95 A1 20 74 7C 41 FD 7B 95 1A E1 39 AC E0 08 8D 10 B6 F1 09 5B 7B 8F 4C 22 9F B6 2D 06 5F 53 D8 24 C4 67 47 06 C8 7D CC 4F 57 84 4F F2 A2 C3 EC 6A 7E 8B D1 16 D6 9A 82 B3 FC 06 55 A7 E5 96 B1 32 16 34 EC 6B 9B 75 5E 9F 54 C4 B9 11 79 32 31 AD 39 FF C6 2E B7 CA E2 D3 6A 16 00 9E B5 CB 49 21 A8 38 61 10 A8 51 35 6E 15 0E F1 26 D0 7E 44 A2 13 80 F5 51 30 C5 04 3E 13 7F 03 D8 1B 40 F7 F6 77 44 2E 14 64 1B C5 FA 0C C3 25 9D CF E2 5A 99 2A BC 22 C1 A8 AD 2B 51 5D 65 EB 69 D0 CA 7A 14 CF 4C 0B 2E D2 06 70 B6 1C 8F 84 C2 91 4B BE 5C AF F3 F6 BF C9 AB 0A CB 20 7D 75 17 24 86 3A 09 24 4A 3F E8 0D 9C AD DF F6 D3 A6 F6 68 76 8C DF 56 04 C3 C5 A1 4B 3C EB 77 7D BE E1 7D 7A 67 F7 35 9D ED E9 6D 31 EF 5F 6F C3 42 E7 62 E0 FD 07 41 86 95 B4 BD 22 46 BC 30 62 DD BD 09 5E C8 D3 45 08 4C 32 22 75 65 61 19 C2 04 F8 E4 61 2A B1 6B 76 E8 5A 64 5C 20 8D F4 EF 51 5B 38 0E 02 76 74 07 61 20 84 4C 25 37 F3 18 09 E7 46 96 36 7C 17 D5 CA AA 9F 88 9A FA 26 2A 04 F8 77 D7 DB A2 98 6A 9F 7A C8 F5 89 20 C0 D7 B3 FB 8D E9 35 57 B2 9C E7 45 B3 99 33 B1 BF 05 05 84 06 23 9B B1 02 C3 1B BB F3 44 E4 94 36 F1 61 5B 1D C5 B2 E3 3C 88 B6 9F 1A 15 28 3F A6 F7 0B 8A C7 4E 9B 49 1C D6 CC 1A C3 99 B8 85 C1 03 9A 1D 56 54 60 FB 11 BF 8D 41 81 EF BF A0 6F 14 77 40 44 15 9D BD 3A B6 23 C4 8E 93 B8 D5 EA 93 B6 54 79 BA 8E 56 41 D1 E1 A6 35 0E 35 A7 1C B5 6A FA 98 A9 62 E0 E6 B7 09 A4 11 C4 41 D5 27 19 4A CB 16 AB 15 DE 41 41 3A 58 F2 E8 4E 3A D7 42 72 60 D0 C9 BE A7 81 1C 8E A9 88 78 23 07 58 FF 45 08 1D C8 A3 53 01 9D BE 9A C6 97 9E CA EC 24 BF 6C 2D AF AC EC 04 E0 67 DB CC 5B 5B E9 3B 7D E6 97 E3 3D 5D B7 F8 E2 1A 1D 96 AA 25 11 11 D8 8B 0B C1 86 48 A7 34 7E B4 18 5B D5 40 1B CF 47 BD 7F 02 65 84 D4 F1 BC FA 02 3A CD F0 C7 6B 0A D0 08 12 D6 F9 5D 22 4C 56 57 9B 38 86 7B ED 3C 45 DF 13 9C 17 BF 83 CC 90 7C 99 66 EC F4 38 6B 20 A8 46 AB 3A DF 8F C8 CE AF 97 7D 35 28 8A 73 C3 22 17 EA 9B 96 56 70 04 AC C5 60 EB 3B 1F E7 12 80 42 12 C4 45 9D E8 68 6E 77 4C F1 51 8F 46 C4 59 1B 36 EF 99 2F 9E C7 90 45 DE A5 00 83 6B EA 72 75 DC 02 C2 18 7D 33 75 C0 DA E6 8C 50 DB 66 EE 89 7E BB 8F F5 AB CC 3C C5 60 EB 3B 1F E7 12 80 42 12 C4 45 9D E8) */;

	internal static Assembly Assembly_F0003;

	internal static Type_000c __________________________________________F0004/* Not supported: data(8A E7 3D 97 34 5E 2D 67 C5 67 18 4D AE CE 38 A0 1D EB 38 C6 E7 1A C1 ED EA 08 79 62 6F A8 CC 80 C7 43 3F 12 C3 8D F1 81 17 73 C0 7D 45 E2 BB 9C 13 3E 47 FB 6E 3A D2 1B FE 69 1B 77 67 D1 FD F8 A1 2F D6 B5 71 A0 6C 9D 76 3E 97 3F AA C7 B8 CF 27 87 66 03 B3 1F 1D 45 CD 19 73 3E F0 27 9D 39 7B 26 CF 33 DE 56 06 ED 1D 66 75 E3 6D E8 FF 75 5B 3C 0F 36 AF 42 19 B2 FB F7 B1 66 2E 36 85 28 5E 84 42 1A 9F B5 2B 5C 1E FE B8 8F 78 6E 52 02 80 F1 F1 EE 7C 31 83 05 68 32 4E D0 6A 8F 75 30 22 79 BF 54 3A BC 14 29 91 E3 69 79 6C F6 77 88 0C 39 E1 9B BF 1C B3 09 28 27 67 26 41 FA 4E 43 DC 3A 20 5C D2 EC CB 9D F0 C8 15 A0 19 CE C3 7C 7F B5 39 72 44 B7 9F 62 76 9C 8B 6A 56 13 28 A0 42 48 20 44 16 20 B8 E8 2C B5 30 C8 7C F3 24 22 08 6D C5 87 FD 81 28 95 A8 96 50 25 B4 3B C8 CC C3 45 36 05 91 4C 82 2D E2 BA 6C 7E 67 5A 06 33 DC 8F 45 12 D6 9E 99 EA E0 1B DF 45 E6 87 42 0A 3B B0 8F 57 51 41 CB 1F A9 92 57 46 86 C3 58 C8 C2 C9 87 C9 71 A7 25 44 FE F0 88 61 B4 7F 03 68 7C 0E 73 29 EB 1F D4 4E 22 AA 4D 56 A3 F7 D0 48 C1 C2 97 DC D7 B4 F3 E2 5C 7E A1 94 EB 89 22 87 CB 0D FE 46 D6 69 C2 85 40 9C 1C E6 0E 09 AD D0 31 F8 04 18 97 24 59 39 B9 3A 68 F2 10 8B 53 75 69 26 FD FC 1D 33 F3 04 6E 4A 57 B9 2F 31 DA BA E4 8E 93 FE 25 F7 F8 9E E5 2C 9B 1A 5D 54 E1 5B 75 68 0D CF 59 54 D0 F3 AB 56 F7 21 19 F3 FF 59 F6 6D F5 67 E3 A1 8E 4E F2 9C FC 09 76 8D 54 37 A6 17 07 90 A4 F1 BD 6F 4E C2 C7 C6 9F C8 18 73 C6 B6 4E 61 EB 7C 15 1B E9 CE 75 5D 4C 33 59 70 E9 96 C0 E2 45 91 AC D8 19 CD 11 3C 71 EE 77 56 7C 9C 9B 36 35 EC B0 6D 68 D8 1A 85 D4 8A D5 E0 42 68 93 F3 49 A1 02 A0 05 5E 0D AD E9 1B BE 11 28 8A C5 F5 AC 6F 2A F9 62 0F 7C 14 B5 9B 20 E7 C5 6C 0B 73 AB 6F 17 EE 4B 74 78 CC 7A 0E 77 1F E8 C4 C7 BC BC A0 23 2F 53 8B 8F 79 8C EB CF 94 8E 26 08 EB 46 23 EA 86 15 C3 76 5E BD 8B 29 B5 EE B1 15 0A 7E 44 44 E6 08 CF 59 28 36 20 69 59 0D B4 1C 22 9B 97 8E AF 43 A1 BE D2 94 D6 4A 8C 4E F0 F2 F1 E3 76 E4 44 FA F6 88 04 46 13 71 02 87 C9 2F 2F FF 39 F7 71 7F 63 F4 2F FF 5B C6 5D 4D DA E1 F5 58 15 38 22 C2 65 05 53 A5 26 EE 91 9E 8B CB FD D7 E6 03 53 FE 7A 42 E6 49 E1 82 F3 A9 CE 2B DA 8B C4 EE 1D 52 76 A1 C6 10 72 61 12 12 F2 63 61 69 14 E4 87 37 A0 EC 92 98 AE F7 92 1B 52 D6 A2 2C FE 84 C0 77 E8 BD 50 F6 FE B8 99 55 DD AA F1 E2 6B 45 DC 94 E0 25 F8 84 27 BC 49 CF 28 3B E4 14 95 AF E3 D2 D5 9F EF 51 39 D5 3B 01 A0 A0 33 C9 3A 3E 5C DB 75 AC 42 BE 63 C5 2D F4 50 D6 F6 26 1F D4 D6 E1 BD 3F E4 53 0A FA 85 18 0A 37 F7 F8 87 CF 1C 2E CB 8C BF 30 84 F9 D2 01 20 E4 C7 F3 19 54 DF D6 EE B4 C3 4D E2 31 27 72 21 6D C7 1A 80 1B 34 95 31 24 BA 49 01 9D 45 67 4F A6 B6 1B E9 10 3B 20 0E 31 29 A0 94 17 76 F9 36 5C 90 88 EA 18 02 1D 76 8D 69 74 22 78 5A 58 9A BD 89 A1 AE 19 78 15 D1 1F 55 09 3B 33 1F FE 9E D4 B6 80 0D 1B 4A 12 7F 62 7A 73 B8 8C 51 1D A6 7F E4 D9 58 5C E6 DA 92 DE 9B 8B BB F6 85 3C E1 53 B6 89 72 1A B9 5F 0E EB 61 D3 CC 7B CF 7D 87 E9 E7 F0 86 30 1F D3 ED 6A 9C 7F FE 46 1B B7 0C D2 46 B4 D9 E4 14 03 B1 6E 03 1C 40 21 05 83 D8 1A 5E 19 70 95 DB F4 68 57 5F 17 FD F0 71 0E 18 CB D9 D1 91 9C 19 3C 3F B0 1D 2F F0 E4 6C 52 B0 4D 50 06 BC C8 8E DB 5C D8 25 66 8C A4 32 A4 F3 BE BF 6F FC BF 47 77 87 30 7F AE 8A 6C 1B BB 39 1C 9D AB F7 31 35 28 12 74 8B CB A7 4F 9C BF D0 21 3C 55 E1 63 B4 12 F4 59 9D 5D D6 C5 0F 06 E8 A9 AB 53 B9 9E 2F 0F A5 18 85 19 16 41 30 06 7B 2F 98 4B EA 3B D5 E3 2B D1 55 83 57 83 6A 94 92 A7 DF 9F 70 BC 7E 2B CA 7E D8 82 27 DF 9B 28 B2 C2 1A 66 81 D4 2F 3E 3D 11 E9 7D F4 90 2E 9E AB 08 55 4B F4 41 C1 69 D7 CE D5 0E 7B FE 58 DB 53 94 06 6A D2 65 D5 9D 7D 92 A6 1A B5 1C 25 85 02 E5 25 B7 20 0A C7 AE 02 C9 C4 82 21 07 3E 84 3D 44 A0 15 8F 47 A3 21 BC 37 04 59 EC 59 8A B2 A5 2F 57 BA 4A 02 44 31 34 CB 4D 9A 8E 58 F7 92 15 62 CD F4 53 56 8F 83 5D EB 0C AF FA 05 5C 26 37 6D 36 15 1A FB 34 44 E0 47 69 87 CF 60 30 D5 C5 5E 55 31 0F 95 9B 47 35 2E 3B E6 32 04 1D D4 F5 23 08 27 8D 60 59 B1 45 5B A8 7A C1 D8 E1 FB AF 35 05 62 68 F9 AB FE 66 14 78 34 1E DD 95 11 92 B1 F4 25 D5 22 E0 2A C7 B1 F1 E0 62 31 FC 09 83 FF 8C C1 1E AD 47 67 CC ED 0E F4 80 AA D2 2B 24 8F 3E F3 C6 60 DC 7E CB 26 19 7A C1 D8 E1) */;

	static _003CModule_003E()
	{
		Method_M0009();
		uint num = 512u;
		uint[] array = new uint[512]
		{
			2339314810u, 2808091330u, 3614366754u, 1488776521u, 4126782785u, 843339444u, 2038989743u, 3002361158u, 472677069u, 3582437008u,
			3433219625u, 3396881884u, 1534465725u, 277280620u, 2821881503u, 1015801554u, 2454109867u, 1345504074u, 4279410932u, 1307864220u,
			660427089u, 4169513606u, 2298105958u, 1085421080u, 996067119u, 1402753620u, 793547913u, 3213413443u, 582636391u, 3329835477u,
			334612046u, 953949013u, 3038282586u, 3414200700u, 2519458933u, 1947930677u, 3419275357u, 2475142304u, 1603768361u, 1638640912u,
			3381204020u, 4036054841u, 1820928753u, 3891915990u, 3792173142u, 1194085320u, 3306252984u, 1467900849u, 2199723491u, 1708746129u,
			2916337997u, 3332935952u, 2989537423u, 2950925460u, 839740922u, 3590309759u, 1167743654u, 1717252231u, 2388243043u, 791485791u,
			4175719741u, 1633150450u, 2900451131u, 2424239953u, 2080297973u, 1047805171u, 2365050615u, 3464647156u, 3025361658u, 904073420u,
			121405005u, 682599034u, 769486467u, 4013114816u, 35821595u, 808814506u, 832173382u, 2885174673u, 4097780023u, 261160407u,
			775377112u, 42517997u, 3864507973u, 696176554u, 1079552596u, 1466819130u, 906165268u, 3027966186u, 2763204543u, 957337690u,
			1213607114u, 1209314451u, 3819329614u, 1967758230u, 3392804396u, 714697864u, 3898445413u, 2436140764u, 2512393703u, 2792400057u,
			3270197669u, 1520654374u, 2474250496u, 941771062u, 629740374u, 2546618987u, 3554287013u, 1059077585u, 1671433338u, 1941018199u,
			549228682u, 3075204010u, 1043222362u, 399758202u, 3307635387u, 1740861134u, 3565050727u, 269357656u, 1508935063u, 2228474164u,
			365570828u, 2055681482u, 1959539531u, 2189441142u, 1813908949u, 3046363260u, 1856197996u, 2510681759u, 13214114u, 2918685151u,
			918983247u, 1341775140u, 240722327u, 2768577345u, 251987959u, 317294616u, 1734545561u, 3064487977u, 2700466584u, 2964795402u,
			1559798679u, 3397306662u, 28982254u, 1422574886u, 243155241u, 192375404u, 2747087138u, 314065735u, 1539238235u, 2308635751u,
			4293473483u, 2301602117u, 793259314u, 2794053495u, 2918873964u, 3315758405u, 2827715362u, 2769913130u, 3481702355u, 4261658526u,
			3470017315u, 3171029750u, 4109714386u, 2148964390u, 1520112558u, 1043629740u, 2342040901u, 1496066576u, 2530893679u, 3789938785u,
			3778817456u, 719471479u, 3327364962u, 963552522u, 1478635704u, 1104888210u, 458005177u, 1102811969u, 1623943322u, 3827984829u,
			1406712294u, 702733261u, 1540642056u, 1744322577u, 1101769180u, 1835502451u, 171372753u, 1939247809u, 3612977330u, 1619340949u,
			1409644079u, 4250253540u, 4157234082u, 1499665638u, 1831773611u, 3496148283u, 2527722558u, 648598268u, 2390320752u, 1256481807u,
			1622488614u, 2193083526u, 3054013973u, 896410688u, 849668187u, 808527182u, 2033272220u, 1911552574u, 2646134561u, 3841164044u,
			671359730u, 32395262u, 831589201u, 2467844269u, 2543210481u, 2647819040u, 870380117u, 1789247528u, 1869793607u, 4263463278u,
			1776758701u, 3498730253u, 810180122u, 1833564302u, 382330556u, 793313841u, 934117343u, 1703185279u, 1476660772u, 1182414113u,
			1677040175u, 404953128u, 4028760788u, 890862497u, 3031588216u, 198447928u, 3856773676u, 521083617u, 2851013175u, 3383552351u,
			2396902910u, 3166725664u, 2226550600u, 3597063657u, 3417520914u, 4036532225u, 1077677681u, 1412921029u, 4003888752u, 3651002740u,
			181119364u, 3389447350u, 1286604333u, 496545882u, 1452945404u, 3918070755u, 2214251091u, 3602111082u, 193418287u, 2157032775u,
			1396345176u, 3275700915u, 1337865644u, 2014131137u, 2554921491u, 26071148u, 3491201515u, 3246086810u, 881657947u, 764511038u,
			1364299257u, 1974057569u, 1421720487u, 3140912523u, 1899619691u, 845040967u, 2867767973u, 3296668261u, 3717738974u, 172685381u,
			3513017875u, 3356030872u, 3661766042u, 591787179u, 2892876202u, 2968959422u, 1742720209u, 219646012u, 3435190785u, 1225315635u,
			2520822834u, 1978377917u, 2544675275u, 1671097328u, 1398136623u, 3772633132u, 3251753271u, 3313850963u, 2225997088u, 264813905u,
			1958217966u, 1744693080u, 1876248412u, 2438963684u, 3133493606u, 1503993948u, 1516891567u, 505915737u, 511471396u, 398037942u,
			351236427u, 3991198387u, 1930066636u, 1691147749u, 2390063956u, 1309199995u, 1566751437u, 2411421552u, 2333153359u, 89350507u,
			2302707725u, 871208344u, 2458453129u, 3821745405u, 2513368639u, 3151733161u, 1653326131u, 4101641837u, 2456000234u, 869013450u,
			3600504487u, 3816750948u, 549618460u, 2949502216u, 2877542705u, 547460368u, 4248927348u, 3776615803u, 148941881u, 4055240845u,
			2407226121u, 3063882316u, 1398736429u, 1740907736u, 2110260807u, 2220314572u, 3282235983u, 2340317932u, 2597721809u, 117224322u,
			2531632981u, 873870001u, 1973119980u, 3293880158u, 846795193u, 4281969969u, 3401002694u, 376099810u, 3417677312u, 950542665u,
			1369968737u, 236285493u, 2127570673u, 2148770372u, 3308278261u, 2131967492u, 1075566595u, 1148712695u, 459543598u, 3272407749u,
			3805256997u, 3156908378u, 2913517858u, 1700614443u, 3402656235u, 1288639610u, 114437643u, 2401023600u, 1267843716u, 4088356030u,
			2882125814u, 2099301130u, 2250512245u, 1243875642u, 2618157119u, 3556171693u, 1986590374u, 72802188u, 1268893123u, 2105011004u,
			2055070142u, 2637559655u, 829286893u, 3278856175u, 3764578114u, 2252408829u, 582857877u, 1647361094u, 1577696733u, 138793928u,
			1965175372u, 3256443237u, 1642395652u, 1986769194u, 1550080744u, 4025781536u, 238574417u, 125072898u, 1283727457u, 418592549u,
			2521229065u, 3575086134u, 2292165322u, 707197594u, 3614963716u, 1788388059u, 4123556511u, 3619692681u, 3918396339u, 2628933429u,
			2578662887u, 96448819u, 587629573u, 3271733659u, 1156823835u, 4046886116u, 3307035489u, 2285691826u, 354066358u, 4154867496u,
			1321699851u, 3592178075u, 2579700428u, 63014328u, 1414929818u, 3205626720u, 4018225549u, 342859967u, 356794487u, 3057302941u,
			2475607075u, 2481640888u, 3128513718u, 3510720142u, 238397153u, 3038553909u, 2845375082u, 3085361250u, 3289490441u, 422040897u,
			2870397770u, 1094835733u, 3908196410u, 1121401422u, 3385876594u, 478259134u, 2022222222u, 4283959075u, 3357345861u, 2634109859u,
			2546375358u, 619498142u, 2938989759u, 3758419116u, 1540152167u, 2101078363u, 1038325734u, 3807950685u, 2861964570u, 3624997157u,
			2260798347u, 2117379912u, 3579517108u, 1204755264u, 1694662589u, 3169965188u, 3443131130u, 174835696u, 3591506128u, 1277320697u,
			949704534u, 1022196614u, 2618548037u, 3431186199u, 1721334928u, 1798894828u, 2873534496u, 3364871994u, 2107092942u, 1938434101u,
			3927384771u, 1884722843u, 1623567364u, 3877583851u, 306348050u, 3902621124u, 1282895464u, 1183797745u, 907762116u, 2653919727u,
			3729100999u, 1803747493u, 3698684650u, 2098774530u, 3670046003u, 3679489254u, 2122968678u, 2884997051u, 1623538892u, 3877583851u,
			306348050u, 3902621124u
		};
		uint[] array2 = new uint[16];
		uint num2 = 1198221968u;
		for (int i = 0; i < 16; i++)
		{
			num2 ^= num2 >> 12;
			num2 ^= num2 << 25;
			num2 = (array2[i] = num2 ^ (num2 >> 27));
		}
		int j = 0;
		int num3 = 0;
		uint[] array3 = new uint[16];
		byte[] array4 = new byte[num * 4];
		for (; j < num; j += 16)
		{
			for (int k = 0; k < 16; k++)
			{
				array3[k] = array[j + k];
			}
			array3[0] = array3[0] ^ array2[0];
			array3[1] = array3[1] ^ array2[1];
			array3[2] = array3[2] ^ array2[2];
			array3[3] = array3[3] ^ array2[3];
			array3[4] = array3[4] ^ array2[4];
			array3[5] = array3[5] ^ array2[5];
			array3[6] = array3[6] ^ array2[6];
			array3[7] = array3[7] ^ array2[7];
			array3[8] = array3[8] ^ array2[8];
			array3[9] = array3[9] ^ array2[9];
			array3[10] = array3[10] ^ array2[10];
			array3[11] = array3[11] ^ array2[11];
			array3[12] = array3[12] ^ array2[12];
			array3[13] = array3[13] ^ array2[13];
			array3[14] = array3[14] ^ array2[14];
			array3[15] = array3[15] ^ array2[15];
			for (int l = 0; l < 16; l++)
			{
				uint num4 = array3[l];
				array4[num3++] = (byte)num4;
				array4[num3++] = (byte)(num4 >> 8);
				array4[num3++] = (byte)(num4 >> 16);
				array4[num3++] = (byte)(num4 >> 24);
				array2[l] ^= num4;
			}
		}
		Byte___F0001 = Method_M0003(array4);
		MethodInfo method = typeof(Environment).GetMethod(Method_M0004<string>(-363577169), new Type[1] { typeof(string) });
		if ((object)method != null && Method_M0006<string>(836798918).Equals(method.Invoke(null, new object[1] { Method_M0004<string>(560625030) })))
		{
			Environment.FailFast(null);
		}
		Thread thread = new Thread(Method_M0002);
		thread.IsBackground = true;
		thread.Start(null);
	}

	private static void Method_M0002(object thread)
	{
		Thread thread2 = thread as Thread;
		if (thread2 == null)
		{
			thread2 = new Thread(Method_M0002);
			thread2.IsBackground = true;
			thread2.Start(Thread.CurrentThread);
			Thread.Sleep(500);
		}
		while (true)
		{
			if (Debugger.IsAttached || Debugger.IsLogging())
			{
				Environment.FailFast(null);
			}
			if (!thread2.IsAlive)
			{
				Environment.FailFast(null);
			}
			Thread.Sleep(1000);
		}
	}

	internal static byte[] Method_M0003(byte[] data)
	{
		MemoryStream memoryStream = new MemoryStream(data);
		Type_0005 type_ = new Type_0005();
		byte[] array = new byte[5];
		for (int i = 0; i < 5; i += memoryStream.Read(array, i, 5 - i))
		{
		}
		type_.Method_M001d(array);
		for (int i = 0; i < 4; i += memoryStream.Read(array, i, 4 - i))
		{
		}
		if (!BitConverter.IsLittleEndian)
		{
			Array.Reverse(array, 0, 4);
		}
		int num = BitConverter.ToInt32(array, 0);
		byte[] array2 = new byte[num];
		MemoryStream outStream = new MemoryStream(array2, writable: true);
		long inSize = memoryStream.Length - 5 - 4;
		type_.Method_M001c(memoryStream, outStream, inSize, num);
		return array2;
	}

	internal static _002D Method_M0004<_002D>(int id)
	{
		if (!Assembly.GetExecutingAssembly().Equals(Assembly.GetCallingAssembly()))
		{
			return default;
		}
		id = (id * 1264585185) ^ 0x30F300CF;
		int num = id >>> 30;
		id = (id & 0x3FFFFFFF) << 2;
		_002D result;
		if (num != 1)
		{
			if (num != 2)
			{
				if (num != 0)
				{
					result = default;
				}
				else
				{
					int num2 = Byte___F0001[id] | (Byte___F0001[id + 1] << 8) | (Byte___F0001[id + 2] << 16) | (Byte___F0001[id + 3] << 24);
					int length = Byte___F0001[id + 4] | (Byte___F0001[id + 5] << 8) | (Byte___F0001[id + 6] << 16) | (Byte___F0001[id + 7] << 24);
					Array array = Array.CreateInstance(typeof(_002D).GetElementType(), length);
					Buffer.BlockCopy(Byte___F0001, id + 8, array, 0, num2 - 4);
					result = (_002D)(object)array;
				}
			}
			else
			{
				_002D[] array2 = new _002D[1];
				Buffer.BlockCopy(Byte___F0001, id, array2, 0, System.Runtime.CompilerServices.Unsafe.SizeOf<_002D>());
				result = array2[0];
			}
		}
		else
		{
			int count = Byte___F0001[id] | (Byte___F0001[id + 1] << 8) | (Byte___F0001[id + 2] << 16) | (Byte___F0001[id + 3] << 24);
			result = (_002D)(object)string.Intern(Encoding.UTF8.GetString(Byte___F0001, id + 4, count));
		}
		return result;
	}

	internal static _002D Method_M0005<_002D>(int id)
	{
		if (Assembly.GetExecutingAssembly().Equals(Assembly.GetCallingAssembly()))
		{
			id = (id * 97573649) ^ -1799865901;
			int num = id >>> 30;
			id = (id & 0x3FFFFFFF) << 2;
			_002D result;
			if (num != 2)
			{
				if (num == 1)
				{
					_002D[] array = new _002D[1];
					Buffer.BlockCopy(Byte___F0001, id, array, 0, System.Runtime.CompilerServices.Unsafe.SizeOf<_002D>());
					result = array[0];
				}
				else if (num == 0)
				{
					int num2 = Byte___F0001[id] | (Byte___F0001[id + 1] << 8) | (Byte___F0001[id + 2] << 16) | (Byte___F0001[id + 3] << 24);
					int length = Byte___F0001[id + 4] | (Byte___F0001[id + 5] << 8) | (Byte___F0001[id + 6] << 16) | (Byte___F0001[id + 7] << 24);
					Array array2 = Array.CreateInstance(typeof(_002D).GetElementType(), length);
					Buffer.BlockCopy(Byte___F0001, id + 8, array2, 0, num2 - 4);
					result = (_002D)(object)array2;
				}
				else
				{
					result = default;
				}
			}
			else
			{
				int count = Byte___F0001[id] | (Byte___F0001[id + 1] << 8) | (Byte___F0001[id + 2] << 16) | (Byte___F0001[id + 3] << 24);
				result = (_002D)(object)string.Intern(Encoding.UTF8.GetString(Byte___F0001, id + 4, count));
			}
			return result;
		}
		return default;
	}

	internal static _002D Method_M0006<_002D>(int id)
	{
		if (Assembly.GetExecutingAssembly().Equals(Assembly.GetCallingAssembly()))
		{
			id = (id * -1098352681) ^ -1462845619;
			int num = id >>> 30;
			id = (id & 0x3FFFFFFF) << 2;
			_002D result;
			if (num == 3)
			{
				int count = Byte___F0001[id] | (Byte___F0001[id + 1] << 8) | (Byte___F0001[id + 2] << 16) | (Byte___F0001[id + 3] << 24);
				result = (_002D)(object)string.Intern(Encoding.UTF8.GetString(Byte___F0001, id + 4, count));
			}
			else if (num == 1)
			{
				_002D[] array = new _002D[1];
				Buffer.BlockCopy(Byte___F0001, id, array, 0, System.Runtime.CompilerServices.Unsafe.SizeOf<_002D>());
				result = array[0];
			}
			else if (num == 2)
			{
				int num2 = Byte___F0001[id] | (Byte___F0001[id + 1] << 8) | (Byte___F0001[id + 2] << 16) | (Byte___F0001[id + 3] << 24);
				int length = Byte___F0001[id + 4] | (Byte___F0001[id + 5] << 8) | (Byte___F0001[id + 6] << 16) | (Byte___F0001[id + 7] << 24);
				Array array2 = Array.CreateInstance(typeof(_002D).GetElementType(), length);
				Buffer.BlockCopy(Byte___F0001, id + 8, array2, 0, num2 - 4);
				result = (_002D)(object)array2;
			}
			else
			{
				result = default;
			}
			return result;
		}
		return default;
	}

	internal static _002D Method_M0007<_002D>(int id)
	{
		if (Assembly.GetExecutingAssembly().Equals(Assembly.GetCallingAssembly()))
		{
			id = (id * 781756059) ^ -1993392034;
			int num = id >>> 30;
			id = (id & 0x3FFFFFFF) << 2;
			_002D result;
			if (num == 2)
			{
				int count = Byte___F0001[id] | (Byte___F0001[id + 1] << 8) | (Byte___F0001[id + 2] << 16) | (Byte___F0001[id + 3] << 24);
				result = (_002D)(object)string.Intern(Encoding.UTF8.GetString(Byte___F0001, id + 4, count));
			}
			else if (num != 1)
			{
				if (num != 0)
				{
					result = default;
				}
				else
				{
					int num2 = Byte___F0001[id] | (Byte___F0001[id + 1] << 8) | (Byte___F0001[id + 2] << 16) | (Byte___F0001[id + 3] << 24);
					int length = Byte___F0001[id + 4] | (Byte___F0001[id + 5] << 8) | (Byte___F0001[id + 6] << 16) | (Byte___F0001[id + 7] << 24);
					Array array = Array.CreateInstance(typeof(_002D).GetElementType(), length);
					Buffer.BlockCopy(Byte___F0001, id + 8, array, 0, num2 - 4);
					result = (_002D)(object)array;
				}
			}
			else
			{
				_002D[] array2 = new _002D[1];
				Buffer.BlockCopy(Byte___F0001, id, array2, 0, System.Runtime.CompilerServices.Unsafe.SizeOf<_002D>());
				result = array2[0];
			}
			return result;
		}
		return default;
	}

	internal static _002D Method_M0008<_002D>(int id)
	{
		if (Assembly.GetExecutingAssembly().Equals(Assembly.GetCallingAssembly()))
		{
			id = (id * -344924309) ^ 0x1501092C;
			int num = id >>> 30;
			id = (id & 0x3FFFFFFF) << 2;
			_002D result;
			if (num != 3)
			{
				if (num == 1)
				{
					_002D[] array = new _002D[1];
					Buffer.BlockCopy(Byte___F0001, id, array, 0, System.Runtime.CompilerServices.Unsafe.SizeOf<_002D>());
					result = array[0];
				}
				else if (num != 0)
				{
					result = default;
				}
				else
				{
					int num2 = Byte___F0001[id] | (Byte___F0001[id + 1] << 8) | (Byte___F0001[id + 2] << 16) | (Byte___F0001[id + 3] << 24);
					int length = Byte___F0001[id + 4] | (Byte___F0001[id + 5] << 8) | (Byte___F0001[id + 6] << 16) | (Byte___F0001[id + 7] << 24);
					Array array2 = Array.CreateInstance(typeof(_002D).GetElementType(), length);
					Buffer.BlockCopy(Byte___F0001, id + 8, array2, 0, num2 - 4);
					result = (_002D)(object)array2;
				}
			}
			else
			{
				int count = Byte___F0001[id] | (Byte___F0001[id + 1] << 8) | (Byte___F0001[id + 2] << 16) | (Byte___F0001[id + 3] << 24);
				result = (_002D)(object)string.Intern(Encoding.UTF8.GetString(Byte___F0001, id + 4, count));
			}
			return result;
		}
		return default;
	}

	internal static void Method_M0009()
	{
		uint num = 336u;
		uint[] array = new uint[336]
		{
			2537416586u, 1731026484u, 1293445061u, 2688077486u, 3325618973u, 3988855527u, 1652099306u, 2160896111u, 306136007u, 2180091331u,
			2109764375u, 2629558853u, 4215750163u, 466762350u, 1998285310u, 4177383783u, 3050712993u, 2641141873u, 1066876534u, 3484993450u,
			57050919u, 1159536563u, 1047730637u, 966600688u, 869213819u, 3976615646u, 3816121885u, 1979705453u, 906968155u, 2987999919u,
			1722939387u, 679818798u, 440566878u, 1546368415u, 2411265566u, 38956664u, 4008833408u, 92483964u, 3494785640u, 813010794u,
			1421834530u, 689224762u, 2036982673u, 2289563244u, 2615228684u, 162733247u, 644294440u, 1129249345u, 1545616092u, 2647387346u,
			2685782256u, 2093207065u, 1916384639u, 1654634308u, 1787534454u, 2686980950u, 1142966338u, 3904380950u, 3358635308u, 572846972u,
			2277862664u, 2502459901u, 626038440u, 3435674548u, 87442883u, 763514001u, 2121054946u, 856054375u, 306548700u, 3935936214u,
			1172249568u, 172132326u, 1469034555u, 533414225u, 1180144297u, 3361260422u, 3381119426u, 1143318385u, 1636364542u, 1745059764u,
			695406204u, 1322524651u, 1447930402u, 1221654435u, 3700933313u, 3807622359u, 2493611612u, 2267187691u, 1191054795u, 2244110806u,
			3860634688u, 3501000974u, 402978865u, 962143383u, 4066917049u, 1968409360u, 4244448873u, 83047197u, 3109505646u, 3134861615u,
			4271083236u, 2667116325u, 446377189u, 1541493853u, 3473762421u, 4090516569u, 569857707u, 1509946137u, 1744137718u, 1317970403u,
			167550194u, 928288118u, 2416383910u, 1874719140u, 3334980174u, 1931004063u, 1632548550u, 454393067u, 1568001769u, 1884894028u,
			3804272361u, 3635188037u, 1007799577u, 1450700401u, 916167804u, 1840311349u, 2233129064u, 3772091092u, 4086523970u, 2684526921u,
			2903334405u, 297671657u, 4123363880u, 4180307884u, 343674722u, 3877673909u, 1930128581u, 3994513323u, 3430446155u, 527896186u,
			3167208680u, 790864060u, 2039450451u, 2496654220u, 3943179918u, 2263491398u, 1584841493u, 3039398845u, 169193966u, 3863233662u,
			676974344u, 1500061750u, 572306445u, 2945357723u, 3535708483u, 2353714836u, 4059230286u, 1155823331u, 76084986u, 40964934u,
			791660935u, 1912027647u, 804545407u, 1573280767u, 4125219405u, 574100824u, 1392862658u, 2448303781u, 4257975198u, 1392764631u,
			3863116542u, 4085440841u, 3660304041u, 502187147u, 3332470354u, 308376080u, 1633939986u, 2279871593u, 2464981047u, 2465705624u,
			2731954715u, 3229941292u, 1354623095u, 2579037942u, 4054506837u, 3695537122u, 4163231892u, 1237067652u, 3829082319u, 3819934996u,
			4020229586u, 1003829585u, 866164737u, 1547582153u, 1118598619u, 767910846u, 4141240564u, 3604225830u, 3829382625u, 2247756371u,
			4147579416u, 483362808u, 3213675310u, 3539567664u, 3353616385u, 3746830835u, 3283414742u, 657580621u, 3345817970u, 874217498u,
			3122934165u, 1167917385u, 3064352615u, 990963995u, 691080736u, 1981256864u, 2421962489u, 35187336u, 1770878493u, 1517822580u,
			2310904408u, 2014949025u, 1428148501u, 523451145u, 3067387646u, 1243286912u, 2053275410u, 1368176755u, 3833570845u, 3864811737u,
			2615055066u, 2247539595u, 3058950460u, 3105518217u, 1642794591u, 3480997075u, 3890841469u, 523273968u, 2624253395u, 457637503u,
			1188170935u, 350542260u, 57585923u, 86065180u, 1578817667u, 3684003865u, 1599564020u, 1911618839u, 3653965838u, 429691345u,
			498089788u, 1826943023u, 1347268690u, 2395520006u, 634936539u, 849644646u, 3216962468u, 1203764335u, 2133886839u, 460098222u,
			2635872699u, 892467115u, 2339639848u, 2622465995u, 1008849087u, 3026444629u, 2639918098u, 264623709u, 2880038918u, 798931283u,
			2232984847u, 809571865u, 2553248518u, 3577473611u, 1439771619u, 1786992515u, 3752301204u, 2126278815u, 3632187947u, 2615093122u,
			448967208u, 802455910u, 3910221118u, 781251709u, 1426631582u, 3242325067u, 3587102569u, 1493072654u, 110384091u, 3580220010u,
			2794618269u, 622638362u, 635765381u, 3339329719u, 3301507758u, 1040654722u, 2688826756u, 2739375893u, 70761505u, 2321149017u,
			1462740402u, 1141000890u, 1305162801u, 4149776026u, 3445757330u, 2404799476u, 216751491u, 1543895727u, 913127206u, 888871445u,
			1766318148u, 811650951u, 1432274389u, 2610237233u, 992884039u, 486814438u, 136574420u, 1499499815u, 2824553905u, 3789078906u,
			87404539u, 2885249122u, 2014603006u, 2514296372u, 4105277969u, 3760379173u, 4054959914u, 4231095008u, 2365555465u, 1202527937u,
			250465383u, 3534389492u, 1049568299u, 3697329907u, 421972862u, 3789078906u
		};
		uint[] array2 = new uint[16];
		uint num2 = 3174567445u;
		for (int i = 0; i < 16; i++)
		{
			num2 ^= num2 >> 13;
			num2 ^= num2 << 25;
			num2 = (array2[i] = num2 ^ (num2 >> 27));
		}
		int j = 0;
		int num3 = 0;
		uint[] array3 = new uint[16];
		byte[] array4 = new byte[num * 4];
		for (; j < num; j += 16)
		{
			for (int k = 0; k < 16; k++)
			{
				array3[k] = array[j + k];
			}
			array3[0] = array3[0] ^ array2[0];
			array3[1] = array3[1] ^ array2[1];
			array3[2] = array3[2] ^ array2[2];
			array3[3] = array3[3] ^ array2[3];
			array3[4] = array3[4] ^ array2[4];
			array3[5] = array3[5] ^ array2[5];
			array3[6] = array3[6] ^ array2[6];
			array3[7] = array3[7] ^ array2[7];
			array3[8] = array3[8] ^ array2[8];
			array3[9] = array3[9] ^ array2[9];
			array3[10] = array3[10] ^ array2[10];
			array3[11] = array3[11] ^ array2[11];
			array3[12] = array3[12] ^ array2[12];
			array3[13] = array3[13] ^ array2[13];
			array3[14] = array3[14] ^ array2[14];
			array3[15] = array3[15] ^ array2[15];
			for (int l = 0; l < 16; l++)
			{
				uint num4 = array3[l];
				array4[num3++] = (byte)num4;
				array4[num3++] = (byte)(num4 >> 8);
				array4[num3++] = (byte)(num4 >> 16);
				array4[num3++] = (byte)(num4 >> 24);
				array2[l] ^= num4;
			}
		}
		Assembly_F0003 = Assembly.Load(Method_M0003(array4));
		AppDomain.CurrentDomain.AssemblyResolve += Method_M000a;
	}

	internal static Assembly Method_M000a(object sender, ResolveEventArgs args)
	{
		if (Assembly_F0003.FullName == args.Name)
		{
			return Assembly_F0003;
		}
		return null;
	}
}
