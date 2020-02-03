using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Begagesorteringssytem.Buffers;
using Begagesorteringssytem.Planes;
using Begagesorteringssytem.Reservations;

namespace Begagesorteringssytem
{
    class FrontDesk
    {
        //used select what buffer it puts the luggage in to 
        private int buffer;
        //used to select a random terminal the luggage will go to
        private static Random rnd;
        //id of the luggage
        private static int id = 0;
        //used to pulse to the thread that it has to open agein
        public object open;
        //used to close the desk if it is true
        public bool closeDesk;

        //constructor
        //that makes it use a random buffer
        public FrontDesk()
        {
            rnd = new Random();
            open = new object();
            buffer = rnd.Next(0, FrontDeskBuffer.FrontDesks.Length);
        }
        //constructor
        //that makes it use a known buffer
        public FrontDesk(int bufferNumber)
        {
            buffer = bufferNumber;
            rnd = new Random(bufferNumber);
            open = new object();
        }

        //
        //starts the frontdesk it can be closed and open, and still run in the method
        //
        public void StartFrontDesk()
        {
            //used to wait if the buffer is full
            bool wait;
            //the luggage
            Luggage luggage = new Luggage();
            while (true)
            {
                //looks if the desk has to close
                if (!closeDesk)
                {
                    //if not "make" a luggage
                    //sets the wait to false as default
                    wait = false;

                    //takes a person and makes the kuggage to for them
                    if (luggage == null)
                    {
                        //locks the person buffer
                        lock (ReservationBuffer.reservationBuffer)
                        {
                            AirplanenTime airplanenTime;
                            Person person = ReservationBuffer.reservationBuffer.GetPerson();
                            int Terminal = 0;
                            bool planeFund = false;
                            //look if the plan has a terminal yet
                            for (int i = 0; i < AirplanenController.GetAirplanenTimes.Count; i++)
                            {
                                airplanenTime = AirplanenController.GetAirplanenTimes[i];
                                if (airplanenTime.Destination == person.Destination && airplanenTime.Landing < DateTime.Now && DateTime.Now < airplanenTime.LeftOff.AddSeconds(-30))
                                {
                                    planeFund = true;
                                    Terminal = airplanenTime.TermainalNumber;
                                }
                            }
                            if (planeFund)
                            {
                                luggage = new Luggage(id++, Terminal);
                                luggage.CheckIn = DateTime.Now;
                                lock (ReservationBuffer.reservationBuffer.full)
                                {
                                    Monitor.Pulse(ReservationBuffer.reservationBuffer.full);
                                }
                            }
                            else
                            {
                                ReservationBuffer.reservationBuffer.AddPerson(person);
                            }

                            lock (Program.writerLock)
                            {
                                Console.SetCursorPosition(0, Thread.CurrentThread.ManagedThreadId);
                                Console.WriteLine("{0,-5}{1,-5} assisting {2} that will go to {3} \t\t", "[" + Thread.CurrentThread.ManagedThreadId + "]", "(FrontDesk)", person.Name, person.Destination);
                                Console.SetCursorPosition(0, 0);
                            }
                        }
                    }
                    //lock the buffer
                    lock (FrontDeskBuffer.FrontDesks[buffer])
                    {
                        //look if it is full 
                        if (!FrontDeskBuffer.FrontDesks[buffer].IsFull && luggage != null)
                        {
                            //if there is room
                            //inform the user
                            //add the luggage
                            FrontDeskBuffer.FrontDesks[buffer].AddLuggage(luggage);
                            //set the luggage to null
                            luggage = null;
                            //pulse to sorter that something is in the buffer
                            lock (FrontDeskBuffer.FrontDesks)
                            {
                                Monitor.Pulse(FrontDeskBuffer.FrontDesks);
                            }
                        }
                        else if (luggage != null)
                        {
                            //if it is full
                            //inform the user

                            lock (Program.writerLock)
                            {
                                Console.SetCursorPosition(0, Thread.CurrentThread.ManagedThreadId);
                                Console.WriteLine("{0,-5}{1,-5} Buffer is full", "[" + Thread.CurrentThread.ManagedThreadId + "]", "(FrontDesk)");
                                Console.SetCursorPosition(0, 0);
                            }
                            //set wait to true
                            wait = true;
                        }
                        //else
                        //{
                        //    //if no terminals are open
                        //    //inform the user
                        //    lock (Program.writerLock)
                        //    {
                        //        Console.SetCursorPosition(0, Thread.CurrentThread.ManagedThreadId);
                        //        Console.WriteLine("{0,-5}{1,-5} No Terminals is open \t\t\t\t\t", "[" + Thread.CurrentThread.ManagedThreadId + "]", "(FrontDesk)");
                        //        Console.SetCursorPosition(0, 0);
                        //    }
                        //}
                    }
                    if (wait)
                    {
                        //if it has to wait lock the full obj in the buffer
                        lock (FrontDeskBuffer.FrontDesks[buffer].full)
                        {
                            //inform the user

                            lock (Program.writerLock)
                            {
                                Console.SetCursorPosition(0, Thread.CurrentThread.ManagedThreadId);
                                Console.WriteLine("{0,-5}{1,-5} I will wait \t\t\t\t\t", "[" + Thread.CurrentThread.ManagedThreadId + "]", "(FrontDesk)");
                                Console.SetCursorPosition(0, 0);
                            }
                            //wait to it gets a pulse
                            Monitor.Wait(FrontDeskBuffer.FrontDesks[buffer].full);
                        }
                    }
                }
                else
                {
                    //if it has to close lock the open obj
                    lock (open)
                    {
                        //infrom the user

                        lock (Program.writerLock)
                        {
                            Console.SetCursorPosition(0, Thread.CurrentThread.ManagedThreadId);
                            Console.WriteLine("{0,-5}{1,-5} Close frontdesk  \t\t\t\t", "[" + Thread.CurrentThread.ManagedThreadId + "]", "(FrontDesk)");
                            Console.SetCursorPosition(0, 0);
                        }
                        //waits to it gets a pulse
                        Monitor.Wait(open);
                    }
                }
                Thread.Sleep(250);
            }
        }
    }
}
