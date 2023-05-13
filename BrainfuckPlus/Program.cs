

namespace BrainfuckPlus
{
    internal class Program
    {


        public const string BF_VALID_CHARS = "[],.+-<>";
        public const string DEBUG_CHARS = @"\:?""|"; //chars that are not allowed in Windows fle names -> cant be methods (on windows). doesnt include /,<,>, these are used for other purposes
        public const char COMMENT_CHAR = '/';
        public const char CODE_INJECTION_START_CHAR = '{';
        public const char CODE_INJECTION_END_CHAR = '}';
        public const char REPETITION_CHAR = '*';
        public const string FILE_EXTENSION = "bfp";
        public static readonly string EXTRA_ALLOWED_CHARS = new string(new char[] { COMMENT_CHAR, CODE_INJECTION_START_CHAR, CODE_INJECTION_END_CHAR, REPETITION_CHAR });

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
             * Debug chars:
             * \ waits for enter key to be pressed (Console.ReadLine();)
             * : outputs the current position of the pointer
             * ? outputs the integer value stored in the current cell (not translated using ascii)
             * " waits 0.1s
             * | increments a hidden counter then outputs its value (can be used to keep track of loops). This counter can only be accessed by this debug character
             * 
            */
            


            string testCodeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // get the folder for test code
            //test code is in this folder so it is tracked by git, so it can be used when cloning the repo
            testCodeDirectory = Directory.GetParent(testCodeDirectory).Parent.Parent.Parent.Parent.ToString() + "\\testcode";
            Console.WriteLine(testCodeDirectory);
            string methodNames;
            string? fileAddress = $"{testCodeDirectory}\\main.bfp";//GetFileAddress();
            string bfpcode = GetSourceCode.GetCode(fileAddress, true, out methodNames);

            string bfcode = ConvertToBF.Convert(bfpcode, methodNames, fileAddress, true);
            Console.WriteLine(bfpcode);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(bfcode);

            Console.ForegroundColor = ConsoleColor.Cyan;
            BFInterpreter.Run(bfcode, false);

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