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
        
        public static string GetCode(string address, bool debugMode, out string methodNames)
        {
            string code = File.ReadAllText(address);
            string allowedCharSet = Program.BF_VALID_CHARS;

            allowedCharSet += Program.EXTRA_ALLOWED_CHARS;
            methodNames = GetAvailableMethodNames(address);
            allowedCharSet += methodNames;
            if (debugMode) allowedCharSet += Program.DEBUG_CHARS;

            code = RemoveCommentsAndNewLines(code);
            code = RemoveInvalidChars(code, allowedCharSet);

            return code;
        }

        public static string? GetAddress(char chr, string directory)
        {
            //gets the first file it finds that starts with the corresponding character
            bool nameCheck(string name) //check if the name begins with chr and is a .bfp file (only checked if the file has an extension, not all OSs enforce file extensions
            {
                return Path.GetFileNameWithoutExtension(name).StartsWith(chr) && (!Path.HasExtension(name) || (Path.GetExtension(name) == $".{Program.FILE_EXTENSION}"));
            };
            return Directory.GetFiles(directory).Where(name => nameCheck(name)).ToArray().FirstOrDefault();
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
            fileNames = fileNames.Select(name => Path.GetFileName(name)).ToArray(); //remove directory to just leave filename
            string fileFirstChars = "";
            for (int i = 0; i < fileNames.Length; i++)
                fileFirstChars += fileNames[i][0];
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
