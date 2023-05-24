
namespace BrainfuckPlus
{
    internal class Program
    {
        static void Main(string[] args)
        {

            string methodNames;
            ParsedOptions options = CLI.Parse(args);
            

            //string testCodeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // get the folder for test code
            //test code is in this folder so it is tracked by git, so it can be used when cloning the repo
            //testCodeDirectory = Directory.GetParent(testCodeDirectory).Parent.Parent.Parent.Parent.ToString() + "\\testcode";
            //fileAddress = $"{testCodeDirectory}\\main.bfp";//GetFileAddress();

            
            if (options.Export)
            {
                Export.CreateCompressedFile(options.FileAddress, options.RemoveComments, !options.Debug, options.OutputPath);
                Console.ReadKey();
                return;
            }
            if (options.BrainfuckCode)
            {
                string code = File.ReadAllText(options.FileAddress);
                BFInterpreter.Run(code, true);
                return;
            }
            if (options.Modify)
            {
                if (options.ShortenMethodNames)
                    Modify.ShortenMethodNames(options.FileAddress);
                return;
            }
            string sourcecode = GetSourceCode.GetCode(options.FileAddress, options.Debug, out methodNames);
            string bfcode = ConvertToBF.Convert(sourcecode, methodNames, options.FileAddress, options.Debug, options.Obfuscation, options.ExtremeObfuscationCount);

            if (!options.RunOutput)
            {
                string outputPath;
                if (string.IsNullOrEmpty(options.OutputPath))
                    outputPath = options.FileAddress;
                else if (Path.IsPathRooted(options.OutputPath))
                    outputPath = options.FileAddress;
                else
                    outputPath = Path.Combine(Path.GetDirectoryName(options.FileAddress), options.OutputPath);

                outputPath = Path.ChangeExtension(outputPath, "bf");
                Utils.WriteToFile(outputPath, "bf", bfcode);
            }

            if (options.RunOutput)
            {
                
                BFInterpreter.Run(bfcode, options.Debug);
                return;
            }

        }

        

        
    }
}