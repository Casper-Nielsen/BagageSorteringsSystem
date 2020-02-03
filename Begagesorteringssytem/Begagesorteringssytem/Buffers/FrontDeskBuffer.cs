using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Begagesorteringssytem.Buffers
{
    /// <summary>
    /// holds all the buffer for the frontdesks
    /// every frontdesk dont need a buffer they can share
    /// uses the buffer first in, last out
    /// </summary>
    class FrontDeskBuffer
    {
        public static Buffer[] FrontDesks = new Buffer[] {
        };
    }
}
