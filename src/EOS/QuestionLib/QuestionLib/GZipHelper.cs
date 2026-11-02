using System.IO;
using System.IO.Compression;

namespace QuestionLib;

public class GZipHelper
{
	// [behavior 0x06000001] Writes input bytes through a GZipStream into memory, closes it and returns the compressed
	// [behavior 0x06000001] allocation. Exceptions are caught and return null.
	public static byte[] Compress(byte[] bytData)
	{
		try
		{
			MemoryStream memoryStream = new MemoryStream();
			Stream stream = new GZipStream(memoryStream, CompressionMode.Compress);
			stream.Write(bytData, 0, bytData.Length);
			stream.Close();
			return memoryStream.ToArray();
		}
		catch
		{
			return null;
		}
	}

	// [behavior 0x06000002] Allocates originSize bytes, makes one GZipStream.Read call and returns the allocation without
	// [behavior 0x06000002] checking the number of bytes read. Oversized requests retain a zero-filled tail.
	public static byte[] DeCompress(byte[] bytInput, int originSize)
	{
		Stream stream = new GZipStream(new MemoryStream(bytInput), CompressionMode.Decompress);
		byte[] array = new byte[originSize];
		stream.Read(array, 0, originSize);
		return array;
	}
}
