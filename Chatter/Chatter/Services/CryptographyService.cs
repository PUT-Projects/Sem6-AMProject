using Chatter.Models;
using Chatter.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Services;

public class CryptographyService
{
    private readonly UserDataRepository _userDataRepository;
    private RSAParameters _myKeyPair;
    private RSAParameters _friendPublicKey;

    public string ReceiverXmlKey {
        set => ImportFriendPublicKey(value);
    }

    public CryptographyService(UserDataRepository userDataRepository)
    {
        _userDataRepository = userDataRepository;
        var userData = _userDataRepository.GetCurrentUserData();


        if (userData is not null) ImportMyKeyPair(userData.XmlKeys);
    }
    public void UpdateKeyPair()
    {
        var userData = _userDataRepository.GetCurrentUserData();

        //if (userData is null) {
        //    string key = GenerateNewKeyPairXml();
        //    _userDataRepository.AddUserData(key);

        //}

        ImportMyKeyPair(userData.XmlKeys);
    }
    public EncryptedMessage EncryptMessage(string message)
    {
        using Aes aes = Aes.Create();
        aes.GenerateKey();
        aes.GenerateIV();

        // Encrypt the message with AES
        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using (var swEncrypt = new StreamWriter(csEncrypt)) {
            swEncrypt.Write(message);
        }

        byte[] encryptedMessage = msEncrypt.ToArray();

        // Encrypt the AES key and IV with the friend's public RSA key
        using var rsa = new RSACryptoServiceProvider();
        rsa.ImportParameters(_friendPublicKey);

        byte[] encryptedAesKey = rsa.Encrypt(aes.Key, false);
        byte[] encryptedIV = rsa.Encrypt(aes.IV, false);

        var output = new EncryptedMessage {
            Content = Convert.ToBase64String(encryptedMessage),
            Key = Convert.ToBase64String(encryptedAesKey),
            IV = Convert.ToBase64String(encryptedIV)
        };

        return output;
    }

    public string DecryptMessage(string encryptedAesKeyStr, string encryptedIVStr, string encryptedMessageStr)
    {
        byte[] encryptedAesKey = Convert.FromBase64String(encryptedAesKeyStr);
        byte[] encryptedIV = Convert.FromBase64String(encryptedIVStr);
        byte[] encryptedMessage = Convert.FromBase64String(encryptedMessageStr);

        using var rsa = new RSACryptoServiceProvider();
        rsa.ImportParameters(_myKeyPair);
        byte[] aesKey = rsa.Decrypt(encryptedAesKey, false);
        byte[] iv = rsa.Decrypt(encryptedIV, false);

        using Aes aes = Aes.Create();
        aes.Key = aesKey;
        aes.IV = iv;

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var msDecrypt = new MemoryStream(encryptedMessage);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        return srDecrypt.ReadToEnd();
    }



    public static string GenerateNewKeyPairXml()
    {
        using var rsa = new RSACryptoServiceProvider(2048);

        return rsa.ToXmlString(true);
    }

    private void ImportMyKeyPair(string keysXml)
    {
        using var rsa = new RSACryptoServiceProvider();
        rsa.FromXmlString(keysXml);

        _myKeyPair = rsa.ExportParameters(true);
    }

    private void ImportFriendPublicKey(string publicKeyXml)
    {
        using var rsa = new RSACryptoServiceProvider();
        rsa.FromXmlString(publicKeyXml);

        _friendPublicKey = rsa.ExportParameters(false);
    }

    public void UpdateRSA()
    {
        _userDataRepository.UpdateConnection();
        UpdateKeyPair();
    }
}
