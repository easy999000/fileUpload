using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace tools
{
    public  class zHttpListener
    {
        HttpListener HListener1 = new HttpListener();
        public void start()
        {
            HListener1.Prefixes.Add("http://*;8888/upload");

            HListener1.Start();

            HttpListenerContext hlcontext = HListener1.GetContext();
             


        }
    }
}
