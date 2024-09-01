using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SubFinder
{
    /// <summary>
    /// Implementation of ISubDownloader for addic7ed website
    /// </summary>
    public class Addic7edSubDownloader : ISubDownloader
    {
        protected const string BASE_URL = @"https://www.addic7ed.com";
        private const string VERSION = "Version ";


        private static readonly HttpClient client = new HttpClient();

        public async Task<string> FindSubtitle(string show, string season, string number, string team, string filename)
        {
            try
            {
                string url = getURL(show, season, number);

                Console.WriteLine("url = " + url);
                Console.WriteLine("time = " + DateTime.Now.ToShortDateString() + "  " + DateTime.Now.ToLongTimeString());

                var stringTask = client.GetStringAsync(url);

                var page = await stringTask;

                var subLinks = parsePageForSubLink(page);

                var filenameWithoutExtension = Utils.getFilenameWithoutExtension(filename);

                // select best sub for this release

                if (subLinks.ContainsKey(team))
                {
                    downloadSub(subLinks[team], url, filenameWithoutExtension + ".srt");
                    return "";
                }
                else
                {
                    foreach (var pair in subLinks)
                    {
                        if (pair.Key.ToLower().Contains(team.ToLower()))
                        {
                            downloadSub(pair.Value, url, filenameWithoutExtension + ".srt");
                            Console.WriteLine("  -> Found exact match");
                            return "";
                        }
                    }

                    // no matching subs => lets donwload all
                    short dlCount = 0;
                    foreach (var pair in subLinks)
                    {
                        if (!Utils.hasSubs(filename, pair.Key))
                        {
                            downloadSub(pair.Value, url, filenameWithoutExtension + "." + pair.Key + ".srt");
                            dlCount++;
                        }
                    }
                    Console.WriteLine("  -> no exact match (found " + subLinks.Count + " subs, downloaded " + dlCount + ")");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                throw e;
            }

            return "";
        }

        /// <summary>
        /// Download the specified sub
        /// </summary>
        /// <param name="url">URL of the subtitle</param>
        /// <param name="referer">Referer, ie page of the episode</param>
        /// <param name="filename">Filename (to name sub file accordingly to the episode file)</param>
        protected void downloadSub(string url, string referer, string filename)
        {
            Console.WriteLine("sub url = " + url);
            Console.WriteLine("sub filename = " + filename);

            string tempFolder = GetTempFolder();

            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("Referer", referer);
                string dlPath = tempFolder == null ? filename : tempFolder + Path.DirectorySeparatorChar + "temp_sub";
                wc.DownloadFile(url, dlPath);

                if (tempFolder != null)
                {
                    Console.WriteLine($"Move file : {dlPath} -> {filename}");
                    File.Move(dlPath, filename);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.InnerException.Message);
                throw e;
            }
        }

        /// <summary>
        /// Parse the web page and find pairs of version/sub url
        /// </summary>
        /// <param name="page">Content of episode page</param>
        /// <returns>Pairs of version/sub url</returns>
        protected Dictionary<string, string> parsePageForSubLink(string page)
        {
            Dictionary<string, string> subVersions = new Dictionary<string, string>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(page);
            var tables = doc.DocumentNode.SelectNodes("//table[contains(@class, 'tabel95') and contains(@align, 'center') and contains(@border, '0') and contains(@width, '100%')]");
            foreach (var table in tables)
            {
                string inner = table.InnerText;
                int start = inner.IndexOf(VERSION);
                string version = inner.Substring(start, table.InnerText.IndexOf(',') - start);
                version = version.Replace(VERSION, "").ToLower();

                var frParts = table.SelectNodes(".//*[contains(@class, 'language')]").Where(a => a.InnerText.StartsWith(Settings.getInstance().Language));

                if (frParts.Count() == 0)
                {
                    continue;
                }

                var frInfos = frParts.First().ParentNode;

                string status = frInfos.SelectNodes(".//b").First().InnerText;

                if (!status.StartsWith("Completed"))
                {
                    continue;
                }

                var nodes = frInfos.SelectNodes(".//a[contains(@class, 'face-button')]");
                try
                {
                    var subUrl = nodes.Where(x => (x.InnerText.Contains("Most Updated") || x.InnerText.Contains("Download"))).First().Attributes["href"].Value;

                    while (subVersions.ContainsKey(version))
                    {
                        version += '_';
                    }
                    subVersions.Add(version, BASE_URL + subUrl);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw e;
                }
            }
            return subVersions;
        }

        /// <summary>
        /// get the url matching the specified parameters
        /// </summary>
        /// <param name="show">tv show</param>
        /// <param name="season">season</param>
        /// <param name="number">number</param>
        /// <returns>the corresponding url</returns>
        protected string getURL(string show, string season, string number)
        {
            string url = string.Format(@"{0}/serie/{1}/{2}/{3}/addic7ed", BASE_URL, Utils.formatSerieName(show), season, number);

            return url;
        }

        /// <summary>
        /// Get temp folder for sub dl
        /// </summary>
        /// <returns>Folder to use for temp download or null if no temp folder</returns>
        protected string GetTempFolder()
        {
            Settings s = Settings.getInstance();

            if (s.UseTempFolder)
            {
                return s.TempFolder;
            }
            return null;
        }

    }
}
