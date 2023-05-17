
namespace BrainfuckPlus
{
    internal class Program
    {


        

        static void Main(string[] args)
        {
            string fileAddress;
            bool debug, runOutput, export, brainfuck, preserveComments;
            ObfuscationLevel obfuscation;
            CLI.Parse(args, out debug, out runOutput, out export, out brainfuck, out preserveComments, out obfuscation);

            string testCodeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // get the folder for test code
            //test code is in this folder so it is tracked by git, so it can be used when cloning the repo
            testCodeDirectory = Directory.GetParent(testCodeDirectory).Parent.Parent.Parent.Parent.ToString() + "\\testcode";
            Console.WriteLine(testCodeDirectory);
            string methodNames;
            fileAddress = $"{testCodeDirectory}\\main.bfp";//GetFileAddress();
            string bfpcode = GetSourceCode.GetCode(fileAddress, true, out methodNames);

            string bfcode = ConvertToBF.Convert(bfpcode, methodNames, fileAddress, true);
            Console.WriteLine(bfpcode);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(bfcode);

            Console.ForegroundColor = ConsoleColor.Cyan;
            if (runOutput)
                BFInterpreter.Run(bfcode, debug);
                

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