using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Diagnostics.CodeAnalysis;

namespace BrainfuckPlus
{
    internal class Export
    {
        public static void CreateCompressedFile(string address, bool removeComments, string? outputName = null)
        {
            string[] paths = GetFileAddresses(address);
            string currentDateTime = DateTime.Now.ToFileDateFormat();
            string tempDir = Path.Combine(Path.GetDirectoryName(address),$"bfcompressiontemp_{currentDateTime}");
            string outputPath;
            try
            {

                Directory.CreateDirectory(tempDir);
                foreach (string path in paths)
                {
                    File.Copy(path, Path.Combine(tempDir, Path.GetFileName(path)));
                }
                if (removeComments)
                    RemoveChars(paths.Select(p => Path.Combine(tempDir, Path.GetFileName(p))).ToArray());

                if (outputName == null)
                    outputPath = Path.Combine(Path.GetDirectoryName(address), $"bfp_Program_{currentDateTime}");
                else if (Path.IsPathRooted(outputName))
                    outputPath = outputName;
                else
                    outputPath = Path.Combine(Path.GetDirectoryName(address), outputName);
                outputPath = Path.ChangeExtension(outputPath, "zip");
                if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                ZipFile.CreateFromDirectory(tempDir, outputPath);
                Console.WriteLine("Zip file successfully created at " + outputPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }
        private static string[] GetFileAddresses(string address)
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

        private static void RemoveChars(string[] paths)
        {
            Console.WriteLine("ASD");
            char[] methods = Array.ConvertAll(paths, p => Path.GetFileName(p)[0]);
            string temp;
            StringBuilder sb;
            foreach (string path in paths)
            {
                temp = File.ReadAllText(path);
                sb = new StringBuilder();
                for (int i = 0; i < temp.Length; i++)
                {
                    if (methods.Contains(temp[i]) || (Syntax.EXTRA_ALLOWED_CHARS.Contains(temp[i]) && Syntax.COMMENT_CHAR != temp[i]) || Syntax.BF_VALID_CHARS.Contains(temp[i]) || char.IsWhiteSpace(temp[i]))
                    {
                        sb.Append(temp[i]);
                    }
                }
                File.WriteAllText(path, sb.ToString());
            }
        }

    }
}
