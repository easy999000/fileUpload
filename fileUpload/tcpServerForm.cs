using BLE;
using DB;
using DB.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        private string path;
        public tcpServerForm()
        {
            InitializeComponent();
            ConfigInfo config = user.GetSave();
            path = config.Path;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        tools.net.zTcpServer tcpServerListener;

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
            b = int.TryParse(textBox3.Text, out port);

            tcpServerListener = new tools.net.zTcpServer(ip, port);
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
                    liaotian(tcpComm, msg);
                    break;
                case msgEnum.fileUpload:
                    showMsg(string.Format("收到文件{0}", msg.value["FileName"]));
                    bool bl = AddFileInfo(msg);
                    break;
                case msgEnum.dengru:
                    denglu(tcpComm, msg);
                    break;
                default:
                    break;
            }

        }

        private bool AddFileInfo(stringMsg msg)
        {
            int userId = Convert.ToInt32(msg.value["UserId"]);
            string filePath = msg.value["value"];
            string firstFloor = msg.value["FirstFloor"];
            string fileName = msg.value["FileName"];
            bool bl = user.AddFileInfo(userId, filePath, firstFloor, fileName);
            if (bl)
            {
                user.Add_Log_Opera(userId, "", string.Format("上传文件{0}", fileName));
            }
            else
            {
                user.Add_Log_Error(userId, "", string.Format("上传文件{0}出现问题", fileName));
            }
            return bl;
        }

        void liaotian(tcpDataCommunication tcpComm, stringMsg msg)
        {
            showMsg(tcpComm.tcpClient1.Client.RemoteEndPoint.ToString() + "---" + msg.value["value"]);

        }
        void denglu(tcpDataCommunication tcpComm, stringMsg msg)
        {
            string address = tcpComm.tcpClientId.Split('&')[2];
            string account = msg.value["account"];
            string pwd = msg.value["pwd"];
            RetUser curr = user.Login(account, pwd);
            if (curr.Success)
            {
                if (Common.tcpList == null)
                {
                    Common.tcpList = new List<TCP>();
                }
                Common.tcpList.Add(new TCP() { ID = curr.User.ID, Name = curr.User.Account, Address = address });
                BindDataGridView(Common.tcpList);
            }

            BLE.stringMsg m1 = new BLE.stringMsg();
            m1.name = BLE.msgEnum.dengru;
            m1.value.Add("return", curr.Success.ToString());

            tcpComm.sendData(m1);
        }
        void BindDataGridView(List<TCP> tcp)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<List<TCP>>(BindDataGridView), tcp);
            }
            else
            {
                this.dataGridView1.DataSource = tcp;
            }

        }
        void showMsg(string msg)
        {
            if (this.richTextBox2.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(showMsg), msg);
            }
            else
            {
                this.richTextBox2.Text += string.Format("[{0}]: ", DateTime.Now.ToString()) + msg + "\r\n\r\n";
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            seting set = new seting();
            set.Show();
        }

        private void tcpServerForm_Load(object sender, EventArgs e)
        {
            ShowFolder();
        }


        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                string folder = this.listView1.SelectedItems[0].SubItems[0].Text.ToString().Trim();
                string fullPath = (path + "\\" + folder).Replace(System.Environment.NewLine, string.Empty);
                string filepath = this.listView1.SelectedItems[0].SubItems[0].Name.ToString();

                if (fullPath.Contains("..."))
                {
                    //返回上一级
                    ShowFolder();
                }
                else if (user.folderList().Contains(folder))
                {
                    //显示文件
                    ShowFile(folder);
                }
                else if (filepath.Length > 0)
                {
                    //选中文件 下载
                    //string msg = "确定要下载 " + folder + " 吗？";
                    //if ((int)MessageBox.Show(msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == 1)
                    //{
                    //    //下载
                    //}
                }
                else
                {

                }
            }
        }


        //显示文件夹 第一级
        private void ShowFolder()
        {
            this.listView1.Items.Clear();
            List<string> list = user.folderList();
            foreach (string item in list)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = item;
                this.listView1.Items.Add(lvi);
            }
            //DirectoryInfo root = new DirectoryInfo(path);
            //this.listView1.Items.Clear();
            //foreach (DirectoryInfo d in root.GetDirectories())
            //{
            //    //文件夹
            //    string folderName = d.Name;
            //    string folderFullName = d.FullName;
            //    ListViewItem lvi = new ListViewItem();
            //    lvi.Text = folderName;
            //    this.listView1.Items.Add(lvi);
            //}
        }

        //显示文件 第二级
        private void ShowFile(string firstFloor)
        {
            this.listView1.Items.Clear();
            ListViewItem ret = new ListViewItem();
            ret.Text = "...";
            this.listView1.Items.Add(ret);
            List<DB.FileInfo> list = user.GetFileList(firstFloor);
            foreach (DB.FileInfo item in list)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = item.FileName;
                lvi.Name = item.FilePath;
                this.listView1.Items.Add(lvi);
            }

            //文件
            //DirectoryInfo root2 = new DirectoryInfo(fullPath);
            //ListViewItem ret = new ListViewItem();
            //ret.Text = "...";
            //this.listView1.Items.Add(ret);
            //System.IO.FileInfo[] fileinfoList = root2.GetFiles();
            //foreach (System.IO.FileInfo fi in fileinfoList)
            //{
            //    string fileName = fi.Name;
            //    string filePath = fi.FullName;
            //    ListViewItem lvi = new ListViewItem();
            //    lvi.Text = fileName;
            //    lvi.Name = filePath;
            //    this.listView1.Items.Add(lvi);
            //}
        }

    }
}
