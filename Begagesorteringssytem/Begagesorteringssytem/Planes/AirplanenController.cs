using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Begagesorteringssytem.Buffers;

namespace Begagesorteringssytem.Planes
{
    class AirplanenController
    {
        // the plan for the airplanen times
        private static List<AirplanenTime> airplanenTimes;
        public static List<AirplanenTime> GetAirplanenTimes { get => airplanenTimes; }

        public AirplanenController()
        {
            airplanenTimes = new List<AirplanenTime>();
            AddFixTimes();
        }

        //
        // Adds 21 times that Plans land and takeoff
        //
        public void AddFixTimes()
        {
            Random random = new Random();
            //when will the terminal be free
            int[] addedMinutesOnTerminal = new int[Program.terminals.Count];
            for (int i = 0; i < 21; i++)
            {
                //picks a random terminal
                int Terminal = random.Next(0, Program.terminals.Count);
                //a random time between the time the terminal will be free and 42 minuttes
                int addedStartTime = random.Next(addedMinutesOnTerminal[Terminal], addedMinutesOnTerminal[Terminal] + 2);
                //a random time between the time the plane lands and 42 minuttes.. yes the plan can land and takeoff the same min
                int addedendTime = random.Next(addedStartTime + 2, addedStartTime + 5);
                addedMinutesOnTerminal[Terminal] = addedendTime;
                //picks a random destination
                string destination = "";
                switch (random.Next(0, 14))
                {
                    case 0:
                        destination = "England";
                        break;
                    case 2:
                        destination = "USA";
                        break;
                    case 3:
                        destination = "Bahamas";
                        break;
                    case 4:
                        destination = "Germany";
                        break;
                    case 5:
                        destination = "Belgium";
                        break;
                    case 6:
                        destination = "Canada";
                        break;
                    case 7:
                        destination = "Netherlands";
                        break;
                    case 8:
                        destination = "Chile";
                        break;
                    case 9:
                        destination = "Turkey";
                        break;
                    case 10:
                        destination = "Norway";
                        break;
                    case 11:
                        destination = "Israel";
                        break;
                    case 12:
                        destination = "Sweden";
                        break;
                    case 13:
                        destination = "Wales";
                        break;
                    default:
                        break;
                }
                //addes the infromation tothe airplanen time plan
                airplanenTimes.Add(new AirplanenTime(DateTime.Now.AddMinutes(addedStartTime), DateTime.Now.AddMinutes(addedendTime), Terminal, destination));
            }
        }

        //
        // runs the airplan times closes and opens the terminals
        //
        public void Start()
        {
            airplanenTimes = airplanenTimes.OrderBy(x => x.Landing).ToList();
            while (true)
            {
                DateTime now = DateTime.Now;
                //runs all the airplanentimes and looks if a plan has landed or a airplan is taking off
                for (int i = 0; i < airplanenTimes.Count; i++)
                {
                    if (airplanenTimes[i].Landing < now && airplanenTimes[i].LeftOff.AddSeconds(-30) > now)
                    {
                        //starts the terminal agien
                        if (Program.terminals[airplanenTimes[i].TermainalNumber].Close)
                        {
                            //opens the terminal
                            Program.terminals[airplanenTimes[i].TermainalNumber].OpenCloseTerminal();
                            //sets the destination
                            Program.terminals[airplanenTimes[i].TermainalNumber].Destination = airplanenTimes[i].Destination;
                            //starts the terminal agein
                            lock (Program.terminals[airplanenTimes[i].TermainalNumber].openPulse)
                            {
                                Monitor.PulseAll(Program.terminals[airplanenTimes[i].TermainalNumber].openPulse);
                            }
                            //adds the terminal to the possibleterminal so the frontdesk know that the terminal is ready
                            Program.possibleTerminals.Add(airplanenTimes[i].TermainalNumber);
                        }
                    }
                    else if (airplanenTimes[i].LeftOff.AddSeconds(-30) < now && airplanenTimes[i].LeftOff < now)
                    {
                        //closes the terminal
                        if (!Program.terminals[airplanenTimes[airplanenTimes[i].TermainalNumber].TermainalNumber].Close)
                        {
                            //closes the terminal 
                            Program.terminals[airplanenTimes[i].TermainalNumber].OpenCloseTerminal();
                            //removes so the frontdesk cant se it anymore
                            Program.possibleTerminals.Remove(airplanenTimes[i].TermainalNumber);
                            //gets it to run ones more time
                            lock (TerminalBuffers.Terminals[airplanenTimes[i].TermainalNumber].empty)
                            {
                                Monitor.Pulse(TerminalBuffers.Terminals[airplanenTimes[i].TermainalNumber].empty);
                            }
                        }
                    }
                }
                //sleeps a 1 sec becourse it dont need to update super fast
                Thread.Sleep(1000);
            }
        }
    }
}
