using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
namespace LhwCode
{
    public class UDPDATA
    {
        public int udp_CmdHead;
        public int udp_isclient;
        public int udp_HostID;
        public int udp_MesageID = 0;
        public UdpMessage udp_Body;
        public IPEndPoint IPendpot = null;
        public bool NeedReply = false;
        public int SendCout = 0;
        public UDPDATA(string msg)
        {
            udp_Body = new UdpMessage(msg);
        }
        public UDPDATA()
        {
            udp_Body = new UdpMessage("");
        }
        public override string ToString()
        {
            return udp_Body.SendTime + ":"+ udp_MesageID+"," + udp_CmdHead+","+ udp_isclient+","+ udp_HostID+","+ udp_Body.ToString ();
        }
    }
    public class UdpMessage {
        public string mainMessage;
        public DateTime SendTime = DateTime.Now;
        public UdpMessage(string mes) {
            mainMessage = mes;
        }
        public byte[] ToBytes() {
            string msg = SendTime.ToString()+ "⊕" + mainMessage;
            return System.Text.Encoding.UTF8.GetBytes (msg);
        }
        public void GetBody(byte[] bt) {
            string ms = System.Text.Encoding.UTF8.GetString(bt);
            string[] resd = ms.Split('⊕');
            mainMessage = resd[1];
            SendTime = DateTime.Parse(resd[0]);
        }
        public override string ToString()
        {
            return mainMessage;
        }
    }
    public class DataConver
    {
        public static UDPDATA BytesToUDPDATA(byte[] res)
        {
            UDPDATA tosend = new UDPDATA();
            byte[] byte_MesageID = BitConverter.GetBytes(tosend.udp_MesageID);
            byte[] byte_head = BitConverter.GetBytes(tosend.udp_CmdHead);
            byte[] byte_isclient = BitConverter.GetBytes(tosend.udp_isclient);
            byte[] byte_HostID = BitConverter.GetBytes(tosend.udp_HostID);

            byte[] udp_Body = new byte[res.Length - byte_head.Length - byte_isclient.Length - byte_HostID.Length- byte_MesageID.Length];

            Array.Copy(res, 0, byte_MesageID, 0, byte_MesageID.Length);
            Array.Copy(res, byte_MesageID.Length, byte_head, 0, byte_head.Length);
            Array.Copy(res, byte_MesageID.Length+byte_head.Length, byte_isclient, 0, byte_isclient.Length);
            Array.Copy(res, byte_MesageID.Length+byte_head.Length + byte_isclient.Length, byte_HostID, 0, byte_HostID.Length);
            Array.Copy(res, byte_MesageID.Length+byte_head.Length + byte_isclient.Length + byte_HostID.Length, udp_Body, 0, udp_Body.Length);
            tosend.udp_CmdHead = BitConverter.ToInt32(byte_head, 0);
            tosend.udp_isclient = BitConverter.ToInt32(byte_isclient, 0);
            tosend.udp_HostID = BitConverter.ToInt32(byte_HostID, 0);
            tosend.udp_MesageID = BitConverter.ToInt32(byte_MesageID, 0);
            tosend.udp_Body = new UdpMessage("");
            tosend.udp_Body.GetBody(udp_Body);
            return tosend;
        }
        public static byte[] UDPDATAToBytes(UDPDATA tosend)
        {
            tosend.udp_Body.SendTime = DateTime.Now;
            byte[] byte_MesageID = BitConverter.GetBytes(tosend.udp_MesageID);
            byte[] byte_head = BitConverter.GetBytes(tosend.udp_CmdHead);
            byte[] byte_isclient = BitConverter.GetBytes(tosend.udp_isclient);
            byte[] byte_HostID = BitConverter.GetBytes(tosend.udp_HostID);
            byte[] udp_Body = tosend.udp_Body.ToBytes();
            byte[] tosenddata = new byte[byte_MesageID.Length +byte_head.Length + byte_isclient.Length + byte_HostID.Length + udp_Body.Length];

            Array.Copy(byte_MesageID, 0, tosenddata, 0, byte_MesageID.Length);
            Array.Copy(byte_head, 0, tosenddata, byte_MesageID.Length,byte_head.Length);
            Array.Copy(byte_isclient, 0, tosenddata, byte_MesageID.Length+byte_head.Length, byte_isclient.Length);
            Array.Copy(byte_HostID, 0, tosenddata,byte_MesageID.Length+byte_isclient.Length + byte_head.Length, byte_HostID.Length);
            Array.Copy(udp_Body, 0, tosenddata, byte_MesageID.Length+byte_isclient.Length + byte_head.Length + byte_HostID.Length, udp_Body.Length);
            return tosenddata;
        }
    }
}
