using BLE;
using DB;
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

namespace clientForm
{
    public partial class Form1 : Form
    {

        UserService user = new UserService();
        public Form1()
        {
            InitializeComponent();
        }

        tools.net.zTcpClient zTcpClient1 = new tools.net.zTcpClient();


        private void button2_Click(object sender, EventArgs e)
        {
            System.Net.IPAddress ip;
            bool b = System.Net.IPAddress.TryParse(textBox1.Text, out ip);
            if (!b)
            {
                MessageBox.Show("ip格式不正确");
                return;
            }
            int port;
            b = int.TryParse(textBox2.Text, out port);


            zTcpClient1.Connect(new System.Net.IPEndPoint(ip, port));


            Login login = new clientForm.Login(zTcpClient1, this);
            login.Show();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            BLE.stringMsg m1 = new BLE.stringMsg();
            m1.name = BLE.msgEnum.liaotian;
            m1.value.Add("value", this.richTextBox2.Text);

            zTcpClient1.tcpComm.sendData(m1);


        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                BLE.bleClass.t12 t2 = new BLE.bleClass.t12();
                t2.sendFileFullPath = openFileDialog1.FileName;

                string fileName = System.IO.Path.GetFileName(t2.sendFileFullPath);


                ConfigInfo config = user.GetSave();
                string reviced = System.IO.Path.Combine(config.Path + "\\" + CurrUser.currUser.ID, fileName);//d:\\
     
                stringMsg sm = new stringMsg();
                sm.name = msgEnum.fileUpload;
                sm.value.Add("value", reviced);

                t2.ReceiveFullMsg = sm.modelToJson();

               t2.toBleStream(zTcpClient1.tcpComm.sendDataGetStream());

                //zTcpClient1.tcpComm.addSendBle(t2);

            }
        }
        //停止连接
        private void button1_Click(object sender, EventArgs e)
        {
            zTcpClient1.tcpComm.stop();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                string folder = this.listView1.SelectedItems[0].SubItems[0].Text.ToString().Trim();
                string filepath = this.listView1.SelectedItems[0].SubItems[0].Name.ToString();
                if (filepath.Length > 0)
                {
                    //选中文件 下载
                    string msg = "确定要下载 " + folder + " 吗？";
                    if ((int)MessageBox.Show(msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == 1)
                    {
                        //下载
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //下载
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //刷新列表
            ResetList();
        }

        private void ResetList()
        {
            this.listView1.Items.Clear();
            List<DB.FileInfo> list = user.GetFileListByUserId(CurrUser.currUser.ID);
            foreach (DB.FileInfo item in list)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = item.FileName;
                lvi.Name = item.FilePath;
                listView1.Items.Add(lvi);
            }
        }
    }
}
