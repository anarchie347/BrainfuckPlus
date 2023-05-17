using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BrainfuckPlus
{
    internal static class Utils
    {
        public static void WriteToFile(string address, string value)
        {
            StreamWriter sw;
            if (!File.Exists(address))
            {
                sw = new StreamWriter(address);
                sw.Write(value);
                sw.Close();
                return;
            }
            while (File.Exists(address) || address == string.Empty)
            {
                Console.WriteLine($"The file {address} already exists");
                Console.WriteLine("Please enter a new filename/path or leave blank to overwrite: ");
                address = Console.ReadLine() ?? string.Empty;
            }

        }
    }
}
