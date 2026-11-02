using System;

namespace IRemote;

[Serializable]
public class ServerEndpointInfo
{
	public string ServerName { get; set; }

	public string IP { get; set; }

	public int Port { get; set; }

	public string PublicIP { get; set; }
}
