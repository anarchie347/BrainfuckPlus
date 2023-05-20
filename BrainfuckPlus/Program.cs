
namespace BrainfuckPlus
{
    internal class Program
    {


        

        static void Main(string[] args)
        {
            ParsedOptions options = new ParsedOptions();
            options = CLI.Parse(args);

            //string testCodeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // get the folder for test code
            //test code is in this folder so it is tracked by git, so it can be used when cloning the repo
            //testCodeDirectory = Directory.GetParent(testCodeDirectory).Parent.Parent.Parent.Parent.ToString() + "\\testcode";
            //fileAddress = $"{testCodeDirectory}\\main.bfp";//GetFileAddress();

            string methodNames;
            if (options.Export)
            {
                Export.CreateCompressedFile(options.FileAddress, options.PreserveComments);
                Console.ReadKey();
                return;
            }
            string bfpcode = GetSourceCode.GetCode(options.FileAddress, true, out methodNames);

            string bfcode = ConvertToBF.Convert(bfpcode, methodNames, options.FileAddress, true);
            Console.WriteLine(bfpcode);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(bfcode);

            Console.ForegroundColor = ConsoleColor.Cyan;
            if (options.RunOutput)
                BFInterpreter.Run(bfcode, options.Debug);
            else
                Utils.WriteToFile(Path.ChangeExtension(options.FileAddress, "bf"), "bf", bfcode);
                
        }

        

        
    }
}