using System;

namespace QuestionLib;

[Serializable]
public class ImagePaperAnswer
{
	public string Answer { get; set; }

	public int PartCount { get; set; }

	public bool IsLongAnswer { get; set; }

	public float SectionMark { get; set; }

	public bool SectionBoolean { get; set; }

	public int NoOfSection { get; set; }
}
