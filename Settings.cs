using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace SubFinder
{
    /// <summary>
    /// Settings for SubFinder
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Class to replace a tv show name by its website counter part (to address ' ', '_', ... mismatches)
        /// </summary>
        public class NameMatch
        {
            [XmlAttribute]
            public string FolderName;
            [XmlAttribute]
            public string SearchName;
        }

        /// <summary>
        /// Singleton
        /// </summary>
        private static Settings instance = null;

        /// <summary>
        /// File name
        /// </summary>
        private const string SETTINGS_FILENAME = "settings.xml";

        /// <summary>
        /// List of folder to scan
        /// </summary>
        public string[] PathToScan;

        /// <summary>
        /// Wanted language
        /// </summary>
        public string Language;

        /// <summary>
        /// Indicates whether or not to use the temps folder to download sub files (may be required depending on file system)
        /// </summary>
        public bool UseTempFolder;

        /// <summary>
        /// Temp folder to use
        /// </summary>
        public string TempFolder;

        /// <summary>
        /// Webhook to use (or null or empty string to disable)
        /// </summary>
        public string Webhook;

        /// <summary>
        /// List of name correspondances
        /// </summary>
        public List<NameMatch> NameMatches;

        /// <summary>
        /// Create a default settings file so the user can modify it easily
        /// </summary>
        public static void CreateSettingsFile()
        {
            Settings test = new Settings();
            test.PathToScan = new string[] { @"/folder" };
            test.Language = "Fre";
            test.UseTempFolder = false;
            test.TempFolder = @"temp";
            test.NameMatches = new List<NameMatch>();
            test.NameMatches.Add(new NameMatch() { FolderName = "Test", SearchName = "T35t" });

            XmlSerializer x = new XmlSerializer(test.GetType());

            using (FileStream file = File.Create(SETTINGS_FILENAME))
            {
                x.Serialize(file, test);
            }
        }
        
        /// <summary>
        /// Load the settings from file
        /// </summary>
        /// <returns>True if settings could be loaded succesfully, false otherwise</returns>
        public static bool LoadSettingsFile()
        {
            try
            {
                XmlSerializer reader = new XmlSerializer(typeof(Settings));
                using (StreamReader file = new StreamReader(SETTINGS_FILENAME))
                {
                    instance = (Settings)reader.Deserialize(file);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get singleton instance
        /// </summary>
        /// <returns>The singleton></returns>
        public static Settings getInstance()
        {
            if (instance == null)
            {
                // try to load file

                if (!LoadSettingsFile())
                {
                    CreateSettingsFile();
                    Console.WriteLine("You must configure settings.xml");
                }
            }

            return instance;
        }
    }
}
