using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EventManager1.Models
{
    public class CusomlogWriter
    {
        public static void Loghacksawgaming(string str, string fileprefix = "")
        {
            try
            {
                string filename = DateTime.UtcNow.Day + "_" + DateTime.UtcNow.Month + "_" + DateTime.UtcNow.Year + ".txt";
                if (!string.IsNullOrEmpty(fileprefix))
                {
                    filename = fileprefix + "_" + filename;
                }
                string path = HttpContext.Current.Server.MapPath("/Content//Log/" + filename);
                File.AppendAllText(path, str + Environment.NewLine + "LogTime:- " + DateTime.UtcNow + Environment.NewLine + Environment.NewLine);
            }
            catch (Exception ex)
            {

            }
        }
    }
}