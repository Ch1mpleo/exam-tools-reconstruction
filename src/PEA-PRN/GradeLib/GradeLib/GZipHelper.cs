using System.IO;
using System.IO.Compression;

namespace GradeLib;

public class GZipHelper
{
	// [behavior 0x06000003] Writes all input through a GZipStream, closes it and returns compressed memory. Any exception
	// [behavior 0x06000003] returns null.
	public static byte[] Compress(byte[] bytData)
	{
		try
		{
			MemoryStream ms = new MemoryStream();
			Stream s = new GZipStream(ms, CompressionMode.Compress);
			s.Write(bytData, 0, bytData.Length);
			s.Close();
			return ms.ToArray();
		}
		catch
		{
			return null;
		}
	}

	// [behavior 0x06000004] Allocates originSize bytes, performs one decompression read and returns the allocation. The
	// [behavior 0x06000004] implementation does not loop or validate the returned byte count.
	public static byte[] DeCompress(byte[] bytInput, int originSize)
	{
		Stream s = new GZipStream(new MemoryStream(bytInput), CompressionMode.Decompress);
		byte[] buf = new byte[originSize];
		s.Read(buf, 0, originSize);
		return buf;
	}
}
