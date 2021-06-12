using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Infrastructore.Core.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting
{
  [TestFixture]
  class EncryptionTests : BaseUnitTest
  {

    [Test]
    public void NormalAesEncryption()
    {
      var data = Randomz.GetRandomNumber(14).ToString();
      byte[] encrypted = null;
      byte[] key = null;
      byte[] iv = null;

      using (Aes aes = Aes.Create())
      {
        key = aes.Key;
        iv = aes.IV;

        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
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
      }

      string plainText = "";
      using (Aes aes = Aes.Create())
      {
        aes.Key = key;
        aes.IV = iv;

        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using (MemoryStream msDecrypt = new MemoryStream(encrypted))
        {
          using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
          {
            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
            {
              plainText = srDecrypt.ReadToEnd();
            }
          }
        }
      }

      Assert.AreEqual(plainText, data);

    }

    [Test]
    public void EncryptionTest()
    {
      var key = Guid.NewGuid().ToByteArray();
      var data = Randomz.GetRandomNumber(14).ToString();
      var enc = AesEncyHelper.Encyrpt(data, key, key);

      var dec = AesEncyHelper.Decyrpt(enc, key, key);
      Assert.AreEqual(dec, data);
    }
  }
}
