using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Begagesorteringssytem
{
    class Luggage
    {
        //a personal ID for the luggage
        public int ID { get; private set; }
        //what terminal the luggage have to go to
        public int Tarminal { get; private set; }
        //the destination the luggage will go to
        public string Destination { get; set; }
        //the checkin time
        public DateTime CheckIn { get; set; }
        //the sorting time
        public DateTime Sorted { get; set; }
        //the bording time
        public DateTime CheckOut { get; set; }

        //constructor for a empty luggage
        public Luggage()
        {

        }
        //constructor for a normal luggage
        public Luggage (int id, int tarminal)
        {
            ID = id;
            Tarminal = tarminal;
        }
    }
}
