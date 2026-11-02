using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace EncryptData;

public class EncryptSupport
{
	// [behavior 0x06000001] Returns null for a null object; otherwise BinaryFormatter-serializes it into a MemoryStream and
	// [behavior 0x06000001] returns the bytes.
	public static byte[] ObjectToByteArray(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		BinaryFormatter bf = new BinaryFormatter();
		MemoryStream ms = new MemoryStream();
		bf.Serialize(ms, obj);
		return ms.ToArray();
	}

	// [behavior 0x06000002] Writes bytes to a MemoryStream, seeks to its start, and BinaryFormatter-deserializes an object.
	// [behavior 0x06000002] Input validation and exception behavior are exactly those in the source.
	public static object ByteArrayToObject(byte[] arrBytes)
	{
		MemoryStream memStream = new MemoryStream();
		BinaryFormatter binForm = new BinaryFormatter();
		memStream.Write(arrBytes, 0, arrBytes.Length);
		memStream.Seek(0L, SeekOrigin.Begin);
		return binForm.Deserialize(memStream);
	}

	// [behavior 0x06000003] Creates/truncates the named file, encrypts the input through DES with the ASCII key reused as
	// [behavior 0x06000003] IV, closes streams, and returns true. Exceptions are rethrown with throw ex.
	public static bool Encrypt_SaveToFile(string fname, byte[] data, string key)
	{
		try
		{
			FileStream stream = new FileStream(fname, FileMode.Create, FileAccess.Write);
			DESCryptoServiceProvider cryptic = new DESCryptoServiceProvider();
			cryptic.Key = Encoding.ASCII.GetBytes(key);
			cryptic.IV = Encoding.ASCII.GetBytes(key);
			CryptoStream crStream = new CryptoStream(stream, cryptic.CreateEncryptor(), CryptoStreamMode.Write);
			crStream.Write(data, 0, data.Length);
			crStream.Close();
			stream.Close();
			return true;
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	// [behavior 0x06000004] Opens and DES-decrypts a file. Returns an allocation sized to encrypted file length and ignores
	// [behavior 0x06000004] the number of decrypted bytes read; unused tail bytes remain zero. File/crypto errors propagate.
	public static byte[] Decrypt_FromFile(string fname, string key)
	{
		try
		{
			FileStream stream = new FileStream(fname, FileMode.Open, FileAccess.Read);
			DESCryptoServiceProvider cryptic = new DESCryptoServiceProvider();
			cryptic.Key = Encoding.ASCII.GetBytes(key);
			cryptic.IV = Encoding.ASCII.GetBytes(key);
			CryptoStream crStream = new CryptoStream(stream, cryptic.CreateDecryptor(), CryptoStreamMode.Read);
			byte[] buf = new byte[stream.Length];
			int len = crStream.Read(buf, 0, (int)stream.Length);
			crStream.Close();
			stream.Close();
			return buf;
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	// [behavior 0x06000005] Uses DES with the ASCII key bytes also used as the IV, PKCS7 padding, and a write-mode
	// [behavior 0x06000005] CryptoStream; returns the encrypted bytes as Base64 text.
	public static string Encryption(byte[] data, string key)
	{
		DES des = new DESCryptoServiceProvider();
		des.Key = Encoding.ASCII.GetBytes(key);
		des.IV = des.Key;
		des.Padding = PaddingMode.PKCS7;
		MemoryStream ms = new MemoryStream();
		ICryptoTransform desencrypt = des.CreateEncryptor();
		CryptoStream stream = new CryptoStream(ms, desencrypt, CryptoStreamMode.Write);
		stream.Write(data, 0, data.Length);
		stream.FlushFinalBlock();
		ms.Position = 0L;
		string encrypted = string.Empty;
		encrypted = Convert.ToBase64String(ms.ToArray());
		stream.Close();
		return encrypted;
	}

	// [behavior 0x06000006] DES-decrypts the input byte array using ASCII key/IV bytes. Allocates an output buffer equal to
	// [behavior 0x06000006] ciphertext length, ignores the read count, then decodes the entire buffer as UTF-16. Trailing
	// [behavior 0x06000006] NUL characters are observable and preserved.
	public static string Decryption(byte[] data, string key)
	{
		DES des = new DESCryptoServiceProvider();
		des.Key = Encoding.ASCII.GetBytes(key);
		des.IV = des.Key;
		des.Padding = PaddingMode.PKCS7;
		MemoryStream ms = new MemoryStream(data);
		CryptoStream stream = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read);
		byte[] fromEncrypt = new byte[data.Length];
		stream.Read(fromEncrypt, 0, fromEncrypt.Length);
		stream.Close();
		return Encoding.Unicode.GetString(fromEncrypt);
	}

	// [behavior 0x06000007] Hashes Encoding.Unicode (UTF-16LE) bytes with MD5, then decodes the raw digest bytes as UTF-16
	// [behavior 0x06000007] text. This is NOT a hexadecimal digest. Preserve its unusual output for compatibility.
	public static string GetMD5(string msg)
	{
		MD5 md5 = new MD5CryptoServiceProvider();
		byte[] result = md5.ComputeHash(Encoding.Unicode.GetBytes(msg));
		return Encoding.Unicode.GetString(result);
	}
}
