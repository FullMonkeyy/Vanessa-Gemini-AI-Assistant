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
    /// Logica di interazione per VanessaGUI.xaml
    /// </summary>
    public partial class VanessaGUI : UserControl
    {
        VanessaGemini vg;
        public VanessaGUI(VanessaGemini vg)
        {
            InitializeComponent();
            this.vg = vg;
        }

        private void TelegramClick(object sender, RoutedEventArgs e)
        {
            User.Children.Clear();
            User.Children.Add(new GUI_TelegramMenager(vg));

        }

        private void MusicClick(object sender, RoutedEventArgs e)
        {
            User.Children.Clear();
            User.Children.Add(new AudioMenager_GUI(vg));

        }

        private void VanessaClick(object sender, RoutedEventArgs e)
        {
            User.Children.Clear();
            User.Children.Add(new VanessaBrainGUI(vg));
    
        }

        private void HistoryClick(object sender, RoutedEventArgs e)
        {
            User.Children.Clear();
            User.Children.Add(new Conversationhystory_GUI(vg));
          
        }
    }
}
