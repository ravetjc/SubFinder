using System.IO;

namespace SubFinder
{
    /// <summary>
    /// Utils class
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Format serie name with uppercase
        /// </summary>
        /// <param name="serie">name to format</param>
        /// <returns>Name formatted</returns>
        public static string formatSerieName(string serie)
        {
            string formattedName = "";

            foreach (string part in serie.Split(' '))
            {
                formattedName += uppercaseFirst(part) + '_';
            }

            return formattedName.Substring(0, formattedName.Length - 1);
        }

        /// <summary>
        /// Switch first letter of string to upper case
        /// </summary>
        /// <param name="s">string to format</param>
        /// <returns>String formatted</returns>
        public static string uppercaseFirst(string s)
        {
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        /// <summary>
        /// Remove extension from filename
        /// </summary>
        /// <param name="filename">Filename to remove extension</param>
        /// <returns>Filename without extension</returns>
        public static string getFilenameWithoutExtension(string filename)
        {
            return filename.Substring(0, filename.Length - Path.GetExtension(filename).Length);
        }

        /// <summary>
        /// Check if filename indicates embedded subs or if an external sub file exists next to it
        /// </summary>
        /// <param name="filename">File to check</param>
        /// <returns>True if file has sub, false otherwise</returns>
        public static bool hasSubs(string filename)
        {
            var basename = filename.Substring(0, filename.Length - Path.GetExtension(filename).Length);

            return basename.ToLowerInvariant().Contains("vostfr") ||
                basename.ToLowerInvariant().Contains("subfrench") ||
                basename.ToLowerInvariant().Contains(".french") ||
                File.Exists(basename + ".srt");
        }

        /// <summary>
        /// Check if a sub file exists next to this filename
        /// </summary>
        /// <param name="filename">File to check</param>
        /// <param name="team">Team name to lok for in sub file name</param>
        /// <returns>True if a matching sub exists, false otherwise</returns>
        public static bool hasSubs(string filename, string team)
        {
            var basename = filename.Substring(0, filename.Length - Path.GetExtension(filename).Length);

            return File.Exists(basename + "." + team + ".srt");
        }
    }
}
