using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class Security
{
    static byte[] ivBytes = new byte[16]; 
    static byte[] keyBytes = new byte[16]; 
    static byte[] ivBytes2 = new byte[16]; 
    static byte[] keyBytes2 = new byte[16]; 
    const string keyString = "Knight_Adventura_A_2D_Platformer_Game[!24`5%$#~43e`5]";
    const string keyString2 = "2D_Platformer_Game[najn!~a67^%$31567820]";
    // private static readonly string XORencryptionCode = "ahu.,?;a56%$#1&*^||][:,magvxhn*@!~";
    // public static string EncryptDecryptUsingXOR(string data) 
    // {
    //     string modifiedData = "";

    //     for (int i = 0; i < data.Length; i++) 
    //     {
    //         modifiedData += (char)(data[i] ^ XORencryptionCode[i % XORencryptionCode.Length]);
    //     }

    //     return modifiedData;
    // }

    public static byte[] EncryptUsingAES(string data,string key,string IV)
    {
        byte[] encrypted = null;

        try
        {
            byte[] iv = Encoding.ASCII.GetBytes(IV);
            byte[] keyBytes = Encoding.ASCII.GetBytes(key);

            using (Aes myAes = Aes.Create())
            {
                myAes.Key = keyBytes;
                myAes.IV = iv;

                // Debug.Log("Encrypt Key : "+ PrintBytes(myAes.Key));
                // Debug.Log("Encrypt IV : " + PrintBytes(myAes.IV));

                // Encrypt the string to an array of bytes.
                encrypted = EncryptStringToBytes(data,myAes.Key,myAes.IV);
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Error : " + e);
        }

        return encrypted;
    }
    
    public static string DecryptUsingAES(byte[] bytes,string key,string IV)
    {
        string result = "";

        try
        {   
            byte[] iv = Encoding.ASCII.GetBytes(IV);
            byte[] keyBytes = Encoding.ASCII.GetBytes(key);

            using (Aes myAes = Aes.Create())
            {
                myAes.Key = keyBytes;
                myAes.IV = iv;

                // Debug.Log("Decrypt Key : "+ PrintBytes(myAes.Key));
                // Debug.Log("Decrypt IV : " + PrintBytes(myAes.IV));
                    
                // Decrypt the bytes to a string.
                result = DecryptStringFromBytes(bytes,myAes.Key,myAes.IV);
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Error : " + e);
        }

        return result;
    }

    static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
    {
        // Check arguments.
        if (plainText == null || plainText.Length <= 0)
        {
            throw new ArgumentNullException("plainText");
        }
        if (Key == null || Key.Length <= 0)
        {
            throw new ArgumentNullException("Key");
        }
        if (IV == null || IV.Length <= 0)
        {
            throw new ArgumentNullException("IV");
        }

        byte[] encrypted;

        // Create an Aes object
        // with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        // Return the encrypted bytes from the memory stream.
        return encrypted;
    }

    static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
    {
        // Check arguments.
        if (cipherText == null || cipherText.Length <= 0)
        {
            throw new ArgumentNullException("cipherText");
        }
        if (Key == null || Key.Length <= 0)
        {
            throw new ArgumentNullException("Key");
        }
        if (IV == null || IV.Length <= 0)
        {
            throw new ArgumentNullException("IV");
        }

        // Declare the string used to hold
        // the decrypted text.
        string plaintext = null;

        // Create an Aes object
        // with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            // Create a decryptor to perform the stream transform.
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {

                        // Read the decrypted bytes from the decrypting stream
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        return plaintext;
    }
 
    static void GenerateIVBytes()
    {
        int randomNumber = UnityEngine.Random.Range(0,1000000);
        System.Random randomizer = new System.Random(randomNumber);
        randomizer.NextBytes(ivBytes);
    }

    static void GenerateKeyBytes()
    {
        int sum = 0;
        foreach (char curChar in keyString)
        {
            sum += curChar;
        }
   
        System.Random randomizer = new System.Random(sum);
        randomizer.NextBytes(keyBytes);
    }
 
    public static string Encrypt(string text)
    {
        GenerateIVBytes();
        GenerateKeyBytes();

        //Debug.Log("Encrypted Key For Key : " + PrintBytes(keyBytes));
        //Debug.Log("Encrypted IV For Key : " + PrintBytes(ivBytes));

        SymmetricAlgorithm algorithm = Aes.Create();
        ICryptoTransform transform = algorithm.CreateEncryptor(keyBytes,ivBytes);
        byte[] inputBuffer = Encoding.Unicode.GetBytes(text);
        byte[] outputBuffer = transform.TransformFinalBlock(inputBuffer,0,inputBuffer.Length);
 
        string ivString = Encoding.Unicode.GetString(ivBytes);
        string encryptedString = Convert.ToBase64String(outputBuffer);
 
        return ivString + encryptedString;
    }
 
    public static string Decrypt(this string text)
    {
        GenerateIVBytes();
        GenerateKeyBytes();
 
        int endOfIVBytes = ivBytes.Length / 2;  // Half length because unicode characters are 64-bit width
 
        string ivString = text.Substring(0,endOfIVBytes);
        byte [] extractedivBytes = Encoding.Unicode.GetBytes(ivString);
 
        string encryptedString = text.Substring(endOfIVBytes);

        // Debug.Log("Decrypted Key For Key : " + PrintBytes(keyBytes));
        // Debug.Log("Decrypted IV For Key : " + PrintBytes(extractedivBytes));

        SymmetricAlgorithm algorithm = Aes.Create();
        ICryptoTransform transform = algorithm.CreateDecryptor(keyBytes,extractedivBytes);
        byte [] inputBuffer = Convert.FromBase64String(encryptedString);
        byte [] outputBuffer = transform.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length );
 
        string decryptedString = Encoding.Unicode.GetString(outputBuffer);
 
        return decryptedString;
    }

    static void GenerateIVBytes2()
    {
        int randomNumber = UnityEngine.Random.Range(2,123456789);
        System.Random randomizer = new System.Random(randomNumber);
        randomizer.NextBytes(ivBytes2);
    }
    
    static void GenerateKeyBytes2()
    {
        int sum = 0;
        foreach (char curChar in keyString2)
        {
            sum += curChar;
        }
    
        System.Random randomizer = new System.Random(sum);
        randomizer.NextBytes(keyBytes2);
    }
 
    public static string Encrypt2(string text)
    {
        GenerateIVBytes2();
        GenerateKeyBytes2();

        // Debug.Log("Encrypted Key For IV : " + PrintBytes(keyBytes2));
        // Debug.Log("Encrypted IV For IV : " + PrintBytes(ivBytes2));

        SymmetricAlgorithm algorithm = Aes.Create();
        ICryptoTransform transform = algorithm.CreateEncryptor(keyBytes2,ivBytes2);
        byte[] inputBuffer = Encoding.Unicode.GetBytes(text);
        byte[] outputBuffer = transform.TransformFinalBlock(inputBuffer,0,inputBuffer.Length);
 
        string ivString = Encoding.Unicode.GetString(ivBytes2);
        string encryptedString = Convert.ToBase64String(outputBuffer);
 
        return ivString + encryptedString;
    }
 
    public static string Decrypt2(this string text)
    {
        GenerateIVBytes2();
        GenerateKeyBytes2();
 
        int endOfIVBytes = ivBytes2.Length / 2;  // Half length because unicode characters are 64-bit width
 
        string ivString = text.Substring(0,endOfIVBytes);
        byte [] extractedivBytes = Encoding.Unicode.GetBytes(ivString);
 
        string encryptedString = text.Substring(endOfIVBytes);

        // Debug.Log("Decrypted Key For IV : " + PrintBytes(keyBytes2));
        // Debug.Log("Decrypted IV For IV : " + PrintBytes(extractedivBytes));

        SymmetricAlgorithm algorithm = Aes.Create();
        ICryptoTransform transform = algorithm.CreateDecryptor(keyBytes2,extractedivBytes);
        byte [] inputBuffer = Convert.FromBase64String(encryptedString);
        byte [] outputBuffer = transform.TransformFinalBlock(inputBuffer,0,inputBuffer.Length);
 
        string decryptedString = Encoding.Unicode.GetString(outputBuffer);
 
        return decryptedString;
    }
    
    static string PrintBytes(this byte[] byteArray)
    {
        var sb = new StringBuilder("{ ");
        for(var i = 0; i < byteArray.Length;i++)
        {
            var b = byteArray[i];
            sb.Append(b);
            if (i < byteArray.Length-1)
            {
                sb.Append(", ");
            }
        }
        sb.Append(" }");
        return sb.ToString();
    }

    public static string RandomKeyGenerator()
    {
        System.Random rand = new System.Random();
  
        // Choosing the size of string
        // Using Next() string
        int randValue;
        string str = "";
        char letter;

        for(int i = 1; i <= 32; i++)
        {
            // Generating a random number.
            randValue = rand.Next(33,126);
            // Generating random character by converting
            // the random number into character.
            letter = Convert.ToChar(randValue);
            // Appending the letter to string.
            str = str + letter;
        }

        return str;
    }

    public static string RandomIVGenerator()
    {
        System.Random rand = new System.Random();
  
        // Choosing the size of string
        // Using Next() string
        int randValue;
        string str = "";
        char letter;

        for(int i = 1; i <= 16; i++)
        {
            // Generating a random number.
            randValue = rand.Next(0,31);
            // Generating random character by converting
            // the random number into character.
            letter = Convert.ToChar(randValue+rand.Next(33,95));
            // Appending the letter to string.
            str = str + letter;
        }

        return str;
    }
}
