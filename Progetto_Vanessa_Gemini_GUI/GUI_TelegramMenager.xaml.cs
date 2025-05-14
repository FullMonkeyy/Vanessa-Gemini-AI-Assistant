using Amazon.Runtime.Internal.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Logica di interazione per GUI_TelegramMenager.xaml
    /// </summary>
    /// BISOGNA GENERARE IL CODICE DI CONVALIDAZIONE E LA FINESTRA PER INSERIRE I CAMPI DEL NUOVO UTENTE
    public partial class GUI_TelegramMenager : UserControl
    {
        VanessaGemini spprt;
        public GUI_TelegramMenager(VanessaGemini spprt)
        {
            InitializeComponent();
            this.spprt = spprt;
        }

        private void AggiungiContatto(object sender, RoutedEventArgs e)
        {
            FinestraCreazioneUtenteTelegram fnt = new FinestraCreazioneUtenteTelegram(spprt);
           
            fnt.ShowDialog();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           if((bool)CB_Elimina.IsChecked)
            {
                Persona darimuovere = LV_List.SelectedItem as Persona;
                spprt.Telemen.EliminaContatto(darimuovere);
            }

        }
       async Task AggiornaLista()
        {

            await Task.Run(() =>
            {
                while (true)
                {
                    if (spprt.Telemen.ListaAggiornata)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            spprt.Telemen.ListaAggiornata = false;
                            LV_List.ItemsSource = null;
                            LV_List.ItemsSource = spprt.TLGWhiteList;
                        });
                    }

                    Thread.Sleep(100);


                }
            });
           

        }
        async Task AggiornaCronologia()
        {

            await Task.Run(() =>
            {
                while (true)
                {
                    if (spprt.Telemen.IsThereNewMessage)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            spprt.Telemen.IsThereNewMessage = false;
                            LB_MESSAGES.ItemsSource = null;
                            LB_MESSAGES.ItemsSource = spprt.Telemen.CronologiaTelegram;
                        });
                    }

                    Thread.Sleep(100);


                }
            });


        }
        
        private void Caricata(object sender, RoutedEventArgs e)
        {
            DataContext = spprt;
            LV_List.ItemsSource = spprt.TLGWhiteList;
            LB_MESSAGES.ItemsSource = spprt.Telemen.CronologiaTelegram;
            AggiornaLista();
            AggiornaCronologia();
        }
        private void SalvaRubrica(object sender, RoutedEventArgs e)
        {
            List<Persona> personas = (List<Persona>)LV_List.ItemsSource;

            (DataContext as VanessaGemini).SaveRubrica(personas);

        }
    }
}
