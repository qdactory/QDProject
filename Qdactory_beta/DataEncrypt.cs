using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Qdactory_beta
{
    public class DataEncrypt
    {
        public static class Global
        {
            public const String strPermutation = "ouiveyxaqtd";
            public const Int32 bytePermutation1 = 0x19;
            public const Int32 bytePermutation2 = 0x59;
            public const Int32 bytePermutation3 = 0x17;
            public const Int32 bytePermutation4 = 0x41;
        }

        public DataEncrypt()
        {

        }

        public string Encrypt(string strData)
        {
            return Convert.ToBase64String(EncryptBt(Encoding.UTF8.GetBytes(strData)));
        }

        public string Decrypt(string strData)
        {
            return Encoding.UTF8.GetString(Decryptbt(Convert.FromBase64String(strData)));
        }

        public string HardEncrypt(string strData)
        {
            byte[] salt = GetSalt();
            var pass = new Rfc2898DeriveBytes(strData, salt, 1000);
            byte[] hash = pass.GetBytes(20);
            byte[] hBytes = new byte[36];
            Array.Copy(salt, 0, hBytes, 0, 16);
            Array.Copy(hash, 0, hBytes, 16, 20);
            string hashedPassword = Convert.ToBase64String(hBytes);
            return hashedPassword;
        }

        public bool CheckHardEncrypt(string strData, string input)
        {
            byte[] HBytes = Convert.FromBase64String(strData);
            var MyB = MySalt(HBytes, input);
            return IsMatch(MyB, HBytes);
        }

        public byte[] MySalt(byte[] hBytes, string strData)
        {
            byte[] salt = new byte[16];
            Array.Copy(hBytes, 0, salt, 0, 16);
            var hashPass = new Rfc2898DeriveBytes(strData, salt, 1000);
            byte[] hash = hashPass.GetBytes(20);
            return hash;
        }

        public bool IsMatch(byte[] hash, byte[] hBytes)
        {
            bool success = true;
            for (int i = 0; i < 20; i++)
                if (hBytes[i + 16] != hash[i])
                    success = false;
            return success;

        }

        public byte[] GetSalt()
        {
            byte[] salt;

            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            return salt;
        }

        public static byte[] EncryptBt(byte[] strData)
        {
            PasswordDeriveBytes passbytes =
            new PasswordDeriveBytes(Global.strPermutation,
            new byte[] { Global.bytePermutation1,
                        Global.bytePermutation2,
                        Global.bytePermutation3,
                        Global.bytePermutation4
            });

            MemoryStream memstream = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = passbytes.GetBytes(aes.KeySize / 8);
            aes.IV = passbytes.GetBytes(aes.BlockSize / 8);

            CryptoStream cryptostream = new CryptoStream(memstream,
            aes.CreateEncryptor(), CryptoStreamMode.Write);
            cryptostream.Write(strData, 0, strData.Length);
            cryptostream.Close();
            return memstream.ToArray();
        }

        public static byte[] Decryptbt(byte[] strData)
        {
            PasswordDeriveBytes passbytes =
            new PasswordDeriveBytes(Global.strPermutation,
            new byte[] { Global.bytePermutation1,
                        Global.bytePermutation2,
                        Global.bytePermutation3,
                        Global.bytePermutation4
            });

            MemoryStream memstream = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = passbytes.GetBytes(aes.KeySize / 8);
            aes.IV = passbytes.GetBytes(aes.BlockSize / 8);

            CryptoStream cryptostream = new CryptoStream(memstream,
            aes.CreateDecryptor(), CryptoStreamMode.Write);
            cryptostream.Write(strData, 0, strData.Length);
            cryptostream.Close();
            return memstream.ToArray();
        }
    }
}
