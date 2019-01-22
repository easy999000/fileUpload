using DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace tools.net
{
    public class zTcpClient
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

        public void send_getUserFileList(UserInfo currUser)
        {

            BLE.stringMsg m1 = new BLE.stringMsg();
            m1.name = BLE.msgEnum.getUserFileList;
            m1.value.Add("UserId",  currUser.ID.ToString());//用户id 
            tcpComm.addSendBle(m1); 
        }





    }
}
