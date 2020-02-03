using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Begagesorteringssytem.Buffers;

namespace Begagesorteringssytem
{
    class Terminal
    {
        //used to save the index of the buffer
        private int buffer;
        //used to close the terminal
        private bool close;
        //used to count how many is on the plane
        private int count;

        //saves the destination
        public string Destination { get; set; }
        public bool Close { get => close; }
        //used to start the thread agein
        public object openPulse = new object();

        //constructor
        public Terminal(int bufferNumber)
        {
            buffer = bufferNumber;
            Destination = "";
            count = 0;
        }

        //
        //the main method that runs everything the tarminal is doing
        //takes from the buffer and use the info
        //
        public void StartTerminal()
        {
            Luggage luggage;
            bool wait;
            while (true)
            {
                wait = false;
                //lock the buffer for the terminal 
                lock (TerminalBuffers.Terminals[buffer])
                {
                    //looks if the terminal have any info in it
                    if (TerminalBuffers.Terminals[buffer].Reserved != 0)
                    {
                        //counts the total luggage on the plane
                        count++;
                        //takes the luggage
                        luggage = TerminalBuffers.Terminals[buffer].GetLuggage();
                        //adds a check out time to the luggage
                        luggage.CheckOut = DateTime.Now;

                        //inform the user
                        //locks the writer obj
                        lock (Program.writerLock)
                        {
                            Console.SetCursorPosition(0, Thread.CurrentThread.ManagedThreadId);
                            Console.WriteLine("{0,-5}{1,-11} takes the luggage \t{2}", "[" + Thread.CurrentThread.ManagedThreadId + "]", "(Terminal)", count);
                            Console.SetCursorPosition(0, 0);
                        }
                        //pulse to the sorter that there is room in the buffer
                        lock (TerminalBuffers.Terminals[buffer].full)
                        {
                            Monitor.Pulse(TerminalBuffers.Terminals[buffer].full);
                        }
                    }
                    else
                    {
                        //if the buffer is empty wait to there is some info in it
                        wait = true;
                    }
                }
                //if the thread have to wait
                if (wait)
                {
                    //looks if the terminal have to close
                    if (close)
                    {
                        //if it need to close wait on the openpulse obj
                        lock (openPulse)
                        {
                            //resets the total count of luggage
                            count = 0;
                            //inform the user 
                            //locking the writer obj
                            lock (Program.writerLock)
                            {
                                Console.SetCursorPosition(0, Thread.CurrentThread.ManagedThreadId);
                                Console.WriteLine("{0,-5}{1,-11} Closed \t\t\t\t\t\t", "[" + Thread.CurrentThread.ManagedThreadId + "]", "(Terminal)");
                                Console.SetCursorPosition(0, 0);
                            }
                            //wait 
                            Monitor.Wait(openPulse);
                        }
                    }
                    else
                    {
                        //if the terminal just have to wait
                        //wait on the buffers empty obj
                        lock (TerminalBuffers.Terminals[buffer].empty)
                        {
                            //inform the user
                            //locking the writer lock
                            lock (Program.writerLock)
                            {
                                Console.SetCursorPosition(0, Thread.CurrentThread.ManagedThreadId);
                                Console.WriteLine("{0,-5}{1,-11} Waits \t{2,-20} {3}", "[" + Thread.CurrentThread.ManagedThreadId + "]", "(Terminal)", Destination, count);
                                Console.SetCursorPosition(0, 0);
                            }
                            //waits
                            Monitor.Wait(TerminalBuffers.Terminals[buffer].empty);
                        }
                    }
                }
            }
        }

        //
        //method use to open or close the terminal 
        //
        public void OpenCloseTerminal()
        {
            close = !close;
        }
    }
}
