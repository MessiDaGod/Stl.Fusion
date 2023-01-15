using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
// using Microsoft.VisualBasic.CompilerServices;
using System.Security.Cryptography;
using System.Globalization;
using NETCore.Encrypt;
using System.Text;

namespace Stl.Fusion.EntityFramework.Authentication;

[Serializable]
public class Cryptography
{
    #region MacOS Cryptography

    private string DoEncrypt(string plaintext)
    {
        //var aesKey = EncryptProvider.CreateAesKey();
        var key = ConvertKey(GetKey());
        var iv = ConvertKey(GetIV());
        return EncryptProvider.AESEncrypt(plaintext, key, iv);
    }
    private string DoDecrypt(string plaintext)
    {
        //var aesKey = EncryptProvider.CreateAesKey();
        var key = ConvertKey(GetKey());
        var iv = ConvertKey(GetIV());
        return EncryptProvider.AESDecrypt(plaintext, key, iv);
    }

    public string Encrypt(string src)
    {
        return DoEncrypt(src);
    }
    public string Decrypt(string src)
    {
        return DoDecrypt(src);
    }
    private string ConvertKey(byte[] bytes)
    {
        return System.Text.Encoding.ASCII.GetString(bytes);
    }
    private byte[] GetKey()
    {
        return new byte[32]
        {
            219, 188, 14, 83, 142, 122, 107, 226, 251, 63,
            66, 149, 52, 105, 91, 3, 231, 103, 30, 83,
            194, 162, 162, 5, 196, 186, 42, 137, 48, 82,
            214, 89
        };
    }
    private byte[] GetIV()
    {
        return new byte[16]
        {
            57, 151, 44, 195, 149, 211, 35, 174, 87, 48,
            199, 178, 158, 211, 231, 26
        };
    }

    public static string GetHash(string Phrase, string Salt)
    {
        // byte[] SaltedBytePhrase = System.Text.Encoding.Unicode.GetBytes();
		// byte[] hash;
		// using SHA384 sha = SHA384.Create();
        // sha.ComputeHash(SaltedBytePhrase);
        // hash = Convert.ToBase64String(sha.Hash[]);

		var saltedBytePhrase = Encoding.Unicode.GetBytes($"y{Salt.ToLower()}ard{Phrase}I");
		byte[] hash;
		using SHA384 sha = SHA384.Create();
		hash = sha.ComputeHash(saltedBytePhrase);
		return Convert.ToBase64String(hash);
    }

    #endregion

    #region Unused

    // [DllImport("/Library/WebServer/Documents/VoyagerDev/bin/QPRO32.DLL", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
    // private static extern void Encrypt2([MarshalAs(UnmanagedType.LPWStr)] ref string Work, [MarshalAs(UnmanagedType.LPWStr)] ref string Password);
    // private string RaiseError()
    // {
    // 	StringBuilder stringBuilder = new StringBuilder();
    // 	stringBuilder.Append('%');
    // 	stringBuilder.Append('4');
    // 	stringBuilder.Append(':');
    // 	stringBuilder.Append('c');
    // 	stringBuilder.Append('=');
    // 	stringBuilder.Append('\u0011');
    // 	stringBuilder.Append('r');
    // 	stringBuilder.Append('Z');
    // 	stringBuilder.Append(' ');
    // 	stringBuilder.Append('K');
    // 	stringBuilder.Append('\u001d');
    // 	stringBuilder.Append('t');
    // 	return stringBuilder.ToString();
    // }
    // private string DecryptNibble(string ss)
    // {
    // 	string sTarget = "";
    // 	int num = Strings.Len(ss);
    // 	for (int i = 1; i <= num; i = checked(i + 2))
    // 	{
    // 		int j = Convert.ToInt16(Conversion.Val("&H" + Strings.Mid(ss, i, 2)));
    // 		sTarget += Conversions.ToString(Strings.Chr(j));
    // 	}
    // 	return sTarget;
    // }
    // /// <summary>
    // ///     Decrypts an encrypted string.
    // /// </summary>
    // internal string Decrypt(string Encrypted)
    // {
    // 	string s = DecryptNibble(Encrypted);
    // 	string Password = RaiseError();
    // 	Encrypt2(ref s, ref Password);
    // 	return s;
    // }
    // /// <summary>
    // ///     Encrypts an string.
    // /// </summary>
    // public string Encrypt(string Unencrypted)
    // {
    // 	string s = Strings.Trim(Unencrypted);
    // 	string Password = RaiseError();
    // 	Encrypt2(ref s, ref Password);
    // 	return EncryptNibble(s);
    // }
    // private string EncryptNibble(string ss)
    // {
    // 	string sTarget = "";
    // 	int num = Strings.Len(ss);
    // 	checked
    // 	{
    // 		for (int i = 1; i <= num; i++)
    // 		{
    // 			int iLeft = Convert.ToInt16(Conversion.Int((double)Strings.Asc(Strings.Mid(ss, i, 1)) / 16.0));
    // 			int iRight = unchecked(Strings.Asc(Strings.Mid(ss, i, 1)) % 16);
    // 			sTarget += Strings.Mid("0123456789ABCDEF", iLeft + 1, 1);
    // 			sTarget += Strings.Mid("0123456789ABCDEF", iRight + 1, 1);
    // 		}
    // 		return sTarget;
    // 	}
    // }

    // internal string EncryptRM(string Value)
    // {
    // 	ASCIIEncoding textConverter = new ASCIIEncoding();
    // 	#pragma warning disable SYSLIB0022
    // 	RijndaelManaged myRijndael = new RijndaelManaged();
    // 	byte[] key = GetKey();
    // 	byte[] IV = GetIV();
    // 	ICryptoTransform encryptor = myRijndael.CreateEncryptor(key, IV);
    // 	MemoryStream msEncrypt = new MemoryStream();
    // 	CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
    // 	byte[] toEncrypt = textConverter.GetBytes(Value);
    // 	csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
    // 	csEncrypt.FlushFinalBlock();
    // 	byte[] encrypted = msEncrypt.ToArray();
    // 	return Convert.ToBase64String(encrypted);
    // }
    // internal string DecryptRM(string Value)
    // {
    // 	ASCIIEncoding textConverter = new ASCIIEncoding();
    // 	RijndaelManaged myRijndael = new RijndaelManaged();
    // 	byte[] key = GetKey();
    // 	byte[] IV = GetIV();
    // 	ICryptoTransform decryptor = myRijndael.CreateDecryptor(key, IV);
    // 	byte[] encrypted = Convert.FromBase64String(Value);
    // 	MemoryStream msDecrypt = new MemoryStream(encrypted);
    // 	CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
    // 	byte[] fromEncrypt = new byte[checked(encrypted.Length + 1)];
    // 	csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);
    // 	return textConverter.GetString(fromEncrypt);
    // }
    // private void ProcessError1(ref byte[] BA)
    // {
    // 	BA[0] = Convert.ToByte(132);
    // 	BA[2] = Convert.ToByte(165);
    // 	BA[4] = Convert.ToByte(192);
    // 	BA[6] = Convert.ToByte(30);
    // 	BA[8] = Convert.ToByte(177);
    // 	BA[10] = Convert.ToByte(122);
    // 	BA[12] = Convert.ToByte(199);
    // 	BA[14] = Convert.ToByte(21);
    // }
    // private void ProcessError2(ref byte[] BA)
    // {
    // 	BA[1] = Convert.ToByte(211);
    // 	BA[3] = Convert.ToByte(83);
    // 	BA[5] = Convert.ToByte(68);
    // 	BA[7] = Convert.ToByte(204);
    // 	BA[9] = Convert.ToByte(241);
    // 	BA[11] = Convert.ToByte(240);
    // 	BA[13] = Convert.ToByte(88);
    // 	BA[15] = Convert.ToByte(158);
    // }
    // private void ProcessError3(ref byte[] BA)
    // {
    // 	BA[0] = Convert.ToByte(192);
    // 	BA[2] = Convert.ToByte(240);
    // 	BA[4] = Convert.ToByte(166);
    // 	BA[6] = Convert.ToByte(117);
    // 	BA[8] = Convert.ToByte(228);
    // 	BA[10] = Convert.ToByte(36);
    // 	BA[12] = Convert.ToByte(49);
    // 	BA[14] = Convert.ToByte(209);
    // }
    // private void ProcessError4(ref byte[] BA)
    // {
    // 	BA[1] = Convert.ToByte(59);
    // 	BA[3] = Convert.ToByte(83);
    // 	BA[5] = Convert.ToByte(231);
    // 	BA[7] = Convert.ToByte(17);
    // 	BA[9] = Convert.ToByte(109);
    // 	BA[11] = Convert.ToByte(72);
    // 	BA[13] = Convert.ToByte(131);
    // 	BA[15] = Convert.ToByte(233);
    // }
    // internal string DecryptAesString(string cipherText)
    // {
    // 	if (cipherText == null || cipherText.Length <= 0)
    // 	{
    // 		throw new ArgumentNullException("cipherText");
    // 	}
    // 	int iSize = Convert.ToInt32((double)cipherText.Length / 2.0 - 1.0);
    // 	checked
    // 	{
    // 		byte[] encrypted = new byte[iSize + 1];
    // 		try
    // 		{
    // 			int num = cipherText.Length - 1;
    // 			for (int i = 0; i <= num; i += 2)
    // 			{
    // 				iSize = Convert.ToInt32((double)i / 2.0);
    // 				encrypted[iSize] = byte.Parse(cipherText.Substring(i, 2), NumberStyles.HexNumber);
    // 			}
    // 		}
    // 		catch (Exception ex3)
    // 		{
    // 			ProjectData.SetProjectError(ex3);
    // 			Exception ex2 = ex3;
    // 			string DecryptAesString2 = "";
    // 			ProjectData.ClearProjectError();
    // 			return DecryptAesString2;
    // 		}
    // 		int RijndaelKeySizeInBits = 128;
    // 		string plaintext = null;
    // 		byte[] RaiseError1 = new byte[16];
    // 		byte[] RaiseError2 = new byte[16];
    // 		ProcessError1(ref RaiseError1);
    // 		ProcessError2(ref RaiseError1);
    // 		ProcessError3(ref RaiseError2);
    // 		ProcessError4(ref RaiseError2);
    // 		RijndaelManaged RijndaelAlg = new RijndaelManaged();
    // 		MemoryStream msDecrypt = new MemoryStream(encrypted);
    // 		try
    // 		{
    // 			RijndaelAlg.KeySize = RijndaelKeySizeInBits;
    // 			RijndaelAlg.Key = RaiseError1;
    // 			RijndaelAlg.IV = RaiseError2;
    // 			ICryptoTransform decryptor2 = RijndaelAlg.CreateDecryptor(RijndaelAlg.Key, RijndaelAlg.IV);
    // 			CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor2, CryptoStreamMode.Read);
    // 			StreamReader srDecrypt = new StreamReader(csDecrypt);
    // 			plaintext = srDecrypt.ReadToEnd();
    // 			csDecrypt = null;
    // 			srDecrypt = null;
    // 		}
    // 		catch (Exception ex4)
    // 		{
    // 			ProjectData.SetProjectError(ex4);
    // 			Exception ex = ex4;
    // 			string DecryptAesString = "";
    // 			ProjectData.ClearProjectError();
    // 			return DecryptAesString;
    // 		}
    // 		finally
    // 		{
    // 			RijndaelAlg = null;
    // 			ICryptoTransform decryptor = null;
    // 			msDecrypt = null;
    // 		}
    // 		return plaintext;
    // 	}
    // }

    #endregion
}

public enum LoginEnum
{
    Register,
    Login
}
