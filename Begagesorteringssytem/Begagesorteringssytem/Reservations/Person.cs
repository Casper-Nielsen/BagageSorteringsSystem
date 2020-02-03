using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Begagesorteringssytem.Reservations
{
    class Person
    {
        private string name;
        private int passengerNumber;
        private string destination;

        public string Name { get => name; }
        public int PassengerNumber { get => passengerNumber; }
        public string Destination { get => destination; }

        //constructor with the information that the frontdesk need
        public Person (string name, int passengerNumber, string destination)
        {
            this.name = name;
            this.passengerNumber = passengerNumber;
            this.destination = destination;
        }
        //constructor that the doc reader is setup to read from a text document
        public Person (string name, string destination)
        {
            this.name = name;
            this.destination = destination;
        }

    }
}
