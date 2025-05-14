using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using System.Windows.Media;

using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Google.Rpc;
using static Google.Api.Gax.Grpc.Gcp.AffinityConfig.Types;
using NAudio.Wave;
using System.IO.Pipes;
using System.IO;
using System.Text.RegularExpressions;
using static Google.Rpc.Context.AttributeContext.Types;
using Grpc.Core;
using System.IO.Packaging;
using System.Threading;
using System.Windows.Controls;
using System.Diagnostics;


namespace Progetto_Vanessa_Gemini_GUI
{
    public class TelegramMenager
    {
     
        const string destinationFilePath = "YOLO/Foto.png";
        const long DavideID = 1140272456;
        const string ApiKeyBot = ; //YOU HAVE TO PUT YOUR TELEGRAM BOT API KEY HERE
        public Queue<Messaggio> MexDaLegg;
        public Queue<Messaggio> MessaggiDavide;
        public List<string> CronologiaTelegram;
        public bool VanessaOccupata;
        public CancellationTokenSource cts { get; set; }
        public StreamWriter DetectorNotify { get; set; }
        public StreamReader DetectorReader { get; set; }
        public StreamWriter VoiceNotify { get; set; }
        public StreamReader VoiceReader { get; set; }
        TelegramBotClient botClient;
        ReceiverOptions receiverOptions;
        VanessaCore vanessacore;
        List<Persona> whitelist;
        public List<Persona> Whitelist { get { return whitelist; } }
        StreamWriter NotifyVisualizer;
        StreamReader WaitVisualizer;
        public Persona Convalidando { get; set; }
        public string CodiceConvalidazione { get; set; }
        public bool ListaAggiornata { get; set; }
        public bool IsThereNewMessage { get; set; }
        public bool ShutDown {  get; set; }
        MediaElement mediaElement;
        public TelegramMenager(VanessaCore vc, List<Persona> rb, StreamWriter ntf, StreamReader listen, NamedPipeServerStream pipe, NamedPipeServerStream pipevoice)
        {
            botClient = new TelegramBotClient(ApiKeyBot);
            cts=new CancellationTokenSource();
             receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
            };

            botClient.StartReceiving(
                        updateHandler: HandleUpdateAsync,
                        pollingErrorHandler: HandlePollingErrorAsync,
                        receiverOptions: receiverOptions,
                        cancellationToken: cts.Token
             );
            SendMessage();
            var me = botClient.GetMeAsync();
            vanessacore = vc;
            whitelist = rb;
            MexDaLegg = new Queue<Messaggio>();
            MessaggiDavide = new Queue<Messaggio>();
            DetectorNotify = ntf;
            DetectorReader = listen;
            WaitVisualizer = new StreamReader(pipe);
            NotifyVisualizer = new StreamWriter(pipe);
            VoiceReader = new StreamReader(pipevoice);
            VoiceNotify = new StreamWriter(pipevoice);
           
            ListaAggiornata = false;
            CronologiaTelegram = new List<string>();
            IsThereNewMessage = false;
            ShutDown = false;
            NewMessage();
        }
       
        //Elena 5704539870
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            if (true)
            {
                if (update.Message is not { } message)
                    return;

                long chatId = message.Chat.Id;
                if (!whitelist.Exists(x => x.Id.Equals(chatId)))
                {
                   if (message.Text.Contains("/code:"))
                    {

                        string[] tmp = message.Text.Split(':');
                        if (tmp.Length > 1)
                        {
                            string tmpcodice = tmp[1];
                            if (tmpcodice.Equals(CodiceConvalidazione))
                            {
                                ListaAggiornata = true;
                                Convalidando.Id = chatId;
                                whitelist.Add(Convalidando);
                                GestioneFile.WriteXMLRubrica(GestioneFile.PATH, whitelist);
                                await botClient.SendTextMessageAsync(
                                 chatId: chatId,
                                 text: "Convalidazione riuscita!",
                                 cancellationToken: cancellationToken);
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(
                               chatId: chatId,
                               text: "Codice errato...",
                               cancellationToken: cancellationToken);
                            }
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(
                              chatId: chatId,
                              text: "Formato comando errato\n /code: CODICE DA INSERIRE",
                              cancellationToken: cancellationToken);
                        }
                    }
                    else
                    {

                        MexDaLegg.Enqueue(new Messaggio(message.Text, $"{chatId}"));

                        await botClient.SendTextMessageAsync(
                                    chatId: chatId,
                                    text: "Mi dispiace, mi è proibito parlare con te...",
                                    cancellationToken: cancellationToken);
                    }


                }
                else if (VanessaUtilities.OCCUPATA)
                {
                    if (message.Text.Contains("/fermati"))
                    {
                        VanessaUtilities.OCCUPATA = false;
                        if (TTS.wa.PlaybackState == PlaybackState.Playing)
                            TTS.wa.Stop();
                        if (VanessaUtilities.VoiceUtilities.PlaybackState == PlaybackState.Playing)
                            VanessaUtilities.VoiceUtilities.Stop();
                        await botClient.SendTextMessageAsync(
                                    chatId: chatId,
                                    text: "Ascolto vocale fermato",
                                    cancellationToken: cancellationToken);

                    }
                    else
                    {
                        MessaggiDavide.Enqueue(new Messaggio(message.Text, $"{whitelist.Find(x => x.Id.Equals(chatId)).CompleteName}"));
                        await botClient.SendTextMessageAsync(
                                      chatId: chatId,
                                      text: $"Mi dispiace {whitelist.Find(x => x.Id.Equals(chatId)).FirstName} ma al momento sono occupata, la tua richiesta verrà presto presa in carico a tempo debito...\n ",
                                      cancellationToken: cancellationToken);
                    }
                    
                }
                // Only process Message updates: https://core.telegram.org/bots/api#message
                else
                {
                    // Only process text messages
                    if (message.Text == null)
                    {

                        if (update.Message.Photo != null)
                        {
                            CronologiaTelegram.Add($"FOTO MANDATA DA {whitelist.Find(x => x.Id.Equals(chatId)).FirstName} CON TELEGRAM\n ");
                            var fileId = update.Message.Photo.Last().FileId;
                            var fileInfo = await botClient.GetFileAsync(fileId);
                            var filePath = fileInfo.FilePath;
                            await using Stream fileStream = System.IO.File.Create(destinationFilePath);
                            await botClient.DownloadFileAsync(
                                filePath: filePath,
                                destination: fileStream,
                                cancellationToken: cancellationToken);
                            fileStream.Close();
                            while (!System.IO.File.Exists(destinationFilePath))
                                Thread.Sleep(100);

                            if (true)
                            {


                                NotifyVisualizer.WriteLine("Cosa vedi telegram?");
                                NotifyVisualizer.Flush();
                                string detect = WaitVisualizer.ReadLine();
                                string response = await vanessacore.SendMessageAsync($"FOTO MANDATA DA {whitelist.Find(x => x.Id.Equals(chatId)).FirstName} CON TELEGRAM:\n " + detect);
                                CronologiaTelegram.Add(response);
                                await botClient.SendTextMessageAsync(
                                            chatId: chatId,
                                            text: response,
                                            cancellationToken: cancellationToken);

                                try
                                {
                                    await using Stream stream = System.IO.File.OpenRead("YOLO/IMMAGINEPRESAVISIONE.png");
                                    await botClient.SendDocumentAsync(
                                        chatId: chatId,
                                        document: InputFile.FromStream(stream: stream, fileName: "IMMAGINEPRESAVISIONE.png"),
                                        caption: "Risultato presavisioni.");

                                }
                                catch (Exception ex)
                                {

                                    Console.WriteLine(ex.Message);

                                }


                            }
                        }
                        if (update.Message.Voice != null)
                        {
                            var fileId = update.Message.Voice.FileId;
                            var fileInfo = await botClient.GetFileAsync(fileId);
                            var filePath = fileInfo.FilePath;


                            const string destinationFilePath = "STT/TELEGRAMVOICES/VoiceDIDavide.wav";

                            await using Stream fileStream = System.IO.File.Create(destinationFilePath);
                            await botClient.DownloadFileAsync(
                                filePath: filePath,
                                destination: fileStream,
                                cancellationToken: cancellationToken);
                            fileStream.Close();
                            VoiceNotify.WriteLine("ElaboraMessaggioVanessa");
                            VoiceNotify.Flush();
                            string messaggioVocale = VoiceReader.ReadLine();
                            CronologiaTelegram.Add($"MESSAGGIO VOCALE  {whitelist.Find(x => x.Id.Equals(chatId)).FirstName} :\n" + messaggioVocale);
                            string response = await vanessacore.SendMessageAsync("SYSTEM:\n" + messaggioVocale);
                            CronologiaTelegram.Add("MESSAGGIO VOCALE:\n" + response);
                            /*
                            await botClient.SendTextMessageAsync(
                                        chatId: chatId,
                                        text: response,
                                        cancellationToken: cancellationToken);*/


                            await TTS.Main(@"VANESSA_VOICE\Output.mp3", VanessaUtilities.GetMessage(response));



                            // Set the source of the media element to the audio file
                            int secondi=GetMP3Duration(@"VANESSA_VOICE\Output.mp3");

                            await using Stream streamtmp = System.IO.File.OpenRead("VANESSA_VOICE\\Output.mp3");
                   
                            await botClient.SendVoiceAsync(
                                chatId: chatId,
                                voice: InputFile.FromStream(streamtmp),
                                duration: secondi,
                                cancellationToken: cancellationToken);
                            streamtmp.Close();


                            string comando = VanessaUtilities.GetComand(response);
                            Console.WriteLine("COMANDO: " + comando);


                            if (Getcmd(comando).Equals("RICONOSCIMENTOTELEGRAM"))
                            {
                                NotifyVisualizer.WriteLine(Getcparams(comando));
                                NotifyVisualizer.Flush();
                                WaitVisualizer.ReadLine();

                                await botClient.SendTextMessageAsync(
                                            chatId: chatId,
                                            text: "Volto imparato (teoricamente)",
                                            cancellationToken: cancellationToken);

                            }
                            else if (!comando.Equals("Nessun comando"))
                            {

                                int cmd = await VanessaUtilities.ExecuteComand(comando);
                                switch (cmd)
                                {

                                    case 2:

                                        response = await vanessacore.SendMessageAsync("SYSTEM:\n" + VanessaUtilities.UTILITY);
                                        CronologiaTelegram.Add(response);
                                        await botClient.SendTextMessageAsync(
                                                    chatId: chatId,
                                                    text: response,
                                                    cancellationToken: cancellationToken);

                                        try
                                        {
                                            await using Stream stream = System.IO.File.OpenRead("YOLO/IMMAGINEPRESAVISIONE.png");
                                            await botClient.SendDocumentAsync(
                                                chatId: chatId,
                                                document: InputFile.FromStream(stream: stream, fileName: "IMMAGINEPRESAVISIONE.png"),
                                                caption: "Risultato presavisioni.");

                                        }
                                        catch (Exception ex)
                                        {

                                            Console.WriteLine(ex.Message);

                                        }


                                        break;
                                    case 3:

                                        string messaggio = VanessaUtilities.UTILITY;
                                        CronologiaTelegram.Add(messaggio);
                                        await botClient.SendTextMessageAsync(
                                                chatId: chatId,
                                                text: messaggio,
                                                cancellationToken: cancellationToken);

                                        break;


                                }

                            }


                        }
                        else
                            return;
                    }
                    else
                    {
                        if (message.Text.StartsWith('/'))
                        {
                          
                       if (message.Text.Contains("/ascolta"))
                            {
                                VanessaUtilities.OCCUPATA = true;

                                await botClient.SendTextMessageAsync(
                                            chatId: chatId,
                                            text: "Ok, da adesso in poi sarò in ascolto",
                                            cancellationToken: cancellationToken);
                            }
                            else if (message.Text.Contains("/spegniti"))
                            {

                                await botClient.SendTextMessageAsync(
                                            chatId: chatId,
                                            text: "Ok, mi sto disattivando...",
                                            cancellationToken: cancellationToken);
                                ShutDown = true;
                            }
                            else
                                return;
                        }
                        else
                        {

                            string response = "";
                            string messageText = message.Text;
                            messageText = messageText + "\n\nDATA E ORA: " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();

                            CronologiaTelegram.Add($"MESSAGGIO DA  {whitelist.Find(x => x.Id.Equals(chatId)).FirstName} TELEGRAM:\n " + messageText);
                            try
                            {
                                response = await vanessacore.SendMessageAsync($"MESSAGGIO DA  {whitelist.Find(x => x.Id.Equals(chatId)).FirstName} TELEGRAM:\n " + messageText);
                                CronologiaTelegram.Add("Vanessa:\n"+response);

                                string realmex = VanessaUtilities.GetMessage(response);
                                char characterToRemove = '\n';
                                string newString = realmex.Replace(characterToRemove.ToString(), "");
                                if (newString.Length <= 1)
                                    realmex = "Lancio comando...";

                                Message sentMessage = await botClient.SendTextMessageAsync(
                                    chatId: chatId,
                                    text: realmex,
                                    cancellationToken: cancellationToken);

                                string comando = VanessaUtilities.GetComand(response);
                                Console.WriteLine("COMANDO: " + comando);

                                if (Getcmd(comando).Equals("RICONOSCIMENTOTELEGRAM"))
                                {
                                    NotifyVisualizer.WriteLine(Getcparams(comando));
                                    NotifyVisualizer.Flush();
                                    WaitVisualizer.ReadLine();

                                    await botClient.SendTextMessageAsync(
                                                chatId: chatId,
                                                text: "Volto imparato (teoricamente)",
                                                cancellationToken: cancellationToken);
                                }
                                else if (!comando.Equals("Nessun comando"))
                                {

                                    int cmd = await VanessaUtilities.ExecuteComand(comando);
                                    switch (cmd)
                                    {

                                        case 2:

                                            response = await vanessacore.SendMessageAsync("SYSTEM:\n" + VanessaUtilities.UTILITY);
                                            CronologiaTelegram.Add(response);
                                            sentMessage = await botClient.SendTextMessageAsync(
                                                        chatId: chatId,
                                                        text: response,
                                                        cancellationToken: cancellationToken);

                                            try
                                            {
                                                await using Stream stream = System.IO.File.OpenRead("YOLO/IMMAGINEPRESAVISIONE.png");
                                                await botClient.SendDocumentAsync(
                                                    chatId: chatId,
                                                    document: InputFile.FromStream(stream: stream, fileName: "IMMAGINEPRESAVISIONE.png"),
                                                    caption: "Risultato presavisioni.");

                                            }
                                            catch (Exception ex)
                                            {

                                                Console.WriteLine(ex.Message);

                                            }


                                            break;
                                        case 3:

                                            string messaggio = VanessaUtilities.UTILITY;

                                            sentMessage = await botClient.SendTextMessageAsync(
                                                    chatId: chatId,
                                                    text: messaggio,
                                                    cancellationToken: cancellationToken);

                                            break;


                                    }

                                }
                            }
                            catch (Exception ex) { Console.WriteLine(ex.Message); }

                        }
                    }
                }

       

            }
        }
        private string Getcmd(string comando)
        {
            ;
            string pattern = @"\[#!(.*?)\((.*?)\)(.*?)\!#]"; ;
            string cmd = "";
            Match match = Regex.Match(comando, pattern);
            if (match.Success)
            {
                cmd = match.Groups[1].Value;

            }

            return cmd;

        }
        private string Getcparams(string comando)
        {

            string pattern = @"\[#!(.*?)\((.*?)\)(.*?)\!#]";
            string parametri = "";
            Match match = Regex.Match(comando, pattern);
            if (match.Success)
            {
                parametri = match.Groups[2].Value;
            }

            return parametri;
        }
        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            botClient.SendTextMessageAsync(
                                               chatId: DavideID,
                                               text: ErrorMessage,
                                               cancellationToken: cancellationToken);

            return Task.CompletedTask;
        }
        public void EliminaContatto(Persona p)
        {
            ListaAggiornata = true;
            Whitelist.Remove(p);
            GestioneFile.WriteXMLRubrica(GestioneFile.PATH,Whitelist);

        }
        async Task NewMessage()
        {
            await Task.Run(() => {

                int len = CronologiaTelegram.Count;

                while (true)
                {

                    if (len != CronologiaTelegram.Count)
                    {
                        IsThereNewMessage = true;
                        len = CronologiaTelegram.Count;
                    }



                    Thread.Sleep(10);  
                }            
            
            });
        }
        public async Task SendMessage()
        {
            long idDestinatario;
            string de;
            Persona destinatario;
            string messaggio;
            VanessaUtilities.MessaggioDaInviare = false;
            await Task.Run(() => {
                while (true)
                {

                    if (VanessaUtilities.MessaggioDaInviare)
                    {
                        VanessaUtilities.MessaggioDaInviare = false;
                        de = VanessaUtilities.Destinatario.ToLower();
                        destinatario = whitelist.Find(x => x.CompleteName.ToLower().Contains(de));      

                        if (destinatario != null)
                        {
                            idDestinatario = destinatario.Id;
                            botClient.SendTextMessageAsync(
                         chatId: idDestinatario,
                         text: VanessaUtilities.Messaggio,
                         cancellationToken: cts.Token);
                            CronologiaTelegram.Add(VanessaUtilities.Messaggio);
                        }


                    }



                    Thread.Sleep(50);
                }
            });
             
                
          
        }
        private static int GetMP3Duration(string filePath)
        {

            var file = TagLib.File.Create(filePath);
            var duration = file.Properties.Duration;
            return duration.Seconds;
        }
        public async Task InviaMessaggioDavide(string mess)
        {
            await botClient.SendTextMessageAsync(
                                                  chatId: DavideID,
                                                  text: mess,
                                                  cancellationToken: cts.Token);
        }
    }
}
