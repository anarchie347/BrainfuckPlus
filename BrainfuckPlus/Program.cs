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
             * ? - display help
             * 
             * the file path should follow the option
             * if the file path is after the main command and their is no option (like if the file was opened) do te equivalent of run
             * 
            */

            string? fileAddress = GetFileAddress();
            GetBFSourceCode.GetCode(fileAddress, true);
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