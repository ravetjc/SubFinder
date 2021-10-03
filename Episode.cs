using System.IO;
using System.Text.RegularExpressions;

namespace SubFinder
{
    /// <summary>
    /// Object representing an episode
    /// </summary>
    class Episode
    {
        /// <summary>
        /// Filename of this episode
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// TV show name
        /// </summary>
        public string Show { get; set; }

        /// <summary>
        /// Season number
        /// </summary>
        public string Season { get; set; }

        /// <summary>
        /// Episode number
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Team name
        /// </summary>
        public string Team { get; set; }

        /// <summary>
        /// Constructor from filename
        /// </summary>
        /// <param name="filename">Name of the file</param>
        public Episode(string filename)
        {
            Filename = filename;
            Show = "";

            string[] parts = Path.GetFileName(filename).Split('.');

            int partIndex = 0;

            foreach (var part in parts)
            {
                Regex regex = new Regex(@"S([0-9]){1,2}E([0-9]){1,2}");

                if (regex.IsMatch(part))
                {
                    partIndex = 1;
                }

                switch (partIndex)
                {
                    case 0:
                        if (Show.Length > 0)
                        {
                            Show += " ";
                        }
                        Show += part;
                        break;
                    case 1:
                        string[] values = part.Split(new char[] { 'S', 'E'});
                        Season = values[1];
                        Number = values[2];
                        partIndex++;
                        break;
                    case 2:
                        string[] infos = filename.Split('-');
                        Team = infos[infos.Length-1].Split('.')[0];
                        return;
                }
            }
        }

    }
}
