using System;

namespace QuestionLib.Entity;

public class EOSLog
{
	private int _LogID;

	private string _Log_Name;

	private string _Log_Account;

	private string _Log_MACAddress;

	private DateTime _CreatedDate;

	public int LogID
	{
		get
		{
			return _LogID;
		}
		set
		{
			_LogID = value;
		}
	}

	public string Log_Name
	{
		get
		{
			return _Log_Name;
		}
		set
		{
			_Log_Name = value;
		}
	}

	public string Log_Account
	{
		get
		{
			return _Log_Account;
		}
		set
		{
			_Log_Account = value;
		}
	}

	public string Log_MACAddress
	{
		get
		{
			return _Log_MACAddress;
		}
		set
		{
			_Log_MACAddress = value;
		}
	}

	public DateTime CreatedDate
	{
		get
		{
			return _CreatedDate;
		}
		set
		{
			_CreatedDate = value;
		}
	}
}
