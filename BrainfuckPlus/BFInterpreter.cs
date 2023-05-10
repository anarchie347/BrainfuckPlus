using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Threading;

namespace BrainFuckPlus
{
    internal class BFInterpreter
    {
        public static void Run(string code, bool mayContainComments)
        {
            Console.OutputEncoding = Encoding.UTF8;

            //given memory number = number of bits
            byte[] Memory = new byte[30000];

            
            int currentMemoryPointerPosition = 0;
            int interpreterPosition = 0;
            int nestedLoopCount;

            //--------------------------------
            //           Run code
            //--------------------------------

            //Console.ForegroundColor = ConsoleColor.DarkCyan;
            //Console.WriteLine(code.Length);
            //Console.ForegroundColor = ConsoleColor.Green;
            //Console.WriteLine(Memory.Length);
            //Console.WriteLine("OK");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (interpreterPosition < code.Length)
            {

                switch (code[interpreterPosition])
                {
                    case '<':
                        if (currentMemoryPointerPosition == 0)
                        {

                            ErrorMessages.NegativePointerPosition(interpreterPosition);
                        }
                        else
                        {
                            currentMemoryPointerPosition--;
                            interpreterPosition++;
                        }
                        break;
                    case '>':
                        if (currentMemoryPointerPosition == Memory.Length - 1)
                        {
                            ErrorMessages.PointerOverflow(Memory.Length, interpreterPosition);
                        }
                        else
                        {
                            currentMemoryPointerPosition++;
                            interpreterPosition++;
                        }
                        break;
                    case '+':
                        Memory[currentMemoryPointerPosition]++;
                        interpreterPosition++;
                        break;
                    case '-':
                        Memory[currentMemoryPointerPosition]--;
                        interpreterPosition++;
                        break;
                    case '.':
                        try
                        {
                            //brainfuck uses 10 as newline. different OSs use different for newline
                            if (Memory[currentMemoryPointerPosition] == 10)
                                Console.WriteLine();
                            else
                                Console.Write((char)Memory[currentMemoryPointerPosition]);
                            interpreterPosition++;
                        }
                        catch
                        {
                            ErrorMessages.NoCorrespondingChar(Memory[currentMemoryPointerPosition], currentMemoryPointerPosition, interpreterPosition);
                        }
                        break;
                    case ',':
                        char keyInput = '\u0000';
                        ConsoleKeyInfo keyInputKeyInfo;
                        try
                        {
                            //brainfuck uses 10 as newline. different OSs use different for newline
                            keyInputKeyInfo = Console.ReadKey();
                            if (keyInputKeyInfo.Key == ConsoleKey.Enter)
                                keyInput = '\u000A';
                            else
                                keyInput = keyInputKeyInfo.KeyChar;
                            
                            Memory[currentMemoryPointerPosition] = (byte)keyInput;
                            interpreterPosition++;
                        }
                        catch
                        {
                            ErrorMessages.InvalidCharEntered(keyInput, interpreterPosition);
                        }
                        break;
                    case '[':
                        try
                        {
                            if (Memory[currentMemoryPointerPosition] == 0)
                            {
                                int searchForCloseBracketIndex = interpreterPosition + 1;
                                nestedLoopCount = 0;
                                while (nestedLoopCount != 1)
                                {
                                    //find the corresponding ], cannot look for previous one because of nested loops
                                    if (code[searchForCloseBracketIndex] == ']')
                                    {
                                        nestedLoopCount++;
                                    }
                                    else if (code[searchForCloseBracketIndex] == '[')
                                    {
                                        nestedLoopCount--;
                                    }
                                    if (searchForCloseBracketIndex == code.Length)
                                    {
                                        ErrorMessages.NoCorrespondinCloseBracket(interpreterPosition);
                                    }
                                    searchForCloseBracketIndex++;
                                }
                                searchForCloseBracketIndex--;
                                interpreterPosition = searchForCloseBracketIndex;
                                //Console.ForegroundColor = ConsoleColor.Green;
                                //Console.WriteLine(interpreterPosition);

                            }
                            else
                            {
                                interpreterPosition++;
                            }


                        }
                        catch
                        {

                        }
                        break;
                    case ']':
                        try
                        {
                            if (Memory[currentMemoryPointerPosition] != 0)
                            {
                                int searchForOpenBracketIndex = interpreterPosition - 1;
                                nestedLoopCount = 0;
                                while (nestedLoopCount != 1)
                                {
                                    //find the corresponding [, cannot look for previous one because of nested loops
                                    if (code[searchForOpenBracketIndex] == '[')
                                    {
                                        nestedLoopCount++;
                                    }
                                    else if (code[searchForOpenBracketIndex] == ']')
                                    {
                                        nestedLoopCount--;
                                    }

                                    if (searchForOpenBracketIndex == -1)
                                    {
                                        ErrorMessages.NoCorrespondingOpenBracket(interpreterPosition);
                                    }
                                    searchForOpenBracketIndex--;
                                }
                                searchForOpenBracketIndex++;
                                interpreterPosition = searchForOpenBracketIndex;
                                //Console.ForegroundColor = ConsoleColor.DarkBlue;
                                //Console.WriteLine(interpreterPosition);
                            }
                            else
                            {
                                interpreterPosition++;
                            }
                        }
                        catch
                        {

                        }
                        break;
                    case '~':
                        Console.ReadLine();
                        break;

                }
                string setTitle = "";
                for (int i = 0; i <= 9; i++)
                {
                    setTitle += Memory[i] + " ";
                }
                Console.Title = setTitle;
                //Console.Title = ($"Position: {currentMemoryPointerPosition}, value: {Memory[currentMemoryPointerPosition]}"); //debug mode
                //Console.Title = $"{Memory[0] - 48} {Memory[1] - 48} {Memory[2] - 48} {Memory[3] - 48} {Memory[4] - 48} {Memory[5] - 48}";
                //Console.Title = $"{Memory[0]} {Memory[1]} {Memory[2]} {Memory[3]} {Memory[4]} {Memory[5]} {Memory[6]} {Memory[7]}";
                //Console.ReadKey(true);  //step by step debugging
                //Thread.Sleep(5);
            }
            sw.Stop();
            Console.WriteLine($"Execution complete. Took {sw.ElapsedMilliseconds}ms");

            Console.ReadKey();
        }

        public static bool ContainsNonCodeChars(string code, string charSet)
        {
            for (int i = 0; i < charSet.Length; i++)
            {
                if (charSet.Contains(code[i]))
                    return true;
            }
            return false;
        }
    }

    public static class ErrorMessages
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

