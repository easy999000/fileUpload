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
using DB.Services;
using fileUpload;
using System.Diagnostics;

namespace clientForm
{
    public partial class Login : Form
    {

        tools.net.zTcpClient tcpClient;
        Form1 form1;
        UserService user = new UserService();
        public Login(tools.net.zTcpClient tcpClient1, Form1 f1)
        {
            InitializeComponent();
            tcpClient = tcpClient1;
            form1 = f1;
        }

        private void Login_Load(object sender, EventArgs e)
        {
            //tcpClient.tcpComm.newBleMessageEvent += newBlemessageEventFun;
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


        //void newBlemessageEventFun(tcpDataCommunication tcpComm, BLEData ble)
        //{
        //    //string jsonText = ((BLE.bleClass.t11)ble).msg;
        //    //JObject jo = (JObject)JsonConvert.DeserializeObject(jsonText);

        //    BLE.bleClass.t11 t11 = (BLE.bleClass.t11)ble;
        //    stringMsg msg = stringMsg.jsonToModel(t11.msg);


        //    switch (msg.name)
        //    {
        //        case msgEnum.dengru:
        //            //bool bl = Convert.ToBoolean(jo["value"]["return"]);
        //            bool bl = Convert.ToBoolean(msg.value["return"]);
        //            if (bl)
        //            {
        //                Stopwatch st = new Stopwatch();
        //                st.Start();

        //                RetUser curr = user.Login(textBox1.Text, textBox2.Text);
        //                st.Stop();


        //                Stopwatch st1 = new Stopwatch();
        //                st1.Start();
        //                ShowFile(form1.listView1, curr.User.ID);
        //                st1.Stop();

        //                MessageBox.Show(string.Format("登陆成功!{0},{1}", st.ElapsedMilliseconds.ToString(), st1.ElapsedMilliseconds.ToString()));

        //                CloseFrom();

        //            }
        //            else
        //            {
        //                tcpClient.tcpComm.stop();
        //                MessageBox.Show("账号或密码错误!");
        //            }
        //            break;
        //        case msgEnum.liaotian:
        //            //string groupSendingMsg = jo["value"]["groupSending"].ToString();
        //            string groupSendingMsg = msg.value["groupSending"];
        //            showForm1Msg(form1.richTextBox1, groupSendingMsg);
        //            break;
        //        default:
        //            break;
        //    }



        //}
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


        void ShowFile(ListView listView1, int userId)
        {
            if (listView1.InvokeRequired)
            {
                this.BeginInvoke(new Action<ListView, int>(ShowFile), listView1, userId);
            }
            else
            {
                listView1.Items.Clear();
                List<DB.FileInfo> list = user.GetFileListByUserId(userId);
                foreach (DB.FileInfo item in list)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = item.FileName;
                    lvi.Name = item.FilePath;
                    listView1.Items.Add(lvi);
                }
            }
        }

        void showForm1Msg(RichTextBox richTextBox1, string msg)
        {
            if (richTextBox1.InvokeRequired)
            {
                this.BeginInvoke(new Action<RichTextBox, string>(showForm1Msg), richTextBox1, msg);
            }
            else
            {
                richTextBox1.Text += string.Format("[{0}]: ", DateTime.Now.ToString()) + msg + "\r\n\r\n";
            }
        }

    }
}
