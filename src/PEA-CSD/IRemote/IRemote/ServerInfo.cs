using System;

namespace IRemote;

[Serializable]
public class ServerInfo
{
	public string IP { get; set; }

	public string Domain { get; set; }

	public int Port { get; set; }

	public string ServerAlias { get; set; }

	public string Version { get; set; }

	public ServerInfo()
	{
	}

	public ServerInfo(string ip, int port, string domain, string serverAlias, string version)
	{
		IP = ip;
		Port = port;
		Domain = domain;
		ServerAlias = serverAlias;
		Version = version;
	}
}
