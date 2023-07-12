using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckPlus
{
    internal static class ConvertToBF
    {
        public static string Convert(string code, string methodNames,string fileAddress, bool debugMode, ObfuscationLevel obfuscation, int eocount)
        {
            string directory = Path.GetDirectoryName(fileAddress) ?? fileAddress;
            code = ExpandRepetitions(code);
            code = RecursiveFindSubstitutions(code, methodNames, directory, debugMode);
            if (obfuscation == ObfuscationLevel.Normal)
                code = Utils.ObfuscateNormal(code);
            else if (obfuscation == ObfuscationLevel.Extreme)
                code = Utils.ObfuscateExtreme(code, Syntax.DEBUG_CHARS + Syntax.BF_VALID_CHARS, eocount);

            return code;
        }
        private static string ExpandRepetitions(string code)
        {
            StringBuilder newCode = new();
            int j;
            string repetitionCounter;
            int endInjectIndex;
            string injectCall;
            int endBlockIndex;
            string repeatingBlock;


            for (int i = 0; i < code.Length; i++)
            {
                if (code[i] == Syntax.REPETITION_CHAR)
                {
                    
                    j = i + 1;
                    repetitionCounter = string.Empty;
                    while (j < code.Length && char.IsDigit(code[j]))
                    {
                        repetitionCounter += code[j];
                        j++;
                    }

                    if (code.Length > j)
                        if (repetitionCounter == string.Empty)
                            throw new Exception("Oh no, there was no number for the shorthand repetition because it was not a number");
                        else if (code[j] == Syntax.REPETITION_CHAR)
                            throw new Exception("Oh no, there was no operater char at the end for the repetition because is was the repetition char");
                        else if (code[j] == Syntax.CODE_INJECTION_CALL_START_CHAR)
                        {
                            endInjectIndex = GetClosingBracketIndex(code, j, Syntax.CODE_INJECTION_CALL_START_CHAR, Syntax.CODE_INJECTION_CALL_END_CHAR);
                            injectCall = code.Substring(j, endInjectIndex - j + 1); //include ( )
                            StringBuilder sb = new();
                            for (int k = 0; k < int.Parse(repetitionCounter); k++)
                            {
                                sb.Append(injectCall);
                            }
                            j = endInjectIndex;
                            newCode.Append(sb);
                        }
                        else if (code[j] == Syntax.CODE_INJECTION_START_CHAR)
                        {
                            endBlockIndex = GetClosingBracketIndex(code, j, Syntax.CODE_INJECTION_START_CHAR, Syntax.CODE_INJECTION_END_CHAR);
                            repeatingBlock = code.Substring(j + 1, endBlockIndex - j - 1); //dont include { }
                            StringBuilder sb = new();
                            for (int k = 0; k < int.Parse(repetitionCounter); k++)
                            {
                                sb.Append(repeatingBlock);
                            }
                            j = endBlockIndex;
                            newCode.Append(sb);
                        }
                        else
                            newCode.Append(new string(code[j], int.Parse(repetitionCounter)));

                    else if (repetitionCounter == string.Empty)
                        throw new Exception("Oh no, there was no number for the shorthand repetition because the end of the string was reached");
                    else
                        throw new Exception("Oh no, there was no operater char at the end for the repetition because the end of the code was reached");
                    i = j;
                }
                else
                    newCode.Append(code[i]);
            }
            return newCode.ToString();
        }

        public static string? GetAddress(char chr, string directory)
        {
            //gets the first file it finds that starts with the corresponding character
            bool nameCheck(string name) //check if the name begins with chr and is a .bfp file (only checked if the file has an extension, not all OSs enforce file extensions
            {
                return Path.GetFileNameWithoutExtension(name).StartsWith(chr) && (!Path.HasExtension(name) || (Path.GetExtension(name) == $".{Syntax.FILE_EXTENSION}"));
            };
            string[] paths = Directory.GetFiles(directory).Where(name => nameCheck(name)).ToArray();
            if (paths.Length > 1) throw new Exception($"Ambiguous method call for {chr}");
            if (paths.Length == 0) return null;
            return paths[0];
        }

        public static string RecursiveFindSubstitutions(string code, string methodNames, string directory, bool debugMode)
        {
            List<string>? injections;
            for (int i = code.Length - 1; i >= 0; i--)
            {
                if (methodNames.Contains(code[i]))
                {
                    if ( i < (code.Length - 1) && code[i + 1] == Syntax.CODE_INJECTION_START_CHAR)
                        injections = GetAndRemoveInjections(ref code, i);
                    else
                        injections = null;
                    code = Substitute(code, i, methodNames, directory, debugMode, injections);
                }
            }
            return code;
        }

        public static string Substitute(string code, int charIndex, string methodNames, string directory, bool debugMode, List<string>? injections = null)
        {
            string codeToInsert;// = File.ReadAllText($"{directory}/{code[charIndex]}.{Program.FILE_EXTENSION}");
            string? address = GetAddress(code[charIndex], directory);
            if (address == null) return code.Remove(charIndex,1);
            codeToInsert = GetSourceCode.GetCode(address, debugMode, out string v);
            codeToInsert = ExpandRepetitions(codeToInsert);
            if (injections != null) codeToInsert = Inject(codeToInsert, injections);


            codeToInsert = RecursiveFindSubstitutions(codeToInsert, methodNames, directory, debugMode);
            code = string.Concat(code.AsSpan(0,charIndex), codeToInsert, code.AsSpan(charIndex+1));

            return code;
        }

        public static List<string> GetAndRemoveInjections(ref string code, int charIndex)
        {
            //also make it work recursively
            List<string> injections = new();
            charIndex++;
            int closebracketIndex;
            while (charIndex < code.Length && code[charIndex] == Syntax.CODE_INJECTION_START_CHAR)
            {
                closebracketIndex = GetClosingBracketIndex(code, charIndex, Syntax.CODE_INJECTION_START_CHAR, Syntax.CODE_INJECTION_END_CHAR);
                injections.Add(code.Substring(charIndex + 1, closebracketIndex - charIndex - 1));

                code = code.Substring(0,charIndex) + code.Substring(closebracketIndex + 1);
            }
            return injections;
        }

        public static string Inject(string code, List<string> injections)
        {
            int index, closeCallIndex, injectionIndex;
            while ((index = code.IndexOf(Syntax.CODE_INJECTION_CALL_START_CHAR)) != -1)
            {
                closeCallIndex = GetClosingBracketIndex(code, index, Syntax.CODE_INJECTION_CALL_START_CHAR, Syntax.CODE_INJECTION_CALL_END_CHAR);
                if (!int.TryParse(code.AsSpan(index + 1, closeCallIndex - index -1), out injectionIndex))
                    throw new Exception("Injection index was not an int");
                if (injectionIndex >= injections.Count)
                    code = code.Substring(0, index) + code.Substring(closeCallIndex + 1); //empty substitution
                else
                    code = code.Substring(0,index) + injections[injectionIndex] + code.Substring(closeCallIndex + 1); //substitute
            }
            return code;
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
