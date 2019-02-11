using BLE;
using DB;
using DB.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using tools.net;

namespace clientForm
{
    public partial class Form1 : Form
    {
        UserService user = new UserService();
        List<BLE.BLEData> waitSendBleData;
        public Form1()
        {
            InitializeComponent();
            this.waitSendMsgTimer = new System.Threading.Timer(waitSendMsgTimerFun, zTcpClient1, TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(200));
        }

        /// <summary>
        /// 处理待发送信息
        /// </summary>
        /// <param name="o"></param>
        void waitSendMsgTimerFun(object o)
        {
            if (zTcpClient1 == null)
            {
                return;
            }
            if (zTcpClient1.tcpComm == null)
            {
                return;
            }
            int c = zTcpClient1.tcpComm.waitSendList.Count;

            if (zTcpClient1.tcpComm.currentSendBleData != null)
            {
                c++;
            }
            string s = "";
            if (c > 0)
            {
                s = c + "条消息正在发送";
            }
            else
            {
                s = "";
            }
            showSendMsgTimerFun(s);

        }

        /// <summary>
        /// 显示待发送信息
        /// </summary>
        /// <param name="str"></param>
        void showSendMsgTimerFun(string str)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(showSendMsgTimerFun), str);
                return;

            }

            this.labelWaitSend.Text = str;
        }

        public tools.net.zTcpClient zTcpClient1 = new tools.net.zTcpClient();
        Login login;

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
            try
            {
                zTcpClient1.Connect(new System.Net.IPEndPoint(ip, port));

            }
            catch (System.Net.Sockets.SocketException ex1)
            {
                MessageBox.Show("连接失败");
                return;
            }
            zTcpClient1.tcpComm.newBleMessageEvent += newBlemessageEventFun;
            zTcpClient1.tcpComm.connectionDisconnectionEvent += connectionDisconnectionEventFun;
            zTcpClient1.tcpComm.isReading += isReading;
            setStartOrEnd(1);
            login = new clientForm.Login(zTcpClient1, this);
            login.Show();

        }
        private void isReading(tcpDataCommunication comm, bool b)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<tcpDataCommunication, bool>(isReading), comm, b);
                return;
            }
            if (b)
            {
                this.label2.Text = "正在接收数据";
            }
            else
            {
                this.label2.Text = "";
            }
        }
        //给服务端发消息
        private void button5_Click(object sender, EventArgs e)
        {
            BLE.stringMsg m1 = new BLE.stringMsg();
            m1.name = BLE.msgEnum.liaotian;
            m1.value.Add("value", this.richTextBox2.Text);

            //      zTcpClient1.tcpComm.sendData(m1);

            //if (zTcpClient1.tcpComm.tcpClient1.Connected)
            //{

            //}

            zTcpClient1.tcpComm.addSendBle(m1);
            this.richTextBox2.Text = "";


        }

        System.Threading.Timer timer;
        System.Threading.Timer waitSendMsgTimer;
        //给服务端发送文件
        private void button7_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //BLE.bleClass.t12 t2 = new BLE.bleClass.t12();

                string sendFileFullPath = openFileDialog1.FileName;

                string fileName = System.IO.Path.GetFileName(sendFileFullPath);

                ////  ConfigInfo config = user.GetSave();
                string saveFileFullPath = System.IO.Path.Combine(CurrUser.config.Path + "\\" + CurrUser.currUser.ID, fileName);//d:\\


                stringMsg sm = new stringMsg();
                sm.name = msgEnum.fileUpload;
                sm.value.Add("saveFileFullPath", saveFileFullPath);//全路径
                sm.value.Add("sendFileFullPath", sendFileFullPath);//全路径
                //sm.value.Add("value", saveFileFullPath);//全路径
                sm.value.Add("UserId", CurrUser.currUser.ID.ToString());//用户id
                sm.value.Add("FirstFloor", CurrUser.currUser.ID.ToString());//用户专属文件夹
                sm.value.Add("FileName", fileName);//文件名称
                //sm.value.Add("fileDirFullPath", CurrUser.config.Path + "\\" + CurrUser.currUser.ID);//文件存储路径
                //t2.ReceiveFullMsg = sm.modelToJson();


                //改为队列
                //t2.toBleStream(zTcpClient1.tcpComm.sendDataGetStream());
                zTcpClient1.tcpComm.addSendBle(sm);

                //  BLE.BLEData currentSend = zTcpClient1.tcpComm.currentSendBleData;
                //发送文件队列 目前问题没有第一个发送的currentSendBleData
                // waitSendBleData = zTcpClient1.tcpComm.waitSendList;

                //this.listView2.Columns.Clear();
                //this.listView2.Columns.Add("文件名", 500, HorizontalAlignment.Left);
                //this.listView2.Columns.Add("进度", 100, HorizontalAlignment.Left);

                //currentSend.currProgress
                //传输的文件
                //if (currentSend != null)
                //{
                //System.Timers.Timer t = new System.Timers.Timer(500);//实例化Timer类，设置间隔时间为10000毫秒；
                //t.Elapsed += new System.Timers.ElapsedEventHandler(currProgress, currentSend,);
                //t.AutoReset = true;


                // NewMethod(currentSend);

                //}
                //  timer = new System.Threading.Timer(currProgress, currentSend, TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(200));

                //队列中尚未传输
                //for (int i = 0; i < currentSendBleData.Count; i++)
                //{
                //    BLE.bleClass.t12 t12_wait = (BLE.bleClass.t12)currentSendBleData[i];
                //    stringMsg msg_wait = stringMsg.jsonToModel(t12_wait.ToString());
                //    string fName_wait = msg_wait.value["FileName"];
                //    ListViewItem lvi_wait = new ListViewItem();
                //    lvi_wait.Text = fName_wait;
                //    lvi_wait.SubItems.Add("等待中...");
                //    this.listView2.Items.Add(lvi_wait);
                //}

            }
        }

        private void currProgress(object currentSend)//
        {
            //if (currentSend == null)
            //{
            //    return;
            //}

            //if (listView2.InvokeRequired)
            //{
            //    listView2.BeginInvoke(new Action<object>(currProgress), currentSend);
            //}
            //else
            //{
            //    timer.Dispose();
            //    BLEData ble = currentSend as BLEData;
            //    stringMsg msg = stringMsg.jsonToModel(ble.ToString());
            //    string fName;
            //    if (msg.name == msgEnum.fileUpload)
            //    {
            //        fName = msg.value["FileName"];
            //    }
            //    else
            //    {
            //        fName = "文字消息";
            //    }
            //    //正在传输的
            //    this.listView2.Items.Clear();
            //    ListViewItem lvi = new ListViewItem();
            //    lvi.Text = fName;
            //    lvi.SubItems.Add(ble.currProgress.ToString() + "%");
            //    this.listView2.Items.Add(lvi);
            //    if (ble.currProgress > 99)
            //    {
            //        //timer.Dispose();
            //        this.listView2.Items.Clear();
            //    }
            //    //队列中尚未传输
            //    for (int i = 0; i < waitSendBleData.Count; i++)
            //    {
            //        BLEData t12_wait = (BLEData)waitSendBleData[i];

            //        stringMsg msg_wait = stringMsg.jsonToModel(t12_wait.ToString());
            //        string fName_wait;
            //        if (msg.name == msgEnum.fileUpload)
            //        {
            //            fName_wait = msg.value["FileName"];
            //        }
            //        else
            //        {
            //            fName_wait = "文字消息";
            //        }
            //        ListViewItem lvi_wait = new ListViewItem();
            //        lvi_wait.Text = fName_wait;
            //        lvi_wait.SubItems.Add("等待中...");
            //        this.listView2.Items.Add(lvi_wait);
            //    }
            //}


        }
        //停止连接
        private void button1_Click(object sender, EventArgs e)
        {
            zTcpClient1.tcpComm.stop();
            setStartOrEnd(0);
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            DownLand();
        }


        /// <summary>
        /// 文件重名验证
        /// </summary>
        /// <param name="path">文件路径(不含文件名)</param>
        /// <param name="filename">文件名</param>
        /// <param name="index">不用传</param>
        /// <returns></returns>
        private string CheckFileName(string path, string filename, int index = 1)
        {
            string newfilename = filename;
            if (File.Exists(path + "\\" + filename))
            {
                string name = filename.Substring(0, filename.IndexOf('.'));
                string suffix = filename.Substring(filename.IndexOf('.'), filename.Length - filename.IndexOf('.'));
                newfilename = name + "(" + index + ")" + suffix;
                if (File.Exists(path + "\\" + newfilename))
                {
                    index++;
                    newfilename = CheckFileName(path, filename, index);
                }
            }
            return newfilename;
        }
        //下载
        private void button4_Click(object sender, EventArgs e)
        {
            //下载
            DownLand2();
        }

        //刷新列表
        private void button3_Click(object sender, EventArgs e)
        {
            zTcpClient1.send_getUserFileList(CurrUser.currUser);
            //刷新列表
            ///  ResetList();
        }

        //显示文件列表
        private void ResetList(stringMsg msg)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<stringMsg>(ResetList), msg);
                return;

            }
            List<DB.FileInfo> list = JsonConvert.DeserializeObject<List<DB.FileInfo>>(msg.value["value"]);

            this.listView1.Items.Clear();
            foreach (DB.FileInfo item in list)
            {
                //ListViewItem lvi = new ListViewItem();
                //lvi.Text = item.FileName;
                //lvi.Name = item.FilePath;
                listView1.Items.Add(createFileItem(item));
            }
        }

        public ListViewItem createFileItem(DB.FileInfo info)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.ImageIndex = 1;
            lvi.Text = info.FileName;
            lvi.Tag = info.FilePath;
            return lvi;
        }

        //下载方法
        private void DownLand2()
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                string folder = this.listView1.SelectedItems[0].SubItems[0].Text.ToString().Trim();
                string sendFileFullPath = this.listView1.SelectedItems[0].Tag == null ? "" : this.listView1.SelectedItems[0].Tag.ToString();
                if (sendFileFullPath.Length > 0)
                {
                    //选中文件 下载
                    string msg = "确定要下载 " + folder + " 吗？";
                    if ((int)MessageBox.Show(msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == 1)
                    {
                        this.saveFileDialog1.FileName = System.IO.Path.GetFileName(sendFileFullPath);
                        if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                //下载
                                string saveFileFullPath = saveFileDialog1.FileName;

                                stringMsg m1 = new stringMsg();
                                m1.name = msgEnum.getDownLandFile;
                                m1.value.Add("sendFileFullPath", sendFileFullPath);
                                m1.value.Add("saveFileFullPath", saveFileFullPath);

                                this.zTcpClient1.tcpComm.addSendBle(m1);

                            }
                            catch (Exception ex)
                            {
                                tools.log.writeLog("DownloadDataException:{0},文件信息:{1}", ex.Message, sendFileFullPath);
                            }
                        }
                    }
                }
            }
        }
        //下载方法
        private void DownLand()
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                string folder = this.listView1.SelectedItems[0].SubItems[0].Text.ToString().Trim();
                string filepath = this.listView1.SelectedItems[0].Tag == null ? "" : this.listView1.SelectedItems[0].Tag.ToString();
                if (filepath.Length > 0)
                {
                    //选中文件 下载
                    string msg = "确定要下载 " + folder + " 吗？";
                    if ((int)MessageBox.Show(msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == 1)
                    {
                        FolderBrowserDialog saveFolder = new FolderBrowserDialog();
                        saveFolder.Description = "选择文件保存目录";
                        if (saveFolder.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                //下载
                                string path = saveFolder.SelectedPath;
                                WebClient myWebClient = new WebClient();

                                if (File.Exists(filepath))
                                {
                                    byte[] data = myWebClient.DownloadData(filepath);
                                    string endPath = path + "\\" + CheckFileName(path, folder);//含重名验证,如重名覆盖则改成:path+"\\"+folder
                                    FileStream fs = new FileStream(endPath, FileMode.Create);
                                    fs.Write(data, 0, data.Length);
                                    fs.Close();
                                    MessageBox.Show("下载成功！", "提示");
                                    user.UpdateDownLand(filepath);
                                    user.Add_Log_Opera(CurrUser.currUser.ID, CurrUser.currUser.Account, string.Format("下载文件{0}", folder));
                                }
                                else
                                {
                                    MessageBox.Show("文件不存在！", "提示");
                                    user.Add_Log_Error(CurrUser.currUser.ID, CurrUser.currUser.Account, string.Format("下载文件{0}出现问题", folder));
                                }
                            }
                            catch (Exception ex)
                            {
                                tools.log.writeLog("DownloadDataException:{0},文件信息:{1}", ex.Message, filepath);
                            }
                        }
                    }
                }
            }
        }

        //新消息事件
        void newBlemessageEventFun(tcpDataCommunication tcpComm, BLEData ble)
        {
            //string jsonText = ((BLE.bleClass.t11)ble).msg;
            //JObject jo = (JObject)JsonConvert.DeserializeObject(jsonText);

            stringMsg msg = stringMsg.jsonToModel(ble.ToString());

            switch (msg.name)
            {
                case msgEnum.dengru:
                    //bool bl = Convert.ToBoolean(jo["value"]["return"]);
                    bool bl = Convert.ToBoolean(msg.value["return"]);
                    if (bl)
                    {


                        RetUser curr = JsonConvert.DeserializeObject<RetUser>(msg.value["jsonCurr"]);
                        ConfigInfo ConfigInfo1 = JsonConvert.DeserializeObject<ConfigInfo>(msg.value["ConfigInfo"]);
                        //RetUser curr = user.Login(login.textBox1.Text, login.textBox2.Text);
                        CurrUser.currUser = curr.User;
                        CurrUser.config = ConfigInfo1;
                        CloseFrom2();
                        ShowFile(curr.User.ID);
                    }
                    else
                    {
                        //   zTcpClient1.tcpComm.stop();
                        MessageBox.Show("账号或密码错误!");
                    }
                    break;
                case msgEnum.liaotian:
                    //string groupSendingMsg = jo["value"]["groupSending"].ToString();
                    string reciveMsg = "";
                    if (msg.value.Keys.Contains("groupSending"))
                    {
                        reciveMsg = msg.value["groupSending"];
                    }
                    else if (msg.value.Keys.Contains("singleSending"))
                    {
                        reciveMsg = msg.value["singleSending"];
                    }
                    showMsg(reciveMsg);
                    break;
                case msgEnum.returnUserFileList:
                    ResetList(msg);
                    break;
                case msgEnum.fileUpload:
                    fileUpload(tcpComm, ble);
                    break;
                default:
                    break;
            }

        }
        void fileUpload(tcpDataCommunication tcpComm, BLEData msg)
        {
            stringMsg m1 = stringMsg.jsonToModel(msg.ToString());
            if (m1.value["saveFileFullPath"].Trim() == "")
            {
                showMsg("<下载失败,服务端文件不存在>");
               // MessageBox.Show("下载失败,服务端文件不存在", "提示");
                return;
            }
            showMsg("<下载完成>");
            //MessageBox.Show("下载完成", "提示");
        }
        //关闭登录框窗体
        void CloseFrom2()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(CloseFrom2));
            }
            else
            {
                MessageBox.Show(string.Format("登陆成功!"));
                CloseFrom(login);
            }

        }
        //关闭登录框窗体
        void CloseFrom(Login login)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<Login>(CloseFrom), login);
            }
            else
            {
                login.Close();
            }

        }
        //读取用户文件信息
        void ShowFile(int userId)
        {

            zTcpClient1.send_getUserFileList(CurrUser.currUser);
            //if (listView1.InvokeRequired)
            //{
            //    this.BeginInvoke(new Action<int>(ShowFile), userId);
            //}
            //else
            //{
            //    listView1.Items.Clear();
            //    List<DB.FileInfo> list = user.GetFileListByUserId(userId);
            //    foreach (DB.FileInfo item in list)
            //    {
            //        ListViewItem lvi = new ListViewItem();
            //        lvi.Text = item.FileName;
            //        lvi.Name = item.FilePath;
            //        listView1.Items.Add(lvi);
            //    }
            //}
        }
        //显示服务端消息
        void showMsg(string msg)
        {
            if (this.richTextBox1.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(showMsg), msg);
            }
            else
            {
                this.richTextBox1.Text += string.Format("[{0}]: ", DateTime.Now.ToString()) + msg + "\r\n\r\n";
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
            }

        }

        /// <summary>
        /// 1是连接
        /// </summary>
        /// <param name="i"></param>
        void setStartOrEnd(int i)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(setStartOrEnd), i);
                return;

            }
            if (i == 1)
            {
                this.buttonDuankai.Enabled = true;
            }
            else
            {
                this.buttonDuankai.Enabled = false;
            }
            this.buttonLianjie.Enabled = !this.buttonDuankai.Enabled;
            this.button3.Enabled = this.buttonDuankai.Enabled;
            this.button4.Enabled = this.buttonDuankai.Enabled;
            this.button5.Enabled = this.buttonDuankai.Enabled;
            this.button7.Enabled = this.buttonDuankai.Enabled;
        }
        public void connectionDisconnectionEventFun(tcpDataCommunication comm)
        {
            setStartOrEnd(0);
        }
    }
}
