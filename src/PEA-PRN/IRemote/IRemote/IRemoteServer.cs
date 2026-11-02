namespace IRemote;

public interface IRemoteServer
{
	PEAData ConductTest(RegisterData rd);

	SubmitStatus Submit(SubmitSolution solution, ref string msg);

	bool Log(string examCode, string login, string logStr);
}
