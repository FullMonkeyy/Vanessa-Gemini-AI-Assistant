using ChatAIze.GenerativeCS.Enums;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Progetto_Vanessa_Gemini_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CancellationTokenSource cancellationTokenSource;
        VanessaGemini VG;
        bool avviato;
        bool terminato;
        public MainWindow()
        {
            InitializeComponent();
            VG=new VanessaGemini();
            avviato = false;
            terminato = false;
            cancellationTokenSource = new CancellationTokenSource();
        }

        private void AvviaVanessa(object sender, RoutedEventArgs e)
        {
            if (!avviato)
            {
                avviato = true;
                VG.VanessaGeminiStart();
                Bt_avvio.Content = "Avvio in corso...";
                AspettaInizializzazione();
            }
        }
        private async Task AspettaInizializzazione()
        {
            await Task.Run(() =>
            {
              
                Caricamento(cancellationTokenSource.Token);
                while (!VG.Inizializzata)
                    Thread.Sleep(100);
                cancellationTokenSource.Cancel();
                Dispatcher.Invoke(() =>
                {
                    grid.Children.Clear();
                    grid.Children.Add(new VanessaGUI(VG));
                });
                AttendiChiusura();
            });
            
        }

        private void Chiusura(object sender, System.ComponentModel.CancelEventArgs e)
        {
            VG.VanessaStop();
        }
        private async Task AttendiChiusura()
        {
            await Task.Run(() =>
            {
                while (VG.ProcessoAttivo)
                {
                    Thread.Sleep(100);
                }
                cancellationTokenSource.Cancel();
                Dispatcher.Invoke(() =>
                {
                    this.Close();
                });

            });

        }
        private async Task Caricamento(CancellationToken ct)
        {
            await Task.Run(() => {
                int i = -1;
                while (!ct.IsCancellationRequested)
                {
                    i++;
                    try
                    {
                        Dispatcher.Invoke(() =>
                        {

                            switch (i)
                            {

                                case 0:
                                    Bt_avvio.Content = "Avvio in corso";
                                    break;
                                case 1:
                                    Bt_avvio.Content = "Avvio in corso.";
                                    break;
                                case 2:
                                    Bt_avvio.Content = "Avvio in corso..";
                                    break;
                                case 3:
                                    Bt_avvio.Content = "Avvio in corso...";
                                    break;
                                case 4:
                                    i = -1;
                                    break;

                            }




                        });
                    }
                    catch { //Boh, sarà un problema del runtime ahahah
                            }   
                    Thread.Sleep(500);
                }
          

            });
        }
    }
}