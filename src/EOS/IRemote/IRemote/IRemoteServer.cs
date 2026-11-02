using QuestionLib;

namespace IRemote;

public interface IRemoteServer
{
	EOSData ConductExam(RegisterData rd);

	SubmitStatus Submit(SubmitPaper submitPaper, ref string msg);

	SubmitPaper ResyncLocalCache(SubmitPaper stalePaper, string examCode, string login, string machine);

	bool Ping();

	bool IsMockExam(string examCode);

	string GetMinRequiredClientVersion();
}
