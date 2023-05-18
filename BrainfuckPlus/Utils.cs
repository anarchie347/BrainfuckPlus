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
        public static void WriteToFile(string defaultAddress, string fileExtension, string value)
        {
            string input;
            if (File.Exists(defaultAddress))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("The file ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(defaultAddress);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" already exists");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Leave blank to overwrite it or enter a new file name: ");
            }
            else
            {
                Console.Write($"Do you want to save the file as ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(defaultAddress);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("?\nLeave blank to confirm or enter a new name");

            }
            input = Console.ReadLine() ?? string.Empty;
            if (input != string.Empty)
            {
                if (!Path.IsPathRooted(input))
                    input = Path.GetDirectoryName(defaultAddress) + Path.DirectorySeparatorChar + input;
                if (!Path.HasExtension(input))
                    input += "." + fileExtension;
                defaultAddress = input;
            }
            if (!Directory.Exists(Path.GetDirectoryName(defaultAddress)))
                Directory.CreateDirectory(Path.GetDirectoryName(defaultAddress));
            
            StreamWriter sw;
            sw = new StreamWriter(defaultAddress);
            sw.Write(value);
            sw.Close();

        }
    }
}
