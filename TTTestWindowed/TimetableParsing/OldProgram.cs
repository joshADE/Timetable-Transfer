using mshtml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using static TTTest.RegexCharacterGenerator;

namespace TTTest
{

    class Program
    {
        #region TestCode
         static string RetrieveContentTest() 
        {
            
            var url = "https://google.ca";
            Console.WriteLine("Retrieving content from : " + url);
            var content = HTMLParser.RetrieveContenet(url);

            Console.WriteLine("Result:");
            Console.WriteLine("--------------------------------------------------------------");
            Console.WriteLine(content);
            return content;

        }

        static string GenerateRegexTest()
        {
            string course = "1X 11 S3";
            string regex = RegexBuilder.GenerateRegex(course);
            Console.WriteLine("For the course code " + course + ". Regex is " + regex);
            //TODO: check if expected == result
            return regex;

        }

        static void ParseTest() 
        {
            Parse(HTMLParser.RetrieveContenet("https://google.ca"), new List<string>() { "search", "google" });
        }

        static void GCalenderEntryTest() 
        {
            string course1 = "PROG W555W";
            GCalenderEntry entry1 = new GCalenderEntry();
            entry1.SetField("Code", course1);
            entry1.SetField("Subject", "School Course");
            entry1.StartDate = DateTime.Now.Date;
            entry1.SetField("StartDate", "10tvg");
            entry1.StartTime = DateTime.Parse("11:30:45");
            entry1.SetField("StartTime", "10:00");
            entry1.Subject = "Course";
            var content = entry1.GetCalenderAsText();
            Console.WriteLine(content);
            GCalenderEntry.GenerateCalenderFile(@"C:\Users\josh_\Documents\Cal", "Course1", content, true);
        }

        static void FullProgramTest() 
        {

            string course1 = "PROG W555W";
            string course2 = "PROG 55294";
            string course3 = "PROG 43544";

            string regex1 = RegexBuilder.GenerateRegex(course1);
            string regex2 = RegexBuilder.GenerateRegex(course2);
            string regex3 = RegexBuilder.GenerateRegex(course3);
            Console.WriteLine($"Course1:{course1} - Regex:{regex1}, Course2:{course2} - Regex:{regex2}.");
            var list = new List<string>();
            list.Add(regex1);
            list.Add(regex2);
            // do not need to add regex for course codes that are similar(or same regex)
            string sampledoc = $"" +
                $"<!DOCTYPE html>" +
                $"<html>" +
                $"<head>" +
                $"</head>" +
                $"<body>" +
                $"<table id='test'>" +
                $"<tr><td>Prof Jame {course1} <span id='substitute'>Prof Shiva</span> 10/10/2019 - 10/10/2020 10:30 </td></tr>" +
                $"<!--<tr><td>Prof Snadra {course2} 10/15/2019 - 10/15/2020 10:30 </td></tr>-->" +
                 $"<!--<tr><td>Prof Klams {course3} 10/13/2019 - 10/13/2020 10:30 </td></tr>-->" +
                $"</table>" +
                $"<p>Hello!<p>" +
                $"</body>" +
                $"</html>";
            Parse(sampledoc, list);



        }

        #endregion

       

        /// <summary>
        /// Parses the html content and retrieves relevant course info using the regexes in the regexlist.</summary>
        /// <param name="htmlContent">A string representation of the html page.</param>
        /// <param name="regexlist">A list of complete string regexes to find the relevant content.</param>
        public static void Parse(string htmlContent, List<string> regexlist)
        {
            /*
            string searchstring = HTMLParser.RetrieveInnerTextContent(htmlContent);
            Console.WriteLine();
            Console.WriteLine("Results"); ;

            // Show the results from using regex on the timetable html file
            // each course should be in the same inline
            Console.WriteLine("These are the matches of what was found given the course codes:");

            List<List<string>> regexResultList = RegexExtractor.ExtractAsList(searchstring, regexlist);

            foreach(var line in regexResultList) 
            { 
                foreach(var text in line) 
                {
                    Console.Write(text + " ");
                }
                Console.WriteLine();
            }


            if (regexResultList.Count == 0)
            {
                Console.WriteLine("Sorry. it looks that there were no matches");
                return;
            }

            // Get the categoryList from the user. This will go through each course and ask te user 
            // what piece of data it is, whether a coursecode, email, prof name
            // some categaories are already added these

            List<string> categoryList = new List<string>();

            categoryList.Add("none");
            categoryList.Add("CourseCode");
            // add the field corresponding to the calender to use
            if (GCalenderEntry.defField.Count > 0)
            {
                categoryList.AddRange(GCalenderEntry.defField);
            }

            PrintCategories(categoryList);

            Console.WriteLine("Enter a list of the other categories(fields) you want identify on the new timetable\n" +
                "(seperated by space, ie Teacher Room# ...,Some of the categores above match the fields in the calender to convert to\n>" +
                "(Don't add any of the ones above. You can use their number when categorizing)\n>");
            Console.WriteLine("Here are some suggestions: Teacher Room# etc.");

            

            string[] inputarray;
            string input = Console.ReadLine();

            while (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Invalid input, try again.\n>");
                input = Console.ReadLine();
            }

            inputarray = input.Trim().Split(" ");

            foreach (var inp in inputarray)
            {
                if (!string.IsNullOrEmpty(inp))
                {
                    categoryList.Add(inp);
                }
            }

            PrintCategories(categoryList);


            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("We will go through each piece of text in the result and classify them given the categories.");
            Console.WriteLine("Assuming each line corresponds to info about a single course. " +
                "\nEnter 'nc' for a new cource, enter a number for the category" +
                "\nEnter 'next' for the next course, enter 'prev' for the previous course" +
                "\nPress enter to continue...\n>");


            Console.ReadKey();

            List<Dictionary<string, string>> coursesInfo = new List<Dictionary<string, string>>();

            string newCourseFlag;
            int i = 0; // always (currentindex + 1) for coursesInfo

            int index = 0; // always refers to index of current course that was found in regexResultList
            int indexInner = 0; // always refers to inner index or current word of the current course in regexResultList
            int value;
            coursesInfo.Add(new Dictionary<string, string>());
            while (true)
            {
                if (indexInner >= regexResultList[index].Count) 
                {
                    index++;
                    indexInner = 0;
                    if (index >= regexResultList.Count)
                    {
                        Console.WriteLine("That it for now.\n");
                        break;
                    }
                    // Same code as when they enter 'nc' for new course
                    coursesInfo.Add(new Dictionary<string, string>());
                    i = coursesInfo.Count - 1;
                }
                
                

                Console.WriteLine($"For course#{(i + 1)}.\n");
                Console.WriteLine($"What is '{regexResultList[index][indexInner]}'?\n>");
                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) 
                {
                    Console.WriteLine("Invalid input, try again.\n>");

                }
                else if ((newCourseFlag = input.Trim()) == "next") // go to next course
                {
                    i = Math.Clamp(i + 1, 0, coursesInfo.Count - 1);
                    Console.WriteLine(CheckAndGetCourseCode(coursesInfo[i], "CourseCode"));
                }
                else if ((newCourseFlag = input.Trim()) == "prev") // go to previous course
                {
                    i = Math.Clamp(i - 1, 0, coursesInfo.Count - 1);
                    Console.WriteLine(CheckAndGetCourseCode(coursesInfo[i], "CourseCode"));
                }
                else if((newCourseFlag = input.Trim()) == "nc") // create a new course and immediatly set the index to that course
                {
                    coursesInfo.Add(new Dictionary<string, string>());
                    i = coursesInfo.Count - 1;
                }
                else
                {

                    try 
                    {
                        value = int.Parse(input);
                    }
                    catch (FormatException) 
                    {
                        Console.WriteLine("Invalid input. Try Again");
                        continue;
                    }
                    if (value > categoryList.Count || value < 1)
                    {
                        Console.WriteLine("There was no corresponding category.\n>");
                    }
                    else
                    {

                        string catgry = categoryList[value - 1];
                        string prevVal;
                        if (coursesInfo[i].TryGetValue(catgry, out prevVal))
                        {
                            coursesInfo[i].Remove(catgry);
                            coursesInfo[i].Add(catgry, prevVal + " " + regexResultList[index][indexInner]);
                        }
                        else
                        {
                            coursesInfo[i].Add(catgry, regexResultList[index][indexInner]);
                        }

                        indexInner++;
                    }
                }


            }
            Console.WriteLine();
            Console.WriteLine("Let us see the result");
            PrintCourseInfo(coursesInfo, categoryList);
            Console.WriteLine();
            Console.WriteLine();

            // Get directory info to save to file
            */
            Console.WriteLine("Would you like to save this to a file?(y = yes, anykey = no)");
            string inpt = Console.ReadLine();
            inpt = inpt.Trim().ToLower();
            if (inpt != "y") 
            {
                Console.WriteLine("Bye!");
                return;
            }
            

            List<GCalenderEntry> entries = new List<GCalenderEntry>();
            /*
            foreach (var courseDictionary in coursesInfo)
            {
                // Converting each cousre into a .csv file
                GCalenderEntry entry = new GCalenderEntry(courseDictionary);
                entry.SetField("Subject", "Course: " + entry.GetField("CourseCode"));
                entries.Add(entry);
            }
            */
            entries.Add(new GCalenderEntry()
            {
                Subject = "Course: PROG 5559",
                Description = "Scholl Course",
                StartDate = DateTime.Now.Date,
                EndDate = DateTime.Now.Date.AddDays(1),
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(2),
                Location = "Building 1045"

            });

            Console.WriteLine("Enter the path: It must be exist.");
            inpt = Console.ReadLine();
            string path = inpt.Trim();
            Console.WriteLine("Enter the filename:");
            inpt = Console.ReadLine();
            string file = inpt.Trim();
            bool finished = false;
            try
            {
                finished = GCalenderEntry.SaveListToFile(entries, @path, file);

            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                finished = false;
            }

            while (!finished)
            {
                Console.WriteLine("Invalid path name.Try again");
                Console.WriteLine(@"Enter the path: It must be exist. eg. C:\Users\mike\Documents\Cal");
                inpt = Console.ReadLine();
                path = inpt;
                Console.WriteLine("You entered: " + path);
                Console.WriteLine("Enter the filename:");
                inpt = Console.ReadLine();
                file = inpt.Trim();
                Console.WriteLine("You entered: " + file);
                try
                {
                    finished = GCalenderEntry.SaveListToFile(entries, file, @path);

                }
                catch (Exception ex2)
                {
                    Console.WriteLine(ex2.Message);
                    finished = false;
                }
            }

            Console.WriteLine("Results have been saved!");

        }

        public static void PrintCategories(List<string> categories) 
        {
            int index = 1;
            Console.WriteLine("These are the categories for organizing so far:");
            foreach (string category in categories)
            {
                Console.WriteLine(index++ + ". " + category);
            }

        }



        public static void SaveCourseInfoToFile(List<Dictionary<string, string>> coursesInfo)
        {
            int fileIndex = 1;
            foreach (var courseDictionary in coursesInfo)
            {
                // Converting each cousre into a .csv file
                GCalenderEntry entry = new GCalenderEntry(courseDictionary);
                entry.SetField("Subject", "Course: " + entry.GetField("CourseCode"));
                var content = entry.GetCalenderAsText();
                Console.WriteLine(content);
                GCalenderEntry.GenerateCalenderFile(@"C:\Users\josh_\Documents\Cal", "Course" + fileIndex++, content, true);


            }
        }

            public static void PrintCourseInfo(List<Dictionary<string,string>> coursesInfo, List<string> categories) 
        {

            
            foreach (var courseDictionary in coursesInfo) 
            {
                
                // Printing out the details to the console
                Console.WriteLine(CheckAndGetCourseCode(courseDictionary, "CourseCode"));
                
                foreach (var category in categories) 
                {
                    // [0] and [1] are 'none' category(ignored) and 'CourseCode' category(already prited)
                    if (category == categories[0] || category == categories[1])
                    {
                        continue;
                    }
                    Console.WriteLine(CheckAndGetDictionaryEntry(courseDictionary, category));

                }
                Console.WriteLine();


            }
        }

        public static string CheckAndGetCourseCode(Dictionary<string, string> dictionary, string courseCodeCategoryName) 
        {
            return CheckAndGetDictionaryEntry(dictionary, courseCodeCategoryName);
        }

        public static string CheckAndGetDictionaryEntry(Dictionary<string, string> dictionary, string keyName)
        {
            string res;
            if (!dictionary.TryGetValue(keyName, out res))
            {
                res = "Not set yet";
            }
            return (keyName + ": " + res);
        }


    }
}
