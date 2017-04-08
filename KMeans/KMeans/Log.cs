using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMeans
{
    public class Log
    {
        private static int indent;

        public static void Section(string info, Action action)
        {
            Write("> " + info);
            indent++;
            action();
            indent--;
            Write("< Ended '" + info + "'.");
        }

        public static void Write(string info)
        {
            Console.WriteLine("[" + DateTime.Now.ToString() + "]\t" + new string('-', indent * 3) + " " + info);
        }
    }
}
