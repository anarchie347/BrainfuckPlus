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
            code = ExpandRepetitions(code);
            code = RecursiveFindSubstitutions(code, methodNames, directory, debugMode);

            return code;
        }
        private static string ExpandRepetitions(string code)
        {
            string newCode = string.Empty;
            int j;
            string repetitionCounter;



            for (int i = 0; i < code.Length; i++)
            {
                if (code[i] == Program.REPETITION_CHAR)
                {
                    
                    j = i + 1;
                    repetitionCounter = string.Empty;
                    while (j < code.Length && char.IsDigit(code[j]))
                    {
                        repetitionCounter += code[j];
                        j++;
                    }

                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(repetitionCounter);
                    Console.WriteLine($"j={j}");
                    Console.ForegroundColor = ConsoleColor.White;

                    if (code.Length > j)
                        if (repetitionCounter == string.Empty)
                            throw new Exception("Oh no, there was no number for the shorthand repetition because it was not a number");
                        else if (code[j] == Program.REPETITION_CHAR)
                            throw new Exception("Oh no, there was no operater char at the end for the repetition because is was the repetition char");
                        else
                            newCode += new string(code[j], int.Parse(repetitionCounter));
                    else if (repetitionCounter == string.Empty)
                        throw new Exception("Oh no, there was no number for the shorthand repetition because the end of the string was reached");
                    else
                        throw new Exception("Oh no, there was no operater char at the end for the repetition because the end of the code was reached");
                    i = j;
                }
                else
                    newCode += code[i];
            }
            return newCode;
        }
 

        public static string RecursiveFindSubstitutions(string code, string methodNames, string directory, bool debugMode)
        {
            for (int i = code.Length - 1; i >= 0; i--)
            {
                if (methodNames.Contains(code[i]))
                {
                    code = Substitute(code, i, methodNames, directory, debugMode);
                }
            }
            return code;
        }

        public static string Substitute(string code, int charIndex, string methodNames, string directory, bool debugMode)
        {
            string codeToInsert;// = File.ReadAllText($"{directory}/{code[charIndex]}.{Program.FILE_EXTENSION}");
            codeToInsert = GetSourceCode.GetCode(GetSourceCode.GetAddress(code[charIndex], directory), debugMode, out string v);
            codeToInsert = RecursiveFindSubstitutions(codeToInsert, methodNames, directory, debugMode);
            code = string.Concat(code.AsSpan(0,charIndex), codeToInsert, code.AsSpan(charIndex+1));

            return code;
        }

        private static List<string> GetAndRemoveInjections(ref string code, int charIndex)
        {
            List<string> injections = new();
            charIndex++;
            while (code[charIndex] == Program.CODE_INJECTION_START_CHAR)
            {
                //code = code.Substring(0, charIndex) + GetBracketedContent(code.Substring(charIndex), Program.CODE_INJECTION_START_CHAR, Program.CODE_INJECTION_END_CHAR) + code.Substring();
            }
            return injections;
        }

        public static int GetClosingBracketIndex(string code, int openBracketIndex, char startBracket, char endBracket)
        {
            int index = openBracketIndex + 1;
            int nestedLoopCount = 0;
            while (nestedLoopCount != 1)
            {
                //find the corresponding close bracket, cannot look for next one because of nested loops
                if (code[index] == endBracket)
                {
                    nestedLoopCount++;
                }
                else if (code[index] == startBracket)
                {
                    nestedLoopCount--;
                }
                if (index == code.Length)
                {
                    throw new Exception($"no end bracket {endBracket} found");
                }
                index++;
            }

            return index - 1;
        }
    }
}
