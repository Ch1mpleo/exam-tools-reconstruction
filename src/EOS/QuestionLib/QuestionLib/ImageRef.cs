using System;

namespace QuestionLib;

[Serializable]
public class ImageRef
{
	public byte[] ImgData { get; set; }

	public int ImgSize { get; set; }

	public int NumberOfPage { get; set; }

	public byte[] Get_ImgData()
	{
		return ImgData;
	}

	public int Get_ImgSize()
	{
		return ImgSize;
	}

	public int Get_NumberOfPage()
	{
		return NumberOfPage;
	}

	public void Preapare2Submit()
	{
		ImgData = null;
	}
}
