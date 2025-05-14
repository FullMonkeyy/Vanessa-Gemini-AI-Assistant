using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Progetto_Vanessa_Gemini_GUI
{
    public static class STT
    {
        //CLASSE NON UTILIZZATA
        private static SemaphoreSlim semaphore;
        static public void LanciaSTTDEPRECATO()
        {
            Process process = new Process();
            process.StartInfo.FileName = "python.exe"; // Sostituisci con il percorso effettivo dell'interprete Python
            process.StartInfo.Arguments = "main.py"; // Sostituisci con il nome del tuo script Python
            process.StartInfo.WorkingDirectory = @"STT"; // Sostituisci con il percorso della directory contenente lo script
            semaphore = new SemaphoreSlim(1, 1);
            // Avvia il processo e attendi che termini
            process.Start();
            process.WaitForExit();
        }


        static public string ReadFile()
        {

            string path = @"STT\test.txt";
            StreamReader sr = new StreamReader(path);
            string testo = "Nulla";

            try
            {
                testo = sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception ex)
            {
                testo = ex.ToString();
            }

            return testo;



        }
        static public async Task<string> RichiediTrascrizione(HttpClient pc)
        {


            HttpClient PythonClient = pc;
            string result;

            try
            {
                Console.WriteLine("Vanessa ti sta ascoltando: ");
                var response = await PythonClient.GetAsync("http://localhost:5000/get_value");
                string text = await response.Content.ReadAsStringAsync();
                result = text;

            }
            catch (Exception e)
            {
                Console.WriteLine("Errore di comunicazione con il centro API");
                result = e.ToString();
            }

            return result;

        }
        static public string STTRequest()
        {


            try
            {
                File.WriteAllText("STT/signal.txt", "proceed");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Wait for Python to finish its loop body
            FileInfo fi = new FileInfo("STT/finished.txt");
            while (!fi.Exists)
            {
                fi = new FileInfo("STT/finished.txt");
                // Wait a short time to avoid busy waiting
                Thread.Sleep(100);
            }
            // Python has finished, clear the finished signal
            File.Delete("STT/finished.txt");

            // Perform any actions after Python finishes (optional)

            // Console.WriteLine("Python loop body executed!");

            return ReadFile();

        }
        static public bool TrascrizioneCompletata()
        {
            List<string> lines = new List<string>();
            string path = @"STT\ComunicationManager.txt";
            try
            {
                lines.AddRange(File.ReadAllLines(path, Encoding.UTF32).ToList());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Non è stato possibile capire se la trascrizione è stata completata");
                lines.Add(ex.ToString());
            }

            return lines[0].Equals("IN ATTESA");

        }
    }
}
