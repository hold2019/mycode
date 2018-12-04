using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace LhwCode
{
   public  class Agreement
    {
 
            protected Dictionary<string, string> LanguageTips = new Dictionary<string, string>();
            public char splitchar = '◇';
            /// <summary>
            /// 加载完成事件，一般是将这些语句放到要使用的地方
            /// </summary>
            public event Action<string> OnLoadingComplete = null;
            /// <summary>
            /// 添加一个语句
            /// </summary>
            /// <param name="lkey"></param>
            /// <param name="lvalue"></param>
            public void AddTips(string lkey, string lvalue)
            {
                LanguageTips.Add(lkey, lvalue);
            }
            /// <summary>
            /// 清除语句
            /// </summary>
            public void ClearTips()
            {
                LanguageTips.Clear();
            }
            /// <summary>
            /// 读取语言列表
            /// </summary>
            /// <param name="pth"></param>
            public virtual void LoadLanguage(string pth)
            {
                if (File.Exists(pth))
                {
                    LanguageTips.Clear();
                    StreamReader sr = new StreamReader(pth, System.Text.Encoding.UTF8);
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        string[] tmline = s.Split(splitchar);
                        if (tmline.Length > 1)
                        {
                            LanguageTips.Add(tmline[0], tmline[1]);
                        }
                    }
                    if (OnLoadingComplete != null)
                    {
                        OnLoadingComplete("");
                    }
                }
                else
                {
                    throw (new Exception("读取语言列表的路径:" + pth + "不存在"));
                }
            }
            /// <summary>
            /// 获取一个语句
            /// </summary>
            /// <param name="lkey"></param>
            /// <returns></returns>
            public string GetTips(string lkey)
            {
                if (LanguageTips.ContainsKey(lkey))
                {
                    return LanguageTips[lkey];
                }
                else
                {
                    return "None";
                }
            }
            /// <summary>
            /// 写语言列表
            /// </summary>
            /// <param name="pth"></param>
            public virtual void WriteLanguage(string pth)
            {
                FileStream fs = new FileStream(pth, FileMode.Create);
                StreamWriter sr = new StreamWriter(fs, System.Text.Encoding.UTF8);
                StringBuilder mbulid = new StringBuilder();
                foreach (var item in LanguageTips)
                {
                    mbulid.Append(item.Key);
                    mbulid.Append(splitchar);
                    mbulid.Append(item.Value);
                    mbulid.Append("\r\n");
                }
                sr.Write(mbulid.ToString());
                sr.Close();
                sr.Dispose();
                fs.Close();
                fs.Dispose();
            }
        }
    
}
