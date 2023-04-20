using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckPlus
{
    internal static class ConvertToBF
    {
        public static string Convert(string code, string extraValidChars,string fileAddress)
        {
            string directory = Path.GetDirectoryName(fileAddress) ?? fileAddress;
            code = RecursiveFindSubstitutions(code, extraValidChars, directory);

            return code;
        }

        public static string RecursiveFindSubstitutions(string code, string extraValidChars, string directory)
        {
            for (int i = 0; i < code.Length; i++)
            {
                if (extraValidChars.Contains(code[i]))
                {
                    code = Substitute(code, code[i], extraValidChars, directory);
                }
            }
            return code;
        }

        public static string Substitute(string code, int charIndex, string extraValidChars, string directory)
        {
            string codeToInsert = File.ReadAllText($"{directory}/{code[charIndex]}.{Program.FILE_EXTENSION}");
            codeToInsert = RecursiveFindSubstitutions(codeToInsert, extraValidChars, directory);
            code = string.Concat(code.AsSpan(0,charIndex), codeToInsert, code.AsSpan(charIndex+1));

            return code;
        }
    }
}
