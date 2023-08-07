using System;
using System.Security.Cryptography;
using Net.Event;
using Net.Helper;
using Net.Share;
using Net.System;

namespace Net.Adapter
{
    /// <summary>
    /// 基础数据包适配器
    /// </summary>
    public class DataAdapter : IPackageAdapter
    {
        public int HeadCount { get; set; }

        public void Pack(ISegment stream)
        {
        }

        public bool Unpack(ISegment stream, int frame, int uid) 
        {
            return true;
        }
    }

    /// <summary>
    /// 使用md5校验数据部分, 并且使用随机加密法加密md5值, 这里防止md5数据重新被校验, 所以用随机加密把md5值也加密了
    /// </summary>
    public class MD5EncryptDataHeadAdapter : IPackageAdapter
    {
        public int HeadCount { get; set; } = 16;
        public int Password { get; set; } = 123456789;

        public void Pack(ISegment stream)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            var retVal = md5.ComputeHash(stream.Buffer, stream.Position, stream.Count - stream.Position);
            EncryptHelper.ToEncrypt(Password, retVal);
            var len = stream.Count;
            stream.Position -= HeadCount;
            stream.Write(retVal, false);
            stream.Position = len;
        }

        public bool Unpack(ISegment stream, int frame, int uid)
        {
            var md5Hash = stream.Read(HeadCount);
            MD5 md5 = new MD5CryptoServiceProvider();
            var retVal = md5.ComputeHash(stream.Buffer, stream.Position, stream.Count - stream.Position);
            EncryptHelper.ToDecrypt(Password, md5Hash, 0, HeadCount);
            for (int i = 0; i < md5Hash.Length; i++)
            {
                if (retVal[i] != md5Hash[i])
                {
                    NDebug.LogError($"[{uid}]MD5CRC校验失败!");
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// 使用md5校验数据部分, 并且使用随机加密法加密数据体
    /// </summary>
    public class MD5EncryptDataBodyAdapter : IPackageAdapter
    {
        public int HeadCount { get; set; } = 16;
        public int Password { get; set; } = 123456789;

        public void Pack(ISegment stream)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            var retVal = md5.ComputeHash(stream.Buffer, stream.Position, stream.Count - stream.Position);
            EncryptHelper.ToEncrypt(Password, stream.Buffer, stream.Position, stream.Count - stream.Position);
            var len = stream.Count;
            stream.Position -= HeadCount;
            stream.Write(retVal, false);
            stream.Position = len;
        }

        public bool Unpack(ISegment stream, int frame, int uid)
        {
            var md5Hash = stream.Read(HeadCount);
            MD5 md5 = new MD5CryptoServiceProvider();
            EncryptHelper.ToDecrypt(Password, stream.Buffer, stream.Position, stream.Count - stream.Position);
            var retVal = md5.ComputeHash(stream.Buffer, stream.Position, stream.Count - stream.Position);
            for (int i = 0; i < md5Hash.Length; i++)
            {
                if (retVal[i] != md5Hash[i])
                {
                    NDebug.LogError($"[{uid}]MD5CRC校验失败!");
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// 使用随机加密法加密数据体
    /// </summary>
    public class EncryptDataBodyAdapter : IPackageAdapter
    {
        public int HeadCount { get; set; } = 0;
        public int Password { get; set; } = 123456789;

        public void Pack(ISegment stream)
        {
            EncryptHelper.ToEncrypt(Password, stream.Buffer, stream.Position, stream.Count - stream.Position);
        }

        public bool Unpack(ISegment stream, int frame, int uid)
        {
            EncryptHelper.ToDecrypt(Password, stream.Buffer, stream.Position, stream.Count - stream.Position);
            return true;
        }
    }
}
