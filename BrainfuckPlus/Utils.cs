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

        public static string ToFileDateFormat(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd_HH-mm-ss");
        }

        public static string ObfuscateNormal(string code)
        {
            Random r = new();
            for (int i = code.Length - 1; i > 0; i--)
            {
                if (r.Next(0, 1) == 0)
                    code = code.Insert(i, "\n");
            }
            return code;
        }
        public static string ObfuscateExtreme(string code, string dissallowedObfuscationChars, int extremeObfuscationCount)
        {
            Random r = new();
            StringBuilder sb = new("!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~\n");//all display chars in ascii + newline
            string obfuscationChars;
            int obfuscationCharsLen;
            for (int i = 0; i < dissallowedObfuscationChars.Length;i++)
                sb.Replace(dissallowedObfuscationChars[i], '\u0000');
            obfuscationChars = sb.ToString();
            obfuscationCharsLen = obfuscationChars.Length;

            for (int i = 0; i <= code.Length; i++)
            {
                if (r.Next(0, extremeObfuscationCount) > 0)
                    code = code.Insert(i, obfuscationChars[r.Next(0, obfuscationCharsLen)].ToString());
            }
            return code;
        }
    }
}
