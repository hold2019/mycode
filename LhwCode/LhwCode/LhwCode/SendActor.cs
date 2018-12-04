using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LhwCode
{
   public  class SendActor
    {
        public IPEndPoint HostName=null;
        public UDPDATA udpd=null;
        public  SendActor(IPEndPoint h, UDPDATA u) {
            HostName = h;
            udpd = u;
        }
    }
}
