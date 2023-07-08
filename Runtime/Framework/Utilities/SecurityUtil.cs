using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace GhysX.Framework.Utilities
{
    public class SecurityUtil
    {
        public static void RSAGenerateKey(ref string privateKey, ref string publicKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            privateKey = rsa.ToXmlString(true);
            publicKey = rsa.ToXmlString(false);
        }

        public static byte[] RSAEncrypt(byte[] data, string publicKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);
            byte[] encryptData = rsa.Encrypt(data, false);
            return encryptData;
        }

        public static byte[] RSADecrypt(byte[] data, string privateKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);
            byte[] decryptData = rsa.Decrypt(data, false);
            return decryptData;
        }

        public static byte[] DESEncrypt(byte[] data, byte[] desrgbKey, byte[] desrgbIV)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, des.CreateEncryptor(desrgbKey, desrgbIV), CryptoStreamMode.Write);
            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.FlushFinalBlock();
            return memoryStream.ToArray();
        }

        public static byte[] DESDecrypt(byte[] data, byte[] desrgbKey, byte[] desrgbIV)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, des.CreateDecryptor(desrgbKey, desrgbIV), CryptoStreamMode.Write);
            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.FlushFinalBlock();
            return memoryStream.ToArray();
        }

        public static string RSAEncryptToBase64String(string publickey, string content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(publickey);
            cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false);

            return Convert.ToBase64String(cipherbytes);
        }

        public static string RSADecryptFromBase64String(string privatekey, string content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(privatekey);
            cipherbytes = rsa.Decrypt(Convert.FromBase64String(content), false);

            return Encoding.UTF8.GetString(cipherbytes);
        }
    }

    public class DESHelper
    {
        public static byte[] Encrypt(byte[] data, byte[] keyBytes)
        {
            try
            {
                DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();

                desProvider.Mode = CipherMode.ECB;
                desProvider.Key = keyBytes;
                MemoryStream memStream = new MemoryStream();
                CryptoStream crypStream = new CryptoStream(memStream, desProvider.CreateEncryptor(), CryptoStreamMode.Write);

                crypStream.Write(data, 0, data.Length);
                crypStream.FlushFinalBlock();
                return memStream.ToArray();
            }
            catch
            {
                return data;
            }
        }

        public static byte[] Decrypt(byte[] data, byte[] keyBytes)
        {
            DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();

            desProvider.Mode = CipherMode.ECB;
            desProvider.Key = keyBytes;
            MemoryStream memStream = new MemoryStream();
            CryptoStream crypStream = new CryptoStream(memStream, desProvider.CreateDecryptor(), CryptoStreamMode.Write);

            crypStream.Write(data, 0, data.Length);
            crypStream.FlushFinalBlock();
            return memStream.ToArray();

        }

        public static string ToDESEncrypt(string encryptString, string sKey)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(sKey);
            byte[] keyIV = keyBytes;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);

            DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();

            desProvider.Mode = CipherMode.ECB;
            MemoryStream memStream = new MemoryStream();
            CryptoStream crypStream = new CryptoStream(memStream, desProvider.CreateEncryptor(keyBytes, keyIV), CryptoStreamMode.Write);

            crypStream.Write(inputByteArray, 0, inputByteArray.Length);
            crypStream.FlushFinalBlock();
            return Convert.ToBase64String(memStream.ToArray());

        }

        public static string ToDESDecrypt(string decryptString, string sKey)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(sKey);
            byte[] keyIV = keyBytes;
            byte[] inputByteArray = Convert.FromBase64String(decryptString);

            DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();

            desProvider.Mode = CipherMode.ECB;
            MemoryStream memStream = new MemoryStream();
            CryptoStream crypStream = new CryptoStream(memStream, desProvider.CreateDecryptor(keyBytes, keyIV), CryptoStreamMode.Write);

            crypStream.Write(inputByteArray, 0, inputByteArray.Length);
            crypStream.FlushFinalBlock();
            return Encoding.Default.GetString(memStream.ToArray());

        }
    }

    public class AESHelper
    {
        public static byte[] Encrypt(byte[] data, byte[] key)
        {
            if (null == data || 0 == data.Length)
            {
                return null;
            }

            try
            {
                var rm = new RijndaelManaged
                {
                    IV = key,
                    Key = key,
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7
                };
                ICryptoTransform cTransform = rm.CreateEncryptor();
                return cTransform.TransformFinalBlock(data, 0, data.Length);
            }
            catch
            {
                return null;
            }
        }

        public static byte[] Decrypt(byte[] data, byte[] key)
        {
            if (null == data || 0 == data.Length)
            {
                return null;
            }

            try
            {
                var rm = new RijndaelManaged
                {
                    IV = key,
                    Key = key,
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7
                };
                ICryptoTransform cTransform = rm.CreateDecryptor();
                return cTransform.TransformFinalBlock(data, 0, data.Length);
            }
            catch
            {
                return null;
            }
        }

        public static string ToEncrypt(string strCon, string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strCon))
                {
                    return null;
                }

                byte[] byCon = Encoding.UTF8.GetBytes(strCon);
                var rm = new RijndaelManaged
                {
                    IV = Encoding.UTF8.GetBytes(key),
                    Key = Encoding.UTF8.GetBytes(key),
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7
                };
                ICryptoTransform cTransform = rm.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(byCon, 0, byCon.Length);
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch
            {
                return "";
            }
        }

        public static string ToDecrypt(string strCon, string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strCon))
                {
                    return null;
                }

                byte[] byCon = Convert.FromBase64String(strCon);
                var rm = new RijndaelManaged
                {
                    IV = Encoding.UTF8.GetBytes(key),
                    Key = Encoding.UTF8.GetBytes(key),
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7
                };
                ICryptoTransform cTransform = rm.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(byCon, 0, byCon.Length);
                return Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                return "";
            }
        }
    }
}