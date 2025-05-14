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
    /// Logica di interazione per VanessaBrainGUI.xaml
    /// </summary>
    public partial class VanessaBrainGUI : UserControl
    {
        VanessaGemini vg;
        bool mute;
        public VanessaBrainGUI(VanessaGemini vg)
        {
            InitializeComponent();
            this.vg = vg;
            mute = false;
        }

        private void BrainGUICaricato(object sender, RoutedEventArgs e)
        {
            TB_User.Text = vg.PrompText;
            TB_Vanessa.Text = vg.PromptResponse;
            update();
        }

        private async Task update()
        {
            string tmp1 = vg.PrompText;
            string tmp2 = vg.PromptResponse;
            await Task.Run(() => { 
            
                while (true)
                {

                    if(!tmp1.Equals(vg.PrompText) || !tmp2.Equals(vg.PromptResponse))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            TB_User.Text = vg.PrompText;
                            tmp1 = vg.PrompText;
                            TB_Vanessa.Text = vg.PromptResponse;
                            tmp2= vg.PromptResponse;
                        });
                    }

                    Thread.Sleep(10);  

                }
            
            });
        }

        private void Bt_avvio_Click(object sender, RoutedEventArgs e)
        {
            if (mute)
            {
                VanessaUtilities.OCCUPATA = true;
                mute= false;
                Bt_avvio.Content = "MUTE";
            }
            else
            {
                VanessaUtilities.OCCUPATA = false;
                Bt_avvio.Content = "ASCOLTA";
                mute = true;
            }
        }
    }
}
