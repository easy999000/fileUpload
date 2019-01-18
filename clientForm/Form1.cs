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
using System.Threading.Tasks;
using System.Windows.Forms;
using tools.net;

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
            zTcpClient1.Connect(new System.Net.IPEndPoint(ip, port));

            zTcpClient1.tcpComm.newBleMessageEvent += newBlemessageEventFun;

            login = new clientForm.Login(zTcpClient1, this);
            login.Show();

        }
        //给服务端发消息
        private void button5_Click(object sender, EventArgs e)
        {
            BLE.stringMsg m1 = new BLE.stringMsg();
            m1.name = BLE.msgEnum.liaotian;
            m1.value.Add("value", this.richTextBox2.Text);

            zTcpClient1.tcpComm.sendData(m1);


        }
        //给服务端发送文件
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
                sm.value.Add("value", reviced);//全路径
                sm.value.Add("UserId", CurrUser.currUser.ID.ToString());//用户id
                sm.value.Add("FirstFloor", CurrUser.currUser.ID.ToString());//用户专属文件夹
                sm.value.Add("FileName", fileName);//文件名称
                sm.value.Add("FirstFloorDir", config.Path + "\\" + CurrUser.currUser.ID);//文件存储路径
                t2.ReceiveFullMsg = sm.modelToJson();

                t2.toBleStream(zTcpClient1.tcpComm.sendDataGetStream());
                //zTcpClient1.tcpComm.addSendBle(t2);
                //改为队列
                //t2.toBleStream(zTcpClient1.tcpComm.sendDataGetStream());
                zTcpClient1.tcpComm.addSendBle(t2);
                
                //发送文件队列 目前问题没有第一个发送的
                List<BLE.BLEData> sendFileList = zTcpClient1.tcpComm.sendFileList;
            }
        }
        //停止连接
        private void button1_Click(object sender, EventArgs e)
        {
            zTcpClient1.tcpComm.stop();
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
            DownLand();
        }

        //刷新列表
        private void button3_Click(object sender, EventArgs e)
        {
            //刷新列表
            ResetList();
        }

        //重置
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

        //下载方法
        private void DownLand()
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

            BLE.bleClass.t11 t11 = (BLE.bleClass.t11)ble;
            stringMsg msg = stringMsg.jsonToModel(t11.msg);


            switch (msg.name)
            {
                case msgEnum.dengru:
                    //bool bl = Convert.ToBoolean(jo["value"]["return"]);
                    bool bl = Convert.ToBoolean(msg.value["return"]);
                    if (bl)
                    {
                        
             
                        RetUser curr = JsonConvert.DeserializeObject<RetUser>(msg.value["jsonCurr"]);
                        //RetUser curr = user.Login(login.textBox1.Text, login.textBox2.Text);
                        CurrUser.currUser = curr.User;
                        ShowFile(curr.User.ID);
                        MessageBox.Show(string.Format("登陆成功!"));
                        CloseFrom(login);
                    }
                    else
                    {
                        zTcpClient1.tcpComm.stop();
                        MessageBox.Show("账号或密码错误!");
                    }
                    break;
                case msgEnum.liaotian:
                    //string groupSendingMsg = jo["value"]["groupSending"].ToString();
                    string groupSendingMsg = msg.value["groupSending"];
                    showMsg(groupSendingMsg);
                    break;
                default:
                    break;
            }



        }
        //关闭登录框窗体
        void CloseFrom(Login login)
        {
            if (login.InvokeRequired)
            {
                login.BeginInvoke(new Action<Login>(CloseFrom), login);
            }
            else
            {
                login.Close();
            }

        }
        //读取用户文件信息
        void ShowFile(int userId)
        {
            if (listView1.InvokeRequired)
            {
                this.BeginInvoke(new Action<int>(ShowFile), userId);
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
            }

        }

    }
}
