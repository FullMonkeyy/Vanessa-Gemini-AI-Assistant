using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using NAudio.Wave;

namespace Progetto_Vanessa_Gemini_GUI
{
    public static class TTS
    {
        static WaveOutEvent outputDevice = new WaveOutEvent();
        public static WaveOutEvent wa { get { return outputDevice; } }
        public static async Task Main(string nomefile, string testo)
        {
            string outputFileName = nomefile;
            string text = testo;
            //YOU HAVE TO PUT YOUR AMAZON POLLY API CREDENTIALS HERE
            var client = new AmazonPollyClient(, , RegionEndpoint.USEast1);
            var response = await PollySynthesizeSpeech(client, text);

            WriteSpeechToStream(response.AudioStream, outputFileName);
        }

        private static async Task<SynthesizeSpeechResponse> PollySynthesizeSpeech(IAmazonPolly client, string text)
        {
            var synthesizeSpeechRequest = new SynthesizeSpeechRequest()
            {
                OutputFormat = OutputFormat.Mp3,
                VoiceId = VoiceId.Bianca,
                TextType = TextType.Ssml,
                Text = "<speak><prosody rate='110%'>" + text + "</prosody></speak>",
                Engine = "neural"

            };

            var synthesizeSpeechResponse =
                await client.SynthesizeSpeechAsync(synthesizeSpeechRequest);

            return synthesizeSpeechResponse;
        }

        /// <summary>
        /// Writes the AudioStream returned from the call to
        /// SynthesizeSpeechAsync to a file in MP3 format.
        /// </summary>
        /// <param name="audioStream">The AudioStream returned from the
        /// call to the SynthesizeSpeechAsync method.</param>
        /// <param name="outputFileName">The full path to the file in which to
        /// save the audio stream.</param>
        private static void WriteSpeechToStream(Stream audioStream, string outputFileName)
        {
            try
            {
                var outputStream = new FileStream(
                    outputFileName,
                    FileMode.Create,
                    FileAccess.Write);
                byte[] buffer = new byte[2 * 1024];
                int readBytes;

                while ((readBytes = audioStream.Read(buffer, 0, 2 * 1024)) > 0)
                {
                    outputStream.Write(buffer, 0, readBytes);
                }

                // Flushes the buffer to avoid losing the last second or so of
                // the synthesized text.
                outputStream.Flush();
                Console.WriteLine($"Saved {outputFileName} to disk.");
                outputStream.Close();
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Non ho potuto salvare output.mp3 IO");
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);

            }
        }
        static public async Task PlayVoice()
        {
            await Task.Run(() =>
            {


                using (var audioFile = new AudioFileReader("VANESSA_VOICE\\Output.mp3"))
                {

                    wa.Init(audioFile);
                    wa.Play();
                    while (wa.PlaybackState == PlaybackState.Playing)
                    {
                    }
                    wa.Stop();

                }
            });
        }


    }
}
