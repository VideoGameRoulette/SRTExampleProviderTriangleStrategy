using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SRTExampleProvider64
{
    /// <summary>
    /// SHA256 hashes for Triangle Strategy game executables.
    /// 
    /// Triangle Strategy (WW): https://steamdb.info/app/1850510/ / https://steamdb.info/depot/1850510/
    /// </summary>
    /// 

    public enum GameVersion
    {
        UNKNOWN,
        RELEASE_16901882
    }

    public static class GameHashes
    {
        public static string ToHexString(this byte[] value) => string.Format("new byte[{0}] {{ {1} }};", value.Length, value.Select((byte a) => "0x" + a.ToString("X2")).Aggregate((string prev, string curr) => prev + ", " + curr));

        private static readonly byte[] Release_16901882 = new byte[32] { 0xAF, 0xDD, 0xDD, 0x15, 0x87, 0x8F, 0x30, 0xBD, 0xE1, 0x1C, 0x78, 0x76, 0xB5, 0xD5, 0x4F, 0x92, 0x82, 0x0A, 0x92, 0x01, 0x68, 0x1D, 0x65, 0xC2, 0xC4, 0xF4, 0xA0, 0x02, 0x16, 0x87, 0xA5, 0x25 };

        public static GameVersion DetectVersion(string filePath)
        {
            byte[] checksum;
            using (SHA256 hashFunc = SHA256.Create())
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                checksum = hashFunc.ComputeHash(fs);

            if (checksum.SequenceEqual(Release_16901882))
                return GameVersion.RELEASE_16901882;
            else
            {
                Console.WriteLine("Unknown Version: ", ToHexString(checksum));
                return GameVersion.UNKNOWN;
            }
        }
    }
}