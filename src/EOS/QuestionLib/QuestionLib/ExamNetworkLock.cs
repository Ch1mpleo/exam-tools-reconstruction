using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace QuestionLib;

public static class ExamNetworkLock
{
	private struct MIB_TCPROW_OWNER_PID
	{
		public uint dwState;

		public uint dwLocalAddr;

		public uint dwLocalPort;

		public uint dwRemoteAddr;

		public uint dwRemotePort;

		public uint dwOwningPid;
	}

	private struct MIB_TCPROW
	{
		public uint dwState;

		public uint dwLocalAddr;

		public uint dwLocalPort;

		public uint dwRemoteAddr;

		public uint dwRemotePort;
	}

	private struct MIB_TCP6ROW_OWNER_PID
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] ucLocalAddr;

		public uint dwLocalScopeId;

		public uint dwLocalPort;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] ucRemoteAddr;

		public uint dwRemoteScopeId;

		public uint dwRemotePort;

		public uint dwState;

		public uint dwOwningPid;
	}

	private const int AF_INET = 2;

	private const int AF_INET6 = 23;

	private const int TCP_TABLE_OWNER_PID_ALL = 5;

	private const int DefaultIntervalMs = 3000;

	private const uint MIB_TCP_STATE_LISTEN = 2u;

	private const uint MIB_TCP_STATE_SYN_SENT = 3u;

	private const uint MIB_TCP_STATE_ESTAB = 5u;

	private const uint MIB_TCP_STATE_DELETE_TCB = 12u;

	private static readonly string[] CriticalProcessNames = new string[17]
	{
		"smss.exe", "csrss.exe", "wininit.exe", "winlogon.exe", "services.exe", "lsass.exe", "svchost.exe", "dwm.exe", "fontdrvhost.exe", "sihost.exe",
		"taskhostw.exe", "ctfmon.exe", "runtimebroker.exe", "dllhost.exe", "conhost.exe", "spoolsv.exe", "lsaiso.exe"
	};

	private static readonly HashSet<string> CriticalProcessPaths = BuildCriticalProcessPaths();

	private static readonly object sync = new object();

	private static Timer sweepTimer;

	private static int excludedPid;

	private static uint? excludedServerAddr;

	private static int sweeping;

	[DllImport("iphlpapi.dll", SetLastError = true)]
	// [behavior 0x06000014] Declaration without a managed method body. For interfaces, the implementation belongs to an
	// [behavior 0x06000014] implementing server/client, not to this contract DLL. P/Invoke declarations identify an external
	// [behavior 0x06000014] native boundary.
	private static extern uint GetExtendedTcpTable(IntPtr pTcpTable, ref int dwOutBufLen, bool sort, int ipVersion, int tblClass, int reserved);

	[DllImport("iphlpapi.dll")]
	// [behavior 0x06000015] Declaration without a managed method body. For interfaces, the implementation belongs to an
	// [behavior 0x06000015] implementing server/client, not to this contract DLL. P/Invoke declarations identify an external
	// [behavior 0x06000015] native boundary.
	private static extern int SetTcpEntry(IntPtr pTcprow);

	// [behavior 0x06000016] Builds an exclusion set from named system processes under Windows system directories and the
	// [behavior 0x06000016] Windows Explorer path.
	private static HashSet<string> BuildCriticalProcessPaths()
	{
		HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		string environmentVariable = Environment.GetEnvironmentVariable("windir");
		string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
		string path = Path.Combine(environmentVariable, "SysWOW64");
		string[] criticalProcessNames = CriticalProcessNames;
		foreach (string path2 in criticalProcessNames)
		{
			hashSet.Add(Path.Combine(folderPath, path2));
			hashSet.Add(Path.Combine(path, path2));
		}
		hashSet.Add(Path.Combine(environmentVariable, "explorer.exe"));
		return hashSet;
	}

	// [behavior 0x06000017] Records the current process and optional exam-server IPv4 exclusion; creates a periodic sweep
	// [behavior 0x06000017] timer only if absent. The default interval is 3000 ms. Activation by a runtime GUI remains
	// [behavior 0x06000017] unresolved.
	public static void Start(string examServerIp)
	{
		Start(examServerIp, 3000);
	}

	// [behavior 0x06000017] Records the current process and optional exam-server IPv4 exclusion; creates a periodic sweep
	// [behavior 0x06000017] timer only if absent. The default interval is 3000 ms. Activation by a runtime GUI remains
	// [behavior 0x06000017] unresolved.
	public static void Start(string examServerIp, int intervalMs)
	{
		lock (sync)
		{
			excludedPid = Process.GetCurrentProcess().Id;
			excludedServerAddr = (string.IsNullOrEmpty(examServerIp) ? ((uint?)null) : new uint?(IpStringToUInt32(examServerIp)));
			if (sweepTimer == null)
			{
				sweepTimer = new Timer(delegate
				{
					SweepSafe();
				}, null, 0, intervalMs);
			}
		}
	}

	// [behavior 0x06000019] Disposes and clears the network sweep timer under its synchronization lock.
	public static void Stop()
	{
		lock (sync)
		{
			if (sweepTimer != null)
			{
				sweepTimer.Dispose();
				sweepTimer = null;
			}
		}
	}

	// [behavior 0x0600001a] Uses an Interlocked concurrency guard, separately attempts IPv4 and IPv6 sweeps and suppresses
	// [behavior 0x0600001a] their exceptions, then resets the guard.
	private static void SweepSafe()
	{
		if (Interlocked.CompareExchange(ref sweeping, 1, 0) == 0)
		{
			try
			{
				SweepIpv4();
			}
			catch
			{
			}
			try
			{
				SweepIpv6();
			}
			catch
			{
			}
			Interlocked.Exchange(ref sweeping, 0);
		}
	}

	// [behavior 0x0600001b] Reads owner-PID IPv4 TCP rows, applies state/process/address exclusions and may ask SetTcpEntry
	// [behavior 0x0600001b] to delete a connection. Allocates/frees each native row buffer. This method was not executed.
	private static void SweepIpv4()
	{
		foreach (MIB_TCPROW_OWNER_PID ipv4Row in GetIpv4Rows())
		{
			if ((ipv4Row.dwState != 5 && ipv4Row.dwState != 3) || ipv4Row.dwOwningPid == (uint)excludedPid || (excludedServerAddr.HasValue && ipv4Row.dwRemoteAddr == excludedServerAddr.Value) || IsPrivateOrLocalIpv4(ipv4Row.dwRemoteAddr))
			{
				continue;
			}
			MIB_TCPROW mIB_TCPROW = new MIB_TCPROW
			{
				dwState = 12u,
				dwLocalAddr = ipv4Row.dwLocalAddr,
				dwLocalPort = ipv4Row.dwLocalPort,
				dwRemoteAddr = ipv4Row.dwRemoteAddr,
				dwRemotePort = ipv4Row.dwRemotePort
			};
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MIB_TCPROW)));
			try
			{
				Marshal.StructureToPtr((object)mIB_TCPROW, intPtr, false);
				SetTcpEntry(intPtr);
			}
			catch
			{
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
	}

	// [behavior 0x0600001c] Classifies selected local/private IPv4 ranges from the integer representation; its exact
	// [behavior 0x0600001c] boundary cases are shown below.
	private static bool IsPrivateOrLocalIpv4(uint addr)
	{
		byte b = (byte)(addr & 0xFF);
		byte b2 = (byte)((addr >> 8) & 0xFF);
		switch (b)
		{
		case 0:
			return true;
		case 127:
			return true;
		case 10:
			return true;
		case 172:
			if (b2 >= 16 && b2 <= 31)
			{
				return true;
			}
			break;
		}
		if (b == 192 && b2 == 168)
		{
			return true;
		}
		if (b == 169 && b2 == 254)
		{
			return true;
		}
		return false;
	}

	// [behavior 0x0600001d] Splits a dotted IPv4 string, parses four components, casts them to bytes and converts them using
	// [behavior 0x0600001d] BitConverter. Returns zero on the explicit parsing failures.
	private static uint IpStringToUInt32(string ip)
	{
		string[] array = ip.Trim().Split('.');
		if (array.Length != 4)
		{
			return 0u;
		}
		byte[] array2 = new byte[4];
		for (int i = 0; i < 4; i++)
		{
			if (!int.TryParse(array[i], out var result))
			{
				return 0u;
			}
			array2[i] = (byte)result;
		}
		return BitConverter.ToUInt32(array2, 0);
	}

	// [behavior 0x0600001e] Queries GetExtendedTcpTable for required IPv4 buffer length, allocates unmanaged memory,
	// [behavior 0x0600001e] marshals owner-PID rows and frees the buffer.
	private static List<MIB_TCPROW_OWNER_PID> GetIpv4Rows()
	{
		List<MIB_TCPROW_OWNER_PID> list = new List<MIB_TCPROW_OWNER_PID>();
		int dwOutBufLen = 0;
		GetExtendedTcpTable(IntPtr.Zero, ref dwOutBufLen, sort: false, 2, 5, 0);
		if (dwOutBufLen <= 0)
		{
			return list;
		}
		IntPtr intPtr = Marshal.AllocHGlobal(dwOutBufLen);
		try
		{
			uint extendedTcpTable = GetExtendedTcpTable(intPtr, ref dwOutBufLen, sort: false, 2, 5, 0);
			if (extendedTcpTable != 0)
			{
				return list;
			}
			int num = Marshal.ReadInt32(intPtr);
			IntPtr intPtr2 = (IntPtr)((long)intPtr + 4);
			int num2 = Marshal.SizeOf(typeof(MIB_TCPROW_OWNER_PID));
			for (int i = 0; i < num; i++)
			{
				list.Add((MIB_TCPROW_OWNER_PID)Marshal.PtrToStructure(intPtr2, typeof(MIB_TCPROW_OWNER_PID)));
				intPtr2 = (IntPtr)((long)intPtr2 + num2);
			}
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
		return list;
	}

	// [behavior 0x0600001f] Examines IPv6 rows and global-address/process conditions; may kill an owning process outside its
	// [behavior 0x0600001f] exclusions. This method was not executed.
	private static void SweepIpv6()
	{
		foreach (MIB_TCP6ROW_OWNER_PID ipv6Row in GetIpv6Rows())
		{
			if (ipv6Row.dwState == 2 || !IsGlobalIpv6(ipv6Row.ucRemoteAddr))
			{
				continue;
			}
			int dwOwningPid = (int)ipv6Row.dwOwningPid;
			if (dwOwningPid <= 4 || dwOwningPid == excludedPid)
			{
				continue;
			}
			Process processById;
			try
			{
				processById = Process.GetProcessById(dwOwningPid);
			}
			catch
			{
				continue;
			}
			string fileName;
			try
			{
				fileName = processById.MainModule.FileName;
			}
			catch
			{
				continue;
			}
			if (!CriticalProcessPaths.Contains(fileName))
			{
				try
				{
					processById.Kill();
				}
				catch
				{
				}
			}
		}
	}

	// [behavior 0x06000020] Excludes unspecified/loopback, link-local, unique-local and multicast address patterns through
	// [behavior 0x06000020] byte tests; returns true for the remaining patterns.
	private static bool IsGlobalIpv6(byte[] addr)
	{
		bool flag = true;
		for (int i = 0; i < 15; i++)
		{
			if (addr[i] != 0)
			{
				flag = false;
				break;
			}
		}
		if (flag && addr[15] <= 1)
		{
			return false;
		}
		if (addr[0] == 254 && (addr[1] & 0xC0) == 128)
		{
			return false;
		}
		if ((addr[0] & 0xFE) == 252)
		{
			return false;
		}
		if (addr[0] == byte.MaxValue)
		{
			return false;
		}
		return true;
	}

	// [behavior 0x06000021] Queries GetExtendedTcpTable for required IPv6 buffer length, allocates unmanaged memory,
	// [behavior 0x06000021] marshals owner-PID rows and frees the buffer.
	private static List<MIB_TCP6ROW_OWNER_PID> GetIpv6Rows()
	{
		List<MIB_TCP6ROW_OWNER_PID> list = new List<MIB_TCP6ROW_OWNER_PID>();
		int dwOutBufLen = 0;
		GetExtendedTcpTable(IntPtr.Zero, ref dwOutBufLen, sort: false, 23, 5, 0);
		if (dwOutBufLen <= 0)
		{
			return list;
		}
		IntPtr intPtr = Marshal.AllocHGlobal(dwOutBufLen);
		try
		{
			uint extendedTcpTable = GetExtendedTcpTable(intPtr, ref dwOutBufLen, sort: false, 23, 5, 0);
			if (extendedTcpTable != 0)
			{
				return list;
			}
			int num = Marshal.ReadInt32(intPtr);
			IntPtr intPtr2 = (IntPtr)((long)intPtr + 4);
			int num2 = Marshal.SizeOf(typeof(MIB_TCP6ROW_OWNER_PID));
			for (int i = 0; i < num; i++)
			{
				list.Add((MIB_TCP6ROW_OWNER_PID)Marshal.PtrToStructure(intPtr2, typeof(MIB_TCP6ROW_OWNER_PID)));
				intPtr2 = (IntPtr)((long)intPtr2 + num2);
			}
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
		return list;
	}
}
