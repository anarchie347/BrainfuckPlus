using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrainfuckPlus
{
    internal static class GetSourceCode
    {
        
        public static string GetCode(string? address, bool debugMode, out string methodNames)
        {
            methodNames = string.Empty;
            string code = (address == null ? InputCode.Input() : File.ReadAllText(address));
            string allowedCharSet = Program.BF_VALID_CHARS;

            allowedCharSet += Program.COMMENT_CHAR;
            if (address != null)
            {
                methodNames = GetAvailableMethodNames(address);
                allowedCharSet += methodNames;
            }
            if (debugMode) allowedCharSet += Program.DEBUG_CHARS;
            Console.WriteLine(allowedCharSet);

            code = RemoveCommentsAndNewLines(code);
            code = RemoveInvalidChars(code, allowedCharSet);


            return code;
        }

        public static string GetAvailableMethodNames(string? address)
        {
            address = Path.GetDirectoryName(address);
            if (address == null)
                return "";
            string[] fileNames;
            fileNames = Directory.GetFiles(address);
            fileNames = fileNames.Where(name => name.EndsWith('.' + Program.FILE_EXTENSION)).ToArray();
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
                index = lines[i].IndexOf(Program.COMMENT_CHAR);
                resultingCode += (index == -1 ? lines[i] : lines[i][..index]);
            }
            return resultingCode;
        }

        
        public static string RemoveInvalidChars(string code, string allowedCharSet)
        {
            string newCode = "";
            for (int i = 0; i < code.Length; i++)
            {
                if (allowedCharSet.Contains(code[i])) newCode += code[i];
            }
            return newCode;
        }
    }

}
