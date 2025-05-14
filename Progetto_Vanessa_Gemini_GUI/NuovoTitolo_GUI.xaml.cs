using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using Telegram.Bot.Types;

namespace Progetto_Vanessa_Gemini_GUI
{
    /// <summary>
    /// Interaction logic for NuovoTitolo_GUI.xaml
    /// </summary>
    public partial class NuovoTitolo_GUI : Window
    {
        VanessaGemini vanessa;
        string filename;
        string filenamesource;
        public NuovoTitolo_GUI(VanessaGemini vg,string fl)
        {
            vanessa = vg;
            filenamesource = fl;
            filename =System.IO.Path.GetFileNameWithoutExtension(fl);
            InitializeComponent();
        }

        private void Salva(object sender, RoutedEventArgs e)
        {
            string NuovoNove = TB_text.Text;
            vanessa.CambiaCanzone(filenamesource, NuovoNove);
        
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TB_text.Text = filename;
        }

        private void Esci(object sender, RoutedEventArgs e)
        {
           this.Close();   
        }
    }
}
