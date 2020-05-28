using System;
using System.Collections.Generic;
using System.Text;

namespace TTTest
{
    /// <summary>
    ///  This interface represent a calender and the method that a 
    ///  calender class would need to implement.
    ///  Will be used much later.</summary>
    public interface ICalender
    {
        string GetCalenderAsText();
        Dictionary<string, string>.KeyCollection GetKeys();
        Dictionary<string, string> GetCalenderColumns();
    }
}
