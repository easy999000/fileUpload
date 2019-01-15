using BLE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace tools.net
{
    public class tcpServer
    {
        #region 接收相关变量
        /// <summary>
        /// tcp通信
        /// </summary>
        TcpListener tcpListener1;

        /// <summary>
        /// 并发的tcp监听数量
        /// </summary>
        public int ListenerThCount { get; set; } = 3;

        /// <summary>
        /// 连接管理
        /// </summary>
        public tcpConnectionControl connControl = new tcpConnectionControl();

        #endregion

        #region 发送相关字段

        #endregion

        public tcpServer(IPAddress ip, int port)
        {
            tcpListener1 = new TcpListener(ip, port);
            connControl.newMessageEvent += newMessageEventFun;
        }

        #region 接收相关函数

        /// <summary>
        /// 开始监听
        /// </summary>
        public void start()
        {
            tcpListener1.Start();

            for (int i = 0; i < ListenerThCount; i++)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(tcpListenerBegin));
            }
        }
        /// <summary>
        /// 停止监听
        /// </summary>
        public void stop()
        {
            tcpListener1.Stop();

        }

        /// <summary>
        /// 开始异步接收连接请求
        /// </summary>
        /// <param name="o"></param>
        void tcpListenerBegin(object o)
        {
#if DEBUG
            //           tools.log.writeLog("tcpListenerControl.tcpListenerBegin 线程:{0},监听等待", System.Threading.Thread.CurrentThread.ManagedThreadId);
#endif
            tcpListener1.BeginAcceptTcpClient(new AsyncCallback(tcpListenerEnd), o);
        }

        /// <summary>
        /// 处理连接请求.
        /// </summary>
        /// <param name="ia"></param>
        void tcpListenerEnd(IAsyncResult ia)
        {
            tcpListenerBegin(null);

            TcpClient tcp2 = tcpListener1.EndAcceptTcpClient(ia);

            connControl.addTcp(tcp2);

            //this.mainService1.tcpConnection0.addTcp(tcp2);
        }
        void newMessageEventFun(tcpDataCommunication comm, stringMsg msg)
        {
        }
        #endregion


        #region tcp连接控制 

        #endregion




        /*
         tcp 监听线程测试结果
         不管在一个线程内,调用多少次 异步的接收函数,BeginAcceptTcpClient
        tcpListener1.BeginAcceptTcpClient(new AsyncCallback(instenerEnd), str2);
        tcpListener1.BeginAcceptTcpClient(new AsyncCallback(instenerEnd), str2);
        tcpListener1.BeginAcceptTcpClient(new AsyncCallback(instenerEnd), str2);
        tcpListener1.BeginAcceptTcpClient(new AsyncCallback(instenerEnd), str2); 

        系统只创建一个线程来接收和处理数据.

        threadId:13,任务id:374935ab-1120-43f3-876f-e9084fbd1504
        threadId:13,任务id:374935ab-1120-43f3-876f-e9084fbd1504,收到的值:aaaa44,本地端口:127.0.0.1:18899,对方端口:127.0.0.1:46750
        threadId:13,任务id:374935ab-1120-43f3-876f-e9084fbd1504
        threadId:13,任务id:374935ab-1120-43f3-876f-e9084fbd1504,收到的值:aaaa45,本地端口:127.0.0.1:18899,对方端口:127.0.0.1:46751
        threadId:13,任务id:374935ab-1120-43f3-876f-e9084fbd1504
        threadId:13,任务id:374935ab-1120-43f3-876f-e9084fbd1504,收到的值:aaaa47,本地端口:127.0.0.1:18899,对方端口:127.0.0.1:46752 

        而且,第一个接收的事件处理完成,才会监听和接收下一个事件.如果第一个处理事件没有完成,不会接收下一个链接请求

        不管有多少个,并发的连接请求.都会在另外一个固定的线程内执行,如果请求很多,就会事件排队,所以想要达到并发效果,还需要对从不同的线程挂起接收请求



        在数据异步处理线程EndAcceptTcpClient , 创建新的异步接收函数的时候,
        会,重新创建一个线程接收数据,如果在当前任务没有处理完成的情况下,又来了新的数据,
        会在新的线程内处理,如果新数据来的时候,当前线程已经处理完成,线程会被重新利用. 
        但是在当前处理函数开始执行,到新的接收任务被创建,中间会有空档期

        对于同一个tcpListener,可以同时开启多个异步线程,同时调用.BeginAcceptTcpClient,线程之间并不会冲突,
        这些创建出来的监听任务,是在多个线程上,并行开始监听的.

        threadId:36,任务id:d4c62dbe-a461-43e2-9191-f4c38c1d3c88,收到的值:aaaa78,本地端口:127.0.0.1:18899,对方端口:127.0.0.1:5044
        threadId:44,任务id:7b2ada03-9d52-465b-a819-dbd199f28aca,收到的值:aaaa79,本地端口:127.0.0.1:18899,对方端口:127.0.0.1:5045
        threadId:28,任务id:4d12e443-07d0-4686-9987-ee3140d4de20,收到的值:aaaa77,本地端口:127.0.0.1:18899,对方端口:127.0.0.1:5043
        threadId:52,任务id:95c77fab-c942-4603-a7fb-29bdacc1025e,收到的值:aaaa76,本地端口:127.0.0.1:18899,对方端口:127.0.0.1:5042

        在多个线程同时监听的时候,如果来了一条数据,系统会自动分发一个任务线程,如果同时来了多个数据连接,系统会自动,给每个数据分配一个处理线程,
        并不会出现冲突,也不会出现,并发问题.

        所以对于这个系统的设计方案应该是,对一个tcpListener,用多个(3到5个)线程创建数据接收工作,连接来了之后,立即另立线程,来进行监听工作,当前接收线程,执行完成退出.


        threadId:36,任务id:d4c62dbe-a461-43e2-9191-f4c38c1d3c88,收到的值:aaaa78,本地端口:127.0.0.1:18899,对方端口:127.0.0.1:5044
        threadId:44,任务id:7b2ada03-9d52-465b-a819-dbd199f28aca,收到的值:aaaa79,本地端口:127.0.0.1:18899,对方端口:127.0.0.1:5045
        threadId:28,任务id:4d12e443-07d0-4686-9987-ee3140d4de20,收到的值:aaaa77,本地端口:127.0.0.1:18899,对方端口:127.0.0.1:5043
        threadId:52,任务id:95c77fab-c942-4603-a7fb-29bdacc1025e,收到的值:aaaa76,本地端口:127.0.0.1:18899,对方端口:127.0.0.1:5042


         */




    }
}
