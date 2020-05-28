using System;
using System.Collections.Generic;
using System.Text;

namespace TTTest
{
    /// <summary>
    ///  This class help in generating the characters in a regex. It has methods 
    ///  responsible for taking a character and finding the corresponding regex symbol for that character.
    ///  </summary>
    class RegexCharacterGenerator
    {
        public enum Chartypes : int
        {
            space,
            digit,
            lowercase,
            uppercase,
            spechar,
            none
        };
        private static Dictionary<Chartypes, string> map = new Dictionary<Chartypes, string>();
        
        static RegexCharacterGenerator()
        {
            map.Add(Chartypes.space, @"\s");
            map.Add(Chartypes.digit, @"\d");
            map.Add(Chartypes.lowercase, @"[a-z]");
            map.Add(Chartypes.uppercase, @"[A-Z]");
            map.Add(Chartypes.spechar, @"\W");
            map.Add(Chartypes.none, "");
        }
        /// <summary>
        /// This method takes a Chartypes value and returns the corresponding regex symbol.</summary>
        /// <param name="chartype"> The type of the character as specified by the enum Chartypes </param>
        public static string GetRegexPart(Chartypes chartype)
        {
            string res = map.GetValueOrDefault(chartype);

            if (string.IsNullOrEmpty(res))
            {
                throw new Exception();
            }
            return res;
        }
        /// <summary>
        /// This method takes a numeric int value and returns a regex representation for it.
        /// It is usually used to specify the number of repeating regex characters ie. [A-Z]{3}.</summary>
        /// <param name="count"> The number of time a Chartype repeats.</param>
        public static string GetRegexCounter(int count)
        {
            return count <= 1 ? "" : "{" + count + "}";
        }

        /// <summary>
        /// This method takes a char value and returns the type of character it is using the Chartype enum.
        /// <param name="character"> The char value to find.</param>
        public static Chartypes GetTypeFor(char character)
        {

            if (char.IsWhiteSpace(character))
                return Chartypes.space;
            else if (char.IsDigit(character))
                return Chartypes.digit;
            else if (char.IsLetter(character))
            {
                if (char.IsUpper(character))
                    return Chartypes.uppercase;
                else
                    return Chartypes.lowercase;
            }
            else
                return Chartypes.spechar;

        }

        public static void CompleteRegex(ref StringBuilder stringBuilder) 
        {
            stringBuilder.Insert(0, @".*");
            stringBuilder.Append(@".*");
            
        }
        public static void CompleteRegex(ref string partial)
        {
            partial = @".*" + partial + @".*";
        }
    }
}
