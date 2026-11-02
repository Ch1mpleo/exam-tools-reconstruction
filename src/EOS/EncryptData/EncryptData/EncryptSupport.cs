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
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memoryStream = new MemoryStream();
		binaryFormatter.Serialize(memoryStream, obj);
		return memoryStream.ToArray();
	}

	// [behavior 0x06000002] Writes bytes to a MemoryStream, seeks to its start, and BinaryFormatter-deserializes an object.
	// [behavior 0x06000002] Input validation and exception behavior are exactly those in the source.
	public static object ByteArrayToObject(byte[] arrBytes)
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		memoryStream.Write(arrBytes, 0, arrBytes.Length);
		memoryStream.Seek(0L, SeekOrigin.Begin);
		return binaryFormatter.Deserialize(memoryStream);
	}

	// [behavior 0x06000003] Creates/truncates a file and encrypts the supplied bytes through DES, using ASCII key bytes as
	// [behavior 0x06000003] both key and IV. Returns true after closing streams; exceptions are rethrown.
	public static bool EncryptQuestions_SaveToFile(string fname, byte[] data, string key)
	{
		try
		{
			FileStream fileStream = new FileStream(fname, FileMode.Create, FileAccess.Write);
			DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
			dESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(key);
			dESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes(key);
			CryptoStream cryptoStream = new CryptoStream(fileStream, dESCryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(data, 0, data.Length);
			cryptoStream.Close();
			fileStream.Close();
			return true;
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	// [behavior 0x06000004] Opens and DES-decrypts a file. The returned allocation is as large as the ciphertext file, not
	// [behavior 0x06000004] the actual plaintext; the unused zero tail is retained.
	public static byte[] DecryptQuestions_FromFile(string fname, string key)
	{
		try
		{
			FileStream fileStream = new FileStream(fname, FileMode.Open, FileAccess.Read);
			DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
			dESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(key);
			dESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes(key);
			CryptoStream cryptoStream = new CryptoStream(fileStream, dESCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Read);
			byte[] array = new byte[fileStream.Length];
			int num = cryptoStream.Read(array, 0, (int)fileStream.Length);
			cryptoStream.Close();
			fileStream.Close();
			return array;
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
		DES dES = new DESCryptoServiceProvider();
		dES.Key = Encoding.ASCII.GetBytes(key);
		dES.IV = dES.Key;
		dES.Padding = PaddingMode.PKCS7;
		MemoryStream memoryStream = new MemoryStream();
		ICryptoTransform transform = dES.CreateEncryptor();
		CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
		cryptoStream.Write(data, 0, data.Length);
		cryptoStream.FlushFinalBlock();
		memoryStream.Position = 0L;
		string empty = string.Empty;
		empty = Convert.ToBase64String(memoryStream.ToArray());
		cryptoStream.Close();
		return empty;
	}

	// [behavior 0x06000006] DES-decrypts the input byte array using ASCII key/IV bytes. Allocates an output buffer equal to
	// [behavior 0x06000006] ciphertext length, ignores the read count, then decodes the entire buffer as UTF-16. Trailing
	// [behavior 0x06000006] NUL characters are observable and preserved.
	public static string Decryption(byte[] data, string key)
	{
		DES dES = new DESCryptoServiceProvider();
		dES.Key = Encoding.ASCII.GetBytes(key);
		dES.IV = dES.Key;
		dES.Padding = PaddingMode.PKCS7;
		MemoryStream stream = new MemoryStream(data);
		CryptoStream cryptoStream = new CryptoStream(stream, dES.CreateDecryptor(), CryptoStreamMode.Read);
		byte[] array = new byte[data.Length];
		cryptoStream.Read(array, 0, array.Length);
		cryptoStream.Close();
		return Encoding.Unicode.GetString(array);
	}

	// [behavior 0x06000007] Hashes Encoding.Unicode (UTF-16LE) bytes with MD5, then decodes the raw digest bytes as UTF-16
	// [behavior 0x06000007] text. This is NOT a hexadecimal digest. Preserve its unusual output for compatibility.
	public static string GetMD5(string msg)
	{
		MD5 mD = new MD5CryptoServiceProvider();
		byte[] bytes = mD.ComputeHash(Encoding.Unicode.GetBytes(msg));
		return Encoding.Unicode.GetString(bytes);
	}

	// [behavior 0x06000008] UTF-8 encodes plaintext, uses RijndaelManaged in ECB mode with PKCS7 and the supplied UTF-8 key
	// [behavior 0x06000008] bytes, transforms the complete block, and Base64-encodes the result. The algorithm object is
	// [behavior 0x06000008] disposed.
	public static string Encrypt(string plainText, string key)
	{
		using RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.Key = Encoding.UTF8.GetBytes(key);
		rijndaelManaged.Mode = CipherMode.ECB;
		rijndaelManaged.Padding = PaddingMode.PKCS7;
		ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor();
		byte[] bytes = Encoding.UTF8.GetBytes(plainText);
		byte[] inArray = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
		return Convert.ToBase64String(inArray);
	}

	// [behavior 0x06000009] Base64-decodes the input, decrypts it with RijndaelManaged in ECB/PKCS7 using UTF-8 key bytes,
	// [behavior 0x06000009] and UTF-8 decodes the result. Invalid key/ciphertext errors propagate.
	public static string Decrypt(string encryptedText, string key)
	{
		using RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.Key = Encoding.UTF8.GetBytes(key);
		rijndaelManaged.Mode = CipherMode.ECB;
		rijndaelManaged.Padding = PaddingMode.PKCS7;
		ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor();
		byte[] array = Convert.FromBase64String(encryptedText);
		byte[] bytes = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
		return Encoding.UTF8.GetString(bytes);
	}
}
