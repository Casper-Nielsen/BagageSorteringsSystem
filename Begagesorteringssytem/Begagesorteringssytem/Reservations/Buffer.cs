using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Begagesorteringssytem.Reservations
{
    //
    //Buffer that run on first in, first out
    //

    class Buffer
    {
        //the buffer itself
        private Person[] people;
        private int reserved;
        public bool IsFull { get => (reserved == people.Length); }
        public Person[] People { get => people; }
        public int Reserved { get => reserved; }

        //used for wait
        public object empty;
        public object full;

        //constructor
        public Buffer(int size)
        {
            reserved = 0;
            people = new Person[size];
            empty = new object();
            full = new object();
        }

        //
        // takes a bottle from the buffer
        //
        public Person GetPerson()
        {
            //looks if there are something in the buffer
            if (people[0] != null)
            {
                //takes one from the buffer
                Person temp = people[0];
                //moves all one down
                for (int i = 0; i < people.Length-1; i++)
                {
                    people[i] = people[i + 1];
                }
                people[people.Length-1] = null;
                reserved--;
                return temp;
            }
            return null;
        }

        //
        // puts a bottle to the buffer
        //
        public void AddPerson(Person person)
        {
            //looks if there are room in the buffer
            if (reserved != people.Length)
            {
                //adds one to the buffer
                people[reserved] = person;
                reserved++;
            }
        }
    }
}

