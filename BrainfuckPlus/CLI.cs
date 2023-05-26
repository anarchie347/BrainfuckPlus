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
			 * transpile, t - transpile and output as a file
			 * run, r - transpile, run without debug characters enabled
			 * export, e - compresses the file and all other needed files into a .zip so it can be sent easily
			 * help, ? - display help
			 * 
			 * 
			 * the file path should follow the option
			 * if the file path is after the main command and their is no option (like if the file was opened) do the equivalent of run
			 * 
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
		public static ParsedOptions Parse(string[] args)//, out string fileAddress, out bool debug, out bool runOutput, out bool export, out bool brainfuck, out bool removeComments, out ObfuscationLevel obfuscation)
		{
			ParsedOptionsBuilder parsedOptionsBuilder = new();
			string command;
			string[] parameters;
			//fileAddress = string.Empty;
			//debug= false;
			//runOutput= false;
			//export = false;
			//brainfuck= false;
			//removeComments= false;
			//obfuscation = ObfuscationLevel.None;

			if (args.Length == 0 || args[0] == "?" || args[0] == "help")
			{
				Help();
				Environment.Exit(0);
			}
			//accounts for ommitting the command when a file is opened with the program
			if (!new string[] { "transpile", "t", "export", "e", "run", "r", "modify", "m" }.Contains(args[0]))
			{
				command = "run";
				parsedOptionsBuilder.FileAddress = args[0];
				parameters = new string[args.Length - 1];
				for (int i = 0; i < parameters.Length; i++)
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
				for (int i = 0; i < parameters.Length; i++)
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
					parsedOptionsBuilder.Modify = false;
					break;
					

				case "transpile":
				case "t":
					//transpile
					parsedOptionsBuilder.RunOutput = false;
					parsedOptionsBuilder.Export = false;
					parsedOptionsBuilder.Modify = false;
					break;

				case "export":
				case "e":
					//export
					parsedOptionsBuilder.RunOutput = false;
					parsedOptionsBuilder.Export = true;
					parsedOptionsBuilder.Modify = false;
					break;

				case "modify":
				case "m":
					//modify
					parsedOptionsBuilder.RunOutput = false;
					parsedOptionsBuilder.Export = false;
					parsedOptionsBuilder.Modify = true;
					break;
					
			}
			if (command == "run" || command == "r" || command == "transpile" || command == "t")
				parsedOptionsBuilder.Debug = parameters.Contains("--debug") || parameters.Contains("-d");
			else
				parsedOptionsBuilder.Debug = !(parameters.Contains("--removedebug") || parameters.Contains("-rd"));           

			if (parameters.Contains("--obfuscate") || parameters.Contains("-o"))
				parsedOptionsBuilder.Obfuscation = ObfuscationLevel.Normal;

			if (parameters.Contains("--extremeobfuscate") || parameters.Contains("-eo"))
				parsedOptionsBuilder.Obfuscation = ObfuscationLevel.Extreme;

			parsedOptionsBuilder.BrainfuckCode = parameters.Contains("--brainfuck") || parameters.Contains("-bf");

			parsedOptionsBuilder.RemoveComments = parameters.Contains("--removecomments") || parameters.Contains("-rc");

			parsedOptionsBuilder.OutputPath = string.Empty;
			for (int i = 0; i < parameters.Length; i++)
				if (parameters[i].StartsWith("--name="))
					if (parsedOptionsBuilder.OutputPath == string.Empty)
						parsedOptionsBuilder.OutputPath = parameters[i].Substring(7);
					else
						throw new Exception("Two output paths given");

			parsedOptionsBuilder.ExtremeObfuscationCount = null;
			for (int i = 0; i < parameters.Length; i++)
				if (parameters[i].StartsWith("--eocount="))
					if (parsedOptionsBuilder.ExtremeObfuscationCount == null)
						if (int.TryParse(parameters[i].Substring(10), out int num) && num > 1)
							parsedOptionsBuilder.ExtremeObfuscationCount = num;
						else
							throw new Exception($"--ecount expected an int greater than 1. '{parameters[i].Substring(10)}' was not an int greater than 1");
					else
						throw new Exception("Two eocounts given");
			
			parsedOptionsBuilder.ShortenMethodNames = parameters.Contains("--shortenmethodnames") || parameters.Contains("-sm");

			if (!File.Exists(parsedOptionsBuilder.FileAddress))
				throw new Exception("File doesnt exist");

			ValidateOptions(parsedOptionsBuilder);

			return parsedOptionsBuilder.Build();
		}
		private static void ValidateOptions(ParsedOptionsBuilder options)
		{

			//options list

			//obfuscation
			//debug
			//brainfuck
			//removecomments
			//shortenmethodnames
			//extremeobfuscationcount
			//outputname

			//if run
			//allowed params: debug, brainfuck
			if (options.RunOutput)
			{
				if (options.Obfuscation != ObfuscationLevel.None)
					throw new Exception("obfuscation is not compatible with run");
				if (options.RemoveComments)
					throw new Exception("removecomments is not compatible with run");
				if (options.ShortenMethodNames)
					throw new Exception("shortenmethodnames is not compatible with run");

				if (options.ExtremeObfuscationCount != null)
                    throw new Exception("eocount is not compatible with run");
				if (options.OutputPath != string.Empty)
                    throw new Exception("name is not compatible with run");
            }
			//if transpile
			//allowed params: obfuscation, debug, outputname, eocount
			else if (!(options.RunOutput || options.Export || options.Modify))
			{
				if (options.BrainfuckCode)
					throw new Exception("brainfuck is not compatible with transpile");
				if (options.RemoveComments)
					throw new Exception("removecomments is not compatible with transpile");
				if (options.ShortenMethodNames)
					throw new Exception("shortenmethodnames is not compatible with transpile");
			}
			//if export
			//allowed params: removecomments, debug, name
			else if (options.Export)
			{
				if (options.Obfuscation != ObfuscationLevel.None)
					throw new Exception("obfuscation is not compatible with export (it is planned though)");
				if (options.BrainfuckCode)
					throw new Exception("brainfuck is not compatible with export");

                if (options.ExtremeObfuscationCount != null)
                    throw new Exception("eocount is not compatible with export");
            }
			//if modify
			//allowed params: removecomments, shortenmethodnames, debug
			else
			{
				if (options.Obfuscation != ObfuscationLevel.None)
					throw new Exception("obfuscation is not compatible with modify (it is planned though)");
				if (options.BrainfuckCode)
					throw new Exception("brainfuck is not compatible with modify");

                if (options.ExtremeObfuscationCount != null)
                    throw new Exception("eocount is not compatible with modify");
                if (options.OutputPath != string.Empty)
                    throw new Exception("name is not compatible with modify");
            }
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
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine("Command                    | Explanation");
			Console.WriteLine("---------------------------+------------------------------------");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("run, r                     | transpiles to brainfuck and executes a given code file"); //implemented
			Console.WriteLine("transpile, t               | transpiles to brainfuck and outputs a brainfuck file"); //implemented
			Console.WriteLine("export, e                  | finds all required methods, and compresses them to a zip file"); //implemented
			Console.WriteLine("modify, m                  | applies given parameter operations to all files in a directory or used by a file"); //implemented
			Console.WriteLine("help, ?                    | show this menu"); //implemented
			Console.WriteLine("");
			Console.WriteLine("");
			Console.ForegroundColor = ConsoleColor.Magenta;
			Console.WriteLine("Flags:");
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine("Flag                       | Valid commands             | Explanation");
			Console.WriteLine("---------------------------+--------------------+------------------------------------");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("--obfuscate,  -o           | transpile                  | Obfuscates the source code by adding random newline characters"); //implemented
			Console.WriteLine("--extremeobfuscate,  -eo   | transpile                  | Obfuscates the source code by adding random newline characters and random characters"); //implemented
			Console.WriteLine("--debug,  -d               | transpile, run             | Allows the use of debug characters. These features will likely not be supported on other brainfuck interpreters"); //implemented
			Console.WriteLine("--brainfuck,  -bf          | run                        | Interprets the code as brainfuck, rather than brainfuckplus"); //implemented
			Console.WriteLine("--removecomments,  -rc     | export, modify             | Removes comments on exported code"); //implemented
			Console.WriteLine("--shortenmethodnames, -sm  | modify, export             | Shortens all method names to only the first character"); //implemented
			Console.WriteLine("--removedebug, -rd         | modify, export             | Removes debug characters from exported/modifed code");
			Console.WriteLine("");
			Console.ForegroundColor = ConsoleColor.Magenta;
			Console.WriteLine("Value Parameters:");
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine("Parameter                  | Value type  | Valid commands     | Explanation");
			Console.WriteLine("---------------------------+-------------+--------------------+------------------------------------");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("--name=value               | string      | transpile, export  | File path (absolute or relative) for the outputted zip/bf file (do not include file extension)"); //implemented
			Console.WriteLine("--eocount=value            | integer     | transpile          | Sets how much a file is extremely obfuscated by. Only used if extreme obfuscation flag is present"); //implemented

		}
	}
	public enum ObfuscationLevel { None, Normal, Extreme }

	public struct ParsedOptions
	{
		public const int DEFAULT_EXTREME_OBFUSCATION_COUNT = 100;
		public string FileAddress { get; init; }
		public bool Debug { get; init; }
		public bool RunOutput { get; init; }
		public bool Export { get; init; }
		public bool BrainfuckCode { get; init; }
		public bool RemoveComments { get; init; }
		public string OutputPath { get; init; }
		public int ExtremeObfuscationCount { get; init; }
		public bool ShortenMethodNames { get; init; }
		public bool Modify { get; init; }
		public ObfuscationLevel Obfuscation { get; init; }
		public ParsedOptions(string fileAddress, bool debug, bool runOutput, bool export, bool brainfuckCode, bool removeComments, string outputPath, int? extremeObfuscationCount, bool shortenMethodNames, bool modify, ObfuscationLevel obfuscation)
		{
			FileAddress = fileAddress;
			Debug = debug;
			RunOutput = runOutput;
			Export = export;
			BrainfuckCode = brainfuckCode;
			RemoveComments = removeComments;
			OutputPath = outputPath;
			ExtremeObfuscationCount = extremeObfuscationCount ?? DEFAULT_EXTREME_OBFUSCATION_COUNT;
			ShortenMethodNames = shortenMethodNames;
			Modify = modify;
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
		public bool RemoveComments { get; set; }
		public string OutputPath { get; set; }
		public int? ExtremeObfuscationCount { get; set; }
		public bool ShortenMethodNames { get; set; }
		public bool Modify { get; set; }
		public ObfuscationLevel Obfuscation { get; set; }


		public ParsedOptions Build()
		{
			return new ParsedOptions(FileAddress, Debug, RunOutput, Export, BrainfuckCode, RemoveComments, OutputPath, ExtremeObfuscationCount, ShortenMethodNames, Modify, Obfuscation);
		}
	}
}
