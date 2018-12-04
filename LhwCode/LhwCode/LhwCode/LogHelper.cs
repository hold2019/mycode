using System;
using System.Collections.Generic;
using System.Text;

namespace LhwCode
{
    public class LogHelper
    {
        public static Action<string> LogAct = null;
        public static void Log(string mes)
        {
            if (LogAct != null)
            {
                LogAct(mes);
            }
        }
        public static void Log(string tag, string ms)
        {
            if (LogAct != null)
            {
                LogAct(tag + ":" + ms);
            }
        }
        public static void Log(object mes)
        {
            if (LogAct != null)
            {
                LogAct(mes.ToString ());
            }
        }
        public static void Log(string tag, object mes)
        {
            if (LogAct != null)
            {
                LogAct(tag+">>"+mes.ToString());
            }
        }
        public static void Log(byte [] bt) {
            string mes = "";
            foreach (var item in bt)
            {
                mes += item.ToString("X2")+" ";
            }
        }
    }
}
