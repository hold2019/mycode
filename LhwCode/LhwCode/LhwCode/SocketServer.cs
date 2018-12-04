using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
namespace LhwCode
{
    public class SocketServer
    {
        public bool ShowSend = false;
        public bool ShowReceiv = false;
        public IPEndPoint ipEndPoint = null;
        public Action<UDPDATA> HandleMessage = null;
        protected Queue<SendActor> SendQueue = new Queue<SendActor>();
        protected List<SendActor> NeedReply = new List<SendActor>();
        private  struct UdpState
        {
            public UdpClient u;
            public IPEndPoint e;
        }
        UdpClient mListenCLient = null;
        public  int clientPort = 0;
        private Thread SendThread = null;
        private Thread checkThread = null;
        private bool IsInSending = false;
        private MessageID mMessageID = new MessageID();
        private object lockobj = new object();
        private bool IsClosing = false;
        public bool StartUDPservice(int port)
        {
            try
            {

                ipEndPoint = new IPEndPoint(IPAddress.Any, port);
                mListenCLient = new UdpClient(ipEndPoint);

                UdpState s = new UdpState();
                s.e = ipEndPoint;
                s.u = mListenCLient;
                LogHelper.Log("开始监听端口"+ port);
                mListenCLient.BeginReceive(new AsyncCallback(ReceiveCallback), s);
                IsInSending = true;
                SendThread = new Thread(SendThread_Method);
                SendThread.Start();
                checkThread = new Thread(CheckThread_Method);
                checkThread.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }
        void SendThread_Method() {
            while (IsInSending) {
                Thread.Sleep(10);
                lock (lockobj)
                {
                    if (SendQueue.Count > 0)
                    {
                        SendActor tm = SendQueue.Dequeue();
                        if (tm.udpd.NeedReply)
                        {
                            tm.udpd.SendCout++;
                            if (!NeedReply.Contains(tm))
                            {
                                tm.udpd.udp_MesageID = mMessageID.GetID();
                                NeedReply.Add(tm);
                            }
                        }
                        RealMUSPSend(tm.HostName, tm.udpd);
                    }
          
                }
            }
        }
        void CheckThread_Method()
        {
            while (IsInSending)
            {
                Thread.Sleep(500);
                for(int i=0;i<NeedReply.Count;i++)
                {
                    SendActor item = NeedReply[i];
                   
                    LogHelper.Log(item.udpd.udp_MesageID+"::"+ item.udpd.SendCout);
                    if (item.udpd.SendCout > 3)
                    {
                        NeedReply.RemoveAt(i);
                    }
                    else {
                        SendQueue.Enqueue(item);
                    }
                }
            }
        }
        public bool StartUDPservice(int port, int client)
        {
            try
            {
                clientPort = client;
                ipEndPoint = new IPEndPoint(IPAddress.Any, port);
                mListenCLient = new UdpClient(ipEndPoint);
                UdpState s = new UdpState();
                s.e = ipEndPoint;
                s.u = mListenCLient;
                LogHelper.Log("开始监听端口" + port);
                mListenCLient.BeginReceive(new AsyncCallback(ReceiveCallback), s);
                IsInSending = true;
                SendThread = new Thread(SendThread_Method);
                SendThread.Start();
                checkThread = new Thread(CheckThread_Method);
                checkThread.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                UdpClient u = (UdpClient)((UdpState)(ar.AsyncState)).u;
                IPEndPoint e = (IPEndPoint)((UdpState)(ar.AsyncState)).e;

                byte[] receiveBytes = u.EndReceive(ar, ref e);
                LogHelper.Log(receiveBytes);
                ipEndPoint = new IPEndPoint(e.Address, e.Port);
                ReciveData(new IPEndPoint (e.Address, e.Port),receiveBytes);
     

            }
            catch (Exception ee)
            {
                LogHelper.Log(ee.ToString());
            }
            if (!IsClosing){
                UdpState s = new UdpState();
                s.e = ipEndPoint;
                s.u = mListenCLient;
                mListenCLient.BeginReceive(new AsyncCallback(ReceiveCallback), s);
            }

        }
        void ReciveData(IPEndPoint ipend,byte[] MSSG)
        {
            if (MSSG.Length == 0)
            {
                return;
            }
            UDPDATA tmpdata = null;
            try
            {
                tmpdata = DataConver.BytesToUDPDATA(MSSG);
            }
            catch (Exception ee)
            {
                LogHelper.Log(ee.ToString());
            }

            if (tmpdata != null)
            {
                if (tmpdata.udp_MesageID !=0) {
                    NeedReply.RemoveAll(delegate (SendActor tm) {
                        return tm.udpd.udp_MesageID == tmpdata.udp_MesageID;
                    });
                    mMessageID.RemoveID(tmpdata.udp_MesageID);
                }
                tmpdata.IPendpot = ipend;
                if (ShowReceiv)
                {
                    LogHelper.Log("收到:", tmpdata);
                }
                if (HandleMessage != null)
                {
                    HandleMessage(tmpdata);
                }
            }
        }
        public bool MUSPSend(int port, string msg, string ip = "127.0.0.1")
        {
            IPEndPoint mip = new IPEndPoint(IPAddress.Parse(ip), port);
            return MUSPSend(mip, msg);
        }
        public bool MUSPSend(IPEndPoint mip, string msg)
        {
            if (mip == null)
            {
                return false;
            }
            try
            {
                UDPDATA udpd = new UDPDATA();
                udpd.udp_CmdHead = 0;
                udpd.udp_isclient = 0;
                udpd.udp_HostID = 0;
                udpd.udp_Body = new LhwCode.UdpMessage(msg);
                MUSPSend(mip, udpd);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public void MUSPSend(IPEndPoint mip, UDPDATA udpd)
        {
            SendActor AC = new LhwCode.SendActor(mip, udpd);
            SendQueue.Enqueue(AC);
        }
        public bool RealMUSPSend(IPEndPoint mip, UDPDATA udpd)
        {
            if (mip == null)
            {
                return false;
            }
            try
            {
                udpd.udp_Body.SendTime = DateTime.Now;
                byte[] mdata = DataConver.UDPDATAToBytes(udpd);
                mListenCLient.Send(mdata, mdata.Length, mip);
                if (ShowSend)
                {
                    LogHelper.Log("发送",udpd);
                }
       
                return true;
            }
            catch(Exception ee)
            {
                LogHelper.Log(ee.ToString ());
                return false;
            }
        }
        public void CloseSocket()
        {
            IsClosing = true;
            mListenCLient.Close();
        }
        public void BroadCastMsg(string msg)
        {

            UDPDATA udpd = new UDPDATA();
            udpd.udp_CmdHead = 0;
            udpd.udp_isclient = 0;
            udpd.udp_HostID = 0;
            udpd.udp_Body = new LhwCode.UdpMessage (msg);
            IPEndPoint mip = new IPEndPoint(IPAddress.Broadcast, clientPort);
            MUSPSend(mip, udpd);

        }
        public void BroadCastMsg(string msg, int port)
        {
                UDPDATA udpd = new UDPDATA();
                udpd.udp_CmdHead = 0;
                udpd.udp_isclient = 0;
                udpd.udp_HostID = 0;
                udpd.udp_Body = new UdpMessage(msg);
                IPEndPoint mip = new IPEndPoint(IPAddress.Broadcast, port);
                MUSPSend(mip, udpd);
        }
        public void BroadCastMsg(UDPDATA udpd)
        {
            IPEndPoint mip = new IPEndPoint(IPAddress.Broadcast, clientPort);
            MUSPSend(mip ,udpd);
        }
        public void BroadCastMsg(UDPDATA udpd, int port)
        {
            IPEndPoint mip = new IPEndPoint(IPAddress.Broadcast, port);
            MUSPSend(mip, udpd);
        }
    }
}
