namespace BrainfuckPlus
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*Options
             * 
             * debug - transpile, run with debug characters enabled
             * transpile - transpile and output as a file
             * run - transpile, run without debug characters enabled
             * export - compresses the file and all other needed files into a .zip so it can be sent easily
             * ? - display help
             * 
             * 
             * the file path should follow the option
             * if the file path is after the main command and their is no option (like if the file was opened) do te equivalent of run
             * 
             * Extra parameters:
             * 
             * --obfuscate, -o
             * Obsfuscates the output by putting random new lines
             * 
             * --extremeobfuscate, -e
             * does regular obfuscate, but also adds lots of random, unnescessary characters
             * 
             * 
             *
             * 
            */

            string testCodeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // get the folder for test code
            //test code is in this folder so it is tracked by git, so it can be used when cloning the repo
            testCodeDirectory = Directory.GetParent(testCodeDirectory).Parent.Parent.Parent.Parent.ToString() + "/testcode";
            Console.WriteLine(testCodeDirectory);
            string? fileAddress = $"{testCodeDirectory}/main.bfp";//GetFileAddress();
            Console.WriteLine(GetSourceCode.GetCode(fileAddress, true));
        }

        static string? GetFileAddress()
        {
            string? FileAddress;
            do
            {
                Console.WriteLine("Enter File address");
                FileAddress = Console.ReadLine() ?? "";
            } while (FileAddress.Length == 0 || !File.Exists(FileAddress));
            return FileAddress.Length > 0 ? FileAddress.Replace('/', '\\') : null;
            

        }
    }
}