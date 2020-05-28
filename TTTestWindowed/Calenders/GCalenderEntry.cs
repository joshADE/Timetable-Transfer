using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace TTTest
{
    /// <summary>
    ///  This class represents a single Google Calender event(or entry) and encapsulates 
    ///  the logic needed to create one. It internally uses a map to represent the collection of fields.
    ///  Any of the default fields that are accepted by Google Calender Import Tool can be set via a 
    ///  property or directly through the set method and enforces a type restriction on those fieids.
    ///  Other addition field which may/may not be dispayed when imported to google calender can also be added to its map.
    ///  Also the fields will not be stored in the mapped untill an initial value is set.</summary>
    ///  <seealso cref="ICalender"/>
    public class GCalenderEntry : ICalender
    {

            
        // Properties

        public string Subject { get => calFields["Subject"]; set { SetField<string>("Subject", value); } }
        public DateTime StartDate { get => DateTime.Parse(calFields["StartDate"]); set { SetField("StartDate", value.ToShortDateString()); } }
        public DateTime StartTime { get => DateTime.Parse(calFields["StartTime"]); set { SetField("StartTime", value.ToShortTimeString()); } }
        public DateTime EndDate { get => DateTime.Parse(calFields["EndDate"]); set { SetField("EndDate", value.ToShortDateString()); } }
        public DateTime EndTime { get => DateTime.Parse(calFields["EndTime"]); set { SetField("EndTime", value.ToShortTimeString()); } }
        public bool AllDayEvent { get => bool.Parse(calFields["AllDayEvent"]); set { SetField<bool>("AllDayEvent", value); } }
        public string Description { get => calFields["Description"]; set { SetField<string>("Description", value); } }
        public string Location { get => calFields["Location"]; set { SetField<string>("Location", value); } }
        public bool Private { get => bool.Parse(calFields["Private"]); set { SetField<bool>("Private", value); } }


        // Public Member Variables


        public static string MissingFieldIndecator = "";


        public static List<string> defField =
            new List<string>()
            {
                "Subject",
                "StartDate",
                "StartTime",
                "EndDate",
                "EndTime",
                "AllDayEvent",
                "Description",
                "Location",
                "Private"
            };



        // Private Member Variables


        private Dictionary<string, string> calFields;

        // Constructors

        public GCalenderEntry()
        {
            Initialize();
        }

        public GCalenderEntry(Dictionary<string, string> fields)
        {
            Initialize();
            // check which field are similar to defauld field - mapped as properties
            foreach (var field in fields)
            {
                SetField(field.Key, field.Value);
            }

        }


        // Private Methods

        private void Initialize()
        {
            calFields = new Dictionary<string, string>();

        }

        private bool SetField<T>(string fieldName, T value)
        {   
            return SetField(fieldName, value.ToString());
        }

        private bool IsADefaultCalenderField(string fieldkey) 
        {
            // should account for case
            return defField.Contains(fieldkey);
        }
        private string GetFormatedName(string unformatedName)
        {
            return Regex.Replace(unformatedName, "([A-Z]{1,2}|[0-9]+)", " $1").TrimStart();
        }

        private bool ReplaceAdd(string fieldkey, string fieldvalue)
        {
            bool didReplace = false;
            string value;
            if (calFields.TryGetValue(fieldkey, out value))
            {
                calFields.Remove(fieldkey);
                didReplace = true;
            }
            calFields.Add(fieldkey, fieldvalue);
            return didReplace;
        }

        private bool TryParse(string fieldkey, string value, out Type type) 
        {
            try
            {
                switch (defField.IndexOf(fieldkey))
                {
                    case 0: // Subject - string
                    case 6: // Description - string
                    case 7: // Location - string
                        type = value.GetType();
                        return true;
                    case 1: // StartDate - DateTime
                    case 3: // EndDate - DateTime
                        
                    case 2: // StartTime - DateTime
                    case 4: // EndTime - DateTime
                        DateTime dateTime = 
                        DateTime.Parse(value);
                        type = dateTime.GetType();
                        return true;
                    case 5: // AllDayEvent - bool
                    case 8: // Private - bool
                        bool val = 
                        bool.Parse(value);
                        type = val.GetType();
                        return true;
                    default:
                        type = null;
                        return false;

                }
            }
            catch (FormatException)
            {
                type = null;
                return false;
            }

        }

        // Public Methods

        public string GetField(string fieldkey) 
        {
            string value;
            if (calFields.TryGetValue(fieldkey, out value))
            {
                return value;
            }
            return null;
        }


        public bool SetField(string fieldkey, string fieldvalue) 
        {
            fieldkey.Trim(',', '\r', '\n', '\t');
            fieldvalue.Trim(',', '\r', '\n', '\t');
            bool wasParsed = false;
            Type valType;
            if (defField.Contains(fieldkey))
            {
                wasParsed = TryParse(fieldkey, fieldvalue, out valType);
                if (!wasParsed) 
                {
                    return false;
                }
            }

            ReplaceAdd(fieldkey, fieldvalue);

            return true;
        }

        public string GetCalenderAsText()
        {
            string resultKeys = "";
            string resultValues = "";
            foreach (var entry in calFields)
            {
                resultKeys += GetFormatedName(entry.Key) + ", ";
                resultValues += entry.Value + ", ";

            }
            resultKeys = resultKeys.Remove(resultKeys.Length - 2, 2);
            string finalResult = resultKeys + "\n" + resultValues;
            finalResult = finalResult.Remove(finalResult.Length - 2, 2);
            return finalResult;

        }

        public Dictionary<string, string> GetCalenderColumns()
        {
            return calFields;

        }

        public static bool GenerateCalenderFile(string path, string filename, string content, bool overwrite = false) 
        {

            string fullQualified = path + Path.DirectorySeparatorChar + filename + ".csv";

            if (!Directory.Exists(path))
            {
                throw new Exception("Directory " + path + " does not exist");
            }
            
            if (!overwrite && File.Exists(fullQualified)) 
            {
                throw new Exception("File already exist exist");
            }
            /*
            Hashtable hashtable = new Hashtable(tables[0]);
            for (int i = 1; i < tables.Length; i++) 
            { 
                
            }
            */            

            using (var sw = new StreamWriter(fullQualified)) 
            {
                sw.Write(content);
                Console.WriteLine("Save in :" + fullQualified);
            }

            return true;
        
        }

        public Dictionary<string,string>.KeyCollection GetKeys() 
        {
            return calFields.Keys;
        }

        public static bool SaveListToFile(List<GCalenderEntry> gCalenders, string file, string path) 
        {


            if (string.IsNullOrEmpty(file) || string.IsNullOrEmpty(path))
            {
                throw new Exception("You must enter a valid file name and directory path.");

            }


            List<string> allkeys = new List<string>();

            foreach (var course in gCalenders)
            {
                allkeys.AddRange(course.GetKeys());
            }

            allkeys = allkeys.AsEnumerable().Distinct().OrderBy(str => str).ToList(); // get all the unique ordered keys of all the calenders 


            List<List<string>> table = new List<List<string>>();


            table.Add(allkeys);

            int k = 0; // column
            int j = 1; // the index/row of the course, set to first course
            // table[row][column] or table[j][k]
            foreach (var course in gCalenders)
            {
                table.Add(new List<string>());
                for (k = 0; k < table[0].Count; k++)
                {
                    string value = course.GetField(table[0][k]);
                    if (value == null)
                    {
                        value = MissingFieldIndecator;
                    }
                    table[j].Add(value);
                    // or table[table.Count - 1].Add(value);
                }
                j++; // increment the row to the next course
            }

            
            string content = "";
            foreach (var row in table)
            {
                foreach (var columndata in row)
                {
                    content += columndata + ", ";
                }
                content = content.Trim(',', ' ');
                content += "\n";

            }


            var fileName = Path.GetFileName(file);
            var pathIncomplete = path;
            var success = false;
            
            success = GCalenderEntry.GenerateCalenderFile(pathIncomplete, fileName, content, true);
            return success;
            

        }


    }
}
