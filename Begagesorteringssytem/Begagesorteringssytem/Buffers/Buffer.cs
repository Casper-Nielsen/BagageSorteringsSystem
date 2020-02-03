using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Begagesorteringssytem.Buffers
{
    //
    //Buffer that run on first in, last out
    //

    class Buffer
    {
        //the buffer itself
        private Luggage[] luggages;
        private int reserved;
        public bool IsFull { get => (reserved == luggages.Length); }
        public Luggage[] Luggages { get => luggages; }
        public int Reserved { get => reserved; }

        //used for wait
        public object empty;
        public object full;

        //constructor
        public Buffer(int size)
        {
            reserved = 0;
            luggages = new Luggage[size];
            empty = new object();
            full = new object();
        }

        //
        // takes a bottle from the buffer
        //
        public Luggage GetLuggage()
        {
            //looks if there are something in the buffer
            if (reserved > 0)
            {
                //takes one from the buffer
                reserved--;
                Luggage temp = luggages[reserved];
                luggages[reserved] = null;
                return temp;
            }
            return null;
        }

        //
        // puts a bottle to the buffer
        //
        public void AddLuggage(Luggage luggage)
        {
            //looks if there are room in the buffer
            if (reserved != luggages.Length)
            {
                //adds one to the buffer
                luggages[reserved] = luggage;
                reserved++;
            }
        }
    }
}

