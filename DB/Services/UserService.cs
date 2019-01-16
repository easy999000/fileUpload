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
        public bool EditPath(string  newPath)
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

    }



    public class RetUser
    {
        public UserInfo User { get; set; }

        public bool Success { get; set; }
    }
}
