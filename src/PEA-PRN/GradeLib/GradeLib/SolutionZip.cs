using System.IO;
using Ionic.Zip;

namespace GradeLib;

public class SolutionZip
{
	// [behavior 0x0600002c] Checks the solution directory, sums all recursive file sizes and rejects zero bytes or more than
	// [behavior 0x0600002c] 10 MiB. Saves the directory as a ZIP and returns the ZIP file bytes.
	public static byte[] CreateZip(string solutionPath, string zipFileName)
	{
		if (Directory.Exists(solutionPath))
		{
			string[] files = Directory.GetFiles(solutionPath, "*", SearchOption.AllDirectories);
			long folderSize = 0L;
			string[] array = files;
			foreach (string file in array)
			{
				FileInfo fileInfo = new FileInfo(file);
				folderSize += fileInfo.Length;
			}
			if (folderSize > 20971520 || folderSize == 0)
			{
				return null;
			}
			using (ZipFile zip = new ZipFile())
			{
				zip.AddDirectory(solutionPath);
				zip.Comment = "comment";
				zip.Save(zipFileName);
			}
			return File.ReadAllBytes(zipFileName);
		}
		return null;
	}
}
