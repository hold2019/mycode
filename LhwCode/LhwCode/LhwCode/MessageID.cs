using System;
using System.Collections.Generic;
using System.Text;

namespace LhwCode
{
    internal class MessageID
    {
        private List<int> mrandomlist = new List<int>();
        public int GetID() {
            Random torand = new Random();
            int tmpr = torand.Next(0,int.MaxValue);
            if (mrandomlist.Contains(tmpr))
            {
                return GetID();
            }
            else {
                return tmpr;
            }
        }
        public void RemoveID(int id) {
            mrandomlist.RemoveAll(delegate(int imk) {
                return imk == id;
            });
        }
    }
}
