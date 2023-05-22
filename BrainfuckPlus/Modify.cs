using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckPlus
{
    internal class Modify
    {
        public static void ShortenMethodNames(string address)
        {
            string[] paths = Utils.GetReferencedFileAddresses(address);
            string newPath, newFileName;
            try
            {
                foreach (string path in paths)
                {
                    newFileName = Path.ChangeExtension(Path.GetFileName(path)[0].ToString(), Syntax.FILE_EXTENSION);
                    newPath = Path.Combine(Path.GetDirectoryName(path), newFileName);
                    File.Move(path, newPath);
                    Console.WriteLine($"Successfully renamed {Path.GetFileName(path)} to {Path.GetFileName(newPath)}");
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
