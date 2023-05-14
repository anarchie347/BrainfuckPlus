# BrainfuckPlus

I have done some brainfuck coding. It is painful (as it is meant to be)

Brainfuck plus aims to keep the same mind hurting style as brainfuck as translates directly to brainfuck at runtime

You can write code in brainfuck plus, then export it as regular brainfuck

I also plan to maybe make a brainfuck plus plus in the future, which deviates more from brainfuck, so wont easily transpile to brainfuck. A planned feature for brainfuck plus plus is functions which can have their own memory, separate to the memory of the program

## Features

Because brainfuckplus is in development, below is a list of which features have been implemented currently

- Transpiling: Done
- Methods: Done
- Injections: Done
- Shorthand repetition: Not done
- Debug chars: Done
- Comment: Done
- Command line commands and interface and usability: Not done

### Transpiles to brainfuck

All code is directly translatable to brainfuck

The interpreter for the code only uses brainfuck code (as well as the debug characters)

All code is converted to brainfuck at runtime

### Methods

Reuse code easier by creating methods.

Methods are replaced with their regular brainfuck code when executing

Methods are written in brainfuck, in a file called `something.bfp`, and the character used to call the method is the first character

Method names have some restrictions. THey cannot use any brainfuck characters `[],.+-<>`, any of the debug chars `\\:?"|` , the comment char `/`, the code injection start end end chars `{}` the repetition char `*`, the characters used for calling injected code `()` or any numbers `0123456789`

Hence the list of dissallowed characters is `[],.+-<>\\:?"|/{}*()0123456789`

Some of these characters may change in the future, but they are not planned to change

These restrictions only apply to the first lettter of the filename because this is the name of the method

All methods that a programme uses must be in the same directory as the main code file (might add a way to make methods available for all projects later)

At runtime, all methods calls are substituted with their corresponding brainfuck code, so **recursion is NOT possible** because it will cause a crash by endlessly substituting the method inside itself. This could be fixed by editing the interpreter, but this go against one of the main principles of brainfuck plus - it transpiles directly (and easily) to brainfuck

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

Brainfuck plus has a dedicated comment character `/` indicating that everything after it on that line should be ignored

The comments character is not needed, youcan still add comments just by typing them regularly, but `/` allows you to use code characters after them.

This is especially useful because methods mean there may be more characters that you cant use in comments

## Undecided features

### String literals

A way to write a string literal to output -> avoids having to write lots of unreadable `+` and `-`

e.g. `\AB\` instead of `+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++.+.`

### Extra functions

Coded in C# to add functionality not possible in brainfuck, such as changing the console color, or clearing the console

