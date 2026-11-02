using System;
using System.Collections.Generic;

namespace GradeLib;

[Serializable]
public class Question
{
	public int QuestionCode { get; set; }

	public string Candidate { get; set; }

	public int TopicCode { get; set; }

	public byte[] QuestionFile { get; set; }

	public string FileType { get; set; }

	public List<TestCase> TestCases { get; set; }

	public byte[] CompressedFile { get; set; }

	public float Mark { get; set; }

	public string Author { get; set; }

	public byte[] Given { get; set; }
}
