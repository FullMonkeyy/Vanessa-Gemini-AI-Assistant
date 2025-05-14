using System;
using System.Collections.Generic;
using System.Dynamic;
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

namespace Progetto_Vanessa_Gemini_GUI
{
    /// <summary>
    /// Interaction logic for ModifierMessage_GUI.xaml
    /// </summary>
    public partial class ModifierMessage_GUI : Window
    {
        dynamic VG;
        dynamic vgtmp;
        public ModifierMessage_GUI(dynamic vc)
        {
            InitializeComponent();
            vgtmp= new ExpandoObject();
            VG = vc;
            vgtmp.A = vc.A;
            vgtmp.B = vc.B;
        }

        private void modiferLoadesd(object sender, RoutedEventArgs e)
        {
            DataContext = vgtmp;
        }

        private void SalvaModifiche(object sender, RoutedEventArgs e)
        {
            VG.A=vgtmp.A;
            VG.B=vgtmp.B;
        }
    }
}
