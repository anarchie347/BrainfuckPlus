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
        public const string BF_VALID_CHARS = "[],.+-<>";
        public const string DEBUG_CHARS = @"\:*?""|"; //chars that are not allowed in Windows fle names -> cant be methods (on windows). doesnt include /,<,>, these are used for other purposes
        public const char COMMENT_CHAR = '/';
        public static void GetCode(string? address, bool debugMode)
        {
            string RawText = (address == null ? InputCode.Input() : File.ReadAllText(address));
            string allowedCharSet = BF_VALID_CHARS;
            if (address != null) allowedCharSet += GetAvailableMethodNames(address);
            if (debugMode) allowedCharSet += DEBUG_CHARS;
            Console.WriteLine(allowedCharSet);
        }

        public static string GetAvailableMethodNames(string? address)
        {
            address = Path.GetDirectoryName(address);
            if (address == null)
                return "";
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

        public static string RemoveCommentsAndNewLines(string code)
        {
            string[] lines = code.Split(Environment.NewLine);
            string resultingCode = "";
            int index;
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Trim();
                index = lines[i].IndexOf(COMMENT_CHAR);
                resultingCode += (index == -1 ? lines[i] : lines[i][..index]);
            }
            return resultingCode;
        }

        
        
    }

}
