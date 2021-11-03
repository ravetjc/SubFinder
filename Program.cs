using Discord.Webhook;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace SubFinder
{
    class Program
    {
        static bool isSilent = false;

        static DiscordWebhookClient discordClient = null;

        /// <summary>
        /// Console app entry point
        /// </summary>
        /// <param name="args">Application arguments</param>
        static void Main(string[] args)
        {
            Console.WriteLine("Subtitle finder by J-Ch");
            Console.WriteLine();


            if (args.Contains("--init"))
            {
                Settings.CreateSettingsFile();
                Console.WriteLine("Settings file created");
                WaitForKey(true);
                return;
            }

            Settings s = Settings.getInstance();

            if (s == null)
            {
                WaitForKey(true);
                return;
            }

            if (args.Contains("--help") || args.Contains("-h"))
            {
                Console.WriteLine("--init to create a new settings file");
                Console.WriteLine("--silent to remove all \"press any key\" message");
                return;
            }

            if (args.Contains("--silent"))
            {
                isSilent = true;
            }

            if (s.Webhook != null && s.Webhook != string.Empty)
            {
                discordClient = new DiscordWebhookClient(s.Webhook);
            }

            Addic7edSubDownloader dl = new Addic7edSubDownloader();

            Console.WriteLine("Languague :" + s.Language);
            foreach (var path in s.PathToScan)
            {
                foreach (var subpath in Directory.GetDirectories(path))
                {
                    /*if (!subpath.ToLower().EndsWith("serie to test"))
                        continue;*/

                    Console.WriteLine("Will scan : " + subpath);

                    var episodes = ScanFolder(subpath);
                    foreach (var e in episodes)
                    {
                        Console.WriteLine("Looking for subtitles for " + e.Filename);

                        if (Utils.hasSubs(e.Filename))
                        {
                            Console.WriteLine("already has subs");
                            continue;
                        }

                        var serie = e.Show;

                        if (s.NameMatches.Any(x => x.FolderName == serie.ToLower()))
                        {
                            serie = s.NameMatches.Find(x => x.FolderName == serie.ToLower()).SearchName;
                        }

                        try
                        {
                            dl.FindSubtitle(serie, e.Season, e.Number, e.Team, e.Filename).Wait();
                        }
                        catch (Exception ex)
                        {
                            SendNotification(new string[] { "Exception while getting sub", $"{serie}, {e.Season}, {e.Number}", ex.Message, ex.StackTrace });

                            WaitForKey(true);
                            break;
                        }

                        Thread.Sleep(5000);
                    }
                }
            }

            WaitForKey(true);
        }

        /// <summary>
        /// Pauses the execution and invites the user to press any key
        /// </summary>
        /// <param name="displayMsg">True to display "press any key" message</param>
        private static void WaitForKey(bool displayMsg)
        {
            if (!isSilent)
            {
                if (displayMsg)
                {
                    Console.WriteLine("Press any key to exit.");
                }
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Scan the specified folder
        /// </summary>
        /// <param name="folderPath">Path of the folder to scan</param>
        /// <returns>List of episode detected in the folder</returns>
        static List<Episode> ScanFolder(String folderPath)
        {
            List<Episode> episodes = new List<Episode>();

            if (File.Exists(Path.Combine(folderPath, ".no_sub_dl")))
            {
                Console.WriteLine("Skipping folder " + folderPath + " because of no_sub_dl");
                return episodes;
            }

            var files = Directory.GetFiles(folderPath);

            foreach (var file in files)
            {
                if (Path.GetExtension(file).Contains("srt"))
                {
                    continue;
                }

                episodes.Add(new Episode(Path.Combine(folderPath, file)));
            }


            return episodes;
        }

        /// <summary>
        /// Send message to discord using configureg webhook (if any)
        /// </summary>
        /// <param name="msg">message parts to send in one message</param>
        static void SendNotification(string[] msg)
        {
            SendNotification(string.Join(Environment.NewLine, msg));
        }

        /// <summary>
        /// Send message to discord using configureg webhook (if any)
        /// </summary>
        /// <param name="msg">message to send</param>
        static void SendNotification(string msg)
        {
            if (discordClient != null)
            {
                discordClient.SendMessageAsync(msg, false);
            }
            Console.WriteLine(msg);
        }
    }
}
