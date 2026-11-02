using System;
using System.Collections.Generic;
using System.IO;

namespace GradeLib;

[Serializable]
public class TestCase
{
	public string TestCaseCode { get; set; }

	public List<string> Input { get; set; }

	public string Output { get; set; }

	public float Mark { get; set; }

	public bool IsCaseSensitive { get; set; }

	public bool RemoveSpaces { get; set; }

	public string DataFile { get; set; }

	public string CorrectOutputFile { get; set; }

	public byte[] data { get; set; }

	public byte[] CorrectOutput { get; set; }

	public TestCase()
	{
		Input = new List<string>();
		Output = "";
	}

	// [behavior 0x06000045] Returns Input.Count > 0 && Mark > 0. This validates a test-case record, not a student program.
	public bool Validate()
	{
		return Input.Count > 0 && Mark > 0f;
	}

	// [behavior 0x06000046] Parses text section markers into a TestCase. Records input/output, whitespace and case flags,
	// [behavior 0x06000046] and mark; parsing stops after the mark. PRN additionally resolves data/output file references.
	// [behavior 0x06000046] See this exact build-specific body.
	private static TestCase File2TestCase(string filePath)
	{
		string text = null;
		TestCase testCase = new TestCase();
		StreamReader streamReader = new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read));
		string text2 = "";
		while ((text = streamReader.ReadLine()) != null)
		{
			text = text.Trim();
			if (text.ToUpper().StartsWith("INPUT:"))
			{
				text2 = "INPUT:";
				continue;
			}
			if (text.ToUpper().StartsWith("OUTPUT:"))
			{
				text2 = "OUTPUT:";
				continue;
			}
			if (text.ToUpper().StartsWith("REMOVE_SPACES:"))
			{
				text2 = "REMOVE_SPACES:";
				continue;
			}
			if (text.ToUpper().StartsWith("CASE_SENSITIVE:"))
			{
				text2 = "CASE_SENSITIVE:";
				continue;
			}
			if (text.ToUpper().StartsWith("MARK:"))
			{
				text2 = "MARK:";
				continue;
			}
			if (text2.Equals("INPUT:"))
			{
				testCase.Input.Add(text);
			}
			if (text2.Equals("OUTPUT:"))
			{
				testCase.Output = testCase.Output + text + "\n";
			}
			if (text2.Equals("REMOVE_SPACES:"))
			{
				testCase.RemoveSpaces = text.ToUpper().Equals("YES");
			}
			if (text2.Equals("CASE_SENSITIVE:"))
			{
				testCase.IsCaseSensitive = text.ToUpper().Equals("YES");
			}
			if (!text2.Equals("MARK:"))
			{
				continue;
			}
			testCase.Mark = Convert.ToSingle(text);
			break;
		}
		streamReader.Close();
		testCase.Output = testCase.Output.Trim();
		testCase.TestCaseCode = Path.GetFileName(filePath);
		return testCase;
	}

	// [behavior 0x06000047] Build-specific test-case discovery. CSD parses the directory file list. PRN filters text files
	// [behavior 0x06000047] and can traverse matching tc* directories. Calls File2TestCase for selected files.
	public static List<TestCase> LoadTestCase(string tcFolder)
	{
		List<TestCase> list = new List<TestCase>();
		string[] files = Directory.GetFiles(tcFolder);
		TestCase testCase = null;
		string[] array = files;
		foreach (string filePath in array)
		{
			testCase = File2TestCase(filePath);
			list.Add(testCase);
		}
		return list;
	}

	// [behavior 0x06000048] CSD-specific directory parser. Recognizes data.txt and the numbered reference-output file, then
	// [behavior 0x06000048] parses metadata sections from other files. Preserves the original filename and stream-handling
	// [behavior 0x06000048] quirks.
	public static TestCase File2TestCaseCSD(string tcFolder, int count)
	{
		TestCase testCase = new TestCase();
		string[] files = Directory.GetFiles(tcFolder);
		string[] array = files;
		foreach (string text in array)
		{
			string text2 = null;
			StreamReader streamReader = new StreamReader(new FileStream(text, FileMode.Open, FileAccess.Read));
			string text3 = "";
			if (text.Substring(text.LastIndexOf("\\") + 1).Equals("data.txt"))
			{
				testCase.data = File.ReadAllBytes(text);
				continue;
			}
			if (text.Substring(text.LastIndexOf("\\") + 1).Equals("R_f" + count + ".txt"))
			{
				testCase.CorrectOutput = File.ReadAllBytes(text);
				continue;
			}
			while ((text2 = streamReader.ReadLine()) != null)
			{
				text2 = text2.Trim();
				if (text2.ToUpper().StartsWith("INPUT:"))
				{
					text3 = "INPUT:";
					continue;
				}
				if (text2.ToUpper().StartsWith("OUTPUT FILE:"))
				{
					text3 = "OUTPUT FILE:";
					continue;
				}
				if (text2.ToUpper().StartsWith("REMOVE_SPACES:"))
				{
					text3 = "REMOVE_SPACES:";
					continue;
				}
				if (text2.ToUpper().StartsWith("CASE_SENSITIVE:"))
				{
					text3 = "CASE_SENSITIVE:";
					continue;
				}
				if (text2.ToUpper().StartsWith("MARK"))
				{
					text3 = "MARK:";
					continue;
				}
				if (text2.ToUpper().StartsWith("DATA FILE:"))
				{
					text3 = "DATA FILE:";
					continue;
				}
				if (text2.ToUpper().StartsWith("CORRECT OUTPUT FILE:"))
				{
					text3 = "CORRECT OUTPUT FILE:";
					continue;
				}
				if (text3.Equals("INPUT:"))
				{
					testCase.Input.Add(text2);
				}
				if (text3.Equals("OUTPUT FILE:"))
				{
					testCase.Output = testCase.Output + text2 + "\n";
				}
				if (text3.Equals("REMOVE_SPACES:"))
				{
					testCase.RemoveSpaces = text2.ToUpper().Equals("YES");
				}
				if (text3.Equals("CASE_SENSITIVE:"))
				{
					testCase.IsCaseSensitive = text2.ToUpper().Equals("YES");
				}
				if (text3.Equals("CORRECT OUTPUT FILE:"))
				{
					testCase.CorrectOutputFile = testCase.CorrectOutputFile + text2 + "\n";
				}
				if (text3.Equals("DATA FILE:"))
				{
					testCase.DataFile = testCase.DataFile + text2 + "\n";
				}
				if (!text3.Equals("MARK:"))
				{
					continue;
				}
				testCase.Mark = Convert.ToSingle(text2);
				break;
			}
			streamReader.Close();
			testCase.TestCaseCode = tcFolder.Substring(text.LastIndexOf("\\"));
		}
		return testCase;
	}
}
