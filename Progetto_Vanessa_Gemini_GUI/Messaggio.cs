using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progetto_Vanessa_Gemini_GUI
{
    public class Messaggio
    {
        public string Message { get; set; }
        public string Role { get; set; }
        public Messaggio() { }
        public Messaggio(string m, string r)
        {

            Message = m;
            Role = r;


        }
        public override string ToString()
        {

            return "ROLE: " + Role + "\n" + Message;

        }

    }
}
