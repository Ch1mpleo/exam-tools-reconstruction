using System;

namespace GradeLib;

[Serializable]
public class Student
{
	public string Login { get; set; }

	public int PaperNo { get; set; }

	// [behavior 0x06000027] Concatenates Login, the literal separator " - ", and PaperNo. This is a display string, not a
	// [behavior 0x06000027] stable account/session identifier.
	public override string ToString()
	{
		return Login + " - " + PaperNo;
	}
}
