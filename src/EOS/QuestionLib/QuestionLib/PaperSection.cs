using System;
using System.Collections.Generic;

namespace QuestionLib;

[Serializable]
public class PaperSection
{
	public int SectionNo { get; set; }

	public int QFrom { get; set; }

	public int QTo { get; set; }

	public string InAnyOrderGroup { get; set; }

	public List<ImagePaperAnswer> Answers { get; set; }

	// [behavior 0x0600007d] Returns QTo - QFrom + 1. It does not validate that the range is ordered or nonnegative.
	public int GetAnswerCount()
	{
		return QTo - QFrom + 1;
	}

	public PaperSection()
	{
		Answers = new List<ImagePaperAnswer>();
	}
}
