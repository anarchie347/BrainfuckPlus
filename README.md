# BrainfuckPlus

I have done some brainfuck coding. It is painful (as it is meant to be)

BrainfuckPlus aims to keep the same mind hurting style as brainfuck and translates directly to brainfuck at runtime

You can write code in brainfuck Plus, then export it as regular brainfuck

I also plan to maybe make a brainfuckPlusPlus in the future, which deviates more from brainfuck, so wont easily transpile to brainfuck. A planned feature for brainfuckPlusPlus is functions which can have their own memory, separate to the memory of the program

## Features

### Transpiles to brainfuck

All code is directly translatable to brainfuck

The interpreter for the code only uses brainfuck code (as well as the debug characters)

All code is converted to brainfuck at runtime

### Methods

Reuse code easier by creating methods.

Methods are replaced with their regular brainfuck code when executing

Methods are written in brainfuck, in a file called `something.bfp`, and the character used to call the method is the first character

Method names have some restrictions. THey cannot use any brainfuck characters `[],.+-<>`, any of the debug chars `\\:?"|` , the comment char `/`, the code injection start end end chars `{}` the repetition char `*`, the characters used for calling injected code `()`, any numbers `0123456789` or whitespace

Hence the list of disallowed characters is `[],.+-<>\\:?"|/{}*()0123456789` and whitespaces

Some of these characters may change in the future, but they are not planned to change

These restrictions only apply to the first lettter of the filename because this is the name of the method

All methods that a programme uses must be in the same directory as the main code file (might add a way to make methods available for all projects later)

At runtime, all methods calls are substituted with their corresponding brainfuck code, so **recursion is NOT possible** because it will cause a crash by endlessly substituting the method inside itself. This could be fixed by editing the interpreter, but this go against one of the main principles of brainfuckPlus - it transpiles directly (and easily) to brainfuck

### Code injections

Allows you to pass code as a parameter to methods

Works similar to passing functions as methods in most languages

For example, you could create a fucnction that functionsd as a for loop: you pass it start and end count paramters using the memory array, and pass it code to run in the loop as a parameter

At runtime, the injected code is substituted into the method which is then substituted into the main code to create valid brainfuck code

The syntax for this is putting the injection code inside curly braces `{}`. Code injections can be nested (injecting code can contain other methods that themselves have injecting code)

You can pass multiple sections of code as parameters. To do this, simply add multiple injections after the call. as long as there are no characters between the end of the last injection `}` and the start of the next injection `{`, they will all be presumed to be for the same method call

If a function calls a code injections that has not been passed (e.g. `a` should have 2 inejctions, but only one is passed) then all undefined injections will be empty strings, meaning no code is executed

The call the injected code from inside your function, put a number inside brackets `()` corresponding to which injection argument you are using. This number must be hard coded

The injection order is 0 indexed

e.g.
In main.bfp
```
a{++}{++}
```
In a.bfp
```
(0)>*34(1).
```
Method `a` takes two injections, applies the first to the current cell, and the second is applied to the cell afterwards 32 times, then outputs that value. The output of this code would be `D` (ascii value for 68)

### Shorthand Repetition

Tired of writing + 65 times in order to be able to output A?

use a \* followed by a positive integer followed by the method to repeat that method that amount of times

Like methods, they are converted to brainfuck when executing

e.g `*10+` is the same as `++++++++++`

### Debug characters

Some characters (currently \:?"|) can be used to output information about the current state of the program. These commands are only allowed if the programme is running in debug mode, so should not be part of a final programme

\ - waits for the enter key to be pressed (similar to Console.ReadLine())

: - outputs the current position of the pointer

? - outputs the integer value stored in the current cell (not translated using ascii)

" - waits 0.1s

| - increments a hidden counter then outputs its value (can be used to keep track of loops). This counter can only be accessed by this debug character

All outputs from debug characters is outputted in magenta text and starts with *DEBUG*, so it is not suitable for use outside of testing

### Comments

Comments in brainfuck are very easy unless you want to incude of `+-<>.,[]` in your comment

BrainfuckPlus has a dedicated comment character `/` indicating that everything after it on that line should be ignored

The comments character is not needed, youcan still add comments just by typing them regularly, but `/` allows you to use code characters after them.

This is especially useful because methods mean there may be more characters that you cant use in comments

## Command Line Interface

This is also available in the program, by passing no command, or using ? or help as the command

Commands should be in the form `bfp <command> <file path> [extra flags/parameters]`. If using the `run` command, the `<command>` can be omitted

### Commands

#### run, r

Transpiles the BrainfuckPlus code to brainfuck, then runs the code using the built in brainfuck interpreter

If no command is given (and the first command line args is a file path) then this command is used

#### transpile, t

Transiples the BrainfuckPlus code to brainfuck, and outputs it as a file

#### export, e

Find all methods referenced by a particular code file, and creates a zip file containing those methods

#### modify, m

Allows the use of some of the flags that can be used with other commands in order to edit exesting BrainfuckPlus files

### Flags

#### --obfuscate, -o

transpile only

Obfuscates the source code by adding random newline characters

#### --extremeobfuscate, -eo

transpile only

Obfuscates the source code by adding random newline characters and random characters

#### --debug, -d

transpile and run only

Allows the use of debug characters. These features will likely not be supported on other brainfuck interpreters

#### --brainfuck, -bf

run only

The file will be interpreted as brainfuck, rather than BrainfuckPlus

This will be enabled if the file extension is .bf (coming soon)

#### --removecomments, -rc

export and modify only

Removes comments on exported code

#### --shortenmethodnames, -sm

export and modify only

Shortens all method names to only the first character

#### --removedebug, -rd

export and modify only

Removes debug characters from exported/modifed code

### Parameters

#### --name=value

Accepts a string

transpile and export only

File path (absolute or relative) for the outputted zip/bf file (do not include file extension)

#### --eocount=value

Accepts an integer

transpile only

Sets how much a file is extremely obfuscated by. Only used if extreme obfuscation flag is present

The value of this is approximately equal to the number of random characters inserted between each code chararacter
