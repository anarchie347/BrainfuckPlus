using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BrainfuckPlus.Program;

namespace BrainfuckPlus
{
    internal class CLI
    {
        /*Options
             * 
             * transpile - transpile and output as a file
             * run - transpile, run without debug characters enabled
             * export - compresses the file and all other needed files into a .zip so it can be sent easily
             * ? - display help
             * 
             * 
             * the file path should follow the option
             * if the file path is after the main command and their is no option (like if the file was opened) do the equivalent of run
             * 
             * Extra parameters:
             * 
             * --obfuscate, -o
             * Obsfuscates the output by putting random new lines
             * 
             * --extremeobfuscate, -e
             * does regular obfuscate, but also adds lots of random, unnescessary characters
             * 
             * --debug, -d
             * Allow debug characters
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
        public static void Parse(string[] args, out bool debug, out bool runOutput, out bool export, out bool brainfuck, out bool preserveComments, out ObfuscationLevel obfuscation)
        {
            string fileAddress;
            string command;
            string[] parameters;
            debug= false;
            runOutput= false;
            export = false;
            brainfuck= false;
            preserveComments= false;
            obfuscation = ObfuscationLevel.None;

            if (args[0] == "?")
            {
                Help();
                Environment.Exit(0);
            }
            //accounts for ommitting the command whe a file is opened with the program
            if (!new string[] { "transpile", "t", "export", "e", "run", "r" }.Contains(args[0]))
            {
                command = "run";
                fileAddress = args[0];
                parameters = new string[args.Length - 1];
                for (int i = 1; i < parameters.Length; i++)
                {
                    parameters[i] = args[i + 1];
                }
            } else
            {
                command = args[0];
                fileAddress= args[1];
                parameters = new string[args.Length - 2];
                for (int i = 2; i < parameters.Length; i++)
                {
                    parameters[i] = args[i + 2];
                }
            }
            

            switch (command)
            {
                case "run":
                case "r":
                    //run
                    runOutput = true;
                    export = false;
                    break;
                    

                case "transpile":
                case "t":
                    //transpile
                    runOutput = false;
                    export = false;
                    break;

                case "export":
                case "e":
                    //export
                    runOutput = false;
                    export = false;
                    break;     
            }

            debug = parameters.Contains("--debug") || parameters.Contains("-d");
            if (parameters.Contains("--obfuscate") || parameters.Contains("-o")) obfuscation = ObfuscationLevel.Normal;
            if (parameters.Contains("--extreme") || parameters.Contains("-e")) obfuscation = ObfuscationLevel.Extreme;
            brainfuck = parameters.Contains("--brainfuck") || parameters.Contains("-b");
            preserveComments = parameters.Contains("--comments") || parameters.Contains("-c");


            if (!File.Exists(fileAddress))
                throw new Exception("File doesnt exist");
        }

        private static void Help()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("--------------");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Brainfuck Plus");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("--------------");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Github: https://github.com/anarchie347/BrainfuckPlus");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Commands:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("run                        | transpiles to brainfuck and executes a given code file");
            Console.WriteLine("transpile                  | transpiles to brainfuck and outputs a brainfuck file");
            Console.WriteLine("export                     | finds all required methods, and compresses it to a zip file");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Parameters:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("--obfuscate,  -o           | Obfuscates the source code by adding random newline characters");
            Console.WriteLine("--extremeobfuscate,  -e    | Obfuscates the source code by adding random newline characters and random characters");
            Console.WriteLine("--debug,  -d               | Allows the use of debug characters. These features will likely not be supported on other brainfuck interpreters");
            Console.WriteLine("--brainfuck,  -b           | Interprets the code as brainfuck, rather than brainfuck plus");
            Console.WriteLine("--comments,  -c            | Preservers comments. Only available for transpile and export");


        }
    }
    public enum ObfuscationLevel { None, Normal, Extreme }
   
}
