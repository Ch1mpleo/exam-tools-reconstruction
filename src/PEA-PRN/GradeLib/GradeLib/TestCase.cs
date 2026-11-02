using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GradeLib;

[Serializable]
public class TestCase
{
	public string TestCaseCode { get; set; }

	public List<string> Input { get; set; }

	public string Output { get; set; }

	public string OutputFile { get; set; }

	public byte[] CorrectOutputFile { get; set; }

	public byte[] DataFile { get; set; }

	public float Mark { get; set; }

	public bool IsCaseSensitive { get; set; }

	public bool RemoveSpaces { get; set; }

	public TestCase()
	{
		Input = new List<string>();
		Output = "";
		OutputFile = "";
		CorrectOutputFile = new byte[0];
		DataFile = new byte[0];
	}

	// [behavior 0x0600004e] Returns Input.Count > 0 && Mark > 0. This validates a test-case record, not a student program.
	public bool Validate()
	{
		return Input.Count > 0 && Mark > 0f;
	}

	// [behavior 0x0600004f] Parses text section markers into a TestCase. Records input/output, whitespace and case flags,
	// [behavior 0x0600004f] and mark; parsing stops after the mark. PRN additionally resolves data/output file references.
	// [behavior 0x0600004f] See this exact build-specific body.
	private static TestCase File2TestCase(string filePath)
	{
		string line = null;
		TestCase tc = new TestCase();
		if (Path.GetExtension(filePath).Equals(".txt", StringComparison.OrdinalIgnoreCase))
		{
			StreamReader sr = new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read));
			string flag = "";
			while ((line = sr.ReadLine()) != null)
			{
				line = line.Trim();
				if (line.ToUpper().StartsWith("INPUT:"))
				{
					flag = "INPUT:";
					continue;
				}
				if (line.ToUpper().StartsWith("DATA_FILE:"))
				{
					flag = "DATA_FILE:";
					continue;
				}
				if (line.ToUpper().StartsWith("OUTPUT:"))
				{
					flag = "OUTPUT:";
					continue;
				}
				if (line.ToUpper().StartsWith("OUTPUT_FILE:"))
				{
					flag = "OUTPUT_FILE:";
					continue;
				}
				if (line.ToUpper().StartsWith("CORRECT_OUTPUT_FILE:"))
				{
					flag = "CORRECT_OUTPUT_FILE:";
					continue;
				}
				if (line.ToUpper().StartsWith("REMOVE_SPACES:"))
				{
					flag = "REMOVE_SPACES:";
					continue;
				}
				if (line.ToUpper().StartsWith("CASE_SENSITIVE:"))
				{
					flag = "CASE_SENSITIVE:";
					continue;
				}
				if (line.ToUpper().StartsWith("MARK:"))
				{
					flag = "MARK:";
					continue;
				}
				if (flag.Equals("INPUT:"))
				{
					tc.Input.Add(line);
				}
				if (flag.Equals("DATA_FILE:"))
				{
					string fname = line;
					string folder = Path.GetDirectoryName(filePath);
					string dataPath = Path.Combine(folder, fname);
					if (File.Exists(dataPath))
					{
						tc.DataFile = File.ReadAllBytes(dataPath);
					}
				}
				if (flag.Equals("OUTPUT:"))
				{
					tc.Output = tc.Output + line + "\n";
				}
				if (flag.Equals("OUTPUT_FILE:"))
				{
					tc.OutputFile = line;
				}
				if (flag.Equals("CORRECT_OUTPUT_FILE:"))
				{
					string fname2 = line;
					string folder2 = Path.GetDirectoryName(filePath);
					string correctPath = Path.Combine(folder2, fname2);
					if (File.Exists(correctPath))
					{
						tc.CorrectOutputFile = File.ReadAllBytes(correctPath);
					}
				}
				if (flag.Equals("REMOVE_SPACES:"))
				{
					tc.RemoveSpaces = line.ToUpper().Equals("YES");
				}
				if (flag.Equals("CASE_SENSITIVE:"))
				{
					tc.IsCaseSensitive = line.ToUpper().Equals("YES");
				}
				if (!flag.Equals("MARK:"))
				{
					continue;
				}
				tc.Mark = Convert.ToSingle(line);
				break;
			}
			sr.Close();
			tc.Output = tc.Output.Trim();
			tc.TestCaseCode = Path.GetFileName(filePath);
		}
		return tc;
	}

	// [behavior 0x06000050] Build-specific test-case discovery. CSD parses the directory file list. PRN filters text files
	// [behavior 0x06000050] and can traverse matching tc* directories. Calls File2TestCase for selected files.
	public static List<TestCase> LoadTestCase(string tcFolder)
	{
		List<TestCase> listTestCase = new List<TestCase>();
		if (DetectTestCasesContainTCFolder(tcFolder))
		{
			string[] subDirs = Directory.GetDirectories(tcFolder, "tc*", SearchOption.TopDirectoryOnly);
			foreach (string dir in subDirs.OrderBy((string d) => d))
			{
				string[] txtFiles = Directory.GetFiles(dir, "tc*.txt", SearchOption.TopDirectoryOnly);
				TestCase tc = null;
				string[] array = txtFiles;
				foreach (string f in array)
				{
					if (Path.GetExtension(f).Equals(".txt", StringComparison.OrdinalIgnoreCase))
					{
						tc = File2TestCase(f);
						listTestCase.Add(tc);
					}
				}
			}
		}
		else
		{
			string[] files = Directory.GetFiles(tcFolder);
			TestCase tc2 = null;
			string[] array2 = files;
			foreach (string f2 in array2)
			{
				if (Path.GetExtension(f2).Equals(".txt", StringComparison.OrdinalIgnoreCase))
				{
					tc2 = File2TestCase(f2);
					listTestCase.Add(tc2);
				}
			}
		}
		return listTestCase;
	}

	// [behavior 0x06000051] Returns true when the directory exists, contains tc* subdirectories, and has no top-level
	// [behavior 0x06000051] tc*.txt files. Chooses the alternate PRN discovery layout.
	private static bool DetectTestCasesContainTCFolder(string testCasesPath)
	{
		if (!Directory.Exists(testCasesPath))
		{
			return false;
		}
		string[] subDirs = Directory.GetDirectories(testCasesPath, "tc*", SearchOption.TopDirectoryOnly);
		string[] txtFiles = Directory.GetFiles(testCasesPath, "tc*.txt", SearchOption.TopDirectoryOnly);
		if (subDirs.Length != 0 && txtFiles.Length == 0)
		{
			return true;
		}
		return false;
	}

	// [behavior 0x06000052] Throws FileNotFoundException for a missing path; otherwise returns File.ReadAllBytes. It does
	// [behavior 0x06000052] not parse ZIP contents.
	public static byte[] ConvertZipToByteArray(string zipFilePath)
	{
		if (!File.Exists(zipFilePath))
		{
			throw new FileNotFoundException("Zip file not found", zipFilePath);
		}
		return File.ReadAllBytes(zipFilePath);
	}
}
