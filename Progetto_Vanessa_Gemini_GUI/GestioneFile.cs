using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Progetto_Vanessa_Gemini_GUI
{
    public static class GestioneFile
    {
        static public string PATH= "RubricaPersona.xml";
        static public string Read(string path)
        {
            string text = "NIENTE";
            try
            {
                StreamReader sr = new StreamReader(path);
                text = sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception ex)
            {

            }
            return text;

        }
        static public bool Write(string path, string text)
        {

            try
            {
                StreamWriter sw = new StreamWriter(@path, false);
                sw.Write(text);
                sw.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }


        }
        static public void WriteXMLConversation(string path, List<Messaggio> lm)
        {

            try
            {

                StreamWriter sw = new StreamWriter(@path, false);
                XmlSerializer xmls = new XmlSerializer(typeof(List<Messaggio>));
                xmls.Serialize(sw, lm);
                sw.Close();

            }
            catch (IOException e)
            {

                Console.WriteLine("Non sono riuscito a salvare la cronologia della conversazione");

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);

            }


        }
        static public List<Messaggio> ReadXMLConversation(string path)
        {

            List<Messaggio> tmp = new List<Messaggio>();

            try
            {

                StreamReader sw = new StreamReader(@path);
                XmlSerializer xmls = new XmlSerializer(typeof(List<Messaggio>));
                tmp.AddRange((List<Messaggio>)xmls.Deserialize(sw));
                sw.Close();
                for(int i=0; i < tmp.Count; i++)
                {
                    tmp[i].Message = tmp[i].Message.Trim(' ', '\n');
                }
            
            }
            catch (IOException e)
            {

                Console.WriteLine("Non sono riuscito a caricare la cronologia della conversazione");

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);

            }

            return tmp;

        }
        static public void WriteXMLRubrica(string path, List<Persona> rb)
        {

            try
            {

                StreamWriter sw = new StreamWriter(@path, false);
                XmlSerializer xmls = new XmlSerializer(typeof(List<Persona>));
                xmls.Serialize(sw, rb);
                sw.Close();

            }
            catch (IOException e)
            {

                Console.WriteLine("Non sono riuscito a salvare la rubrica");

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);

            }


        }
        static public List<Persona> ReadXMLRubrica(string path)
        {

            List<Persona> tmp = new List<Persona>();

            try
            {

                StreamReader sw = new StreamReader(@path);
                XmlSerializer xmls = new XmlSerializer(typeof(List<Persona>));
                tmp = (List<Persona>)xmls.Deserialize(sw);
                sw.Close();

            }
            catch (IOException e)
            {

                Console.WriteLine("Non sono riuscito a caricare la rubrica");

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);

            }
    

            return tmp;

        }
    }
}
