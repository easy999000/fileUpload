using BLE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using tools.net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace clientForm
{
    public partial class Login : Form
    {
        // tools.net.tcpClient tcpClient1 = new tools.net.tcpClient();

        tools.net.tcpClient tcpClient = new tools.net.tcpClient();
        //Form1 ff = new Form1();
        //Control button5 = new Control();
        public Login(tools.net.tcpClient tcpClient1, Form1 form1)
        {
            InitializeComponent();
            tcpClient = tcpClient1;
            //ff = form1;
            //button5 = form1.button5;
            //button5.Enabled = false;
       

        }

        //登陆
        private void button1_Click(object sender, EventArgs e)
        {
            string account = textBox1.Text;
            string password = textBox2.Text;
            stringMsg sm = new stringMsg();
            sm.name = msgEnum.dengru;
            sm.value.Add("account", account);
            sm.value.Add("pwd", password);
            //Form1.tcpClient1.tcpComm.sendData(sm);
            tcpClient.tcpComm.sendData(sm);
            tcpClient.tcpComm.newBleMessageEvent += newBlemessageEventFun;
            //Form1.tcpClient1.tcpComm.newBleMessageEvent += newBlemessageEventFun;
        }


        void newBlemessageEventFun(tcpDataCommunication tcpComm, BLEData ble)
        {
            string jsonText = ((BLE.bleClass.t11)ble).msg;
            JObject jo = (JObject)JsonConvert.DeserializeObject(jsonText);
            bool bl = Convert.ToBoolean(jo["value"]["return"]);
            if (bl)
            {
                MessageBox.Show("登陆成功!");
                // ff.Controls["button5"].Enabled = false;
                //button5.Enabled = true;
            }
            else
            {
                MessageBox.Show("账号或密码错误!");
            }
        }
        void denglu(tcpDataCommunication tcpComm, stringMsg msg)
        {
            string value = msg.value["return"];

            MessageBox.Show(value);

        }
    }
}
