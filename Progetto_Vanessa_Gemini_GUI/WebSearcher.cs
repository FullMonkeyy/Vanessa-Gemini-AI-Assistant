using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using YoutubeExplode;
using YoutubeExplode.Common;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using System.IO;

namespace Progetto_Vanessa_Gemini_GUI
{
    public static class WebSearcher
    {

        static public CancellationTokenSource Cts { get; set; }
        static public Task Job { get; set; }


        static public async Task<string> Notizie(int notizierichieste)
        {
            // Download the webpage
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync("https://www.rassegnastampaquotidiani.com/feed/corriere-della-sera");

            // Load the webpage into an HtmlDocument
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Extract the paragraph texts with the class "chakra-text css-rovqx6"
            var paragraphs = doc.DocumentNode.SelectNodes("//p[contains(@class, 'chakra-text') and contains(@class, 'css-rovqx6')]");
            var titles = doc.DocumentNode.SelectNodes("//div[contains(@class, 'css-722v25')]");
            var texts = new string[paragraphs.Count];
            string Utility = "";

            for (int i = 0; i < paragraphs.Count && i < notizierichieste; i++)
            {
                Console.WriteLine(titles[i].InnerText.Trim());
                Console.WriteLine(paragraphs[i].InnerText.Trim());
                Utility += "\n\\n" + titles[i].InnerText.Trim() + paragraphs[i].InnerText.Trim();

                Console.WriteLine("\n\n\n");

            }

            return Utility;
        }
        static public async Task PlayMusic(string title)
        {
            var youtube = new YoutubeClient();

            // You can specify the video title here
            var videoTitle = title;

            // Search for the video by title
            var searchResult = await youtube.Search.GetVideosAsync(videoTitle);
            var video = searchResult.FirstOrDefault();

            if (video != null)
            {
                // Get the stream manifest
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
                var audioStreams = streamManifest.GetAudioOnlyStreams();
                // Select an audio-only stream
                var streamInfo = audioStreams.OrderByDescending(s => s.Bitrate).FirstOrDefault();

                if (streamInfo != null)
                {
                    // Download the audio
                    Console.WriteLine("Downlaod iniziato");
                    await youtube.Videos.Streams.DownloadAsync(streamInfo, $"YTVideo/{video.Title}.mp3");
                    Console.WriteLine("Download terminato");
                    while (!File.Exists($"YTVideo/{video.Title}.mp3"))
                        Thread.Sleep(100);

                    Cts = new CancellationTokenSource();
                    Console.WriteLine("Donwload confermato");

                    await VanessaUtilities.ReproduceMusic($"YTVideo/{video.Title}.mp3");
                    VanessaUtilities.IsMusicPlaying = false;
                    Console.WriteLine("Canzone Terminata");
                    //File.Delete($"YTVideo/{video.Title}.mp3");



                }
                else
                {
                    Console.WriteLine("Could not find an audio-only stream.");
                }
            }
            else
            {
                Console.WriteLine("Video not found.");
            }
        }


    }
}
