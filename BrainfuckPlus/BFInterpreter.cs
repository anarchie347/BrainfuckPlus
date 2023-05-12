using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Threading;

namespace BrainfuckPlus
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
            int debugHiddenCounter = 0;

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

                            Errors.BFInterpreter.NegativePointerPosition(interpreterPosition);
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
                            Errors.BFInterpreter.PointerOverflow(Memory.Length, interpreterPosition);
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
                            Errors.BFInterpreter.NoCorrespondingChar(Memory[currentMemoryPointerPosition], currentMemoryPointerPosition, interpreterPosition);
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
                            Errors.BFInterpreter.InvalidCharEntered(keyInput, interpreterPosition);
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
                                        Errors.BFInterpreter.NoCorrespondinCloseBracket(interpreterPosition);
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
                                        Errors.BFInterpreter.NoCorrespondingOpenBracket(interpreterPosition);
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


                    //---------------
                    //-----DEBUG-----
                    //---------------
                    case '\\':
                        Console.ReadLine();
                        interpreterPosition++;
                        break;
                    case ':':
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine($"DEBUG: Current pointer position = {currentMemoryPointerPosition}");
                        Console.ForegroundColor = ConsoleColor.White;
                        interpreterPosition++;
                        break;       
                    case '?':
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine($"DEBUG: Current cell value = {Memory[currentMemoryPointerPosition]}");
                        Console.ForegroundColor = ConsoleColor.White;
                        interpreterPosition++;
                        break;
                    case '"':
                        Thread.Sleep(100);
                        interpreterPosition++;
                        break;
                    case '|':
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        debugHiddenCounter++;
                        Console.WriteLine($"DEBUG: Current debug hidden counter value  = {debugHiddenCounter}");
                        Console.ForegroundColor = ConsoleColor.White;
                        interpreterPosition++;
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
}

