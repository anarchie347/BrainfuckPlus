﻿using System;
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
        public static void CreateCompressedFile(string address, bool preserveComments)
        {
            string[] paths = GetFileAddresses(address);
            string currentDateTime = DateTime.Now.ToFileDateFormat();
            Path.GetTempFileName();
            string tempDir = Path.Combine(Path.GetDirectoryName(address),$"bfcompressiontemp_{currentDateTime}");
            try
            {

                Directory.CreateDirectory(tempDir);
                foreach (string path in paths)
                {
                    File.Copy(path, Path.Combine(tempDir, Path.GetFileName(path)));
                }
                ZipFile.CreateFromDirectory(tempDir, Path.Combine(Directory.GetParent(tempDir).ToString(), $"bfp_Program_{currentDateTime}.zip"));
                Console.WriteLine("Zip file successfully created");
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

    }
}
