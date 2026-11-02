using System;

namespace IRemote;

[Serializable]
public class SubmitHistory : IComparable<SubmitHistory>
{
	public int PaperNo { get; set; }

	public byte QuestionNo { get; set; }

	public string Path { get; set; }

	public DateTime Time { get; set; }

	public SubmitHistory()
	{
	}

	public SubmitHistory(int paperNo, byte questionNo, string path, DateTime time)
	{
		PaperNo = paperNo;
		QuestionNo = questionNo;
		Path = path;
		Time = time;
	}

	// [behavior 0x0600000d] Compares the OTHER history item Time against this item Time, so sorting places newer submissions
	// [behavior 0x0600000d] first. A null other item returns 1. Equal times compare as zero.
	public int CompareTo(SubmitHistory sh)
	{
		return sh?.Time.CompareTo(Time) ?? 1;
	}
}
