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
    /// Interaction logic for Conversationhystory_GUI.xaml
    /// </summary>
    public partial class Conversationhystory_GUI : UserControl
    {
        VanessaGemini vg;
        public Conversationhystory_GUI(VanessaGemini vg)
        {
            InitializeComponent();
            this.vg = vg;
        }

        private void Caricata(object sender, RoutedEventArgs e)
        {
            LSB_CHRONO.ItemsSource = vg.data;
        }

        private void RichiestaModifica(object sender, MouseButtonEventArgs e)
        {

            dynamic obj = LSB_CHRONO.SelectedItem;


            ModifierMessage_GUI fnt = new ModifierMessage_GUI(obj);

            fnt.ShowDialog();
        }

        private void SalvaModificheConversation(object sender, RoutedEventArgs e)
        {
            vg.COnversationCoreUpdate((List<dynamic>)LSB_CHRONO.ItemsSource);
        }
    }
}
