using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hu.MachineVision.SerialSy
{
    public class SignalEventArgs
    {
        public int Signal { get; set; }
        public int OldSignal { get; set; }

        public SignalEventArgs(int signal, int oldSignal)
        {
            Signal = signal;
            OldSignal = oldSignal;
        }
    }
}
