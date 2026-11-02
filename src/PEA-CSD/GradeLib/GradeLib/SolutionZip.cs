using System.IO;
using Ionic.Zip;

namespace GradeLib;

public class SolutionZip
{
	// [behavior 0x06000021] Checks the solution directory, sums all recursive file sizes and rejects zero bytes or more than
	// [behavior 0x06000021] 10 MiB. Saves the directory as a ZIP and returns the ZIP file bytes.
	public static byte[] CreateZip(string solutionPath, string zipFileName)
	{
		if (Directory.Exists(solutionPath))
		{
			string[] files = Directory.GetFiles(solutionPath, "*", SearchOption.AllDirectories);
			long num = 0L;
			string[] array = files;
			foreach (string fileName in array)
			{
				FileInfo fileInfo = new FileInfo(fileName);
				num += fileInfo.Length;
			}
			if (num > 10485760 || num == 0)
			{
				return null;
			}
			using (ZipFile zipFile = new ZipFile())
			{
				zipFile.AddDirectory(solutionPath);
				zipFile.Comment = "comment";
				zipFile.Save(zipFileName);
			}
			return File.ReadAllBytes(zipFileName);
		}
		return null;
	}
}
