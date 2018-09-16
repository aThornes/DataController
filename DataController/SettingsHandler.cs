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
        private static string Setting_fileName;
        private static List<string> SettingsOption;
        private static string[] SettingsFileText;

        private static string[] DefaultSettingsOptions = new string[5] {
            "#DatabaseCommunicator default settings file",
            "DatabaseServer=127.0.0.1,14333",
            "DatabaseName=Database1",
            "DatabaseUser=SA",
            "DatabasePassword=Password"};

        /// <summary>
        /// Initialise Settings Handler
        /// </summary>
        /// <param name="defaultSettingsOptions">Input custom settings options, leave null for default</param>
        public SettingsHandler(string[] defaultSettingsOptions, string settingFilename = "Settings.info")
        {
            Setting_fileName = settingFilename;

            if (defaultSettingsOptions != null) DefaultSettingsOptions = defaultSettingsOptions;
            FileHandler.LoadFile(new File_Inst(FileHandler.DefaultPath, Setting_fileName));
            FileHandler.OverriteFile(Setting_fileName, DefaultSettingsOptions, true); //Add default settings if file is already empty

            Load();
        }

        private void Load()
        {
            SettingsOption = new List<string>();
            string[] fileText = FileHandler.ReadAll(Setting_fileName);
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
        /// <summary>
        /// Return the value for a specified setting
        /// </summary>
        /// <param name="settingOption"></param>
        /// <returns></returns>
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
            if (SettingsOption.Contains(settingOption)) //Fails if setting already exists
                return false;
            string[] newSettings = new string[SettingsFileText.Count() + 1];
            string toAdd = settingOption + '=' + value;
            newSettings[newSettings.Count() - 1] = toAdd.Remove(' '); //Removes all space in sting
            FileHandler.OverriteFile(Setting_fileName, newSettings); //Rewrite file for new updated settings
            return true;           
        }

        /// <summary>
        /// Edit an existing setting
        /// </summary>
        /// <param name="settingOption"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public bool EditSetting(string settingOption, string newValue)
        {
            if (!SettingsOption.Contains(settingOption))
                return false;
            List<string> newSettings = new List<string>();
            foreach (string s in SettingsFileText)
            {
                string[] splitString = s.Remove(' ').Split('='); //Removes all spaces in string and split string around symbol
                if (splitString[0] == settingOption)
                    newSettings.Add(settingOption + '=' + newValue); //Save updated setting instead of original
                else
                    newSettings.Add(s);
            }
            FileHandler.OverriteFile(Setting_fileName, SettingsFileText); //Rewrite file for new updated settings
            SettingsFileText = newSettings.ToArray();
            return true;
        }

        /// <summary>
        /// Remove a setting from the file
        /// </summary>
        /// <param name="settingOption"></param>
        /// <returns></returns>
        public bool RemoveSetting(string settingOption)
        {
            if (!SettingsOption.Contains(settingOption))
                return false;
            List<string> newSettings = new List<string>();
            foreach (string s in SettingsFileText)
            {
                string[] splitString = s.Remove(' ').Split('='); //Removes all spaces in string and split string around symbol
                if (splitString[0] != settingOption)
                    newSettings.Add(s);
            }
            FileHandler.OverriteFile(Setting_fileName, SettingsFileText); //Rewrite file for new updated settings
            SettingsFileText = newSettings.ToArray();
            return true;
        }

        /// <summary>
        /// Returns the name of the settings file
        /// </summary>
        public string Filename { get => Setting_fileName; }
    }
}

