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
        public static ParsedOptions Parse(string[] args)//, out string fileAddress, out bool debug, out bool runOutput, out bool export, out bool brainfuck, out bool preserveComments, out ObfuscationLevel obfuscation)
        {
            ParsedOptionsBuilder parsedOptionsBuilder = new();
            string command;
            string[] parameters;
            //fileAddress = string.Empty;
            //debug= false;
            //runOutput= false;
            //export = false;
            //brainfuck= false;
            //preserveComments= false;
            //obfuscation = ObfuscationLevel.None;

            if (args.Length == 0 || args[0] == "?")
            {
                Help();
                Environment.Exit(0);
            }
            //accounts for ommitting the command when a file is opened with the program
            if (!new string[] { "transpile", "t", "export", "e", "run", "r" }.Contains(args[0]))
            {
                command = "run";
                parsedOptionsBuilder.FileAddress = args[0];
                parameters = new string[args.Length - 1];
                for (int i = 1; i < parameters.Length; i++)
                {
                    parameters[i] = args[i + 1];
                }
            }
            else if (args.Length == 1) //only command, no file address
            {
                throw new Exception("No file address given");
            }
            else
            {
                command = args[0];
                parsedOptionsBuilder.FileAddress = args[1];
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
                    parsedOptionsBuilder.RunOutput = true;
                    parsedOptionsBuilder.Export = false;
                    break;
                    

                case "transpile":
                case "t":
                    //transpile
                    parsedOptionsBuilder.RunOutput = false;
                    parsedOptionsBuilder.Export = false;
                    break;

                case "export":
                case "e":
                    //export
                    parsedOptionsBuilder.RunOutput = false;
                    parsedOptionsBuilder.Export = true;
                    break;     
            }

            parsedOptionsBuilder.Debug = parameters.Contains("--debug") || parameters.Contains("-d");
            if (parameters.Contains("--obfuscate") || parameters.Contains("-o")) parsedOptionsBuilder.Obfuscation = ObfuscationLevel.Normal;
            if (parameters.Contains("--extreme") || parameters.Contains("-e")) parsedOptionsBuilder.Obfuscation = ObfuscationLevel.Extreme;
            parsedOptionsBuilder.BrainfuckCode = parameters.Contains("--brainfuck") || parameters.Contains("-b");
            parsedOptionsBuilder.PreserveComments = parameters.Contains("--comments") || parameters.Contains("-c");


            if (!File.Exists(parsedOptionsBuilder.FileAddress))
                throw new Exception("File doesnt exist");

            return parsedOptionsBuilder.Build();
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

    public struct ParsedOptions
    {
        public string FileAddress;
        public bool Debug;
        public bool RunOutput;
        public bool Export;
        public bool BrainfuckCode;
        public bool PreserveComments;
        public ObfuscationLevel Obfuscation;
        public ParsedOptions(string fileAddress, bool debug, bool runOutput, bool export, bool brainfuckCode, bool preserveComments, ObfuscationLevel obfuscation)
        {
            FileAddress = fileAddress;
            Debug = debug;
            RunOutput = runOutput;
            Export = export;
            BrainfuckCode = brainfuckCode;
            PreserveComments = preserveComments;
            Obfuscation = obfuscation;
        }
    }

    public sealed class ParsedOptionsBuilder
    {
        public string FileAddress { get; set; }
        public bool Debug { get; set; }
        public bool RunOutput { get; set; }
        public bool Export { get; set; }
        public bool BrainfuckCode { get; set; }
        public bool PreserveComments { get; set; }
        public ObfuscationLevel Obfuscation { get; set; }

        public ParsedOptions Build()
        {
            return new ParsedOptions(FileAddress, Debug, RunOutput, Export, BrainfuckCode, PreserveComments, Obfuscation);
        }
    }
}
