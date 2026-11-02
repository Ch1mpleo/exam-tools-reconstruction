namespace QuestionLib.Entity;

public class Course
{
	private string _cid;

	private string _name;

	private byte[] _imagedata;

	private string _imagename;

	private int _imagesize;

	private int _numberofpage;

	public string CID
	{
		get
		{
			return _cid;
		}
		set
		{
			_cid = value;
		}
	}

	public string Name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
		}
	}

	public byte[] ImageData
	{
		get
		{
			return _imagedata;
		}
		set
		{
			_imagedata = value;
		}
	}

	public string ImageName
	{
		get
		{
			return _imagename;
		}
		set
		{
			_imagename = value;
		}
	}

	public int ImageSize
	{
		get
		{
			return _imagesize;
		}
		set
		{
			_imagesize = value;
		}
	}

	public int NumberOfPage
	{
		get
		{
			return _numberofpage;
		}
		set
		{
			_numberofpage = value;
		}
	}

	public Course()
	{
	}

	public Course(string _cid, string _name, byte[] _imagedata, string _imagename, int _imagesize, int _numberofpage)
	{
		this._cid = _cid;
		this._name = _name;
		this._imagedata = _imagedata;
		this._imagename = _imagename;
		this._imagesize = _imagesize;
		this._numberofpage = _numberofpage;
	}
}
