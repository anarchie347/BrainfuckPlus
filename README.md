# BrainfuckPlus

I have done some brainfuck coding. It is painful (as it is meant to be)

Brainfuck plus aims to keep the same mind hurting style as brainfuck as translates directly to brainfuck at runtime

You can write code in brainfuck plus, then export it as regular brainfuck

## Features

### Transpiles to brainfuck

All code is directly translatable to brainfuck

The interpreter for the code only uses brainfuck code (as well as the debug characters)

All code is converted to brainfuck at runtime

### Methods

Reuse code easier by creating methods.

Methods are replaced with their regular brainfuck code when executing

Methods are written in brainfuck, in a file called `something.bfp`, and the character used to call the method is the first character

All names are allowed for methods so long as they are allowed by Windows as a file name and are not part of regular brainfuck syntax

All methods that a programme uses must be in the same directory as the main code file (might add a way to make methods available for all projects later)

### Shorthand Repetition

Tired of writing + 65 times in order to be able to output A?

use a * followed by a positive integer followed by the method to repeat that method that amount of times

Like methods, they are converted to brainfuck when executing

e.g `*10+` is the same as `++++++++++`

### Debug characters

Some characters (curently planned to be `\:?"|` can be used to output information about the current state of the program. These commands are only allowed if the programme is running in debug mode, so should not be part of a final programme

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

