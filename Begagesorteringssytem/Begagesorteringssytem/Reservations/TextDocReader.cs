using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Begagesorteringssytem.Reservations
{
    class TextDocReader
    {
        private Random Random;
        private string filePath = "";
        private List<Person> people;
        public Person GetRandomPerson { get => people[Random.Next(0, people.Count)]; }

        //constructor uses to get the filepath of the document
        public TextDocReader(string filePath)
        {
            this.filePath = filePath;
            people = new List<Person>();
            Random = new Random();
        }

        //
        //replace the list with a new
        //
        public void UpdateList()
        {
            //resets the list
            people = new List<Person>();
            //opens the file
            FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate);
            StreamReader reader = new StreamReader(stream);
            try
            {
                //reads from the file
                while (!reader.EndOfStream)
                {
                    //takes a line
                    string rawInput = reader.ReadLine();
                    //splits it op into name and destination
                    string name = rawInput.Split('|')[0];
                    string destination = rawInput.Split('|')[1];
                    //makes the person
                    people.Add(new Person(name, destination));
                }
            }
            finally
            {
                reader.Close();
                stream.Close();
            }
        }
    }
}
