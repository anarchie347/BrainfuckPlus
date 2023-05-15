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
        public static void Parse(string[] args, out bool debug, out bool runOutput, out bool export, ObfuscationLevel obfuscation)
        {
            string fileAddress;
            string command;
            string[] parameters;
            debug= false;
            runOutput= false;
            export= false;
            obfuscation = ObfuscationLevel.None;

            if (args[0] == "?")
            {
                Help();
                return;
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
                    break;

                case "transpile":
                case "t":
                    //transpile
                    break;

                case "export":
                case "e":
                    //export
                    break;

                
            }

        }

        private static void Help()
        {
            Console.WriteLine("HELP");
        }
    }
    public enum ObfuscationLevel { None, Normal, Extreme }
   
}
