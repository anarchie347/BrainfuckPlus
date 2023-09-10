using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckPlus
{
    internal class Modify
    {
        public static void ShortenMethodNames(string address, bool showOutputMessages)
        {
            string[] paths = Utils.GetReferencedFileAddresses(address);
            ShortenMethodNames(address, paths, showOutputMessages);
        }
        public static void ShortenMethodNames(string address, string[] paths, bool showOutputMessages)
        {
            string newPath, newFileName;
            try
            {
                foreach (string path in paths)
                {
                    newFileName = Path.ChangeExtension(Path.GetFileName(path)[0].ToString(), Syntax.FILE_EXTENSION);
                    newPath = Path.Combine(Path.GetDirectoryName(path), newFileName);
                    File.Move(path, newPath);
                    if (showOutputMessages)
                        Console.WriteLine($"Successfully renamed {Path.GetFileName(path)} to {Path.GetFileName(newPath)}");
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void Filter(string address, bool removeDebug, bool removeComments)
        {
            string[] paths = Utils.GetReferencedFileAddresses(address);
            Utils.FilterCode(paths, removeDebug, removeComments, true);
            if (removeDebug)
                Console.WriteLine("Debug charactes successfully removed");
            if (removeComments)
                Console.WriteLine("Comments successfully removed");
            if (!(removeComments || removeDebug))
                Console.WriteLine("You somehow modified nothing... hmmm\nThis shouldn't happen");
        }
    }
}
