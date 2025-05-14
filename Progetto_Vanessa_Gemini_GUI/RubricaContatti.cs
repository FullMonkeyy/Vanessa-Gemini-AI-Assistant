using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progetto_Vanessa_Gemini_GUI
{
    public class RubricaContatti
    {
        public List<Persona> People { get; set; }
        public int NPeople { get { return People.Count; } }
        public RubricaContatti()
        {
            People = new List<Persona>();
        }
       
    }
}
