using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Begagesorteringssytem.Buffers
{
    /// <summary>
    /// holds the buffer for the reservation
    /// uses the first in, first out
    /// </summary>
    class ReservationBuffer
    {
        public static Reservations.Buffer reservationBuffer = new Reservations.Buffer(30);
    }
}
