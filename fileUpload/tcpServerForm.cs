using BLE;
using DB.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using tools.net;


namespace fileUpload
{
    public partial class tcpServerForm : Form
    {

        UserService user = new UserService();

        public tcpServerForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        tools.net.tcpServer tcpServerListener;

        private void button1_Click_1(object sender, EventArgs e)
        {
            System.Net.IPAddress ip;
            bool b = System.Net.IPAddress.TryParse(textBox2.Text, out ip);
            if (!b)
            {
                MessageBox.Show("ip格式不正确");
                return;
            }
            int port;
            b = int.TryParse(textBox3.Text,out port);

            tcpServerListener = new tools.net.tcpServer(ip, port);

            tcpServerListener.connControl.newMessageEvent += newBlemessageEventFun;

            tcpServerListener.start();


        }
        /// <summary>
        /// 消息接收事件处理程序
        /// </summary>
        /// <param name="tcpComm"></param>
        /// <param name="msg"></param>
        void newBlemessageEventFun(tcpDataCommunication tcpComm, stringMsg msg)
        {
            switch (msg.name)
            {
                case msgEnum.liaotian:
                    liaotian(tcpComm, msg );
                    break;
                case msgEnum.fileUpload:
                    showMsg(string.Format("收到文件",msg.value["value"]));
                    break;
                case msgEnum.dengru:
                    denglu(tcpComm, msg);
                    break;
                default:
                    break;
            }

        }

        void liaotian(tcpDataCommunication tcpComm, stringMsg msg)
        {
            showMsg(tcpComm.tcpClient1.Client.RemoteEndPoint.ToString()+"---"+msg.value["value"]);

        }
        void denglu(tcpDataCommunication tcpComm, stringMsg msg)
        {
            string account = msg.value["account"];
            string pwd = msg.value["pwd"];
            bool bl = user.Login(account, pwd);

            BLE.stringMsg m1 = new BLE.stringMsg();
            m1.name = BLE.msgEnum.dengru;
            m1.value.Add("return", bl.ToString());



            tcpComm.sendData(m1);
        }
        void showMsg(string msg)
        {
            if (this.richTextBox2.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(showMsg),msg);
            }
            else
            {
                this.richTextBox2.Text +=string.Format("[{0}]: ",DateTime.Now.ToString())+ msg+ "\r\n\r\n";
            }
            
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }
    }
}
