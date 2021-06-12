using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Geeky.POSK.Infrastructore.Core
{
  public class AesEncyHelper
  {
    public static byte[] Encyrpt(string data, byte[] key, byte[] iv)
    {
      using (Aes aes = Aes.Create())
      {
        if (key == null || key.Length < 16) throw new Exception("Encryption Key not supplied, or key is invalid");
        key = key.Take(16).ToArray();

        if (iv == null || iv.Length < 16) throw new Exception("Encryption Key not supplied, or key is invalid");
        iv = iv.Take(16).ToArray();

        aes.Key = key;
        aes.IV = iv;
        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        byte[] encrypted = null;
        using (MemoryStream msEncrypt = new MemoryStream())
        {
          using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
          {
            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
            {
              swEncrypt.Write(data);
            }
            encrypted = msEncrypt.ToArray();
          }
        }

        return encrypted;
      }
    }

    public static string Decyrpt(byte[] data, byte[] key, byte[] iv)
    {
      using (Aes aes = Aes.Create())
      {
        if (key == null || key.Length < 16) throw new Exception("Encryption Key not supplied, or key is invalid");
        key = key.Take(16).ToArray();

        if (iv == null || iv.Length < 16) throw new Exception("Encryption Key not supplied, or key is invalid");
        iv = iv.Take(16).ToArray();

        aes.Key = key;
        aes.IV = iv;

        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        string plainText = "";
        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using (MemoryStream msDecrypt = new MemoryStream(data))
        {
          using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
          {
            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
            {
              plainText = srDecrypt.ReadToEnd();
            }
          }
        }

        return plainText;
      }
    }

    public static byte[] ReEncrypt(byte[] data, Guid key, Guid newKey)
    {
      var decrypted = Decyrpt(data, key.ToByteArray(), key.ToByteArray());
      var encrypted = Encyrpt(decrypted, newKey.ToByteArray(), newKey.ToByteArray());
      return encrypted;
    }

  }
}
