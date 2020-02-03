using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Begagesorteringssytem.Buffers;

namespace Begagesorteringssytem
{
    class FrontDeskController
    {
        //
        //opens and closes the frontdesks so the "right" amount is open. More people more frontdesks is open
        //
        public void Start()
        {
            while (true)
            {
                //runs through all the frontdesks
                for (int i = 0; i < Program.frontDesks.Count; i++)
                {
                    //locks the reservation Buffer
                    lock (ReservationBuffer.reservationBuffer)
                    {
                        //looks if the buffer times the index of the frontdesk have to be more then the max length of the buffer before it opens
                        if (ReservationBuffer.reservationBuffer.Reserved * (i+1)+2 > ReservationBuffer.reservationBuffer.People.Length)
                        {
                            //opens the frontdesk
                            Program.frontDesks[i].closeDesk = false;
                            lock (Program.frontDesks[i].open)
                            {
                                Monitor.Pulse(Program.frontDesks[i].open);
                            }
                        }
                        else
                        {
                            //closes the frontdesk
                            Program.frontDesks[i].closeDesk = true;
                        }
                    }
                }
                //waits for a update from the reservation buffer
                lock (ReservationBuffer.reservationBuffer)
                {
                    Monitor.Wait(ReservationBuffer.reservationBuffer);
                }
            }
        }
    }
}
