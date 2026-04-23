using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sprh
{
    internal class ConsoleAddon
    {
        public void Error(string ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n[Error] " + ex);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void Error(string[] ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            for (int i = 0; i < ex.Length; i++)
            {
                Console.WriteLine("[Error] " + ex[i]);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void Warn(string ex)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[Warn] "+ex);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void Warn(string[] ex)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            for (int i = 0; i < ex.Length; i++)
            {
                Console.WriteLine("[Warn] " + ex[i]);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void log(string ex)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[Log] " + ex);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
