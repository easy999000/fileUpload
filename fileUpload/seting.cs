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

namespace fileUpload
{
    public partial class seting : Form
    {
        private string sPath;
        UserService user = new UserService();

        public seting()
        {
            InitializeComponent();
        }

        private void seting_Load(object sender, EventArgs e)
        {
            DB.ConfigInfo config =user.GetSave();
            this.textBox5.Text = config.Path;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.textBox5.Text.Length > 0)
            {
                bool bl = user.EditPath(this.textBox5.Text);
                //WriteFile(sPath);
                if (bl)
                {
                    MessageBox.Show("保存成功！", "提示");
                }
                else
                {
                    MessageBox.Show("保存失败！", "提示");
                }
                      
               
            }
            else
            {
                MessageBox.Show("请选择文件保存目录！", "提示");
            }
            //this.DialogResult = DialogResult.OK;
        }

        //取消
        private void button4_Click(object sender, EventArgs e)
        {
            // this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        //选择文件夹
        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.Description = "选择文件保存目录";
            if (folder.ShowDialog() == DialogResult.OK)
            {
                sPath = folder.SelectedPath;
                this.textBox5.Text = sPath;
            }
        }
        //写入文本
        void WriteFile(string str)
        {
            StreamWriter sr;
            if (File.Exists(Common.FILE_NAME)) 
            {
                sr = File.CreateText(Common.FILE_NAME);
            }
            else
            {
                sr = File.CreateText(Common.FILE_NAME);
            }
            sr.WriteLine(str);
            sr.Close();
        }


    }
}
