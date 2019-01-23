using BLE;
using DB;
using DB.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
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
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        tools.net.zTcpServer tcpServerListener;
        tools.net.tcpConnectionControl tcpConnection;

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

            tcpConnection = tcpServerListener.connControl;
            setStartOrEnd(1);


        }

        /// <summary>
        /// 1是启动
        /// </summary>
        /// <param name="i"></param>
        void setStartOrEnd(int i)
        {
            if (i == 1)
            {
                this.button1.Enabled = false;
                this.button2.Enabled = !this.button1.Enabled;
            }
            else
            {
                this.button1.Enabled = true;
                this.button2.Enabled = !this.button1.Enabled;
            }
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
                case msgEnum.getUserFileList:
                    getUserFileList(tcpComm, msg);
                    break;
                default:
                    break;
            }

        }
        void getUserFileList(tcpDataCommunication tcpComm, stringMsg msg)
        {
            if (tcpComm.user==null)
            {
                return;
            }
            List<DB.FileInfo> list = user.GetFileListByUserId(tcpComm.user.ID);
            stringMsg ret = new stringMsg();
            ret.name = msgEnum.returnUserFileList;
            ret.value["value"] = JsonConvert.SerializeObject(list);

            tcpComm.addSendBle(ret);


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
                tcpComm.user = curr.User;
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
            m1.value.Add("ID", curr.User.ID.ToString());

            string jsonCurr = JsonConvert.SerializeObject(curr);
            m1.value.Add("jsonCurr", jsonCurr);

            ConfigInfo config = user.GetSave();
            string jsonConfig = JsonConvert.SerializeObject(config);
            m1.value.Add("ConfigInfo", jsonConfig);

            tcpComm.sendData(m1);

           // UserService user = new UserService();
        }
        void BindDataGridView(List<TCP> tcp)
        {
            if (this.InvokeRequired)
            {
                //this.BeginInvoke(new Action<List<TCP>>(BindDataGridView), null);
                this.BeginInvoke(new Action<List<TCP>>(BindDataGridView), tcp);
            }
            else
            {
                this.dataGridView1.DataSource = null;
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
            ConfigInfo config = user.GetSave();
            path = config.Path;
            ShowFolder();
        }


        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                string folder = this.listView1.SelectedItems[0].SubItems[0].Text.ToString().Trim();
                string fullPath = (path + "\\" + folder).Replace(System.Environment.NewLine, string.Empty);
                string filepath = this.listView1.SelectedItems[0].SubItems[0].Name.ToString();

                if (fullPath.Contains("上一页"))
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

        /// <summary>
        /// 创建文件项
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ListViewItem createFileItem(DB.FileInfo info)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.ImageIndex = 1;
            lvi.Text = info.FileName;
            lvi.Tag = info.FilePath;
            lvi.SubItems.Add(info.UpLoadTime.ToString());
            lvi.SubItems.Add(info.Download.ToString());
            return lvi;
        }

        /// <summary>
        /// 创建文件项
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ListViewItem createFolderItem(string name)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.ImageIndex = 0;
            lvi.Text = name;
           
            return lvi;
        }

        //显示文件夹 第一级
        private void ShowFolder()
        {
            DirectoryInfo root = new DirectoryInfo(path);
            if (!root.Exists)
            {
                root.Create();

            }
            this.listView1.Items.Clear();
            List<string> list = user.folderList();
            foreach (string item in list)
            { 
                this.listView1.Items.Add(createFolderItem(item));
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
            
             
            this.listView1.Items.Add(createFolderItem("上一页"));
            List<DB.FileInfo> list = user.GetFileList(firstFloor);
            foreach (DB.FileInfo item in list)
            {

                this.listView1.Items.Add(createFileItem(item));
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
        //群发消息
        private void button6_Click(object sender, EventArgs e)
        {
            ConcurrentDictionary<string, tcpDataCommunication> tcpList = tcpConnection.tcpList;
            foreach (KeyValuePair<string, tcpDataCommunication> tcp in tcpList)
            {
                tcpDataCommunication tcpComm = tcp.Value;
                BLE.stringMsg m1 = new BLE.stringMsg();
                m1.name = BLE.msgEnum.liaotian;
                m1.value.Add("groupSending", this.richTextBox1.Text);
                tcpComm.sendData(m1);

            }
            this.richTextBox1.Text = String.Empty;
        }
        //指定用户发送消息
        private void button5_Click_1(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rows = this.dataGridView1.SelectedRows;
            if (rows.Count > 0)
            {


                foreach (DataGridViewRow item in rows)
                {
                    DataGridViewRow itemx = item;
                    string address = item.Cells[2].Value.ToString();

                    ConcurrentDictionary<string, tcpDataCommunication> tcpList = tcpConnection.tcpList;
                    List<string> keyList = tcpList.Keys.ToList();
                    string key = "";
                    foreach (string keyAddress in keyList)
                    {
                        string k = keyAddress.Substring(keyAddress.LastIndexOf("&") + 1, keyAddress.Length - keyAddress.LastIndexOf('&') - 1);
                        if (k.Equals(address))
                        {
                            key = keyAddress;
                        }
                    }
                    tcpDataCommunication tcpComm = tcpList[key];
                    BLE.stringMsg m1 = new BLE.stringMsg();
                    m1.name = BLE.msgEnum.liaotian;
                    m1.value.Add("singleSending", this.richTextBox1.Text);
                    tcpComm.sendData(m1);
                    //foreach (KeyValuePair<string, tcpDataCommunication> tcp in tcpList)
                    //{
                    //    tcpDataCommunication tcpComm = tcp.Value;



                    //}
                    this.richTextBox1.Text = String.Empty;

                }
            }
            else
            {
                MessageBox.Show("至少选择一个用户");
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //foreach (DataGridViewRow item in this.dataGridView1.Rows)
            //{
            //    if ((e.RowIndex).Equals(item.Index))
            //    {
            DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
            this.textBox1.Text = row.Cells[2].Value.ToString();
            //        item.Selected = true;
            //    }
            //}
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.tcpServerListener.stop();

            setStartOrEnd(0);
        }
    }
}
