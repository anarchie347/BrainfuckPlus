using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckPlus
{
    internal class Syntax
    {
        public const string BF_VALID_CHARS = "[],.+-<>";
        public const string DEBUG_CHARS = @"\:?""|"; //chars that are not allowed in Windows fle names -> cant be methods (on windows). doesnt include /,<,>, these are used for other purposes
        public const char COMMENT_CHAR = '/';
        public const char CODE_INJECTION_START_CHAR = '{';
        public const char CODE_INJECTION_END_CHAR = '}';
        public const char CODE_INJECTION_CALL_START_CHAR = '(';
        public const char CODE_INJECTION_CALL_END_CHAR = ')';
        public const string NUMBERS = "0123456789";
        public const char REPETITION_CHAR = '*';
        public const string FILE_EXTENSION = "bfp";
        public static readonly string EXTRA_ALLOWED_CHARS = NUMBERS + new string(new char[] { COMMENT_CHAR, CODE_INJECTION_START_CHAR, CODE_INJECTION_END_CHAR, CODE_INJECTION_CALL_START_CHAR, CODE_INJECTION_CALL_END_CHAR, REPETITION_CHAR });
    }
}
