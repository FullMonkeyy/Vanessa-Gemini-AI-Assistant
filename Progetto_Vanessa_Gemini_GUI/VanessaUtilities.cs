using NAudio.Wave;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using ChatAIze.GenerativeCS.Models;

namespace Progetto_Vanessa_Gemini_GUI
{
    public static class VanessaUtilities
    {

        static Thread giradischi;
        public static WaveOutEvent outputDevice { get; set; }
        public static WaveOutEvent VoiceUtilities { get; set; }
        public static StreamWriter DetectorNotify { get; set; }
        public static StreamReader DetectorReader { get; set; }
        public static bool MessaggioDaInviare {  get; set; }
        public static string Destinatario {  get; set; }
        public static string Messaggio {  get; set; }
        public static bool MessaggioDaLeggere { get; set; }
        static bool occupata;
        
        public static bool OCCUPATA { get { return occupata;  } set { 
                mutex.WaitOne();
                occupata = value;
                mutex.ReleaseMutex();
         } }
        public static string UTILITY { get; set; }
        private static bool ismusicplaying;
        public static bool IsMusicPlaying { get { return ismusicplaying; } set { ismusicplaying = value; } }
        static Mutex mutex=new Mutex();

        static public string GetComand(string input)
        {

            string originale = input;
            string pattern = @"\[#!(.*?)!#\]";

            Match match = Regex.Match(originale, pattern);
            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return "Nessun comando";
            }
        }
        static public string GetMessage(string input)
        {

            string originale = input;
            string pattern = @"\[#!(.*?)!#\]";

            // Sostituisce la porzione corrispondente al pattern con una stringa vuota
            string result = Regex.Replace(originale, pattern, "");
            result = result.Replace(('*').ToString(), "");
            result = result.TrimEnd('\n');
            result = result.TrimEnd(' ');
            result = result.TrimStart(' ');
            return result;
        }
        static public async Task<int> ExecuteComand(string comand)
        {

            int result = -1;
            string input = comand;
            string pattern = @"\[#!(.*?)\((.*?)\)(.*?)\!#]";
            string comando;
            string parametri;
            string tmp1;
            Match match = Regex.Match(input, pattern);
            if (match.Success)
            {
                comando = match.Groups[1].Value;
                parametri = match.Groups[2].Value;
                switch (comando)
                {
                    case "CANZONE":
                        if (!ismusicplaying)
                        {
                            result = 0;
                            tmp1 = IsMusicCached(parametri);
                            if (tmp1.Equals("Non presente"))
                            {
                                parametri = parametri.Replace(';', ' ');
                                WebSearcher.PlayMusic(parametri);
                            }
                            else
                            {
                                ReproduceMusic(tmp1);
                            }                                
                                                       
                            Console.WriteLine("Ricerca alla canzone avviata");
                        }
                        break;
                    case "CANZONESTOP":

                        if (outputDevice != null)
                            outputDevice.Stop();
                        Console.WriteLine("Canzone terminata");
                        ismusicplaying = false;
                        result = 1;


                        break;
                    case "PRESAVISIONE":

                        DetectorNotify.WriteLine("Cosa vedi?");
                        DetectorNotify.Flush();
                        UTILITY = DetectorReader.ReadLine();


                        result = 2;
                        break;
                    case "NOTIZIE":
                        int Nnotizie;
                        int.TryParse(parametri, out Nnotizie);
                        UTILITY = await WebSearcher.Notizie(Nnotizie);
                        result = 3;
                        break;
                    case "RICONOSCIMENTO":

                        DetectorNotify.WriteLine(parametri);
                        DetectorNotify.Flush();
                        UTILITY = DetectorReader.ReadLine();
                        result = 4;
                        break;
                    case "MESSAGGIO":
          
                        string[] tmp=parametri.Split(';');

                        Destinatario = tmp[0];
                        Messaggio = tmp[1];
                        MessaggioDaInviare = true;
                        result = 5;
                        break;


                }

            }


            return result;

        }
        static public async Task ReproduceAudio(string path, CancellationToken c)
        {

            await Task.Run(() =>
            {
                using (var audioFile = new AudioFileReader(path))
                {
                    VoiceUtilities = new WaveOutEvent();
                    VoiceUtilities.Volume = 1;
                    if (VoiceUtilities.PlaybackState == PlaybackState.Playing)
                        VoiceUtilities.Stop();
                    VoiceUtilities.Init(audioFile);
                    VoiceUtilities.Play();
                    while (VoiceUtilities.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(200);
                    }
                    if (VoiceUtilities.PlaybackState == PlaybackState.Playing)
                    {
                        VoiceUtilities.Stop();
                    
                    }
                  

                }
            });

        }
        static public async Task ReproduceAudio(string path)
        {

            await Task.Run(() =>
            {
                using (var audioFile = new AudioFileReader(path))
                {
                    VoiceUtilities = new WaveOutEvent();
                    VoiceUtilities.Volume = 1;
                    if (VoiceUtilities.PlaybackState == PlaybackState.Playing)
                        VoiceUtilities.Stop();
                    VoiceUtilities.Init(audioFile);
                    VoiceUtilities.Play();
                    while (VoiceUtilities.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(200);
                    }
                    if (VoiceUtilities.PlaybackState == PlaybackState.Playing)
                    {
                        VoiceUtilities.Stop();
                        
                    }


                }
            });

        }
        static public async Task ReproduceMusic(string path)
        {


            await Task.Run(() =>
            {
                try
                {
                    using (var audioFile = new AudioFileReader(path))
                    {
                        OCCUPATA = false;
                        ismusicplaying = true;
                        outputDevice = new WaveOutEvent();
                        if (outputDevice.PlaybackState == PlaybackState.Playing)
                            outputDevice.Stop();
                        outputDevice.Init(audioFile);
                        outputDevice.Play();
                        while (outputDevice.PlaybackState == PlaybackState.Playing)
                        {
                            Thread.Sleep(200);
                        }
                        if (outputDevice.PlaybackState == PlaybackState.Playing)
                        {
                            outputDevice.Stop();
                            ismusicplaying = false;
                        }
                           

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });

        }
        static public string GetObjectView()
        {
            string str = "";
            try
            {
                StreamReader sr = new StreamReader(@"YOLO\detection.txt");
                str = sr.ReadToEnd();
                sr.Close();


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return str;
        }
        static public ChatConversation CaricaCronologiaDiChatting(ChatConversation ctc, List<Messaggio> lm)
        {
            ChatConversation conv = ctc;

            foreach (Messaggio mess in lm)
            {

                if (mess.Role.Equals("Vanessa"))
                {
                    conv.FromChatbot(mess.Message);
                }
                else if (mess.Role.Equals("User"))
                {
                    conv.FromUser(mess.Message);
                }



            }

            return conv;
        }
        static string IsMusicCached(string name)
        {
            List<string> list = Directory.GetFiles("YTVideo/").ToList();
            list.ForEach(x => x = x.ToLower());
            List<string>  list2=name.Split(';').ToList();
            list2.ForEach(x => x = x.ToLower());
            string path = "Non presente";
            bool trovata = false;
            foreach (string file in list) {

                if (Path.GetFileName(file).Contains(list2[0])) { 
                
                    path = file;
                    trovata = true;
              
                    break;
                
                }

            
            }   
            return path;
        }


    }
}
