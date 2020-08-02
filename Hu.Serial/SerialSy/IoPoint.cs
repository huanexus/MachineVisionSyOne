using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reactive.Subjects;
using System.Reactive.Linq;

using System.Threading.Tasks.Dataflow;

namespace Vision.SerialSy
{
    public class IoPoint
    {
        internal enum IoOperation { Up, Down }
        public bool Status { get; set; }
        public Subject<int> TrigUp { get; set; }
        public Subject<int> TrigDown { get; set; }
        public int TrigUpCount { get; set; }
        public int TrigDownCount { get; set; }

        public int TrigUpInfo { get; set; }
        public int TrigDownInfo { get; set; }
        internal Subject<IoOperation> TrigEvent { get; set; }
       

        public IoPoint()
        {
            Status = false;
            TrigUp = new Subject<int>();
            TrigDown = new Subject<int>();
            TrigUpCount = 0;
            TrigDownCount = 0;
            TrigUpInfo = -1;
            TrigDownInfo = -1;
            TrigEvent = new Subject<IoOperation>();
            ProcessAsync();                      
        }

       

        public void Reset()
        {
            TrigUpCount = 0;
            TrigDownCount = 0;
            Process(false);            
            
        }

        private void Process(IoOperation status)
        {
            TrigEvent.OnNext(status);
        }

        public void Process(bool status)
        {
            IoOperation op;
            if (status == true)
                op = IoOperation.Up;
            else
                op = IoOperation.Down;
            TrigEvent.OnNext(op);
        }

        public async void ProcessAsync()
        {
            while (true)
            {
                await TrigEvent.Where(i => i == IoOperation.Up).FirstAsync();
                {
                    Status = true;
                    TrigUpCount++;
                    TrigUp.OnNext(TrigUpInfo);                   
                }

                await TrigEvent.Where(i => i == IoOperation.Down).FirstAsync();
                {
                    Status = false;
                    TrigDownCount++;
                    TrigDown.OnNext(TrigUpInfo);                                       
                }
            }
        }
    }
}
