using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Begagesorteringssytem.Planes
{
    class AirplanenTime
    {
        private DateTime landing;
        private DateTime leftOff;
        private int terminalNumber;
        private string destination;

        public DateTime Landing { get => landing; }
        public DateTime LeftOff { get => leftOff; }
        public int TermainalNumber { get => terminalNumber; }
        public string Destination { get => destination; }

        //constructer
        public AirplanenTime(DateTime landing, DateTime leftOff, int terminalNumber, string destination)
        {
            this.landing = landing;
            this.leftOff = leftOff;
            this.terminalNumber = terminalNumber;
            this.destination = destination;
        }
    }
}
