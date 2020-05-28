using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TTTest
{
    /// <summary>
    ///  This class provides support for using a regex to extract matches from a peice of multilined text.
    /// </summary>
    public class RegexExtractor
    {

        /// <summary>
        /// Evaluates a list of regexes againts a text and puts each word of each line 
        /// that matches inside a list of string, the line will also be in another list of matching lines.</summary>
        /// <param name="extractionText"> Text to perform extraction on.</param>
        /// <param name="regexlist"> The list of regexes to find.</param>
        /// <returns>A list of List of text, outside list - matching lines, inside - list of words inside mathing lines</returns>
        public static List<List<string>> ExtractAsList(string extractionText, List<string> regexlist) 
        {
            List<List<string>> regexResultList = new List<List<string>>();
            
            foreach (string regex in regexlist)
            {
                var result1 = Regex.Matches(extractionText, regex);
                
                foreach (Match match in result1)
                {
                    var wordsInMatch = match.Value.Trim('\r', '\t', '\n').Split(" ");
                    List<string> parsedWords = new List<string>();

                    foreach (var word in wordsInMatch)
                    {
                        if (!string.IsNullOrEmpty(word))
                        {
                            parsedWords.Add(word);
                        }
                    }
                    regexResultList.Add(parsedWords);
                }


            }

            return regexResultList;

        }
    }
}
