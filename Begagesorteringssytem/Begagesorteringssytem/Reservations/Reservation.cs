using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Begagesorteringssytem.Buffers;

namespace Begagesorteringssytem.Reservations
{
    class Reservation
    {
        TextDocReader docReader;
        Random random;
        int passengerNumeber = 0;

        public Reservation()
        {
            random = new Random();
            docReader = new TextDocReader(@".\PersonList.txt");
            docReader.UpdateList();
        }

        //
        //main method for reservation of persons
        //takes a person and puts it into the buffer
        //
        public void Start()
        {
            Person person;
            bool wait;
            while (true)
            {
                wait = false;
                lock (ReservationBuffer.reservationBuffer)
                {
                    //looks if the buffer is full
                    if (!ReservationBuffer.reservationBuffer.IsFull)
                    {
                        //makes a new passenger and adds it to the buffer
                        person = MakePerson();
                        ReservationBuffer.reservationBuffer.AddPerson(person);

                        //inform the user
                        //locks the writer obj
                        lock (Program.writerLock)
                        {
                            Console.SetCursorPosition(0, Thread.CurrentThread.ManagedThreadId);
                            Console.WriteLine("{0,-5}{1,-11}  {2} wanna go to {3}\t\t", "[" + Thread.CurrentThread.ManagedThreadId + "]", "(Reservation)",person.Name, person.Destination);
                            Console.SetCursorPosition(0, 0);
                        }
                        //pulse that there have been added a new passenger
                        Monitor.Pulse(ReservationBuffer.reservationBuffer);
                    }
                    else
                    {
                        wait = true;
                    }
                }
                if (wait)
                {
                    //waits while the buffer is full
                    lock (ReservationBuffer.reservationBuffer.full)
                    {
                        Monitor.Wait(ReservationBuffer.reservationBuffer.full);
                    }
                }
            }
        }

        //
        //makes a person with a name number and the destination
        //
        private Person MakePerson()
        {
            //makes a new test person with the destination
            Person personFromDoc = docReader.GetRandomPerson;
            return new Person(personFromDoc.Name, passengerNumeber++, personFromDoc.Destination );
        }
    }
}
