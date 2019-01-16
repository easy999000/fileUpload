using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileUpload
{
    public class Common
    {
        public const string FILE_NAME = "FileSavePath.txt";
        public static List<TCP> tcpList;
        
    }

    public class TCP
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
    }
}
