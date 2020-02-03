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
    class Program
    {
        public static object writerLock = new object();
        //the possible terminals that luggage can go to
        public static List<int> possibleTerminals = new List<int>() { 0, 1, 2 };
        //lists with frontdesks
        public static List<FrontDesk> frontDesks = new List<FrontDesk>();
        public static List<Thread> frontDesksThreads = new List<Thread>();

        public static List<Sorter> sorters = new List<Sorter>();
        public static List<Thread> sorterThreads = new List<Thread>();

        public static List<Terminal> terminals = new List<Terminal>();
        public static List<Thread> terminalThreads = new List<Thread>();
        static void Main()
        {

            //makes the buffers
            TerminalBuffers.Terminals = new Buffers.Buffer[] { new Buffers.Buffer(10), new Buffers.Buffer(8), new Buffers.Buffer(6), new Buffers.Buffer(4) };
            FrontDeskBuffer.FrontDesks = new Buffers.Buffer[] { new Buffers.Buffer(8), new Buffers.Buffer(8), new Buffers.Buffer(8) };
            ReservationBuffer.reservationBuffer = new Reservations.Buffer(50);


            //makes frondesk(s)
            frontDesks.Add(new FrontDesk(0));
            frontDesks.Add(new FrontDesk(1));
            frontDesks.Add(new FrontDesk(2));
            frontDesks.Add(new FrontDesk(2));

            //makes the sorter(s)
            sorters.Add(new Sorter());

            //makes the terminal(s)
            terminals.Add(new Terminal(0));
            terminals.Add(new Terminal(1));
            terminals.Add(new Terminal(2));
            terminals.Add(new Terminal(3));

            //starts and makes all the threads
            for (int i = 0; i < frontDesks.Count; i++)
            {
                frontDesks[i].closeDesk = true;
                frontDesksThreads.Add(new Thread(frontDesks[i].StartFrontDesk) { Name = $"Front Desk {i}" });
                frontDesksThreads[i].Start();
            }
            for (int i = 0; i < sorters.Count; i++)
            {
                sorterThreads.Add(new Thread(sorters[i].StartSorting) { Name = $"Sorter {i}" });
                sorterThreads[i].Start();
            }
            for (int i = 0; i < terminals.Count; i++)
            {
                terminalThreads.Add(new Thread(terminals[i].StartTerminal) { Name = $"terminal {i}" });
                terminalThreads[i].Start();
                //closes therminals as default
                terminals[i].OpenCloseTerminal();
                possibleTerminals.Remove(i);
            }

            //makes ad starts the controller that controlls all the airplanens
            AirplanenController airplanenController = new AirplanenController();
            Thread ControllerThread = new Thread(airplanenController.Start)
            {
                Name = "airplanen Controller",
                Priority = ThreadPriority.AboveNormal
            };
            ControllerThread.Start();

            //makes the reservation that takes the information from a text document and makes personens from it
            Reservation reservation = new Reservation();
            Thread reservationThread = new Thread(reservation.Start)
            {
                Name = "reservation",
            };
            reservationThread.Start();
            
            //make the frondesk controller that controlls all the frontdesk
            //opens and closes all the desks after how many is in the "queue"
            FrontDeskController frontDeskController = new FrontDeskController();
            Thread frontDeskControllerThread = new Thread(frontDeskController.Start)
            {
                Name = "frontDesk Controller",
            };
            frontDeskControllerThread.Start();

            //waits for a enter to close the program and the threads
            Console.ReadLine();
            //aborts all threads 
            for (int i = 0; i < frontDesksThreads.Count; i++)
            {
                frontDesksThreads[i].Abort();
            }
            for (int i = 0; i < sorterThreads.Count; i++)
            {
                sorterThreads[i].Abort();
            }
            for (int i = 0; i < terminalThreads.Count; i++)
            {
                terminalThreads[i].Abort();
            }
            ControllerThread.Abort();
            reservationThread.Abort();
            frontDeskControllerThread.Abort();
        }
    }
}
