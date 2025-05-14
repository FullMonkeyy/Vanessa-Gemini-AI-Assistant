using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progetto_Vanessa_Gemini_GUI
{
    public class Persona
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompleteName { get {  return (FirstName+" "+LastName).ToLower(); } }   
        public long Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public string Level {  get; set; }

        public Persona() { }
        public Persona(string firstName, string lastName, long id)
        {

            FirstName = firstName;
            LastName = lastName;
            Id = id;
            PhoneNumber = "";
        }
        public Persona(string firstName, string lastName)
        {

            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = "";
        }
        public override string ToString()
        {
            return FirstName + " " + LastName + " " + Id + " " + PhoneNumber;
        }

    }
}
