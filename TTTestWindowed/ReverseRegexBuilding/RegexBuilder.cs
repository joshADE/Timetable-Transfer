using System;
using System.Collections.Generic;
using System.Text;
using static TTTest.RegexCharacterGenerator;

namespace TTTest
{
    /// <summary>
    ///  This class uses the RegexCharacterGenerator.cs class to build the corresponding regex 
    ///  from a string.
    /// </summary>
    public class RegexBuilder
    {

        /// <summary>
        /// This method takes a string/word and reverse builds the corresponing regex.
        /// It is case sensitive. It converts each character into a 
        /// RegexCharacterGenerator.Chartypes value before the conversion.</summary>
        /// <param name="inputString"> The input string</param>
        /// <seealso cref="RegexCharacterGenerator">
        /// Contains a public member enum used in categorizing the character type </seealso>
        /// <seealso cref="System.Text.RegularExpressions.Regex">
        /// The resultant output string is meant to be used in the Regex class.</seealso>
        public static string GenerateRegex(string inputString)
        {
            
            
            // Stores the type of each characher in the input
            List<Chartypes> list = new List<Chartypes>();   

            // Stores the resulting regexp returned by the function
            string regex;

            // Alot of string manipulation will be done, so use SB class
            StringBuilder stringBuilder = new StringBuilder();

            // Go through each character in the input and categorize it based on it type given by
            // RegexCharacterGenerator.Chartypes
            foreach (char c in inputString)
            {
                list.Add(RegexCharacterGenerator.GetTypeFor(c));
            }


            // If there are no charcters, return empty regexp
            if (list.Count == 0)
            {
                regex = "//";
                return regex;
            }

            // If its just one characted, there is no need to perform algorithm just get the regex for the charater
            if (list.Count == 1)
            {
                var result = RegexCharacterGenerator.GetRegexPart(list[0]);
                RegexCharacterGenerator.CompleteRegex(ref result);
                return result;
                
            }

            int counter = 1;

            Chartypes last = list[0];
            list.RemoveAt(0);


            foreach (Chartypes ct in list)
            {
                
                // if the last type is the same as current type
                if (last == ct)
                {
                    // increase counter to show the 
                    counter++;
                }
                else
                {
                    if (last != Chartypes.none)
                    {
                        stringBuilder.Append(RegexCharacterGenerator.GetRegexPart(last));
                        if (counter > 1)
                        {
                            stringBuilder.Append(RegexCharacterGenerator.GetRegexCounter(counter));
                        }
                    }
                    counter = 1;
                }
                last = ct;
            }

            if (counter > 0)
            {
                
                stringBuilder.Append(RegexCharacterGenerator.GetRegexPart(last));
                stringBuilder.Append(RegexCharacterGenerator.GetRegexCounter(counter));
            }


            RegexCharacterGenerator.CompleteRegex(ref stringBuilder);
            regex = stringBuilder.ToString();

            return regex;
        }

    }
}
