using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Services
{
    public class UserService
    {
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public RetUser Login(string account, string pwd)
        {
            using (DB db = new DB())
            {
                try
                {
                    UserInfo user = db.UserInfo.Where(x => x.Account == account && x.PWD == pwd).FirstOrDefault();
                    if (user != null)
                    {
                        return new RetUser() { Success = true, User = user };
                    }
                    else
                    {
                        return new RetUser() { Success = false };
                    }
                }
                catch (Exception ex)
                {
                    return new RetUser() { Success = false };
                }
            }
        }

        /// <summary>
        /// 获取配置路径
        /// </summary>
        /// <returns></returns>
        public ConfigInfo GetSave()
        {
            using (DB db = new DB())
            {
                try
                {
                    ConfigInfo config = db.ConfigInfo.FirstOrDefault();
                    if (config == null)
                    {
                        config = new ConfigInfo();
                        config.Effect = "FileSavePath";
                        config.Path = "D:\\";
                        config.Remark = "自动生成";
                        db.SaveChanges();
                    }
                    return config;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="newPath"></param>
        /// <returns></returns>
        public bool EditPath(string newPath)
        {
            using (DB db = new DB())
            {
                try
                {
                    ConfigInfo config = db.ConfigInfo.FirstOrDefault();
                    config.Path = newPath;
                    db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 获取所有文件夹
        /// </summary>
        /// <returns></returns>
        public List<string> folderList()
        {
            using (DB db = new DB())
            {
                try
                {
                    List<string> str = db.FileInfo.OrderBy(x => x.FirstFloor).Select(x => x.FirstFloor).Distinct().ToList<string>();
                    return str;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取某层下文件
        /// </summary>
        /// <param name="firstFloor"></param>
        /// <returns></returns>
        public List<FileInfo> GetFileList(string firstFloor)
        {
            using (DB db = new DB())
            {
                try
                {
                    List<FileInfo> list = db.FileInfo.Where(x => x.FirstFloor == firstFloor).ToList<FileInfo>();
                    return list;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }


        /// <summary>
        /// 通过用户id获取文件列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<FileInfo> GetFileListByUserId(int userId)
        {
            using (DB db = new DB())
            {
                try
                {
                    List<FileInfo> list = db.FileInfo.Where(x => x.UserId == userId).ToList<FileInfo>();
                    return list;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 添加文件记录到数据库
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="filePath"></param>
        /// <param name="firstFloor"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool AddFileInfo(int userId,string filePath,string firstFloor,string fileName)
        {
            using (DB db=new DB())
            {
                try
                {
                    FileInfo file = new FileInfo();
                    file.Download = 0;
                    file.FileName = fileName;
                    file.FilePath = filePath;
                    file.FirstFloor = firstFloor;
                    file.UpLoadTime = DateTime.Now;
                    file.UserId = userId;
                    db.FileInfo.Add(file);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

    }



    public class RetUser
    {
        public UserInfo User { get; set; }

        public bool Success { get; set; }
    }
}
