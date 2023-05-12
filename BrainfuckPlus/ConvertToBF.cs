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
        public static string Convert(string code, string methodNames,string fileAddress, bool debugMode)
        {
            string directory = Path.GetDirectoryName(fileAddress) ?? fileAddress;
            int j;
            string repetitionCounter;
            bool exit;

            for (int i = code.Length - 1; i >= 0; i--)
            {
                if (code[i] == Program.REPETITION_CHAR)
                {
                    j = i;
                    repetitionCounter = string.Empty;
                    exit = false;
                    do
                    { 
                        j++;

                        if (code.Length - 1 > j) {
                            exit = char.IsDigit(code[j]);
                            Console.WriteLine(exit);
                        }
                        
                        if (code.Length - 1 > j && (exit = char.IsDigit(code[j])))
                            repetitionCounter += code[j];
                    } while (!exit);
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(repetitionCounter);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            code = RecursiveFindSubstitutions(code, methodNames, directory, debugMode);

            return code;
        }

        public static string RecursiveFindSubstitutions(string code, string extraValidChars, string directory, bool debugMode)
        {
            for (int i = code.Length - 1; i >= 0; i--)
            {
                if (extraValidChars.Contains(code[i]))
                {
                    code = Substitute(code, i, extraValidChars, directory, debugMode);
                }
            }
            return code;
        }

        public static string Substitute(string code, int charIndex, string extraValidChars, string directory, bool debugMode)
        {
            string codeToInsert;// = File.ReadAllText($"{directory}/{code[charIndex]}.{Program.FILE_EXTENSION}");
            codeToInsert = GetSourceCode.GetCode(GetSourceCode.GetAddress(code[charIndex], directory), debugMode, out string v);
            codeToInsert = RecursiveFindSubstitutions(codeToInsert, extraValidChars, directory, debugMode);
            code = string.Concat(code.AsSpan(0,charIndex), codeToInsert, code.AsSpan(charIndex+1));

            return code;
        }
    }
}
