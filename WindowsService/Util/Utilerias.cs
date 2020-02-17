using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Npgsql;

namespace WindowsService1.Util
{
    public class Utilerias
    {
        NpgsqlConnection con;
        Conexion.Conexion conex = new Conexion.Conexion();
        NpgsqlCommand comP = new NpgsqlCommand();

        public Utilerias()
        {
            //Constructor
            con = conex.ConnexionDB();
        }

        public string encriptar(string cadena)
        {
            HashAlgorithm hashAlgorithm = (HashAlgorithm)new SHA512Managed();
            for (int index = 1; index <= 5; ++index)
                cadena = Convert.ToBase64String(hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(cadena + 5)));
            hashAlgorithm.Clear();
            return cadena;
        }

        /// <summary>
        /// Metodo que recibira la contrasenia para la conexion y la encriptara
        /// </summary>
        /// <param name="cadenaOriginal"></param>
        public string encriptacion(string cadenaOriginal)
        {
            try
            {
                // Crea una nueva instancia de Rijndael
                // clase. Esto genera una nueva clave e inicialización
                // vector (IV).
                using (Rijndael myRijndael = Rijndael.Create())
                {
                    string cadenaBytes = string.Empty;
                    //Cifrar la cadena a una matriz de bytes.
                    byte[] encrypted = EncryptStringToBytes(cadenaOriginal, myRijndael.Key, myRijndael.IV);

                    //covierte el arreglo de bytes a estring para guardar en la base de datos
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < encrypted.Length; i++)
                    {
                        cadenaBytes += builder.Append(encrypted[i].ToString("x2"));
                    }
                    return cadenaBytes;
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                throw;
            }
        }

        public void desencripta(byte[] encriptado, Int64 idEmpresa)
        {
            //Descifrar los bytes a una cadena.
            using (Rijndael myRijndael = Rijndael.Create())
            {
                con.Open();
                string select_contra = "select contra_bytes from empresa where id = " + idEmpresa;
                comP = new NpgsqlCommand(select_contra, con);
                con.Open();
                byte[] contrasenia = comP.ExecuteScalar() as byte[];
                con.Close();

                string select_llave = "select llave from empresa where id = " + idEmpresa;
                comP = new NpgsqlCommand(select_llave, con);
                con.Open();
                byte[] llave = comP.ExecuteScalar() as byte[];
                con.Close();

                string select_apuntador = "select apuntador from empresa where id = " + idEmpresa;
                comP = new NpgsqlCommand(select_apuntador, con);
                con.Open();
                byte[] apuntador = comP.ExecuteScalar() as byte[];
                con.Close();
                string roundtrip = DecryptStringFromBytes(encriptado, myRijndael.Key, myRijndael.IV);
            }
        }

        /// <summary>
        /// Metodo para encriptar
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="Key"></param>
        /// <param name="IV"></param>
        /// <returns></returns>
        public static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            //verifican los argumentos
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Crear un objeto Rijndael
            // con la clave especificada y IV.
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = Key;
                Console.WriteLine("Error: {0}" + Key);
                rijAlg.IV = IV;

                //Cree un encriptador para realizar la transformación de flujo
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Crea las secuencias utilizadas para el cifrado.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Escribe todos los datos en la secuencia
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            //Devuelve los bytes encriptados de la secuencia de memoria.
            return encrypted;
        }

        public static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Valida argumentos.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declara la cadena que va a contener el texto descrifado
            string plaintext = null;

            //Crear un objeto Rijndael con la clave especificada y IV.
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Crea un descifrador para realizar la transformación de flujo.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Cree las secuencias utilizadas para el descifrado.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Lea los bytes descifrados de la secuencia de descifrado y los coloca en una cadena.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
    }
}
