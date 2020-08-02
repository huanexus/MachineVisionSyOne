using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace Vision.SerialSy
{
    public class DoPoint
    {
        internal enum IoOperation { Up, Down }
        public bool Status { get; set; }
        public Subject<int> TrigUp { get; set; }
        public Subject<int> TrigDown { get; set; }
        public int TrigUpCount { get; set; }
        public int TrigDownCount { get; set; }

        internal Subject<IoOperation> TrigEvent { get; set; }

        public SerialSy Sy { get; set; }
        int Port { get; set; }

        public DoPoint(int port = 0)
        {
            Status = false;
            TrigUp = new Subject<int>();
            TrigDown = new Subject<int>();
            TrigUpCount = 0;
            TrigDownCount = 0;
            TrigEvent = new Subject<IoOperation>();
            ProcessAsync();
            Port = port;           
        }


        public bool SetPoint(bool status)
        {
            bool bSuccess = false;
            if (Sy != null)
            {
                bSuccess = Sy.WritePort(Port, status);
            }
            return bSuccess;            
        }

        public bool SetPort()
        {
            return SetPoint(true);
        }

        public bool ResetPort()
        {
            return SetPoint(false);
        }

        private void Process(IoOperation status)
        {
            TrigEvent.OnNext(status);
        }

        public void Process(bool status)
        {
            IoOperation op;
            if (status == true)
            {
                SetPoint(true);
                op = IoOperation.Up;               
            }
              
            else
            {
                SetPoint(false);
                op = IoOperation.Down;
            }
                
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
                    TrigUp.OnNext(TrigUpCount);
                }

                await TrigEvent.Where(i => i == IoOperation.Down).FirstAsync();
                {
                    Status = false;
                    TrigDownCount++;
                    TrigDown.OnNext(TrigDownCount);
                }
            }
        }
    }
}
