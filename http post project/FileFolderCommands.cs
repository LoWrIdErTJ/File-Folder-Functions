using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using UBotPlugin;
using System.Linq;
using System.Windows;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Security.Cryptography;
using System.Configuration;
using System.Media;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Net;
using System.Management;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Data.SqlClient;
using Microsoft.VisualBasic;
using System.Reflection;
using System.Data.OleDb;
using System.IO.Compression;

namespace FileFolderCommands
{

    // API KEY HERE
    public class PluginInfo
    {
        public static string HashCode { get { return "537e8acd741741cdb30d2e8a4f69c3f49c53b170"; } }
    }

    // ---------------------------------------------------------------------------------------------------------- //
    //
    // ---------------------------------               COMMANDS               ----------------------------------- //
    //
    // ---------------------------------------------------------------------------------------------------------- //

    //
    //
    // DELETE ALL IN FOLDER
    //
    //
    public class DeleteFolderContents : IUBotCommand
    {

        private List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();

        public DeleteFolderContents()
        {
            _parameters.Add(new UBotParameterDefinition("Path to folder", UBotType.String));

        }

        public string Category
        {
            get { return "File Commands"; }
        }

        public string CommandName
        {
            get { return "delete folder contents"; }
        }


        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {

            string folder = parameters["Path to folder"];

            Array.ForEach(Directory.GetFiles(folder), delegate(string path) { File.Delete(path); });

        }

        public bool IsContainer
        {
            get { return false; }
        }

        public IEnumerable<UBotParameterDefinition> ParameterDefinitions
        {
            get { return _parameters; }
        }

        public IEnumerable<string> Options
        {
            get;
            set;
        }

        public UBotVersion UBotVersion
        {
            get { return UBotVersion.Standard; }
        }
    }


    //
    //
    // DOWNLOAD GZIP ITEM
    //
    //
    public class DownLoadGZIPitem : IUBotCommand
    {

        private List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();

        public DownLoadGZIPitem()
        {
            _parameters.Add(new UBotParameterDefinition("URL to gzip item", UBotType.String));
            _parameters.Add(new UBotParameterDefinition("Save to file", UBotType.String));

        }

        public string Category
        {
            get { return "File Commands"; }
        }

        public string CommandName
        {
            get { return "Download GZIP item"; }
        }


        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {

            string url_of_file = parameters["URL to gzip item"];
            string save_to_file = parameters["Save to file"];

            DownloadFile(url_of_file, save_to_file);
            
        }

        private void DownloadFile(string url, string file)
        {
            byte[] result;
            byte[] buffer = new byte[4096];

            WebRequest wr = WebRequest.Create(url);
            wr.ContentType = "application/x-bittorrent";
            using (WebResponse response = wr.GetResponse())
            {
                bool gzip = response.Headers["Content-Encoding"] == "gzip";
                var responseStream = gzip
                                        ? new GZipStream(response.GetResponseStream(), CompressionMode.Decompress)
                                        : response.GetResponseStream();

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = responseStream.Read(buffer, 0, buffer.Length);
                        memoryStream.Write(buffer, 0, count);
                    } while (count != 0);

                    result = memoryStream.ToArray();

                    using (BinaryWriter writer = new BinaryWriter(new FileStream(file, FileMode.Create)))
                    {
                        writer.Write(result);
                    }
                }
            }
        }
        public bool IsContainer
        {
            get { return false; }
        }

        public IEnumerable<UBotParameterDefinition> ParameterDefinitions
        {
            get { return _parameters; }
        }

        public IEnumerable<string> Options
        {
            get;
            set;
        }

        public UBotVersion UBotVersion
        {
            get { return UBotVersion.Standard; }
        }
    }



    // ---------------------------------------------------------------------------------------------------------- //
    //
    // ---------------------------------               FUNCTIONS              ----------------------------------- //
    //
    // ---------------------------------------------------------------------------------------------------------- //


    //
    //
    // GET FILE INFO
    //
    //
    public class FileInfoFunct : IUBotFunction
    {

        private List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();
        private string _returnValue;

        public FileInfoFunct()
        {
            _parameters.Add(new UBotParameterDefinition("Path to file", UBotType.String));
            var xParameter = new UBotParameterDefinition("File info wanted?", UBotType.String);
            xParameter.Options = new[] { "", "File Name", "Date Time Created", "Extension", "Total Size", "File Path", "Full File Name", "Created Date/Time", "Last Accessed Date/Time", "Last Modified Date/Time" };
            _parameters.Add(xParameter);

        }

        public string Category
        {
            get { return "File Functions"; }
        }

        public string FunctionName
        {
            get { return "$get file info"; }
        }


        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {

            string ckpcinfo = parameters["File info wanted?"];
            string file = parameters["Path to file"];

            FileInfo nFileInfo = new FileInfo(file);
            string fData = string.Empty;

            if (nFileInfo != null)
            {

                if (ckpcinfo == "File Name")
                {

                    fData = nFileInfo.Name;
                    _returnValue = fData;

                }
                else if (ckpcinfo == "Date Time Created")
                {

                    DateTime dtCreationTime = nFileInfo.CreationTime;
                    fData = dtCreationTime.ToString();
                    _returnValue = fData;

                }
                else if (ckpcinfo == "Extension")
                {

                    fData = nFileInfo.Extension;
                    _returnValue = fData;

                }
                else if (ckpcinfo == "Total Size")
                {

                    fData = nFileInfo.Length.ToString();
                    _returnValue = fData;

                }
                else if (ckpcinfo == "File Path")
                {

                    fData = nFileInfo.DirectoryName;
                    _returnValue = fData;

                }
                else if (ckpcinfo == "Full File Name")
                {

                    fData = nFileInfo.FullName;
                    _returnValue = fData;

                }
                else if (ckpcinfo == "Created Date/Time")
                {
                    // local times
                    DateTime creationTime = nFileInfo.CreationTime;

                    fData = creationTime.ToString();
                    _returnValue = fData;

                }
                else if (ckpcinfo == "Last Accessed Date/Time")
                {
                    // local times
                    DateTime lastAccessTime = nFileInfo.LastAccessTime;

                    fData = lastAccessTime.ToString();
                    _returnValue = fData;

                }
                else if (ckpcinfo == "Last Modified Date/Time")
                {
                    // local times
                    // local times
                    DateTime lastWriteTime = nFileInfo.LastWriteTime;
                    DateTime lastAccessTime = nFileInfo.LastAccessTime;

                    fData = lastWriteTime.ToString();
                    _returnValue = fData;

                }

                else { }
            }
        }

        public object ReturnValue
        {
            // We return our variable _returnValue as the result of the function.
            get { return _returnValue; }
        }

        public UBotType ReturnValueType
        {
            // Our result is text, so the return value type is String.
            get { return UBotType.String; }
        }


        public bool IsContainer
        {
            get { return false; }
        }

        public IEnumerable<UBotParameterDefinition> ParameterDefinitions
        {
            get { return _parameters; }
        }

        public IEnumerable<string> Options
        {
            get;
            set;
        }

        public UBotVersion UBotVersion
        {
            get { return UBotVersion.Standard; }
        }
    }


    //
    //
    // SAFE FILE NAME
    //
    //
    public class SafeFileName : IUBotFunction
    {

        private List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();
        private string _returnValue;

        public SafeFileName()
        {
            _parameters.Add(new UBotParameterDefinition("Name of file", UBotType.String));

        }

        public string Category
        {
            get { return "File Functions"; }
        }

        public string FunctionName
        {
            get { return "$safe file name"; }
        }


        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {

            string fname = parameters["Name of file"];

            // remove spaces
            //var res1 = Regex.Replace(fname, "\\s+", "-");

            // a-z A-Z 0-9 and - only
            var res2 = Regex.Replace(fname, "[^a-zA-Z0-9-]+", "");

            _returnValue = res2.ToString();

        }



        public object ReturnValue
        {
            // We return our variable _returnValue as the result of the function.
            get { return _returnValue; }
        }

        public UBotType ReturnValueType
        {
            // Our result is text, so the return value type is String.
            get { return UBotType.String; }
        }


        public bool IsContainer
        {
            get { return false; }
        }

        public IEnumerable<UBotParameterDefinition> ParameterDefinitions
        {
            get { return _parameters; }
        }

        public IEnumerable<string> Options
        {
            get;
            set;
        }

        public UBotVersion UBotVersion
        {
            get { return UBotVersion.Standard; }
        }
    }


    //
    //
    // GET FOLDERS FROM SPECIFIED LOCATION
    //
    //
    public class GETMyFolders : IUBotFunction
    {

        private List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();
        private string _returnValue;

        public GETMyFolders()
        {
            _parameters.Add(new UBotParameterDefinition("Path to Location", UBotType.String));
            var returnFType = new UBotParameterDefinition("Return Data Type", UBotType.String);
            returnFType.Options = new[] { "", "Full Path", "Folder Name Only" };
            _parameters.Add(returnFType);
        }

        public string Category
        {
            get { return "Folder Functions"; }
        }

        public string FunctionName
        {
            get { return "$get folders"; }
        }


        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {
            string Location = parameters["Path to Location"];
            string StructureType = parameters["Return Data Type"];

            if (StructureType == "Full Path")
            {
                string[] result = System.IO.Directory.GetDirectories(Location);
                string c = "";

                //FIND ALL FOLDERS IN FOLDER
                foreach (string dir in result)
                {
                    c = c + "\n" + dir;
                }

                _returnValue = c;
            }
            else if (StructureType == "Folder Name Only")
            {
                string[] result = System.IO.Directory.GetDirectories(Location);
                string c = "";

                //FIND ALL FOLDERS IN FOLDER
                foreach (string dir in result)
                {
                    var dirName = new DirectoryInfo(dir).Name;
                    c = c + "\n" + dirName;
                }
                _returnValue = c;
            }
            else
            { }


        }


        public object ReturnValue
        {
            // We return our variable _returnValue as the result of the function.
            get { return _returnValue; }
        }

        public UBotType ReturnValueType
        {
            // Our result is text, so the return value type is String.
            get { return UBotType.String; }
        }

        public bool IsContainer
        {
            get { return false; }
        }

        public IEnumerable<UBotParameterDefinition> ParameterDefinitions
        {
            get { return _parameters; }
        }

        public UBotVersion UBotVersion
        {
            get { return UBotVersion.Standard; }
        }
    }


    //
    //
    // Create Temp File
    //
    //
    public class createTempFile : IUBotFunction
    {

        private List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();
        private string _returnValue;

        public createTempFile()
        {
            // no input
        }

        public string Category
        {
            get { return "File Functions"; }
        }

        public string FunctionName
        {
            get { return "$create temp file"; }
        }


        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {


            string tempFileName = Path.GetTempFileName();


            _returnValue = tempFileName.ToString();

        }

        public object ReturnValue
        {
            // We return our variable _returnValue as the result of the function.
            get { return _returnValue; }
        }

        public UBotType ReturnValueType
        {
            // Our result is text, so the return value type is String.
            get { return UBotType.String; }
        }

        public bool IsContainer
        {
            get { return false; }
        }

        public IEnumerable<UBotParameterDefinition> ParameterDefinitions
        {
            get { return _parameters; }
        }

        public UBotVersion UBotVersion
        {
            get { return UBotVersion.Standard; }
        }
    }



    //
    //
    // TITLE CASE FOR SENTENSES
    //
    //
    public class SentenceChange : IUBotFunction
    {

        private List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();
        private string _returnValue;

        public SentenceChange()
        {

            _parameters.Add(new UBotParameterDefinition("String to alter", UBotType.String));

        }

        public string Category
        {
            get { return "Text Functions"; }
        }

        public string FunctionName
        {
            get { return "$title case"; }
        }


        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {


            string sourcestring = parameters["String to alter"];

            // start by converting entire string to lower case
            var lowerCase = sourcestring.ToLower();

            // matches the first sentence of a string, as well as subsequent sentences
            var r = new Regex(@"(^[a-z])|\.\s+(.)", RegexOptions.ExplicitCapture);

            // MatchEvaluator delegate defines replacement of setence starts to uppercase
            var result = r.Replace(lowerCase, s => s.Value.ToUpper());

            _returnValue = result;

        }


        public object ReturnValue
        {
            // We return our variable _returnValue as the result of the function.
            get { return _returnValue; }
        }

        public UBotType ReturnValueType
        {
            // Our result is text, so the return value type is String.
            get { return UBotType.String; }
        }

        public bool IsContainer
        {
            get { return false; }
        }

        public IEnumerable<UBotParameterDefinition> ParameterDefinitions
        {
            get { return _parameters; }
        }

        public UBotVersion UBotVersion
        {
            get { return UBotVersion.Standard; }
        }
    }


    //
    //
    // ENCRYPT TEXT
    //
    //
    public class EncryptText : IUBotFunction
    {

        private List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();
        private string _returnValue;

        public EncryptText()
        {
            _parameters.Add(new UBotParameterDefinition("String to Encrypt", UBotType.String));
            _parameters.Add(new UBotParameterDefinition("Secret key", UBotType.String));
        }

        public string Category
        {
            get { return "Text Functions"; }
        }

        public string FunctionName
        {
            get { return "$encrypt text"; }
        }


        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {


            string stringToEncrypt = parameters["String to Encrypt"];
            string secretkey = parameters["Secret key"];

            string c = Encrypt(stringToEncrypt, secretkey);

            _returnValue = c;

        }

        private static readonly byte[] salt = Encoding.ASCII.GetBytes("bds187r1sh1muy84iylndf891wfcvdeswvrevrey8452");

        public static string Encrypt(string textToEncrypt, string encryptionPassword)
        {
            var algorithm = GetAlgorithm(encryptionPassword);

            byte[] encryptedBytes;
            using (ICryptoTransform encryptor = algorithm.CreateEncryptor(algorithm.Key, algorithm.IV))
            {
                byte[] bytesToEncrypt = Encoding.UTF8.GetBytes(textToEncrypt);
                encryptedBytes = InMemoryCrypt(bytesToEncrypt, encryptor);
            }
            return Convert.ToBase64String(encryptedBytes);
        }

        // Performs an in-memory encrypt/decrypt transformation on a byte array.
        private static byte[] InMemoryCrypt(byte[] data, ICryptoTransform transform)
        {
            MemoryStream memory = new MemoryStream();
            using (Stream stream = new CryptoStream(memory, transform, CryptoStreamMode.Write))
            {
                stream.Write(data, 0, data.Length);
            }
            return memory.ToArray();
        }

        // Defines a RijndaelManaged algorithm and sets its key and Initialization Vector (IV) 
        // values based on the encryptionPassword received.
        private static RijndaelManaged GetAlgorithm(string encryptionPassword)
        {
            // Create an encryption key from the encryptionPassword and salt.
            var key = new Rfc2898DeriveBytes(encryptionPassword, salt);

            // Declare that we are going to use the Rijndael algorithm with the key that we've just got.
            var algorithm = new RijndaelManaged();
            int bytesForKey = algorithm.KeySize / 8;
            int bytesForIV = algorithm.BlockSize / 8;
            algorithm.Key = key.GetBytes(bytesForKey);
            algorithm.IV = key.GetBytes(bytesForIV);
            return algorithm;
        }

        public object ReturnValue
        {
            // We return our variable _returnValue as the result of the function.
            get { return _returnValue; }
        }

        public UBotType ReturnValueType
        {
            // Our result is text, so the return value type is String.
            get { return UBotType.String; }
        }

        public bool IsContainer
        {
            get { return false; }
        }

        public IEnumerable<UBotParameterDefinition> ParameterDefinitions
        {
            get { return _parameters; }
        }

        public UBotVersion UBotVersion
        {
            get { return UBotVersion.Standard; }
        }
    }



    //
    //
    // DECRYPT TEXT
    //
    //
    public class DecryptText : IUBotFunction
    {

        private List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();
        private string _returnValue;

        public DecryptText()
        {
            _parameters.Add(new UBotParameterDefinition("String to Decrypt", UBotType.String));
            _parameters.Add(new UBotParameterDefinition("Secret key", UBotType.String));
        }

        public string Category
        {
            get { return "Text Functions"; }
        }

        public string FunctionName
        {
            get { return "$decrypt text"; }
        }


        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {


            string stringToEncrypt = parameters["String to Decrypt"];
            string secretkey = parameters["Secret key"];

            string c = Decrypt(stringToEncrypt, secretkey);

            _returnValue = c;

        }

        private static readonly byte[] salt = Encoding.ASCII.GetBytes("bds187r1sh1muy84iylndf891wfcvdeswvrevrey8452");

        public static string Decrypt(string encryptedText, string encryptionPassword)
        {
            var algorithm = GetAlgorithm(encryptionPassword);

            byte[] descryptedBytes;
            using (ICryptoTransform decryptor = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV))
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                descryptedBytes = InMemoryCrypt(encryptedBytes, decryptor);
            }
            return Encoding.UTF8.GetString(descryptedBytes);
        }

        // Performs an in-memory encrypt/decrypt transformation on a byte array.
        private static byte[] InMemoryCrypt(byte[] data, ICryptoTransform transform)
        {
            MemoryStream memory = new MemoryStream();
            using (Stream stream = new CryptoStream(memory, transform, CryptoStreamMode.Write))
            {
                stream.Write(data, 0, data.Length);
            }
            return memory.ToArray();
        }

        // Defines a RijndaelManaged algorithm and sets its key and Initialization Vector (IV) 
        // values based on the encryptionPassword received.
        private static RijndaelManaged GetAlgorithm(string encryptionPassword)
        {
            // Create an encryption key from the encryptionPassword and salt.
            var key = new Rfc2898DeriveBytes(encryptionPassword, salt);

            // Declare that we are going to use the Rijndael algorithm with the key that we've just got.
            var algorithm = new RijndaelManaged();
            int bytesForKey = algorithm.KeySize / 8;
            int bytesForIV = algorithm.BlockSize / 8;
            algorithm.Key = key.GetBytes(bytesForKey);
            algorithm.IV = key.GetBytes(bytesForIV);
            return algorithm;
        }

        public object ReturnValue
        {
            // We return our variable _returnValue as the result of the function.
            get { return _returnValue; }
        }

        public UBotType ReturnValueType
        {
            // Our result is text, so the return value type is String.
            get { return UBotType.String; }
        }

        public bool IsContainer
        {
            get { return false; }
        }

        public IEnumerable<UBotParameterDefinition> ParameterDefinitions
        {
            get { return _parameters; }
        }

        public UBotVersion UBotVersion
        {
            get { return UBotVersion.Standard; }
        }
    }


    //
    //
    // Round Number
    //
    //
    public class RoundNUmber : IUBotFunction
    {

        private List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();
        private string _returnValue;

        public RoundNUmber()
        {
            _parameters.Add(new UBotParameterDefinition("Item to round", UBotType.String));
        }

        public string Category
        {
            get { return "Math Functions"; }
        }

        public string FunctionName
        {
            get { return "$round"; }
        }


        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {


            string roundnum = parameters["Item to round"];

            var result = decimal.Round(Convert.ToDecimal(roundnum));
            _returnValue = result.ToString();

        }


        public object ReturnValue
        {
            // We return our variable _returnValue as the result of the function.
            get { return _returnValue; }
        }

        public UBotType ReturnValueType
        {
            // Our result is text, so the return value type is String.
            get { return UBotType.String; }
        }

        public bool IsContainer
        {
            get { return false; }
        }

        public IEnumerable<UBotParameterDefinition> ParameterDefinitions
        {
            get { return _parameters; }
        }

        public UBotVersion UBotVersion
        {
            get { return UBotVersion.Standard; }
        }
    }



    //
    //
    // TEXT FREQUENCY
    //
    //
    public class TextFrequency : IUBotCommand
    {

        private List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();

        public TextFrequency()
        {
            _parameters.Add(new UBotParameterDefinition("Text", UBotType.String));
            _parameters.Add(new UBotParameterDefinition("Minimum Character Length", UBotType.String));
            _parameters.Add(new UBotParameterDefinition("Return Top How Many?", UBotType.String));
            // Add our parameter (Table) with a UBotTable type since it is a table.
            _parameters.Add(new UBotParameterDefinition("Table Name", UBotType.UBotTable));
        }

        public string Category
        {
            get { return "Text Functions"; }
        }

        public string CommandName
        {
            get { return "word frequency"; }
        }


        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {

            string minimum_chart = parameters["Minimum Character Length"];
            int minimum_char = Convert.ToInt32(minimum_chart); 
            
            string top_how_manyt = parameters["Return Top How Many?"];
            int top_how_many = Convert.ToInt32(top_how_manyt);

            string table_name = parameters["Table Name"];

            // Read a file into a string (this file must live in the same directory as the executable)
            string inputString = parameters["Text"];

            // Convert our input to lowercase
            inputString = inputString.ToLower();

            // Define characters to strip from the input and do it
            // REMOVED FOR NOW
            
            /*
            string[] stripChars = { ";", ",", ".", "-", "_", "^", "(", ")", "[", "]",
						"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "\n", "\t", "\r" };
            foreach (string character in stripChars)
            {
                inputString = inputString.Replace(character, "");
            }
            */
            string[] stripChars = { ";", ",", ".", "^", "(", ")", "[", "]", "\n", "\t", "\r" };
            foreach (string character in stripChars)
            {
                inputString = inputString.Replace(character, "");
            }
            // Split on spaces into a List of strings
            List<string> wordList = inputString.Split(' ').ToList();

            // Define and remove stopwords
            // REMOVED FOR NOW
            
            /*
            string[] stopwords = new string[] { "and", "the", "she", "for", "this", "you", "but" };
            foreach (string word in stopwords)
            {
                // While there's still an instance of a stopword in the wordList, remove it.
                // If we don't use a while loop on this each call to Remove simply removes a single
                // instance of the stopword from our wordList, and we can't call Replace on the
                // entire string (as opposed to the individual words in the string) as it's
                // too indiscriminate (i.e. removing 'and' will turn words like 'bandage' into 'bdage'!)
                while (wordList.Contains(word))
                {
                    wordList.Remove(word);
                }
            }
            */

            // Create a new Dictionary object
            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            // Loop over all over the words in our wordList...
            foreach (string word in wordList)
            {
                // If the length of the word is at least three letters...
                if (word.Length >= minimum_char)
                {
                    // ...check if the dictionary already has the word.
                    if (dictionary.ContainsKey(word))
                    {
                        // If we already have the word in the dictionary, increment the count of how many times it appears
                        dictionary[word]++;
                    }
                    else
                    {
                        // Otherwise, if it's a new word then add it to the dictionary with an initial count of 1
                        dictionary[word] = 1;
                    }

                } // End of word length check

            } // End of loop over each word in our input

            // Create a dictionary sorted by value (i.e. how many times a word occurs)
            var sortedDict = (from entry in dictionary orderby entry.Value descending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);

            // Loop through the sorted dictionary and output the top most frequently occurring words
            int count = 1;

            int count_dictionary_org = dictionary.Count;
            int count_dictionary = count_dictionary_org + 1;
            
            // Create a table to store the data returned.
            string[,] table = new string[count_dictionary, 2];

                table[0, 0] = "Word";
                table[0, 1] = "Number of times shown";
                //sw.WriteLine("---- Most Frequent Terms in the Text: " + inputString + " ----");
                //sw.WriteLine("");

                //sw.WriteLine("Word" + delimiter_table + "Number of times shown");
                int i = 1;
                foreach (KeyValuePair<string, int> pair in sortedDict)
                {
                    table[i, 0] = pair.Key;
                    table[i, 1] = pair.Value.ToString();
                    // Output the most frequently occurring words and the associated word counts
                    //sw.WriteLine(count + delimiter_table + pair.Key + delimiter_table + pair.Value);
                    //sw.WriteLine(pair.Key + delimiter_table + pair.Value);
                    //sw.WriteLine(pair.Key + "," + pair.Value);
                    count++;
                    i++;

                    // Only display the top 10 words then break out of the loop!
                    if (count > top_how_many)
                    {
                        break;
                    }
                }


                // Wait for the user to press a key before exiting
                //string console_read = sw.GetStringBuilder().ToString();
                //string[,] stringArray = new string[,] { console_read };
                //_returnValue = console_read.ToString();
                ubotStudio.SetTable(table_name, table);
            
        }


        public bool IsContainer
        {
            get { return false; }
        }

        public IEnumerable<UBotParameterDefinition> ParameterDefinitions
        {
            get { return _parameters; }
        }

        public UBotVersion UBotVersion
        {
            get { return UBotVersion.Standard; }
        }
    }



    //
    //
    // WORD COUNT
    //
    //
    public class WordCount : IUBotFunction
    {

        private List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();
        private string _returnValue;

        public WordCount()
        {
            _parameters.Add(new UBotParameterDefinition("Text input", UBotType.String));
        }

        public string Category
        {
            get { return "Text Functions"; }
        }

        public string FunctionName
        {
            get { return "$word count"; }
        }


        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {


            string ttxt = parameters["Text input"];

            var result = totalWords(ttxt);
            _returnValue = result.ToString();

        }

        private int indx;
        private int Total;


        //ka a 222 ti
        public int totalWords(string text)
        {
            indx = 1;
            Total = 0;
            for (int i = indx; i < text.Length; i++)
            {
                if (char.IsWhiteSpace(text[i - 1]) == true)
                {
                    if ((char.IsLetterOrDigit(text[i]) == true) || (char.IsLetterOrDigit(text[i]) == true))
                    { Total++; }
                }
            }

            if (text.Length > 2)
            {
                Total++;

            }


            return Total;
        }

        public int CountWords1(string s)
        {
            MatchCollection collection = Regex.Matches(s, @"[\S]+");
            return collection.Count;
        }

        public object ReturnValue
        {
            // We return our variable _returnValue as the result of the function.
            get { return _returnValue; }
        }

        public UBotType ReturnValueType
        {
            // Our result is text, so the return value type is String.
            get { return UBotType.String; }
        }

        public bool IsContainer
        {
            get { return false; }
        }

        public IEnumerable<UBotParameterDefinition> ParameterDefinitions
        {
            get { return _parameters; }
        }

        public UBotVersion UBotVersion
        {
            get { return UBotVersion.Standard; }
        }
    }


    //
    //
    // Remove redirection get landing url
    //
    //
    public class GetlandingUrl : IUBotFunction
    {

        private List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();
        private string _returnValue;

        public GetlandingUrl()
        {
            _parameters.Add(new UBotParameterDefinition("Url to get final URL", UBotType.String));
        }

        public string Category
        {
            get { return "Browser Functions"; }
        }

        public string FunctionName
        {
            get { return "$return final url"; }
        }


        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {


            string FirstURL = parameters["Url to get final URL"];

            string result = UrlLengthen(FirstURL);

            _returnValue = result.ToString();

        }

        private string UrlLengthen(string url)
        {
            string newurl = url;

            bool redirecting = true;

            while (redirecting)
            {

                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(newurl);
                    request.AllowAutoRedirect = false;
                    request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.1.3) Gecko/20090824 Firefox/3.5.3 (.NET CLR 4.0.20506)";
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if ((int)response.StatusCode == 301 || (int)response.StatusCode == 302)
                    {
                        string uriString = response.Headers["Location"];
                        newurl = uriString;
                        // and keep going
                    }
                    else
                    {
                        redirecting = false;
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add("url", newurl);
                    redirecting = false;
                }
            }
            return newurl;
        }

        public object ReturnValue
        {
            // We return our variable _returnValue as the result of the function.
            get { return _returnValue; }
        }

        public UBotType ReturnValueType
        {
            // Our result is text, so the return value type is String.
            get { return UBotType.String; }
        }

        public bool IsContainer
        {
            get { return false; }
        }

        public IEnumerable<UBotParameterDefinition> ParameterDefinitions
        {
            get { return _parameters; }
        }

        public UBotVersion UBotVersion
        {
            get { return UBotVersion.Standard; }
        }
    }


    //
    //
    // Get all redirects to url
    //
    //
    public class GetRedirectsUrl : IUBotFunction
    {

        private List<UBotParameterDefinition> _parameters = new List<UBotParameterDefinition>();
        private string _returnValue;

        public GetRedirectsUrl()
        {
            _parameters.Add(new UBotParameterDefinition("URL", UBotType.String));
        }

        public string Category
        {
            get { return "Browser Functions"; }
        }

        public string FunctionName
        {
            get { return "$return redirect urls"; }
        }


        public void Execute(IUBotStudio ubotStudio, Dictionary<string, string> parameters)
        {


            string FirstURL = parameters["URL"];

            string result = RedirectPath(FirstURL);

            _returnValue = result.ToString();

        }

        public static string RedirectPath(string url)
        {
            StringBuilder sb = new StringBuilder();
            string location = string.Copy(url);
            while (!string.IsNullOrWhiteSpace(location))
            {
                sb.AppendLine(location); // you can also use 'Append'
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(location);
                request.AllowAutoRedirect = false;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    location = response.GetResponseHeader("Location");
                }
            }
            return sb.ToString();
        }

        public object ReturnValue
        {
            // We return our variable _returnValue as the result of the function.
            get { return _returnValue; }
        }

        public UBotType ReturnValueType
        {
            // Our result is text, so the return value type is String.
            get { return UBotType.String; }
        }

        public bool IsContainer
        {
            get { return false; }
        }

        public IEnumerable<UBotParameterDefinition> ParameterDefinitions
        {
            get { return _parameters; }
        }

        public UBotVersion UBotVersion
        {
            get { return UBotVersion.Standard; }
        }
    }



}
