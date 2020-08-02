using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;

using System.Windows.Forms;
using System.Drawing;

using System.Threading.Tasks.Dataflow;


namespace Vision.SerialSy
{
    public class VirtualIoUi
    {
        public Panel UiPanel { get; set; }
        public Panel MyTabPage { get; set; }
        public int DoPort { get; set; }
        public int DiPort { get; set; }
        public int DiCount { get; set; }
        public int DoCount { get; set; }

        public int Channel { get; set; }
        public int CcdId { get; set; }
        public VirtualIo Io { get; set; }
        public SerialSy Sy { get { return SerialSy.GetDevice(Channel); } }

        private GroupBox gboxMain = new GroupBox();
        private GroupBox gboxDi = new GroupBox();
        private GroupBox gboxDo = new GroupBox();

        private ComboBox cmbSy = new ComboBox();
        private NumericUpDown nudDoPort = new NumericUpDown();
        private Button btnConnect = new Button();
        private Button btnDisconnect = new Button();
        private Button btnOn = new Button();
        private Button btnOff = new Button();
        private Button btnRefesh = new Button();

        private Button btnOk = new Button();
        private Button btnNg = new Button();
        private Button btnDone = new Button();

        private CheckBox[] chkDiBoxes;
        private CheckBox[] chkDoBoxes;
        private TextBox txtDi = new TextBox();
        private TextBox txtDo = new TextBox();
        private int[] Data { get; set; }
        public ActionBlock<int> DiBlock { get; set; }
        public ActionBlock<int> DoBlock { get; set; }
        public Dictionary<string, Button> Buttons { get; set; }

        public int Di
        {
            get
            {
                return Data[0];
            }
            set
            {
                if (Data[0] != value)
                {
                    Data[0] = value;
                    DiBlock.Post(value);
                }
            }
        }

        public int Do
        {
            get
            {
                return Data[1];
            }
            set
            {
                if (Data[1] != value)
                {
                    Data[1] = value;
                    DoBlock.Post(value);
                }
            }
        }

        public VirtualIoUi(VirtualIo vIo)
        {
            Io = vIo;
            Channel = vIo.Channel;
            CcdId = vIo.Id;

            Io.DiChanged += (s, e) => Di = Io.Di;
            Io.DoChanged += (s, e) => Do = Io.Do;
            DiCount = Io.DiPortCount;
            DoCount = Io.DoPortCount;
            UiPanel = new Panel();
            UiPanel.Width = 640;
            UiPanel.Height = 360;
            gboxMain = new GroupBox();
            gboxMain.Text = "串口IO通信模块";
            gboxMain.Location = new Point(20, 20);
            gboxMain.Width = UiPanel.Width - 40;
            gboxMain.Height = UiPanel.Height - 40;
            UiPanel.Controls.Add(gboxMain);
            AddMainUI(gboxMain);
            Data = new int[2];
            DiBlock = new ActionBlock<int>(x => UiShowDi(x));
            DoBlock = new ActionBlock<int>(x => UiShowDo(x));
            Buttons = new Dictionary<string, Button>();
            Buttons["Connect"] = btnConnect;
            Buttons["Disconnect"] = btnDisconnect;
            Buttons["On"] = btnOn;
            Buttons["Off"] = btnOff;
            Buttons["Refresh"] = btnRefesh;
            Buttons["Ok"] = btnOk;
            Buttons["Ng"] = btnNg;
            Buttons["Done"] = btnDone;

            foreach (var btn in Buttons.Values)
            {
                btn.Enabled = false;
            }

            Buttons["Connect"].Click += (s, e) => ConnectDevice();
            Buttons["Disconnect"].Click += (s, e) => DisconnectDevice();
            Buttons["Connect"].Enabled = true;
            Buttons["Disconnect"].Enabled = false;

            Buttons["Refresh"].Enabled = true;
            Buttons["Refresh"].Click += (s, e) => RefreshIo();

            Buttons["On"].Enabled = true;
            Buttons["Off"].Enabled = true;
            Buttons["On"].Click += (s, e) => SetOn();
            Buttons["Off"].Click += (s, e) => SetOff();

            Buttons["Ok"].Enabled = true;
            Buttons["Ng"].Enabled = true;
            Buttons["Done"].Enabled = true;

            Buttons["Ok"].Click += (s, e) => SetOk();
            Buttons["Ng"].Click += (s, e) => SetNg();
            Buttons["Done"].Click += (s, e) => SetDone();

            ToolTip myTip = new ToolTip();
            myTip.SetToolTip(Buttons["Ok"], Io.DoPorts["Ok"].ToString());
            myTip.SetToolTip(Buttons["Ng"], Io.DoPorts["Ng"].ToString());
            myTip.SetToolTip(Buttons["Done"], Io.DoPorts["Done"].ToString());

            ConnectDevice();
        }

        private void SetDone()
        {
            Io.SetDone();
        }

        private void SetNg()
        {
            Io.SetNg();
        }

        private void SetOk()
        {
            Io.SetOk();
        }

        private void SetOff()
        {            
            Io.ResetPort(DoPort);
            if(Sy != null)
            {
                Sy.WritePort(DoPort, false);
            }
        }

        private void SetOn()
        {
            Io.SetPort(DoPort);
            if(Io.Sy != null)
            {
                Sy.WritePort(DoPort, true);
            }
        }

        private void RefreshIo()
        {
            if (Sy != null)
            {
                Io.GetDiStatus();
                Io.GetDoStatus();
                ShowDo(Di);
                ShowDo(Do);
            }
        }

        private void DisconnectDevice()
        {
            bool status = Sy.Disconnect();
            if (status)
            {
                Buttons["Connect"].Enabled = true;
                Buttons["Disconnect"].Enabled = false;
                ChangeDioBackColor(Color.White);
            }
        }

        private void ConnectDevice()
        {
            if (Sy != null)
            {
                bool status = Sy.Connect();
                if (status)
                {
                    ChangeDioBackColor(Color.Green);
                    Buttons["Disconnect"].Enabled = true;
                    Buttons["Connect"].Enabled = false;
                }
                else
                {
                    ChangeDioBackColor(Color.Red);
                }
            }
        }

        public int SelectDevice(int index = 0)
        {
            if (index < cmbSy.Items.Count)
            {
                cmbSy.SelectedIndex = index;
            }
            else
            {
                cmbSy.SelectedIndex = -1;
            }
            return cmbSy.SelectedIndex;
        }

        private void ShowDo(int x)
        {
            BitArray bits = new BitArray(new byte[] { (byte)x });
            for (int i = 0; i < DiCount; i++)
            {
                chkDoBoxes[i].Checked = bits[i];
            }
            txtDo.Text = Convert.ToString(Data[1], 2).PadLeft(6, '0');
        }

        private void ShowDi(int x)
        {
            BitArray bits = new BitArray(new byte[] { (byte)x });
            for (int i = 0; i < DiCount; i++)
            {
                chkDiBoxes[i].Checked = bits[i];
            }
            txtDi.Text = Convert.ToString(Data[0], 2).PadLeft(6, '0');
        }

        private void UiShowDo(int x)
        {

            if (txtDo.InvokeRequired)
            {
                txtDo.Invoke(new Action(() => ShowDo(x)));
            }
            else
            {
                ShowDo(x);
            }
        }

        private void UiShowDi(int x)
        {

            if (txtDi.InvokeRequired)
            {
                txtDi.Invoke(new Action(() => ShowDi(x)));
            }
            else
            {
                ShowDi(x);
            }
        }

        public void ChangeDioBackColor(Color color)
        {
            foreach (var cbox in chkDiBoxes)
            {
                cbox.BackColor = color;
            }

            foreach (var cbox in chkDoBoxes)
            {
                cbox.BackColor = color;
            }
        }

        public void Associate(TabPage tp)
        {
            MyTabPage = tp;
            MyTabPage.Controls.Add(UiPanel);
        }

        private void AddMainUI(GroupBox gbox, int marginLeft = 20, int marginTop = 30, int marginRight = 20, int marginBottom = 20)
        {
            int left = marginLeft;
            int top = marginTop;
            Label lblSerial = new Label();
            lblSerial.Text = "模块型号: SY-COM-6DI-0.5A";
            lblSerial.Width = 160;
            lblSerial.Location = new Point(left, top);
            gbox.Controls.Add(lblSerial);



            left = lblSerial.Right + 50;
            Label lblPort = new Label();
            lblPort.Text = "端口:";
            lblPort.Width = 40;
            lblPort.Location = new Point(left, top);
            gbox.Controls.Add(lblPort);

            left = lblPort.Right;
            cmbSy.Location = new Point(left, top - 5);
            cmbSy.Width = 80;
            gbox.Controls.Add(cmbSy);

            if (Sy != null)
            {
                cmbSy.Items.Add(Sy.PortName);
                cmbSy.SelectedIndex = 0;
            }

            //for (int i = 0; i < SerialSy.DeviceNum; i++)
            //{
            //    cmbSy.Items.Add(Sy.PortName);
            //}


            left = cmbSy.Right + 30;
            btnConnect.Text = "连接";
            btnConnect.Location = new Point(left, top - 5);
            gbox.Controls.Add(btnConnect);

            left = btnConnect.Right + 30;
            btnDisconnect.Text = "断开";
            btnDisconnect.Location = new Point(left, top - 5);
            gbox.Controls.Add(btnDisconnect);

            left = marginLeft;
            top = top + 30;
            int width = btnDisconnect.Right - left;
            int height = 80;

            gboxDi.Text = "输入信号";
            gboxDi.Tag = "输入";
            gboxDi.Location = new Point(left, top);
            gboxDi.Width = width;
            gboxDi.Height = height;
            gbox.Controls.Add(gboxDi);

            top = gboxDi.Bottom + 10;
            gboxDo.Text = "输出信号";
            gboxDo.Tag = "输出";
            gboxDo.Width = width;
            gboxDo.Height = height;
            gboxDo.Location = new Point(left, top);
            gbox.Controls.Add(gboxDo);

            top = gboxDo.Bottom + 30;
            Label lblDo = new Label();
            lblDo.Text = "DO:";
            lblDo.Width = 30;
            lblDo.Location = new Point(left, top);
            gbox.Controls.Add(lblDo);

            left = lblDo.Right;
            nudDoPort.Location = new Point(left, top - 5);
            nudDoPort.Value = 0;
            nudDoPort.Minimum = 0;
            nudDoPort.Maximum = 5;
            nudDoPort.Width = 60;
            gbox.Controls.Add(nudDoPort);

            nudDoPort.ValueChanged += (s, e) => DoPort = Decimal.ToInt32((s as NumericUpDown).Value);

            left = nudDoPort.Right + 20;
            btnOn.Text = "开";
            btnOn.Width = 30;
            btnOn.Location = new Point(left, top - 5);
            gbox.Controls.Add(btnOn);

            left = btnOn.Right + 20;
            btnOff.Text = "关";
            btnOff.Width = 30;
            btnOff.Location = new Point(left, top - 5);
            gbox.Controls.Add(btnOff);

            left = btnOff.Right + 20;
            btnRefesh.Text = "刷新";
            btnRefesh.Width = 50;
            btnRefesh.Location = new Point(left, top - 5);
            gbox.Controls.Add(btnRefesh);

            string[] btnNames = { "OK", "NG", "DONE" };
            string[] btnTexts = { "良品", "次品", "拍照完成" };
            int[] btnWidths = { 40, 40, 80 };
            int btnCount = btnNames.Count();
            Button[] btnList = { btnOk, btnNg, btnDone };
            left = width - 60;

            for (int i = 0; i < btnCount; i++)
            {
                int btnIndex = btnCount - i - 1;
                var btn = btnList[btnIndex];

                btn.Width = btnWidths[btnIndex];
                if (i > 0)
                {
                    left = btnList[btnIndex + 1].Left - btn.Width - 10;
                }

                btn.Location = new Point(left, top - 5);
                btn.Name = btnNames[btnIndex];
                btn.Tag = btnIndex;
                btn.Text = btnTexts[btnIndex];
                gbox.Controls.Add(btn);
            }

            chkDiBoxes = new CheckBox[DiCount];
            chkDoBoxes = new CheckBox[DoCount];

            for (int i = 0; i < DiCount; i++)
            {
                chkDiBoxes[i] = new CheckBox();
            }

            for (int i = 0; i < DoCount; i++)
            {
                chkDoBoxes[i] = new CheckBox();
            }

            txtDi = new TextBox();
            txtDo = new TextBox();
            AddDioUI(gboxDi, chkDiBoxes, txtDi);
            AddDioUI(gboxDo, chkDoBoxes, txtDo);
            txtDi.Text = Convert.ToString(0, 2).PadLeft(6, '0');
            txtDo.Text = Convert.ToString(0, 2).PadLeft(6, '0');
        }

        private void AddDioUI(GroupBox gbox, CheckBox[] boxes, TextBox tbox)
        {
            int num = 6;
            int left = 20;
            int top = 30;
            int width = 58;
            int boxIndex = 0;

            for (int i = 0; i < num; i++)
            {
                boxes[i].Tag = i;
            }

            for (int i = 0; i < num; i++)
            {
                boxIndex = num - i - 1;
                var box = boxes[boxIndex];
                box.Width = width;
                if (i > 0)
                {
                    left = boxes[boxIndex + 1].Right;
                }
                box.Text = string.Format("{0}{1}", (string)gbox.Tag, boxIndex);
                box.Location = new Point(left, top);
                gbox.Controls.Add(box);
            }

            Label myLabel = new Label();
            myLabel.Location = new Point(boxes.First().Right + 5, top + 5);
            myLabel.Width = 60;
            myLabel.Text = "当前信号:";
            gbox.Controls.Add(myLabel);

            tbox.Location = new Point(myLabel.Right, top);
            tbox.Width = 75;
            gbox.Controls.Add(tbox);
        }
    }
}
