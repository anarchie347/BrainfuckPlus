using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckPlus
{
    internal static class GetBFSourceCode
    {
        public static void GetCode(string? address)
        {
            string RawText = (address == null ? InputCode.Input() : File.ReadAllText(address));
            string allowedCharSet = "+-,.<>[]";
            allowedCharSet += (address == null ? "" : GetAvailableMethodNames(address));
            Console.WriteLine(allowedCharSet);
        }

        public static string GetAvailableMethodNames(string address)
        {
            address = address.Substring(0, address.LastIndexOf('\\'));
            string[] fileNames;
            fileNames = Directory.GetFiles(address);
            fileNames = fileNames.Where(name => name.EndsWith(".bfp")).ToArray();
            fileNames = fileNames.Select(name => name[..^4]).ToArray(); //remove file extension
            fileNames = fileNames.Select(name => name[(name.LastIndexOf('\\') + 1)..]).ToArray(); //remove path to just leave filename
            string fileFirstChars = "";
            for (int i = 0; i < fileNames.Length; i++)
            {
                Console.WriteLine(fileNames[i]);
                fileFirstChars += fileNames[i][0];
            }
            return fileFirstChars;
        }


        
    }

}
