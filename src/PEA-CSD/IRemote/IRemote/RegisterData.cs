using System;

namespace IRemote;

[Serializable]
public class RegisterData
{
	public string Login { get; set; }

	public string Password { get; set; }

	public DateTime StartTime { get; set; }

	public string Machine { get; set; }

	public string TestName { get; set; }

	public int PaperNo { get; set; }
}
