using System;
using System.Security.Cryptography;
using System.Text;

namespace Net.Helper
{
    /// <summary>
    /// 加密解密帮助类
    /// </summary>
    public class EncryptHelper
    {
        /// <summary>
        /// 随机数形式加密法
        /// </summary>
        /// <param name="password"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static byte[] ToEncrypt(int password, byte[] buffer)
        {
            return ToEncrypt(password, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 随机数形式加密法
        /// </summary>
        /// <param name="password"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static byte[] ToEncrypt(int password, byte[] buffer, int index, int count)
        {
            if (password < 10000000)
                throw new Exception("密码值不能小于10000000");
            var random = new Random(password);
            for (int i = index; i < index + count; i++)
            {
                buffer[i] += (byte)random.Next(0, 255);
            }
            return buffer;
        }

        /// <summary>
        /// 随机数形式解密法
        /// </summary>
        /// <param name="password"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static byte[] ToDecrypt(int password, byte[] buffer)
        {
            return ToDecrypt(password, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 随机数形式解密法
        /// </summary>
        /// <param name="password"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static byte[] ToDecrypt(int password, byte[] buffer, int index, int count)
        {
            if (password < 10000000)
                throw new Exception("密码值不能小于10000000");
            var random = new Random(password);
            for (int i = index; i < index + count; i++)
            {
                buffer[i] -= (byte)random.Next(0, 255);
            }
            return buffer;
        }

        /// <summary> 
        /// 加密字符串  
        /// </summary> 
        /// <param name="text">要加密的字符串</param> 
        /// <returns>加密后的字符串</returns> 
        public static string DESEncrypt(string encryptKey, string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            var keyArray = Encoding.UTF8.GetBytes(encryptKey);
            var toEncryptArray = Encoding.UTF8.GetBytes(text);
            var rDel = new RijndaelManaged
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            var cTransform = rDel.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary> 
        /// 解密字符串  
        /// </summary> 
        /// <param name="text">要解密的字符串</param> 
        /// <returns>解密后的字符串</returns>   
        public static string DESDecrypt(string encryptKey, string text)
        {
            if (text.Length < 2)
                return string.Empty;
            var keyArray = Encoding.UTF8.GetBytes(encryptKey);
            var toEncryptArray = Convert.FromBase64String(text);
            var rDel = new RijndaelManaged
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            var cTransform = rDel.CreateDecryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.UTF8.GetString(resultArray);
        }

        public static string GetMD5(string text)
        {
            var md5 = new MD5CryptoServiceProvider();
            byte[] bytValue, bytHash;
            bytValue = Encoding.UTF8.GetBytes(text);
            bytHash = md5.ComputeHash(bytValue);
            md5.Clear();
            string sTemp = "";
            for (int i = 0; i < bytHash.Length; i++)
            {
                sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
            }
            return sTemp.ToLower();
        }
    }
}
