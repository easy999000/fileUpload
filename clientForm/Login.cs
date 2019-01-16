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

        tools.net.zTcpClient tcpClient ;
        Form1 form1;

        public Login(tools.net.zTcpClient tcpClient1,Form1 f1)
        {
            InitializeComponent();
            tcpClient = tcpClient1;
            form1 = f1;
        }

        private void Login_Load(object sender, EventArgs e)
        {
            tcpClient.tcpComm.newBleMessageEvent += newBlemessageEventFun;
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
                //this.Close();
                CloseFrom();
            }
            else
            {
                tcpClient.tcpComm.stop();
                MessageBox.Show("账号或密码错误!");
            }

        }
        void denglu(tcpDataCommunication tcpComm, stringMsg msg)
        {
            string value = msg.value["return"];
            MessageBox.Show(value);
        }

        //关闭窗体
        void CloseFrom()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(CloseFrom));
            }
            else
            {
                this.Close();
            }

        }


    }
}
