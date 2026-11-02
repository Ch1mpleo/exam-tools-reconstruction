using System;

namespace QuestionLib;

[Serializable]
public class SubmitPaper
{
	public string LoginId;

	public int TimeLeft;

	public int IndexFill;

	public int IndexReading;

	public int IndexG;

	public int IndexIndiM;

	public int IndexMatch;

	public bool Finish;

	public Paper SPaper;

	public int TabIndex;

	public DateTime SubmitTime;

	public byte[] byteImage;

	public bool imageSaved;

	public string ID;

	// [behavior 0x060000b6] Casts the argument to SubmitPaper, then compares ID and SPaper.ExamCode using instance Equals.
	// [behavior 0x060000b6] Null/wrong-type/missing-field inputs can throw; there is no matching GetHashCode override in
	// [behavior 0x060000b6] this type.
	public override bool Equals(object obj)
	{
		SubmitPaper submitPaper = (SubmitPaper)obj;
		return ID.Equals(submitPaper.ID) && SPaper.ExamCode.Equals(submitPaper.SPaper.ExamCode);
	}
}
