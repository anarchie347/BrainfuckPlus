using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace BrainfuckPlus
{
    internal static class Utils
    {
        public static void WriteToFile(string defaultAddress, string fileExtension, string value)
        {
            StreamWriter sw;
            sw = new StreamWriter(GetOutputAddress(defaultAddress, fileExtension));
            sw.Write(value);
            sw.Close();

        }

        public static string GetOutputAddress(string defaultAddress, string fileExtension)
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

            return defaultAddress;
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


        public static string[] GetReferencedFileAddresses(string address)
        {
            List<string> paths = RecursiveGetFiles(address);
            List<string> pathsNoDuplicates = new();
            pathsNoDuplicates.Add(address);
            for (int i = 0; i < paths.Count; i++)
            {
                if (!pathsNoDuplicates.Contains(paths[i]))
                    pathsNoDuplicates.Add(paths[i]);
            }
            return pathsNoDuplicates.ToArray();
        }
        private static List<string> RecursiveGetFiles(string address)
        {
            List<string> paths = new();
            string? methodpath;
            string[] code = File.ReadAllText(address).Split(Environment.NewLine).Select(x => x.Trim()).ToArray();
            for (int i = 0; i < code.Length; i++)
            {
                for (int j = 0; j < code[i].Length; j++)
                {
                    if (code[i][j] == Syntax.COMMENT_CHAR)
                        break;
                    if (!(Syntax.EXTRA_ALLOWED_CHARS.Contains(code[i][j]) || Syntax.BF_VALID_CHARS.Contains(code[i][j]) || char.IsWhiteSpace(code[i][j])))
                    {
                        methodpath = ConvertToBF.GetAddress(code[i][j], Path.GetDirectoryName(address));
                        if (methodpath != null && !paths.Contains(methodpath))
                        {
                            paths.Add(methodpath);
                            paths.AddRange(RecursiveGetFiles(methodpath));
                        }

                    }
                }
            }

            return paths;
        }

        public static void FilterCode(string[] paths, bool removeDebug, bool removeComments, bool removeWhiteSpace, string methods = "")
        {
            if (methods == "")
                methods = string.Join("", Array.ConvertAll(paths, p => Path.GetFileName(p)[0]));
            string fileText;
            string allowedCharSet = Syntax.BF_VALID_CHARS + methods;
            if (!removeDebug)
                allowedCharSet += Syntax.DEBUG_CHARS;
            foreach (string path in paths)
            {
                fileText = File.ReadAllText(path);
                if (removeComments)
                {
                    fileText = Utils.RemoveComments(fileText);
                    fileText = Utils.RemoveInvalidChars(fileText, allowedCharSet, !removeWhiteSpace);
                }
                File.WriteAllText(path, fileText);
            }
        }

        public static void FilterCodeOLD(string[] paths, bool removeDebug, bool removeComments)
        {
            char[] methods = Array.ConvertAll(paths, p => Path.GetFileName(p)[0]);
            string fileText;
            StringBuilder filteredCode;
            foreach (string path in paths)
            {
                fileText = File.ReadAllText(path);
                filteredCode = new StringBuilder();
                for (int i = 0; i < fileText.Length; i++)
                {
                    if (fileText.Length > i + Syntax.BLOCK_COMMENT_START_STRING.Length - 1 && fileText.Substring(i, Syntax.BLOCK_COMMENT_START_STRING.Length) == Syntax.BLOCK_COMMENT_START_STRING)
                    {
                        int endBlockComment;
                        if (fileText.Length == i + Syntax.BLOCK_COMMENT_START_STRING.Length - 1)
                            endBlockComment = fileText.Length - 1;
                        else
                        {
                            endBlockComment = fileText.IndexOf(Syntax.BLOCK_COMMENT_END_STRING, i + Syntax.BLOCK_COMMENT_START_STRING.Length + 1);
                            if (endBlockComment == -1)
                                endBlockComment= fileText.Length - 1;
                        }
                        i = endBlockComment - 1; //i is incremented at the end of the for loop
                    }
                    else if (fileText[i] == Syntax.COMMENT_CHAR)
                    {
                        if (!removeComments)
                            filteredCode.Append(fileText.Substring(i));
                        break;
                    }
                    else if (Syntax.DEBUG_CHARS.Contains(fileText[i]))
                    {
                        if (!removeDebug)
                            filteredCode.Append(fileText[i]);
                    }
                    else
                    {
                        if (removeComments)
                        {
                            if (methods.Contains(fileText[i]) || Syntax.EXTRA_ALLOWED_CHARS.Contains(fileText[i]) || Syntax.BF_VALID_CHARS.Contains(fileText[i]) || char.IsWhiteSpace(fileText[i]))
                                filteredCode.Append(fileText[i]);
                        }
                        else
                            filteredCode.Append(fileText[i]);
                    }
                }
                File.WriteAllText(path, filteredCode.ToString());
            }
        }
        public static bool CheckChar(string text, int index, char checkChar)
        {
            if (index >= text.Length)
                return false;
            return text[index] == checkChar;
        }
        public static bool CheckSequence(string text, int startIndex, string checkSequence)
        {
            if (startIndex + checkSequence.Length > text.Length)
                return false;
            for (int i = 0; i < checkSequence.Length; i++)
            {
                if (text[startIndex + i] != checkSequence[i])
                    return false;
            }
            return true;
        }
        public static int GetInclusiveLengthBetweenIndexes(int index1, int index2)
        {
            return index2 - index1 + 1;
        }

        public static string RemoveComments(string code)
        {
            string[] lines = code.Split(Environment.NewLine);
            bool isInComment = false;
            int blockCommentStart, blockCommentEnd, commentStart;
            bool keepLooping;

            for (int i = 0; i < lines.Length; i++)
            {
                blockCommentStart = -1;
                blockCommentEnd = -1;
                commentStart = -1;
                keepLooping = true;
                do
                {
                    if (!isInComment)
                    {
                        blockCommentStart = lines[i].IndexOf(Syntax.BLOCK_COMMENT_START_STRING);//start search at beginning because the previous comment will have been removed from the string
                        if (blockCommentStart > -1)
                        {
                            isInComment = true;
                        }
                        else
                        {
                            commentStart = lines[i].IndexOf(Syntax.COMMENT_CHAR);
                            if (commentStart > -1)
                            {
                                lines[i] = lines[i].Substring(0, commentStart);
                            }
                            keepLooping = false;
                        }
                    }
                    else
                    {
                        blockCommentEnd = LengthCheckIndexOf(lines[i], Syntax.BLOCK_COMMENT_END_STRING, GetSearchStartPoint(blockCommentStart, false));
                        if (blockCommentEnd > -1)
                        {
                            isInComment = false;
                            if (blockCommentStart == -1)
                            {
                                //block comment from previous line
                                lines[i] = lines[i].Substring(blockCommentEnd + Syntax.BLOCK_COMMENT_END_STRING.Length);
                            }
                            else
                            {
                                //block comment from same line
                                lines[i] = lines[i].Remove(blockCommentStart, Utils.GetInclusiveLengthBetweenIndexes(blockCommentStart, blockCommentEnd + Syntax.BLOCK_COMMENT_END_STRING.Length - 1));
                            }
                        }
                        else
                        {
                            keepLooping = false;
                        }
                    }
                } while (keepLooping);
                if (isInComment)
                {
                    if (blockCommentStart == -1)
                    {
                        lines[i] = ""; //line was a full block comment
                    }
                    else
                    {
                        lines[i] = lines[i].Substring(0, blockCommentStart);
                    }

                }
            }
            //this should remove comments
            return string.Join("\n", lines);
        }

        private static int LengthCheckIndexOf(string str, string searchFor, int start)
        {
            if (start >= str.Length)
                return -1;
            return str.IndexOf(searchFor, start);
        }
        private static int GetSearchStartPoint(int PrevCommentTokenIndex, bool IsSearchingForStartComment)
        {
            if (PrevCommentTokenIndex == -1)
                return 0;

            int PrevCommentTokenLength = IsSearchingForStartComment ? Syntax.BLOCK_COMMENT_END_STRING.Length : Syntax.BLOCK_COMMENT_START_STRING.Length;

            return PrevCommentTokenIndex + PrevCommentTokenLength;
        }

        public static string RemoveInvalidChars(string code, string allowedCharSet, bool allowWhiteSpace)
        {
            string newCode = "";
            bool followingRepetitionChar = false;
            for (int i = 0; i < code.Length; i++)
            {
                if (allowedCharSet.Contains(code[i]) || followingRepetitionChar && char.IsDigit(code[i]) || (allowWhiteSpace && char.IsWhiteSpace(code[i])))
                    newCode += code[i];
                followingRepetitionChar = (code[i] == Syntax.REPETITION_CHAR) || (followingRepetitionChar && char.IsDigit(code[i])); //preserves number after repetition char
            }
            return newCode;
        }
    }
}
