using System;
using System.Collections.Generic;

namespace IRemote;

[Serializable]
public class PEAData
{
	public string SessionID;

	public RegisterStatus Status;

	public byte[] GUI;

	public int OriginSize;

	public ServerInfo ServerInfomation;

	public RegisterData RegData;

	public List<SubmitHistory> SubmitHistory;

	public byte PaperCount;

	public byte QuestionCount;

	public List<GivenMaterials> GivenMaterials;

	public List<byte[]> PaperImage;

	public int NumberOfPage;

	public string Language;

	public bool LockClient;

	public int TimeAllow = 0;
}
