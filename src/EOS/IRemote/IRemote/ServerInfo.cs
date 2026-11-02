using System;
using System.Collections.Generic;

namespace IRemote;

[Serializable]
public class ServerInfo
{
	private List<ServerEndpointInfo> _servers = new List<ServerEndpointInfo>();

	private string _campusName;

	private int _authenticationType;

	private string _domain;

	private string _ipRangeWlan;

	private string _monitorIp;

	private int _monitorPort;

	private string _ip;

	private int _port;

	private string _publicIp;

	private string _portName;

	private string _serverAlias;

	public List<ServerEndpointInfo> Servers
	{
		get
		{
			return _servers;
		}
		set
		{
			_servers = value;
		}
	}

	public string CampusName
	{
		get
		{
			return _campusName;
		}
		set
		{
			_campusName = value;
		}
	}

	public int AuthenticationType
	{
		get
		{
			return _authenticationType;
		}
		set
		{
			_authenticationType = value;
		}
	}

	public string Domain
	{
		get
		{
			return _domain;
		}
		set
		{
			_domain = value;
		}
	}

	public string IP_Range_WLAN
	{
		get
		{
			return _ipRangeWlan;
		}
		set
		{
			_ipRangeWlan = value;
		}
	}

	public string MonitorServer_IP
	{
		get
		{
			return _monitorIp;
		}
		set
		{
			_monitorIp = value;
		}
	}

	public int MonitorServer_Port
	{
		get
		{
			return _monitorPort;
		}
		set
		{
			_monitorPort = value;
		}
	}

	public string IP
	{
		get
		{
			return _ip;
		}
		set
		{
			_ip = value;
		}
	}

	public int Port
	{
		get
		{
			return _port;
		}
		set
		{
			_port = value;
		}
	}

	public string Public_IP
	{
		get
		{
			return _publicIp;
		}
		set
		{
			_publicIp = value;
		}
	}

	public string PortName
	{
		get
		{
			return _portName;
		}
		set
		{
			_portName = value;
		}
	}

	public string ServerAlias
	{
		get
		{
			return _serverAlias;
		}
		set
		{
			_serverAlias = value;
		}
	}
}
