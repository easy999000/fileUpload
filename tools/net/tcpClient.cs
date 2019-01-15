using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace tools.net
{
    public class tcpClient
    {


        public tcpDataCommunication tcpComm;
        public int Connect(System.Net.IPEndPoint ip)
        {
            System.Net.Sockets.TcpClient tcp = new System.Net.Sockets.TcpClient();
            tcp.Connect(ip);
            tcpComm = new tcpDataCommunication(tcp);
            return 0;
        }

        public int sendString(string str1)
        {
            if (tcpComm==null)
            {
                return -1;
            }

            return 0;
        }




    }
}
