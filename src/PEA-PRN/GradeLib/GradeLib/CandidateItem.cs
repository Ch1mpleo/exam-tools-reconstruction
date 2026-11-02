using System;

namespace GradeLib;

[Serializable]
public class CandidateItem
{
	public string Candidate { get; set; }

	public byte[] CompressedFile { get; set; }

	public float Mark { get; set; }
}
