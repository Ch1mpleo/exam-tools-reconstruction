using System;
using Ionic.Zip;

namespace GradeLib;

public class GivenExtract
{
	// [behavior 0x06000001] Reads a ZIP and extracts entries to the selected directory with overwrite enabled. Returns true
	// [behavior 0x06000001] on completion; exceptions set the ref log message and return false.
	public static bool Extract(string _pathZipFile, string _pathUnpack, ref string log)
	{
		try
		{
			using (ZipFile zipFile = ZipFile.Read(_pathZipFile))
			{
				foreach (ZipEntry item in zipFile)
				{
					item.Extract(_pathUnpack, ExtractExistingFileAction.OverwriteSilently);
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			log = ex.Message;
			return false;
		}
	}
}
