using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace QuestionLib;

public static class ExamAppLock
{
	private const int DefaultIntervalMs = 3000;

	private static readonly object sync = new object();

	private static Timer sweepTimer;

	private static int excludedPid;

	private static int sweeping;

	// [behavior 0x06000023] Starts a periodic application sweep if no timer exists. Stores the current process ID as an
	// [behavior 0x06000023] exclusion. Overload without an interval uses 3000 ms. Existence does not prove activation by
	// [behavior 0x06000023] this shell.
	public static void Start()
	{
		Start(3000);
	}

	// [behavior 0x06000023] Starts a periodic application sweep if no timer exists. Stores the current process ID as an
	// [behavior 0x06000023] exclusion. Overload without an interval uses 3000 ms. Existence does not prove activation by
	// [behavior 0x06000023] this shell.
	public static void Start(int intervalMs)
	{
		lock (sync)
		{
			excludedPid = Process.GetCurrentProcess().Id;
			if (sweepTimer == null)
			{
				sweepTimer = new Timer(delegate
				{
					SweepSafe();
				}, null, 0, intervalMs);
			}
		}
	}

	// [behavior 0x06000025] Under the shared lock, disposes the sweep timer and clears the reference.
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

	// [behavior 0x06000026] Uses an Interlocked guard to prevent concurrent sweeps, suppresses sweep exceptions, and resets
	// [behavior 0x06000026] the guard in finally.
	private static void SweepSafe()
	{
		if (Interlocked.CompareExchange(ref sweeping, 1, 0) != 0)
		{
			return;
		}
		try
		{
			Sweep();
		}
		catch
		{
		}
		finally
		{
			Interlocked.Exchange(ref sweeping, 0);
		}
	}

	// [behavior 0x06000027] Enumerates processes. Under the conditions shown, kills another process with a main window while
	// [behavior 0x06000027] excluding this process and explorer.exe. Disposes each Process object. This method was not
	// [behavior 0x06000027] executed during analysis.
	private static void Sweep()
	{
		Process[] processes = Process.GetProcesses();
		foreach (Process process in processes)
		{
			try
			{
				if (process.Id != excludedPid && !(process.MainWindowHandle == IntPtr.Zero))
				{
					string fileName;
					try
					{
						fileName = Path.GetFileName(process.MainModule.FileName);
					}
					catch
					{
						goto end_IL_0014;
					}
					if (!string.Equals(fileName, "explorer.exe", StringComparison.OrdinalIgnoreCase))
					{
						process.Kill();
					}
				}
				end_IL_0014:;
			}
			catch
			{
			}
			finally
			{
				process.Dispose();
			}
		}
	}
}
