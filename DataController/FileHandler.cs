using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataController
{
    /// <summary>
    /// File handler class, 
    /// </summary>
    class FileHandler
    {
        private static List<File_Inst> OnHandFiles;
        private static string defaultPath = Environment.ExpandEnvironmentVariables("%AppData%");

        private static string fileEncryptionPass = "SomePassword"; //Note it is advised that this is not 'hardcoded' and is instead stored securely (possibly linked to a login)

        #region File Commands
        /// <summary>Load a file into the file handler, note will create file if does not exist</summary>
        /// <param name="fileToAdd">The file to be added</param>
        public static void LoadFile(File_Inst fileToAdd)
        {
            if (!IsDuplicate(fileToAdd.Name))
            {
                if (!File.Exists(fileToAdd.Path))
                {
                    //Create file if does not exist
                    File.Create(fileToAdd.Path + "\\" + fileToAdd.Name).Dispose();
                }
                OnHandFiles.Add(fileToAdd); //Add file to handler memory
            }
        }

        /// <summary>Overrite a file text with new text</summary>
        /// <param name="toWrite">New file text</param>
        public static bool OverriteFile(string fileName, string[] toWrite, bool ifEmpty = false)
        {
            File_Inst loadedFile = GetLoadedFile(fileName);
            if (loadedFile == null) return false; //Ensure file exists and is loaded

            if (ifEmpty)
            {
                if (!loadedFile.IsEmpty()) return false; //Only overrite if file is empty
            }

            bool written = WriteToFile(loadedFile, toWrite);

            if (written) //Inform whether file was written successfully
                return true;
            else return false;
        }

        #region Read File
        /// <summary>
        /// Returns a single line from a file
        /// </summary>
        /// <param name="fileName">Name of file loaded</param>
        /// <param name="line">Line requested</param>
        /// <returns></returns>
        public static string ReadLine(string fileName, int line)
        {
            File_Inst loadedFile = GetLoadedFile(fileName);
            if (loadedFile == null) return null; //Ensure file exists and is loaded
            string[] allText = ReadFromFile(loadedFile);
            return allText[line];
        }
        /// <summary>
        /// Returns a single line from a file matching the search criteria
        /// </summary>
        /// <param name="fileName">Name of file loaded</param>
        /// <param name="searchTerm">Search criteria</param>
        /// <returns></returns>
        public static string ReadLine(string fileName, string searchTerm)
        {
            File_Inst loadedFile = GetLoadedFile(fileName);
            if (loadedFile == null) return null; //Ensure file exists and is loaded
            string[] allText = ReadFromFile(loadedFile);
            foreach (string s in allText)
            {
                if (s.Contains(searchTerm))
                {
                    return s;
                }
            }
            return null;
        }
        /// <summary>
        /// Returns all file text
        /// </summary>
        /// <param name="fileName">Name of file loaded</param>
        /// <returns></returns>
        public static string[] ReadAll(string fileName)
        {
            File_Inst loadedFile = GetLoadedFile(fileName);
            if (loadedFile == null) return null; //Ensure file exists and is loaded
            string[] allText = ReadFromFile(loadedFile);
            return allText;
        }
        #endregion

        #region Edit File
        /// <summary>Edit a specific line of text in the file</summary>
        public static bool EditFile(string fileName, int line, string text)
        {
            bool written = EditFile(fileName, new int[1] { line }, new string[1] { text });

            if (written) //Inform whether file was written successfully
                return true;
            else return false;
        }
        /// <summary>Edit a line of text based on the given search term</summary>
        public static bool EditFile(string fileName, string term, string text)
        {
            bool written = EditFile(fileName, new string[1] { term }, new string[1] { text });

            if (written) //Inform whether file was written successfully
                return true;
            else return false;
        }
        /// <summary>Edit specific lines of text in the file</summary>
        public static bool EditFile(string fileName, int[] lines, string[] text)
        {
            ValidateArray(lines, text); //Ensure lines and text have the same length
            File_Inst loadedFile = GetLoadedFile(fileName);
            if (loadedFile == null) return false; //Ensure file exists and is loaded

            string[] fileContents = ReadFromFile(loadedFile);
            for (int n = 0; n < lines.Length; n++)
                fileContents[lines[n]] = text[n];

            bool written = WriteToFile(loadedFile, fileContents);

            if (written) //Inform whether file was written successfully
                return true;
            else return false;
        }
        /// <summary>Edit lines of text based on the given search terms</summary>
        public static bool EditFile(string fileName, string[] terms, string[] text)
        {
            ValidateArray(terms, text); //Ensure lines and text have the same length
            File_Inst loadedFile = GetLoadedFile(fileName);
            if (loadedFile == null) return false; //Ensure file exists and is loaded

            string[] fileContents = ReadFromFile(loadedFile);
            for (int n = 0; n < terms.Length; n++)
            {
                if (fileContents[n][0] != '#') //Ignore comment lines
                {
                    string splitString = fileContents[n].Split('=')[0]; //Split the string around the '=' symbol giving the setting term
                    if (splitString == terms[n])
                        fileContents[n] = terms[n] + "=" + text;
                }
            }

            bool written = WriteToFile(loadedFile, fileContents);

            if (written) //Inform whether file was written successfully
                return true;
            else return false;
        }
        #endregion

        /// <summary>Delete the file requested</summary>
        public static void DeleteFile(string fileName)
        {
            foreach (File_Inst file in OnHandFiles)
            {
                if (file.Name == fileName)
                {
                    file.Delete();
                    break;
                }
            }

        }
        #endregion

        #region Get Set Commands
        /// <summary> Get or set the default path associated with a new file(i.e. folder directory)</summary>
        public static string DefaultPath { get => defaultPath; set => defaultPath = value; }
        /// <summary>
        /// Password used for encrypting files, not changing this password will make any associated files inaccessible
        /// </summary>
        public static string FileEncryptionPassword { get => fileEncryptionPass; set => fileEncryptionPass = value; }
        #endregion

        #region Supporting Commands
        /// <summary>Write to file</summary>
        private static bool WriteToFile(File_Inst f, string[] contents)
        {
            try
            {
                //Encrypt data and write file
                string generatedSalt = SecurityManager.GenerateNewSALT();
                string[] encryptedData = SecurityManager.EncryptFile(contents, fileEncryptionPass, generatedSalt);
                File.WriteAllLines(f.FullPath, contents); //Write information to file
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>Read entire file</summary>
        private static string[] ReadFromFile(File_Inst f)
        {
            try
            {          
                //Return decrypted data
                string[] allLines = File.ReadAllLines(f.FullPath);
                return SecurityManager.DecryptFile(allLines, fileEncryptionPass);
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Returns the file specified (must already have been loaded)
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static File_Inst GetLoadedFile(string fileName)
        {
            foreach (File_Inst file in OnHandFiles)
            {
                if (file.Name == fileName)
                {
                    return file;
                }
            }
            return null;
        }
        /// <summary>
        /// Check if file with given name has already been loaded
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static bool IsDuplicate(string fileName)
        {
            foreach (File_Inst file in OnHandFiles)
            {
                if (fileName == file.Name)
                    return true;
            }
            return false;
        }

        /// <summary>Used to ensure arrays match, throws exception</summary>
        private static void ValidateArray(int[] intArray, string[] stringArray)
        {
            if (intArray.Length != stringArray.Length)
                throw new ArrayTypeMismatchException("Array size must match");
        }
        /// <summary>Used to ensure arrays match, throws exception</summary>
        private static void ValidateArray(string[] stringArray1, string[] stringArray2)
        {
            if (stringArray1.Length != stringArray2.Length)
                throw new ArrayTypeMismatchException("Array size must match");
        }
        #endregion
    }

    class File_Inst
    {
        private static string fileLocation;
        private static string fileName;
        public File_Inst(string path, string name)
        {
            fileLocation = path;
            fileName = name;
        }

        /// <summary>Returns the path of the file</summary>
        public string Path { get => fileLocation; }
        /// <summary>Returns the name of the file</summary>
        public string Name { get => fileName; }

        public string FullPath { get => Path + "\\" + Name; }
        /// <summary>Delete the file</summary>
        public void Delete() { File.Delete(fileLocation + "\\" + fileName); }
        /// <summary>
        /// Returns if the file has any contents
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            if (new FileInfo(FullPath).Length == 0) return true;
            return false;
        }
    }
}
