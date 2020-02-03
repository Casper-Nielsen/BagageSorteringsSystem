using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Begagesorteringssytem.Buffers;

namespace Begagesorteringssytem
{
    class Sorter
    {
        //
        // starts the sorter
        // sorts all the luggages and puts them in a buffer
        //
        public void StartSorting()
        {
            Luggage luggage = null;
            bool wait;
            while (true)
            {
                //counts so if the all the frontdesk buffer is empty then wait to get a pulse
                byte count = 0;
                //runs through every frontdesk buffer
                for (int i = 0; i < FrontDeskBuffer.FrontDesks.Length; i++)
                {
                    wait = false;
                    //looks if it has a luggage
                    if (luggage == null)
                    {
                        //locks a buffer
                        lock (FrontDeskBuffer.FrontDesks[i])
                        {
                            //looks if the buffer is full
                            if (FrontDeskBuffer.FrontDesks[i].Reserved != 0)
                            {
                                //if not take a luggage from a buffer

                                //infrom the user
                                //by locking the writer obj
                                lock (Program.writerLock)
                                {
                                    Console.SetCursorPosition(0, Thread.CurrentThread.ManagedThreadId);
                                    Console.WriteLine("{0,-5}{1,-11} takes a luggage from frontdeskbuffer {2}", "[" + Thread.CurrentThread.ManagedThreadId + "]", "(Sorter)", i);
                                    Console.SetCursorPosition(0, 0);
                                }
                                //take the luggage
                                luggage = FrontDeskBuffer.FrontDesks[i].GetLuggage();
                                //pulse to the frontdesk that it is not full any more
                                lock (FrontDeskBuffer.FrontDesks[i].full)
                                {
                                    Monitor.Pulse(FrontDeskBuffer.FrontDesks[i].full);
                                }
                            }
                            else
                            {
                                //if the buffer is empty count one up
                                count++;
                            }
                        }
                    }
                    //looks if it have a luggage
                    if (luggage != null)
                    {
                        //lock the terminal buffer the luggage is going 
                        lock (TerminalBuffers.Terminals[luggage.Tarminal])
                        {
                            //looks if the buffer is full
                            if (!TerminalBuffers.Terminals[luggage.Tarminal].IsFull)
                            {
                                //if not inform the user 
                                //by locking the writer obj
                                lock (Program.writerLock)
                                {
                                    Console.SetCursorPosition(0, Thread.CurrentThread.ManagedThreadId);
                                    Console.WriteLine("{0,-5}{1,-11} Adds the luggage to {2}", "[" + Thread.CurrentThread.ManagedThreadId + "]", "(Sorter)", luggage.Tarminal);
                                    Console.SetCursorPosition(0, 0);
                                }
                                //adds the time the luggage got sorted
                                luggage.Sorted = DateTime.Now;
                                //insert the luggage to the terminal buffer
                                TerminalBuffers.Terminals[luggage.Tarminal].AddLuggage(luggage);
                                //informs the terminal that it is not empty any more
                                lock (TerminalBuffers.Terminals[luggage.Tarminal].empty)
                                {
                                    Monitor.Pulse(TerminalBuffers.Terminals[luggage.Tarminal].empty);
                                }
                                //removes the luggage
                                luggage = null;
                            }
                            else
                            {
                                //if the buffer is full set wait to true
                                wait = true;
                            }
                        }
                        //if it has to wait before it can insert the luggage
                        if (wait)
                        {
                            //wait to the terminal has pulse that the buffer is not full anymore
                            lock (TerminalBuffers.Terminals[luggage.Tarminal].full)
                            {
                                Monitor.Wait(TerminalBuffers.Terminals[luggage.Tarminal].full);
                            }
                        }
                    }
                }
                //if all three buffer is empty
                if (count == FrontDeskBuffer.FrontDesks.Length)
                {
                    //wait to get a pulse from one of the frontdesk
                    lock (FrontDeskBuffer.FrontDesks)
                    {
                        //inform the user 
                        //by locking the writer obj
                        lock (Program.writerLock)
                        {
                            Console.SetCursorPosition(0, Thread.CurrentThread.ManagedThreadId);
                            Console.WriteLine("{0,-5}{1,-11} I will wait \t\t\t\t\t\t", "[" + Thread.CurrentThread.ManagedThreadId + "]", "(Sorter)");
                            Console.SetCursorPosition(0, 0);
                        }
                        Monitor.Wait(FrontDeskBuffer.FrontDesks);
                    }
                }
            }
        }
    }
}
