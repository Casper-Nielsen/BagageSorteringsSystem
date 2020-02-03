using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Begagesorteringssytem.Buffers
{

    /// <summary>
    /// holds all the buffer for the terminals
    /// every terminal needs it own buffer
    /// uses the buffer first in, last out
    /// </summary>
    class TerminalBuffers
    {
        public static Buffer[] Terminals = new Buffer[] { };
    }
}
