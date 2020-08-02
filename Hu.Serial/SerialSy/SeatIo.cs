using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Threading.Tasks.Dataflow;

namespace Vision.SerialSy
{
    public class SeatIo
    {
        public static int Data { get; set; }
        public static Dictionary<int, int>Seats { get; set; }
        public static Dictionary<int, int>[] SignalInfo { get; set; }
        public static event EventHandler<SignalEventArgs> SeatChanged;

        static SeatIo()
        {
            Data = 0;
            Seats = new Dictionary<int, int>();
            SignalInfo = new Dictionary<int, int>[2];
            SignalInfo[0] = new Dictionary<int, int>()
            {    
                {0, 8},
                {1, 4},
                {2, 2},
                {3, 6},
                {4, 1},
                {5, 5},
                {6, 3},
                {7, 7},               
            };

            for (int i = 0; i < 2; i++)
            {
                Seats[i] = 0;
            }
        }

 
        public void OnSeatChanged()
        {
            if (SeatChanged != null)
            {
                SeatChanged(this, null);
            }
        }

        public int Seat
        {
            get
            {
                return Data;
            }
            set
            {
                if (Data != value)
                {
                    Data = value;
                    Seats[0] = SignalInfo[0][value];
                    Seats[1] = 0;
                    OnSeatChanged();   
                }
            }
        }
    }
}
