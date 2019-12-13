using System;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Diagnostics;

namespace DataEncryption
{
    class Program
    {
        public static string path, path2;
        /// <summary>
        /// Contains the instructions to manage files
        /// </summary>
        public static List<string> fileformatNotAllowed;

        /// <summary>
        /// (optional) 
        /// This part is used to define a list of file extensions
        ///     autorized to be encrypted
        /// </summary>
        public static List<string> loadConf()
        {
            /// give the path to your file config.json. In my case it is :
            string pathConf = @"C:\Users\Thibault\source\repos\DataEncryption\config.json";
            List<string> fileNotAllowed = new List<string>();
            if (File.Exists(pathConf))
            {
                using (StreamReader reader = new StreamReader(new FileStream(pathConf, FileMode.Open)))
                {
                    string s;
                    while ((s = reader.ReadLine()) != null)
                    {
                        fileNotAllowed.Add(s);
                    }
                }
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(pathConf))
                {
                    writer.Write("");
                }
            }
            return fileNotAllowed;
        }
        public static void definePath()
        {
            /*
            ******************* Defining the path of the file to encrypt *******************
            * The program asks the user to tell him where to search for the file to encrypt
            */
            Console.WriteLine("Please give me the path of the file to encrypt.");
            path = Console.ReadLine();
            // Testing if the path is correct
            while (!File.Exists(path))
            {
                Console.WriteLine("\nInvalid path, please try again...");
                path = Console.ReadLine();
            }
            Console.WriteLine("Valid path, moving to next step...");

            /*
            ***************** Defining the path to save the encrypted file *****************
            * The program asks the user to tell him where to save the encrypted file
            */
            Console.WriteLine("\nPlease tell me where i have to save the encrypted file.");
            path2 = Console.ReadLine();
            // Testing if the path is correct
            while (!File.Exists(path2))
            {
                Console.WriteLine("\nInvalid path, please try again...");
                path2 = Console.ReadLine();
            }
            Console.WriteLine("Valid path, moving to next step... \n");
            foreach (string ext in fileformatNotAllowed)
            {
                Console.WriteLine(ext + "=====" + Path.GetExtension(path));
                if (Path.GetExtension(path) == ext)
                {
                    Console.WriteLine("Crypting useful => moving to next step...\n");
                }
                else
                {
                    Console.WriteLine("Crypting useless =>ENDING PROGRAM");
                    Environment.Exit(1);
                }
            }
        }

        /// <summary>
        /// Contains all the encryption and decryption steps
        /// </summary>
        public static void crypting()
        {
            /*
            ****** Accessing the managed version of the Rijndael encryption algorithm ******
            * The program uses Rijndael, the symmetric encryption algorithm used by the AES
            */
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            // Creating a new file using path2 to save the encrypted data
            using (FileStream filestream = new FileStream(path2, FileMode.Create))
            {
                // Encrypting the data
                using (CryptoStream stream = new CryptoStream(filestream, rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, rijndaelManaged.IV), CryptoStreamMode.Write))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        using (StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open)))
                        {
                            // Reading the file to encrypt
                            string contents = reader.ReadToEnd();
                            // Writing the encrypted data in the new file
                            writer.Write(contents);
                        }
                    }
                }
            }

            // Once encryption is complete, updating the status in the console
            Console.WriteLine("Encryption is done ! \n");
            // Displaying the encrypted data
            Console.WriteLine("The encrypted data is : ");
            using (StreamReader reader = new StreamReader(new FileStream(path2, FileMode.Open)))
            {
                string contents = reader.ReadToEnd();
                Console.WriteLine(contents);
            }

            /*
            * Rijndael is a symmetric algorithm, the program follows the same process
            *   as above to decrypt the encrypted file
            */
            Console.WriteLine("\nThe decrypted data is : ");
            using (FileStream filestream = new FileStream(path2, FileMode.Open))
            {
                using (CryptoStream stream = new CryptoStream(filestream, rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV), CryptoStreamMode.Read))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string contents = reader.ReadToEnd();
                        Console.WriteLine(contents);
                    }
                }
            }
            Console.WriteLine("\nDecryption is done !");
        }

        /// <summary>
        /// Contains all the methods to run the program
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                path = args[0];
                path2 = Path.GetFileNameWithoutExtension(args[0]) + "crypted" + Path.GetExtension(args[0]);
            }

            // Calling the method definePath() defined above to get the file path
            if (args.Length == 0)
            {
                // Initializing the program
                Console.WriteLine("      ____________________________________________________________________________________\n");
                Console.WriteLine("       .o88b. d8888b. db    db d8888b. d888888b  .d88b.  .d8888.  .d88b.  d88888b d888888b  ");
                Console.WriteLine("      d8P  Y8 88  `8D `8b  d8' 88  `8D `~~88~~' .8P  Y8. 88'  YP .8P  Y8. 88'     `~~88~~'  ");
                Console.WriteLine("      8P      88oobY'  `8bd8'  88oodD'    88    88    88 `8bo.   88    88 88ooo      88     ");
                Console.WriteLine("      8b      88`8b      88    88~~~      88    88    88   `Y8b. 88    88 88~~~      88     ");
                Console.WriteLine("      Y8b  d8 88 `88.    88    88         88    `8b  d8' db   8D `8b  d8' 88         88     ");
                Console.WriteLine("       `Y88P' 88   YD    YP    88         YP     `Y88P'  `8888Y'  `Y88P'  YP         YP     ");
                Console.WriteLine("      ____________________________________________________________________________________  ");
                Console.WriteLine("                                                                                            ");
                Console.WriteLine("                     Encryption and decryption solution for personnal usage                 ");
                Console.WriteLine("      ____________________________________________________________________________________  ");
                Console.WriteLine("                                                                                            ");

                //
                Console.WriteLine("Hello and welcome on CryptoSoft !\n");
                Console.WriteLine("Press any key to start the process...\n");
                Console.ReadKey();

                fileformatNotAllowed = loadConf();
                definePath();
            }


            // Calling the method crypting() defined above to crypt the file content
            Stopwatch sw = Stopwatch.StartNew();
            crypting();
            sw.Stop();
            Console.WriteLine("\nTime taken : {0} ms", (sw.Elapsed.TotalMilliseconds).ToString(".000"));


            // Waiting for the user to press a key before ending process
            if (args.Length == 0)
            {
                Console.WriteLine("\nPress any key to exit the process...");
                Console.ReadKey();
            }
            else
            {
                Environment.Exit(1);
            }
        }
    }
}

