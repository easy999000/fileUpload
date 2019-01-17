using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tools
{
    /// <summary>
    /// 日志
    /// </summary>
    public class log
    {
        public static string dirName = "log";

        /// <summary>
        /// 指定id设备的当前数据列表
        /// </summary>
        static ConcurrentDictionary<string, object> lockObject
             = new ConcurrentDictionary<string, object>();

        public static string formatException(Exception ex)
        {
            string s1 = string.Format(@"Message:{0},
StackTrace:{1},
Source:{2},
TargetSite:{3},
HResult:{4}
", ex.Message, ex.StackTrace, ex.Source, ex.TargetSite, ex.HResult);
            return s1;

        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="text"></param>
        /// <param name="o"></param>
        public static void writeLog(Exception ex)
        {
            string value = formatException(ex);

            writeLogToFile(value, getFileNameTime() + "-Exception");
        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="text"></param>
        /// <param name="o"></param>
        public static void writeLog(string text, params object[] o)
        {
            writeLogToFile(text, getFileNameTime(), o);

        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="text"></param>
        /// <param name="o"></param>
        public static void writeLog_NoParams(string text )
        {
            writeLogToFile_NoParams(text, getFileNameTime() );

        }
        /// <summary>
        /// 写日志指定文件名
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fileName"></param>
        public static void writeLogToFile(string text, string fileName, params object[] o)
        {
            string time = string.Format("[{0}] ", DateTime.Now.ToString());
            text = string.Format(text + "\r\n" + "\r\n", o);
#if DEBUG
            Console.WriteLine(text);
#endif
            object o1 = lockObject.GetOrAdd(fileName, new object());
            lock (o1)
            {
                System.IO.Directory.CreateDirectory(getDir());

                System.IO.File.AppendAllText(System.IO.Path.Combine(getDir(), fileName + ".txt"), time + text);
            }
        }
        /// <summary>
        /// 写日志指定文件名
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fileName"></param>
        public static void writeLogToFile_NoParams(string text, string fileName)
        {
            string time = string.Format("[{0} ] ", DateTime.Now.ToString());
            text = text + "\r\n" + "\r\n";
#if DEBUG
         //   Console.WriteLine(text);
#endif
            object o1 = lockObject.GetOrAdd(fileName, new object());
            lock (o1)
            {
                System.IO.Directory.CreateDirectory(getDir());

                System.IO.File.AppendAllText(System.IO.Path.Combine(getDir(), fileName + ".txt"), time + text);
            }
        }
        public static string getDir()
        {
            return System.IO.Path.Combine(System.Threading.Thread.GetDomain().BaseDirectory, dirName);
        }
        public static string getFileNameTime()
        {
            return System.DateTime.Now.ToString("yyyyMMdd");
        }
    }
}
