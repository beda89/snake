using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WindowsGame1
{
    class Client
    {
        private String ip;
        private int port;
        private TcpClient c;


        public Client(String ipString, int port)
        {
           this.ip = ipString;
           this.port = port;
        }

        public void start(){
            c = new TcpClient(ip, port);
        }

    }
}
