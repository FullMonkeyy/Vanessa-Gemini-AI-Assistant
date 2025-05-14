using ChatAIze.GenerativeCS.Options.Gemini;
using ChatAIze.GenerativeCS.Clients;
using ChatAIze.GenerativeCS.Constants;
using ChatAIze.GenerativeCS.Models;
using Microsoft.VisualBasic;
using ChatAIze.GenerativeCS.Enums;
using Progetto_Vanessa_Gemini_GUI;
using System.IO;
using NAudio.Wave;
using System.Diagnostics;
using System.Threading;
using System.IO.Pipes;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using System.Net.NetworkInformation;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Windows.Threading;
using System.Diagnostics.Metrics;


namespace Progetto_Vanessa_Gemini_GUI
{
    public class VanessaGemini
    {
        public bool ProcessoAttivo { get; set; }

        Process Yolo;
        Process APIS;
        Task MainThread;
        TelegramMenager telmen;
        public TelegramMenager Telemen { get { return telmen; } }
        CancellationTokenSource mainCTS;
        Timer timerstandby;
        VanessaCore core;

        bool segnalestandby;
        public List<dynamic> data
        {
            get
            {
                List<dynamic> listtmp = new List<dynamic>();

                // Create a new dynamic object for each iteration
                for (int i = 0; i < core.ConversationHystory.Count; i += 2)
                {
                    dynamic tmpdyn = new ExpandoObject(); // Or use a custom class

                    tmpdyn.A = core.ConversationHystory[i].Message;
                    tmpdyn.B = core.ConversationHystory[i + 1].Message;

                    listtmp.Add(tmpdyn);
                }

                return listtmp;
            }
        }
        public string PrompText { get { return core.PromptText; } }
        public string PromptResponse { get { return VanessaUtilities.GetMessage(core.PromptResponse); } }
        bool inizializzata;
        bool noconnessione;
   
        public bool MusicaAggiornata { get; set; }
        public bool Inizializzata { get { return inizializzata; } }
        public bool Noconnessione { get { return noconnessione; } }
        public List<string> CachedSongs { 

            get { 
                
                List<string> list = Directory.GetFiles("YTVideo/").ToList();
                List<string> listtmp = new List<string>();
                list.ForEach(s => listtmp.Add(Path.GetFileName(s)));
                return listtmp;              

             } 
        }
        public List<Persona> TLGWhiteList{get{ return telmen.Whitelist; } }
        public void VanessaGeminiStart()
        {
            inizializzata = false;
            Yolo = new Process();
            APIS = new Process();
            mainCTS=new CancellationTokenSource();
            StandByTask(mainCTS.Token);

            Task.Run(() => { VanessaGeminiOnline(mainCTS.Token); }, mainCTS.Token);
        
        }
        public void VanessaStop()
        {
            if (inizializzata)
            {

                mainCTS.Cancel();

                if (!Yolo.HasExited)
                {
                    Yolo.Kill();
                }
        
                if (!APIS.HasExited)
                {
                    APIS.Kill();
                }
                SaveHystory();
            }

  

        }
        public async Task VanessaGeminiOnline(CancellationToken token)
        {
            ProcessoAttivo = true;

            List<string> MessaggiDaLeggere = new List<string>();
            CancellationTokenSource tmp = new CancellationTokenSource();
            bool connessione = false;
            if (true)
            {

                await VanessaUtilities.ReproduceAudio(@"VOICE_UTILITIES\InizializzazioneSistema.mp3", tmp.Token);
                try
                {
                    string host = "www.google.com";
                    Ping pingsender = new Ping();
                    PingReply reply = pingsender.Send(host);
                    if (reply.Status == IPStatus.Success)
                    {
                        Console.WriteLine("Connessione stabile.");
                        connessione = true;
                    }
                    else
                    {
                        Console.WriteLine("Connessione instabile...");

                    }
                }
                catch
                {
                    Console.WriteLine("Nessuna connessione...");
                }

                if (connessione)
                {
                    //THIS IS MY FILE CREDENTIAL VANESSA GEMINI AND THE FILE WON'T BE ABLE FOR THIS SOURCE CODE
                    //Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"VanessaKey.json");

                    //HERE PUT THE PATH OF YOUR GEMINI CREDENTIALS
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",);
                    var options = new ChatCompletionOptions
                    {
                        Model = ChatCompletionModels.Gemini.GeminiPro,
                        MaxAttempts = 900,
                        MessageLimit = 900,
                    };

                    string lsitaoggetti = "";
                    bool ascolta = false;
                    var pipe1 = new NamedPipeServerStream(@"STT", PipeDirection.InOut);                    
                    var pipe2 = new NamedPipeServerStream(@"VISUALIZER", PipeDirection.InOut);
                    var pipe4 = new NamedPipeServerStream(@"TELEGRAMVOICERECOGNITION", PipeDirection.InOut);
                    var pipe3 = new NamedPipeServerStream(@"TELEGRAMVISUALIZER", PipeDirection.InOut);
                    Thread threadVisualizer = new Thread(() => { UnitàVisiva(Yolo); });
                    Thread threadAPI = new Thread(() => { CentroApi(APIS); });
                    threadVisualizer.Start();
                    threadAPI.Start();
                    Console.WriteLine("In attesa di connessione voice recognition...");
                    await VanessaUtilities.ReproduceAudio(@"VOICE_UTILITIES\AttesaConnesioneVoice.mp3", tmp.Token);
                    Console.WriteLine("In attesa di connessione telecamera...");
                    await VanessaUtilities.ReproduceAudio(@"VOICE_UTILITIES\AttesaConnesioneTele.mp3", tmp.Token);
                    Console.WriteLine("In attesa collegamento telegram visualizer...");
                    await VanessaUtilities.ReproduceAudio(@"VOICE_UTILITIES\AttesaTelegramVis.mp3", tmp.Token);
                    
                    Console.WriteLine("In attesa collegamento telegram voice recognition...");
                    pipe4.WaitForConnection();
                    pipe3.WaitForConnection();
                    await VanessaUtilities.ReproduceAudio(@"VOICE_UTILITIES\TelegramVisualizer.mp3", tmp.Token);
                    pipe2.WaitForConnection();
                    await VanessaUtilities.ReproduceAudio(@"VOICE_UTILITIES\TeleOnline.mp3", tmp.Token);
                    pipe1.WaitForConnection();
                    await VanessaUtilities.ReproduceAudio(@"VOICE_UTILITIES\VoiceOnline.mp3", tmp.Token);
                    var reader1 = new StreamReader(pipe1);
                    var writer1 = new StreamWriter(pipe1);

                    var reader2 = new StreamReader(pipe2);
                    var writer2 = new StreamWriter(pipe2);

                    VanessaUtilities.DetectorNotify = writer2;
                    VanessaUtilities.DetectorReader = reader2;
                    VanessaUtilities.OCCUPATA = false;
                    HttpClient APICENTRE = new HttpClient();

                    // Set for entire client:
                    //var client = new GeminiClient("AIzaSyA4zLFMk-kv6YrATc9U-GUy4BukuxWcudM", options); // via constructor
                    //var conversation = new ChatConversation();
                    List<Persona> rubrica = new List<Persona>();
                    rubrica = GestioneFile.ReadXMLRubrica("RubricaPersona.xml");               
                    core = await GenerateContent();
                    List<Messaggio> conversazioneComandi = GestioneFile.ReadXMLConversation("InizializeXMLConversation.xml");
                    core.InitializeKnowdelge(conversazioneComandi);
                    core.InitializeKnowdelge(core.ConversationHystory);
                    //Codice gestione Telegram                
                    int UserId = 1140272456;
                   // ConversationHystory = GestioneFile.ReadXMLConversation("CronologiaMessaggiVanessa.xml");
                    telmen = new TelegramMenager(core, rubrica, writer2, reader2, pipe3, pipe4);                   
                    TelegramShutDownReques();
    

                    //Fine  codice gestione telegram
                    string message;

                    telmen.DetectorNotify = writer2;
                    telmen.DetectorReader = reader2;
                    VanessaUtilities.MessaggioDaLeggere = false;
               
                    await VanessaUtilities.ReproduceAudio(@"VOICE_UTILITIES\CompletamentoProceduraVanessaOper.mp3", tmp.Token);
           

                    int cmd = -1;
                  
                    string response;
                    string messaggio;
                    string comando;
                    string detections;
                    List<string> inascolto = new List<string>();
                    inascolto.Add("vanessa in ascolto");
                    inascolto.Add("va ne sa in ascolto");
                    inascolto.Add("vanessa inascolto");
                    inascolto.Add("vanessa è in ascolto");
                    inascolto.Add("va nesse è inascolto");   
                    
                    inizializzata = true;
                    telmen.InviaMessaggioDavide("Vanessa operativa e pronta per assisterla");

                    while (!token.IsCancellationRequested)
                    {
                        telmen.VanessaOccupata = false;
                        if(VanessaUtilities.OCCUPATA)
                            segnalestandby = true;

                        writer1.WriteLine("Ascolta");
                        writer1.Flush();
                        message = "";
                        message = reader1.ReadLine();
                        segnalestandby = false;
                        if (token.IsCancellationRequested)
                            break;
                        message = message.ToLower();
                        message = message.Trim(' ');
                        string mex = message;

                        if (VanessaUtilities.OCCUPATA)
                        {
                            if (token.IsCancellationRequested)
                                break;
                            if (message != null)
                            {
                                message.ToLower();
                                message.Trim();

                                if (message.Contains("vanessa fermati"))
                                {
                                    if (TTS.wa.PlaybackState == PlaybackState.Playing)
                                        TTS.wa.Stop();
                                    if (VanessaUtilities.VoiceUtilities.PlaybackState == PlaybackState.Playing)
                                        VanessaUtilities.VoiceUtilities.Stop();
                                    VanessaUtilities.ReproduceAudio(@"VOICE_UTILITIES\TerminoAscolto.mp3", tmp.Token);
                                    VanessaUtilities.OCCUPATA=false;           
                                }
                            }
                        }
                        if (VanessaUtilities.OCCUPATA)
                        {
                           
                       
                                if (token.IsCancellationRequested)
                                    break;                            
                                //message = Console.ReadLine();
                                //Console.WriteLine(message + "\nOGGETTI:  " + VanessaUtilities.GetObjectView());
                                try
                                {
                                    Console.WriteLine("ORARIO: " + DateTime.Now.ToLongDateString());
                                    message = message + "\n\nDATA E ORA: " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();

                  
                                    if (VanessaUtilities.VoiceUtilities.PlaybackState == PlaybackState.Playing)
                                        VanessaUtilities.VoiceUtilities.Stop();
                                    VanessaUtilities.ReproduceAudio(@"VOICE_UTILITIES\LasciamiElaborareFrase.mp3", tmp.Token);

                                    response = await core.SendMessageAsync(message);
                  
                                    messaggio = VanessaUtilities.GetMessage(response);
                                    Console.WriteLine("MESSAGGIO: " + messaggio);
                                    comando = VanessaUtilities.GetComand(response);
                                    Console.WriteLine("COMANDO: " + comando);

                                    await TTS.Main(@"VANESSA_VOICE\Output.mp3", messaggio);
                                    if (VanessaUtilities.VoiceUtilities.PlaybackState == PlaybackState.Playing)
                                        VanessaUtilities.VoiceUtilities.Stop();
                                    if (TTS.wa.PlaybackState == PlaybackState.Playing)
                                        TTS.wa.Stop();
                                    await TTS.PlayVoice();

                                    if (!comando.Equals("Nessun comando"))
                                    {

                                        cmd = await VanessaUtilities.ExecuteComand(comando);
                                        switch (cmd)
                                        {

                                            case 2:


                             
                                                response = await core.SendMessageAsync("SYSTEM:\n" + VanessaUtilities.UTILITY);
                               



                                                messaggio = VanessaUtilities.GetMessage(response);
                                                Console.WriteLine("MESSAGGIO DI VANESSA IN RISPOSTA DEL SISTEMA:\n " + messaggio);

                                                if (TTS.wa.PlaybackState == PlaybackState.Playing)
                                                    TTS.wa.Stop();
                                                await TTS.Main(@"VANESSA_VOICE\Output.mp3", messaggio);
                                                await TTS.PlayVoice();
                                                break;
                                            case 3:

                                                messaggio = VanessaUtilities.UTILITY;

                                                if (TTS.wa.PlaybackState == PlaybackState.Playing)
                                                    TTS.wa.Stop();
                                                await TTS.Main(@"VANESSA_VOICE\Output.mp3", messaggio);
                                                await TTS.PlayVoice();
                                                break;
                                            case 4:




                                                break;


                                        }

                                    }
     
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Qualcosa e' andato storto... " + ex.ToString());
                                }
                            
                        }
                        else if (inascolto.Exists(x => mex.Contains(x)))
                        {
                            VanessaUtilities.OCCUPATA = true;
                            telmen.VanessaOccupata = true;
                            await VanessaUtilities.ReproduceAudio(@"VOICE_UTILITIES\UnitàVanessaInAscolto.mp3", tmp.Token);
                        }
                        else if (message.Contains("vanessa spegniti"))
                        {
                            await VanessaUtilities.ReproduceAudio(@"VOICE_UTILITIES\DisconnessioneInCorso.mp3", tmp.Token);
                            break;
                        }

                    }
                    if (!Yolo.HasExited)
                    {
                        Yolo.Kill();
                    }
                    if (threadVisualizer.IsAlive)
                    {
                        threadVisualizer.Join();
                    }
                    if (!APIS.HasExited)
                    {
                        APIS.Kill();
                    }
                    if (threadAPI.IsAlive)
                    {
                        threadAPI.Join();
                    }
                    telmen.cts.Cancel();//Scollega bot da telegram
                    /*  do
                       {
                           Console.WriteLine("Vuoi salvare la conversazione?[Y/N]: ");
                           message = Console.ReadLine();
                       } while (!message.Equals("Y") && !message.Equals("N"));
                       if (message.Equals("Y"))
                       {
                           GestioneFile.WriteXMLConversation("CronologiaMessaggiVanessa.xml", ConversationHystory);

                       }*/
                    SaveHystory();
                }
                else
                {
                    noconnessione = true;
                }
            }
            static void UnitàVisiva(Process p)
            {

                p.StartInfo.FileName = "python.exe"; // Sostituisci con il percorso effettivo dell'interprete Python
                p.StartInfo.Arguments = "main.py"; // Sostituisci con il nome del tuo script Python
                p.StartInfo.WorkingDirectory = @"YOLO"; // Sostituisci con il percorso della directory contenente lo script
                                                        // Avvia il processo e attendi che termini
                p.Start();
                p.WaitForExit();

            }
            static void CentroApi(Process p)
            {

                p.StartInfo.FileName = "python.exe"; // Sostituisci con il percorso effettivo dell'interprete Python
                p.StartInfo.Arguments = "main.py"; // Sostituisci con il nome del tuo script Python
                p.StartInfo.WorkingDirectory = @"STT"; // Sostituisci con il percorso della directory contenente lo script
                                                       // Avvia il processo e attendi che termini
                p.Start();
                p.WaitForExit();

            }
            async Task<VanessaCore> GenerateContent(
                    string projectId = "gen-lang-client-0630332209",
                    string location = "us-central1",
                    string publisher = "google",
                    string model = "gemini-1.5-pro-preview-0514"


                )
            {
                // Create a chat session to keep track of the context
                VanessaCore chatSession = new VanessaCore($"projects/{projectId}/locations/{location}/publishers/{publisher}/models/{model}", location);


                return chatSession;
            }

            ProcessoAttivo = false;
            
        }
        public async Task TelegramShutDownReques()
        {

            await Task.Run(() => {
                while (true)
                {

                    if (telmen.ShutDown)
                    {
                        VanessaStop();
                    }
                    Thread.Sleep(50);
                }
            });

        }  
        public void SaveHystory()
        {

            GestioneFile.WriteXMLConversation("CronologiaMessaggiVanessa.xml",core.ConversationHystory);
        }
        private List<Messaggio> DynamicToMexList(List<dynamic> ldn)
        {

            List<Messaggio> list = new List<Messaggio>();

            foreach(dynamic d in ldn)
            {
                list.Add(new Messaggio(d.A,"User"));
                list.Add(new Messaggio(d.B, "Vanessa"));
            }

            return list;    

        }
        public void COnversationCoreUpdate(List<dynamic> ldn)
        {
            core.UpdateConversation(DynamicToMexList(ldn));
        }
        public async Task StandByTask(CancellationToken ct)
        {

            await Task.Run(() =>
            {
                int i = 0;
                const int n = 3600;
                while (!ct.IsCancellationRequested)
                {

                    if (i >= n)//3 minuti di attesa fino allo standby
                    {
                        segnalestandby = false;
                        VanessaUtilities.OCCUPATA = false;
                        i = 0;
                        VanessaUtilities.ReproduceAudio(@"VOICE_UTILITIES\TerminoAscolto.mp3");
                    }
                    else if (segnalestandby)
                    {
                        i++;
                    }

                    if (!segnalestandby)
                    {
                        i = 0;
                    }                   

                    Thread.Sleep(50);
                    
                }
            });



        }
        public async Task PlaySongRequest(string path)
        {

            await Task.Run(() =>
            {
                
                if (VanessaUtilities.outputDevice != null)
                    VanessaUtilities.outputDevice.Stop();  
                VanessaUtilities.ReproduceMusic(path);
            });
        }
        public async Task SaveRubrica(List<Persona> lp) {

            await Task.Run(() => {

                GestioneFile.WriteXMLRubrica(GestioneFile.PATH, lp);
            
            });
        
        }
        public void PauseSongRequest()
        {
                  
              if (VanessaUtilities.outputDevice != null)
                    VanessaUtilities.outputDevice.Pause();
          
        }
        public bool StartSongRequest()
        {

          

                if (VanessaUtilities.outputDevice != null)
                {
                    if (VanessaUtilities.outputDevice.PlaybackState == PlaybackState.Paused)
                    {
                        VanessaUtilities.outputDevice.Play();
                        return true;
                    }
                    else return false;
                   
                }
                else return false;
           
        }
        public void CambiaCanzone(string path, string nuovonome) { 

            string ext=Path.GetExtension(path);
            string nuovoFileName= @$"YTVideo\{nuovonome}.{ext}";
            try
            {
                File.Move(@$"YTVideo\{path}", nuovoFileName);
                MusicaAggiornata = true;
            }
            catch (Exception ex) { 
            
                Console.WriteLine(ex.ToString());
            
            }   
          

        }

    }
}
