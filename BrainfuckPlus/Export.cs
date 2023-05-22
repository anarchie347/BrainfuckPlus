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
            string[] paths = Utils.GetReferencedFileAddresses(address);
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

                if (string.IsNullOrEmpty(outputName))
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
        

        private static void RemoveChars(string[] paths)
        {
            char[] methods = Array.ConvertAll(paths, p => Path.GetFileName(p)[0]);
            string temp;
            StringBuilder sb;
            foreach (string path in paths)
            {
                temp = File.ReadAllText(path);
                sb = new StringBuilder();
                for (int i = 0; i < temp.Length; i++)
                {
                    if (Syntax.COMMENT_CHAR == temp[i])
                        break;
                    if (methods.Contains(temp[i]) || Syntax.EXTRA_ALLOWED_CHARS.Contains(temp[i]) || Syntax.BF_VALID_CHARS.Contains(temp[i]) || Syntax.DEBUG_CHARS.Contains(temp[i]) ||  char.IsWhiteSpace(temp[i]))
                    {
                        sb.Append(temp[i]);
                    }
                }
                File.WriteAllText(path, sb.ToString());
            }
        }

    }
}
