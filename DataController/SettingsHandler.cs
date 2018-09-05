using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataController
{
    /// <summary>
    /// Handles all database settings, this includes tables and table information along with permissions systems
    /// </summary>
    class SettingsHandler
    {
        private static string FileName;
        private static List<string> SettingsOption;
        private static string[] SettingsFileText;

        private static string[] DefaultSettingsOptions = new string[5] {
            "#DatabaseCommunicator default settings file",
            "DatabaseServer=127.0.0.1",
            "DatabaseName=Database1",
            "DatabaseUser=SA",
            "DatabasePassword=Password"};
        /// <summary>
        /// Initialise Settings Handler
        /// </summary>
        /// <param name="defaultSettingsOptions">Input custom settings options, leave null for default</param>
        public SettingsHandler(string[] defaultSettingsOptions, string settingFilename = "Settings.info")
        {
            settingFilename = FileName;

            if (defaultSettingsOptions != null) DefaultSettingsOptions = defaultSettingsOptions;
            FileHandler.LoadFile(new File_Inst(FileHandler.DefaultPath, settingFilename));
            FileHandler.OverriteFile(settingFilename, DefaultSettingsOptions, true); //Add default settings if file is already empty

            Load();
        }

        private void Load()
        {
            SettingsOption = new List<string>();
            string[] fileText = FileHandler.ReadAll(FileName);
            SettingsFileText = fileText;
            foreach (string s in fileText)
            {
                if (s[0] != '#' || s[0] != ' ') //Ignore comment lines
                {
                    string splitString = s.Remove(' ').Split('=')[0]; //Removes all spaces in string and gets string before equals symbol
                    SettingsOption.Add(s); //Adds the setting option to the global list
                }
            }
        }

        public string RequestSettingValue(string settingOption)
        {
            if (!SettingsOption.Contains(settingOption)) return null;

            foreach (string s in SettingsFileText)
            {
                if (s[0] != '#' || s[0] != ' ') //Ignore comment lines
                {
                    string[] splitString = s.Remove(' ').Split('='); //Removes all spaces in string and split string around symbol
                    if (splitString[0] == settingOption)
                        return splitString[1];
                }
            }
            return null;
        }

        /// <summary>
        /// Add a settings to the file
        /// </summary>
        /// <param name="settingOption"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddSetting(string settingOption, string value)
        {

            return false;
        }

        /// <summary>
        /// Edit an existing setting
        /// </summary>
        /// <param name="settingOption"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public bool EditSetting(string settingOption, string newValue)
        {
            return false;
        }

        /// <summary>
        /// Remove a setting from the file
        /// </summary>
        /// <param name="settingOption"></param>
        /// <returns></returns>
        public bool RemoveSetting(string settingOption)
        {
            int fileLength = SettingsFileText.Count();
            if (!SettingsOption.Contains(settingOption))
                return false;
            List<string> newSettings = new List<string>();
            foreach (string s in SettingsFileText)
            {
                string[] splitString = s.Remove(' ').Split('='); //Removes all spaces in string and split string around symbol
                if (splitString[0] != settingOption)
                    newSettings.Add(s);
            }
            SettingsFileText = newSettings.ToArray();
            FileHandler.OverriteFile(FileName, SettingsFileText); //Rewrite file for new updated settings
            if (fileLength > SettingsFileText.Count()) //If file has gotten smaller, delete must have been successful
                return true;
            else return false;
        }

        

    }
}

