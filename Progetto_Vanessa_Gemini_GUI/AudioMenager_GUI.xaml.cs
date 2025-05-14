using AngleSharp.Text;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Progetto_Vanessa_Gemini_GUI
{
    /// <summary>
    /// Interaction logic for AudioMenager_GUI.xaml
    /// </summary>
    public partial class AudioMenager_GUI : UserControl
    {
        VanessaGemini Vanessa;
        bool pause;
        CancellationTokenSource cancellationToken;
        public AudioMenager_GUI(VanessaGemini vg)
        {
            Vanessa= vg;
            pause = false;
            cancellationToken=new CancellationTokenSource();
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = Vanessa;
            LV_Music.ItemsSource = Vanessa.CachedSongs;
            UpdateMusicList(cancellationToken.Token);
        }

        private void PlaySong(object sender, RoutedEventArgs e)
        {

            if (!(DataContext as VanessaGemini).StartSongRequest())
            {
                string path = @"YTVideo\" + ((Button)sender).Tag;
                (DataContext as VanessaGemini).PlaySongRequest(path);
            }
          
        }
        
        private void PauseSong(object sender, RoutedEventArgs e)
        {  
            (DataContext as VanessaGemini).PauseSongRequest();
            pause = true;
        }
        
        private void RiavviaSong(object sender, MouseButtonEventArgs e)
        {
            string path = @"YTVideo\" + ((Button)sender).Tag;
            (DataContext as VanessaGemini).PlaySongRequest(path);
        }
        private void CambiaNomeClick(object sender, RoutedEventArgs e)
        {
            NuovoTitolo_GUI win = new NuovoTitolo_GUI(Vanessa, ((Button)sender).Tag.ToString());
            win.ShowDialog();
        }
        private async Task UpdateMusicList(CancellationToken ct)
        {

            await Task.Run(() => {

                while (!ct.IsCancellationRequested)
                {

                    if (Vanessa.MusicaAggiornata)
                    {
                        Dispatcher.Invoke(() =>
                        {

                            LV_Music.ItemsSource=null;
                            LV_Music.ItemsSource=Vanessa.CachedSongs;
                            Vanessa.MusicaAggiornata = false;

                        });

                    }


                }
            
            });
        }
    }
}
