using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckPlus
{
    internal static class Errors
    {
        public static void InvalidMemory(string errorInfo)
        {
            StartOfErrorMessage(-1);

            Console.WriteLine("Error: Invalid memory setting");
            Console.WriteLine("Memory setting must be a postive integer value and must be the only thing on the first line of the program");
            Console.WriteLine("The memory number represents the amount of memory cells required");
            Console.WriteLine("'" + errorInfo + "' is not a positive integer");
            EndOfErrorMessage();
        }

        public static void NegativePointerPosition(int interpreterPointerPosition)
        {
            StartOfErrorMessage(interpreterPointerPosition);
            Console.WriteLine("Error: Negative pointer position");
            Console.WriteLine("The position of the pointer must be >= 0");
            EndOfErrorMessage();
        }
        public static void PointerOverflow(int maxMemory, int interpreterPointerPosition)
        {
            StartOfErrorMessage(interpreterPointerPosition);
            Console.WriteLine("Error: pointer position must be less than the amount of available memory cells");
            Console.WriteLine("The maximum index of a memory cell is " + (maxMemory - 1) + " and the programme is trying to acces memory cell " + (maxMemory));
            EndOfErrorMessage();
        }

        //Interpreter now uses Byte data type (like most brainfuck programs) so the number wraps around and there is no overflow
        //public static void IntegerOverflow(int memoryPointerPostion , int interpreterPointerPosition)
        //{
        //    StartOfErrorMessage(interpreterPointerPosition);
        //    Console.WriteLine("Error: Integer overflow");
        //    Console.WriteLine("The maximum value for an integer is 2,147,483,647, and memory cell " + memoryPointerPostion + " exceeded this value");
        //    EndOfErrorMessage();
        //}

        //public static void IntegerUnderflow(int memoryPointerPostion, int interpreterPointerPosition)
        //{
        //    StartOfErrorMessage(interpreterPointerPosition);
        //    Console.WriteLine("Error: Integer underflow");
        //    Console.WriteLine("The minimum value for an integer is -2,147,483,648, and memory cell " + memoryPointerPostion + " was below this value");
        //    EndOfErrorMessage();
        //}

        public static void NoCorrespondingChar(int value, int memoryPointerPosition, int interpreterPointerPosition)
        {
            StartOfErrorMessage(interpreterPointerPosition);
            Console.WriteLine("Error: integer value cannot be converted to a unicode character");
            Console.WriteLine($"{value} in memory position {memoryPointerPosition} does not have a corresponding character");
            EndOfErrorMessage();
        }

        public static void InvalidCharEntered(char value, int interpreterPointerPosition)
        {
            StartOfErrorMessage(interpreterPointerPosition);
            Console.WriteLine("Error: invalid character entered by user");
            Console.WriteLine($"The user entered {value}, but this cannot be converted to an integer");
            EndOfErrorMessage();
        }

        public static void NoCorrespondingOpenBracket(int interpreterPointerPosition)
        {
            StartOfErrorMessage(interpreterPointerPosition);
            Console.WriteLine("Error: no corresponding [ was found");
            Console.WriteLine($"The ] at position {interpreterPointerPosition} did not have a corresponding [");
            EndOfErrorMessage();
        }

        public static void NoCorrespondinCloseBracket(int interpreterPointerPosition)
        {
            StartOfErrorMessage(interpreterPointerPosition);
            Console.WriteLine("Error: no corresponding ] was found");
            Console.WriteLine($"The [ at position {interpreterPointerPosition} did not have a corresponding ]");
            EndOfErrorMessage();
        }


        private static void StartOfErrorMessage(int interPreterPointerPosition)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Red;
            //Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Encountered an error at character " + interPreterPointerPosition.ToString());

        }

        private static void EndOfErrorMessage()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
