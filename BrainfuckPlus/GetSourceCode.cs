using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            string allowedCharSet = Syntax.BF_VALID_CHARS;

            allowedCharSet += Syntax.EXTRA_ALLOWED_CHARS;
            methodNames = GetAvailableMethodNames(address);
            allowedCharSet += methodNames;
            if (debugMode) allowedCharSet += Syntax.DEBUG_CHARS;

            code = Utils.RemoveComments(code);
            code = Utils.RemoveInvalidChars(code, allowedCharSet, false);
            
            return code;
        }



        public static string GetAvailableMethodNames(string? address)
        {
            address = Path.GetDirectoryName(address);
            if (address == null)
                return "";
            string[] fileNames;
            fileNames = Directory.GetFiles(address);
            fileNames = fileNames.Where(name => Path.GetExtension(name) == $".{Syntax.FILE_EXTENSION}").ToArray();
            fileNames = fileNames.Where(name => !char.IsWhiteSpace(name[0])).ToArray();
            fileNames = fileNames.Select(name => Path.GetFileNameWithoutExtension(name)).ToArray();
            string fileFirstChars = "";
            for (int i = 0; i < fileNames.Length; i++)
                fileFirstChars += fileNames[i][0];
            return fileFirstChars;
        }



        
        /*public static string RemoveCommentsAndNewLinesOLD(string code)
        {
            string[] lines = code.Split(Environment.NewLine);
            string resultingCode = "";
            int index;
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Trim();
                index = lines[i].IndexOf(Syntax.COMMENT_CHAR);
                if (index == -1)
                {
                    resultingCode += lines[i];
                } else if (i > Syntax.BLOCK_COMMENT_END_STRING.Length - 2 && lines[i].Substring(i - Syntax.BLOCK_COMMENT_END_STRING.Length + 1) == Syntax.BLOCK_COMMENT_END_STRING)
                {
                    //end comment char, so leave in
                    //ARGH THIS NEEDS TO BE IN A LOOP COS MULTIPLE bLOCK COMMENTS PER LINE

                    //REWRITE THIS AND REMOVEBLOCKCOMMENTS FROM SCRATCH
                }
                    
                resultingCode += (index == -1 ? lines[i] : lines[i][..index]);
            }
            resultingCode = RemoveBlockComments(resultingCode);
            return resultingCode;
            
        }*/

        /*public static string RemoveBlockCommentsOLD(string code)
        {
            int startOfBlockComment = 0;
            int endOfBlockComment;
            StringBuilder newCode = new(code);
            while ((startOfBlockComment = code.IndexOf(Syntax.BLOCK_COMMENT_START_STRING, startOfBlockComment)) != -1)
            {
                if (code.Length > startOfBlockComment + Syntax.BLOCK_COMMENT_START_STRING.Length - 1)
                {
                    endOfBlockComment = code.IndexOf(Syntax.BLOCK_COMMENT_END_STRING, startOfBlockComment + Syntax.BLOCK_COMMENT_END_STRING.Length + 1);
                    if (endOfBlockComment == -1)
                        endOfBlockComment = code.Length - 1;
                }
                else
                {
                    endOfBlockComment = -1;

                }
                newCode.Remove(startOfBlockComment, endOfBlockComment - startOfBlockComment + 1);
            }
            return newCode.ToString();
        }*/

        
        
    }

}
