using System;
using System.Collections.Generic;

namespace GradeLib;

[Serializable]
public class PaperJPG
{
	public int PaperCode { get; set; }

	public byte[] PaperFile { get; set; }

	public List<byte[]> ListPaperFile { get; set; }

	public byte[] Given { get; set; }

	public byte[] GradingGuide { get; set; }

	public int NumberOfPage { get; set; }
}
