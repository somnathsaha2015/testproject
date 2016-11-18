using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Runtime.Serialization.Json;

namespace TBUtility
{
    public class CommonUtility
    {
        public static void CopyProperties(object objSource, object objDestination)
        {
            //get the list of all properties in the destination object
            var destProps = objDestination.GetType().GetProperties();

            //get the list of all properties in the source object
            foreach (var sourceProp in objSource.GetType().GetProperties())
            {
                foreach (var destProperty in destProps)
                {
                    //if we find match between source & destination properties name, set
                    //the value to the destination property
                    if (destProperty.Name == sourceProp.Name)
                    {
                        destProperty.SetValue(objDestination, sourceProp.GetValue(objSource));
                        //destProperty.SetValue(destProps, sourceProp.GetValue(
                        //    sourceProp, new object[] { }), new object[] { });
                        break;
                    }
                }
            }
        }

        public static string SerializeAsJSONString<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms,obj);
                StringBuilder sb = new StringBuilder(Encoding.Default.GetString(ms.GetBuffer()).Replace("\0", string.Empty));
                byte[] bArray = ms.ToArray();
                return (Encoding.ASCII.GetString(bArray));
            }
        }

        public static T DeSerializeJSONStringAsObject<T>(string jsonString)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            var obj = Encoding.ASCII.GetBytes(jsonString);
            T t;
            using (MemoryStream ms = new MemoryStream(obj))
            {
                t = (T)serializer.ReadObject(ms);
            }
            return (t);
        }

        public static string SerializeAsXML<T>(T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.Serialize(ms, obj);
                StringBuilder sb = new StringBuilder(Encoding.Default.GetString(ms.GetBuffer()).Replace("\0", string.Empty));
                byte[] bArray = ms.ToArray();
                return (Encoding.ASCII.GetString(bArray));
            }
        }

        public static T DeSerializeAsObject<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            var obj = Encoding.Default.GetBytes(xml);
            T t;
            using (MemoryStream ms = new MemoryStream(obj))
            {
                t = (T)serializer.Deserialize(ms);
            }
            return (t);
        }

        //new version from web site http://dotnetspeak.com/index.php/2010/08/encryption-in-silverlight-and-net-applications/
        public static string Decrypt(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return (string.Empty);
            }
            const string password = "15857566";
            byte[] encryptedBytes = Convert.FromBase64String(input);
            byte[] saltBytes = Encoding.UTF8.GetBytes(password);
            string decryptedString = string.Empty;
            using (var aes = new AesManaged())
            {
                using (Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, saltBytes))
                {
                    aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                    aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                    aes.Key = rfc.GetBytes(aes.KeySize / 8);
                    aes.IV = rfc.GetBytes(aes.BlockSize / 8);
                }

                using (ICryptoTransform decryptTransform = aes.CreateDecryptor())
                {
                    using (MemoryStream decryptedStream = new MemoryStream())
                    {
                        CryptoStream decryptor =
                            new CryptoStream(decryptedStream, decryptTransform, CryptoStreamMode.Write);
                        decryptor.Write(encryptedBytes, 0, encryptedBytes.Length);
                        decryptor.Flush();
                        decryptor.Close();

                        byte[] decryptBytes = decryptedStream.ToArray();
                        decryptedString =
                            UTF8Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);
                    }
                }
            }
            return decryptedString;
        }

        public static string Encrypt(string input)
        {
            const string password = "15857566";
            byte[] utfData = UTF8Encoding.UTF8.GetBytes(input);
            byte[] saltBytes = Encoding.UTF8.GetBytes(password);
            string encryptedString = string.Empty;
            using (AesManaged aes = new AesManaged())
            {
                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, saltBytes);

                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                aes.Key = rfc.GetBytes(aes.KeySize / 8);
                aes.IV = rfc.GetBytes(aes.BlockSize / 8);

                using (ICryptoTransform encryptTransform = aes.CreateEncryptor())
                {
                    using (MemoryStream encryptedStream = new MemoryStream())
                    {
                        using (CryptoStream encryptor =
                            new CryptoStream(encryptedStream, encryptTransform, CryptoStreamMode.Write))
                        {
                            encryptor.Write(utfData, 0, utfData.Length);
                            encryptor.Flush();
                            encryptor.Close();

                            byte[] encryptBytes = encryptedStream.ToArray();
                            encryptedString = Convert.ToBase64String(encryptBytes);
                        }
                    }
                }
            }
            return encryptedString;
        } 
    }
}