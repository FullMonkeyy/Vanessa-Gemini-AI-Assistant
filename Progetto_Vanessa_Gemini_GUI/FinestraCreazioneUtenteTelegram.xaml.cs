using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Progetto_Vanessa_Gemini_GUI
{
    /// <summary>
    /// Logica di interazione per FinestraCreazioneUtenteTelegram.xaml
    /// </summary>
    public partial class FinestraCreazioneUtenteTelegram : Window
    {
        VanessaGemini vgg;
        CancellationTokenSource cancellationTokenSource;
        public FinestraCreazioneUtenteTelegram(VanessaGemini vg)
        {
            cancellationTokenSource=new CancellationTokenSource();
            InitializeComponent();
            vgg = vg;
        }

        private void Caricato(object sender, RoutedEventArgs e)
        {
            DataContext = new Persona();
            for (int i = 0; i < 4; i++)
                CB_Livello.Items.Add(i.ToString());

          
        }

        private void GeneraCodice(object sender, RoutedEventArgs e)
        {

            cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            LastnameTB.IsEnabled = false;
            FirstnameTB.IsEnabled = false;
            PhoneNumber.IsEnabled = false;
            CB_Livello.IsEnabled = false;
            vgg.Telemen.Convalidando=DataContext as Persona;    
            string caratteri = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            var codice = new string(Enumerable.Repeat(caratteri, 7)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            vgg.Telemen.CodiceConvalidazione = codice;
            CodiceShow.Text = codice;    
            Countdown(cancellationTokenSource.Token);




        }
        private async Task Countdown(CancellationToken ct)
        {
            int i = 60;
            await Task.Run(() => {

                while (i>=0 && !ct.IsCancellationRequested) {

                    Thread.Sleep(1000);
                    i--;

                    Dispatcher.Invoke(() => {

                        LB_COUNT.Content = "Codice valido per 0:" +i.ToString()+" secondi";

                    });
                
                
                }


                Dispatcher.Invoke(() => {

                    LB_COUNT.Content = "Codice scaduto";

                });

                vgg.Telemen.CodiceConvalidazione = "NESSUNA CONVALIDAZIONE";

            });


        }
    }
}
