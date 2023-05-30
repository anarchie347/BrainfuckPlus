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
        public static void CreateCompressedFile(string address, bool removeDebug, bool removeComments, bool shortenMethodNames, string? outputName = null)
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
                paths = paths.Select(p => Path.Combine(tempDir, Path.GetFileName(p))).ToArray();
                if (removeComments || removeDebug)
                    Utils.FilterCode(paths, removeDebug, removeComments);
                if (shortenMethodNames)
                    Modify.ShortenMethodNames(address, paths, false);
                if (string.IsNullOrEmpty(outputName))
                    outputPath = Path.Combine(Path.GetDirectoryName(address), $"bfp_Program_{currentDateTime}");
                else if (Path.IsPathRooted(outputName))
                    outputPath = outputName;
                else
                    outputPath = Path.Combine(Path.GetDirectoryName(address), outputName);
                outputPath = Path.ChangeExtension(outputPath, "zip");
                if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                outputPath = Utils.GetOutputAddress(outputPath, "zip");
                if (File.Exists(outputPath))
                {
                    File.Delete(outputPath);
                    Console.WriteLine($"Original file {outputPath} has been deleted");
                }
                    
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
        

        

    }
}
