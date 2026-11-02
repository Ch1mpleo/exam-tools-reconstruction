using System;

namespace IRemote;

[Serializable]
public class SubmitSolution
{
	public string SessionID { get; set; }

	public string Login { get; set; }

	public string TestName { get; set; }

	public int PaperNo { get; set; }

	public byte QuestionNo { get; set; }

	public byte[] SolutionBuf { get; set; }

	public string CompressType { get; set; }

	public string Path { get; set; }

	public int FileSize { get; set; }
}
