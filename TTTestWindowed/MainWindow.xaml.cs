using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using TTTest;

namespace TTTestWindowed
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<UIElement> catogorizationGroup;


        List<string> courseList;
        List<string> categoryList;
        List<Dictionary<string,string>> courseInfo;
        //List<int> lboxSorterCategories; // parellel to the items in lboxSorter/regextResult

        List<GCalenderEntry> courses;
        //List<string> regexResult;    // used for storing the result from the regex filter
        List<int> regexResultCourseIndicator;
        List<KeyValuePair<string, int>> sortList;
        int numberOfDefaultCategories;

        public bool IntelligentMatch { get; set; }


        private void SetupBindings()
        {
            courseList = new List<string>();
            lboxCourses.ItemsSource = courseList;
            //AddItemToScrollList<string>("PROG 9921", scrollCourses, lboxCourses, courseList);


            categoryList = new List<string>() { "none", "CourseCode" };
            categoryList.AddRange(GCalenderEntry.defField);
            numberOfDefaultCategories = categoryList.Count;
            lboxCategory.ItemsSource = categoryList;

            sortList = new List<KeyValuePair<string, int>>();
            //regexResult = new List<string>();
            //lboxSorter.ItemsSource = regexResult;
            //lboxSorterCategories = new List<int>(); // parallel to regexResult shows the categories of the item
            lboxSorter.ItemsSource = sortList;
            regexResultCourseIndicator = new List<int>();
            courseInfo = new List<Dictionary<string, string>>();
            //lboxSorter.ItemStringFormat = "{}{0}";

            IntelligentMatch = false;
            

        }



        public MainWindow()
        {
            InitializeComponent();
            SetupBindings();
            catogorizationGroup = new List<UIElement>()
                {
                    cbAutoMatch, tbCategory, tbDirectoryName, tbFileName, btnAddCategory, btnRemoveCategory, btnSetCategory, btnGetCategory, btnSave, lboxCategory, lboxSorter
                };
            DisableSubControlGroup(catogorizationGroup);
            ResetInstructions();
        }



        #region CommonControlOperations


        private void ResetInstructions()
        {
            tblockInstructions.Text = "Messages Will Appear Here";
        }

        private void AddItemToScrollList<T>(T item, ScrollViewer viewer, ListBox listBox, List<T> bindedList)
        {
            bindedList.Add(item);
            listBox.Items.Refresh();
            viewer.UpdateLayout();
            listBox.SelectedIndex = listBox.Items.Count - 1;
            listBox.ScrollIntoView(listBox.SelectedItem);
        }

        private void RemoveItemFromScrollList<T>(int removeIndex, ScrollViewer viewer, ListBox listBox, List<T> bindedList)
        {
            bindedList.RemoveAt(removeIndex);
            listBox.Items.Refresh();
            viewer.UpdateLayout();
        }

        private void ResetTextSubControlGroup(List<TextBox> uIElements)
        {
            foreach (TextBox tb in uIElements)
            {
                tb.Clear();
            }
        }

        private void ResetListSubControlGroup<T>(List<ListBox> uIElements)
        {
            foreach (ListBox lb in uIElements)
            {
                (lb.ItemsSource as List<T>).Clear();

            }
        }

        private void ResetCatogorizationControls()
        {

            List<ListBox> lists = catogorizationGroup.ConvertAll<ListBox>(ui => ui as ListBox).FindAll(e => e != null && e.Items.GetType() == typeof(string));
            List<ListBox> nonString = catogorizationGroup.ConvertAll<ListBox>(ui => ui as ListBox).FindAll(e => e != null && e.Items.GetType() != typeof(string));
            ResetListSubControlGroup<string>(lists);
            // need to clear all list in non string
            sortList.Clear();
            lboxSorter.ItemsSource.OfType<int>().ToList().Clear();



            foreach (var item in nonString)
            {
                item.ItemsSource.OfType<List>().ToList().Clear();
            }

            List<TextBox> lists2 = catogorizationGroup.ConvertAll<TextBox>(ui => ui as TextBox).FindAll(e => e != null);
            ResetTextSubControlGroup(lists2);

            categoryList.Add("none");
            categoryList.Add("CourseCode");
            categoryList.AddRange(GCalenderEntry.defField);

        }

        private void ResetSortingControls()
        {
            //regexResult.Clear();
            //lboxSorterCategories.Clear();
            sortList.Clear();
            lboxSorter.Items.Refresh();
            scrollSorter.UpdateLayout();
            regexResultCourseIndicator.Clear();
        }

        private void ResetCoursesControls()
        {
            courseList.Clear();
            lboxCourses.Items.Refresh();
            scrollCourses.UpdateLayout();
        }


        private void EnableSubControlGroup(List<UIElement> uIElements)
        {
            foreach (UIElement ui in uIElements)
            {
                ui.IsEnabled = true;
            }
        }

        private void DisableSubControlGroup(List<UIElement> uIElements)
        {
            foreach (UIElement ui in uIElements)
            {
                ui.IsEnabled = false;
            }
        }

        private bool WithinBoundsOfItems<T>(int index, List<T> list)
        {
            return (index >= 0 && index < list.Count);
        }

        private int FindCorrespondingCourse(int index)
        {
            if (index < 0) 
            {
                return -1;
            }
            int sum = 0;
            int numberOfCourses = regexResultCourseIndicator.Count;
            for (int i = 0; i < numberOfCourses; i++)
            {

                sum += regexResultCourseIndicator[i];

                if (index < sum)
                {
                    return i;
                }
                
            }
            return -1;
        }


        #endregion



        private void btnGetHTML_Click(object sender, RoutedEventArgs e)
        {
            ResetInstructions();
            var url = tbUrl.Text.Trim();
            if (string.IsNullOrEmpty(url))
            {
                return;
            }
            DisableSubControlGroup(catogorizationGroup);

            try
            {
                var pagecontents = TTTest.HTMLParser.RetrieveInnerTextContent(TTTest.HTMLParser.RetrieveContenet(url));
                tblockResult.Text = pagecontents;
            }
            catch (UriFormatException)
            {
                tblockInstructions.Text = "Something was wrong with the uri you provided. Try again.";
            }
            catch (NotSupportedException)
            {
                tblockInstructions.Text = "The feature isn't implemented yet.";
            }
            catch (InvalidOperationException)
            {
                tblockInstructions.Text = "That website wasn't retrurning the right results";
            }
            catch (InvalidCastException) 
            {
                tblockInstructions.Text = "Sorry application does not support that uri";
            }
            

        }


        private void btnAddCourse_Click(object sender, RoutedEventArgs e)
        {
            ResetInstructions();
            var course = tbCourse.Text.Trim();
            if (string.IsNullOrEmpty(course))
            {
                return;
            }
            DisableSubControlGroup(catogorizationGroup);
            AddItemToScrollList<string>(course, scrollCourses, lboxCourses, courseList);
            tbCourse.Clear();
        }


        private void btnRemoveCourse_Click(object sender, RoutedEventArgs e)
        {
            var item = lboxCourses.SelectedIndex;
            if (item >= 0)
            {
                RemoveItemFromScrollList<string>(item, scrollCourses, lboxCourses, courseList);
            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            var htmlContent = tblockResult.Text.Trim();
            if (string.IsNullOrEmpty(htmlContent))
            {
                tblockInstructions.Text = "You need to get the content from the url first.";
                return;
            }

            if (courseList.Count == 0) 
            {
                tblockInstructions.Text = "You need add the course code to search/filter for.";
                return;
            }

            EnableSubControlGroup(catogorizationGroup);
            ResetCatogorizationControls();
            ResetSortingControls();
            ResetInstructions();
            List<string> regexList = new List<string>();
            foreach (var course in courseList) {
                regexList.Add(RegexBuilder.GenerateRegex(course));
            }
            int numberOfCourses = regexList.Count;
            regexList = regexList.AsEnumerable().Distinct().ToList();

            List<List<string>> regexResultList = RegexExtractor.ExtractAsList(tblockResult.Text, regexList);
            
            if (numberOfCourses != regexResultList.Count) 
            {
                tblockInstructions.Text = "Sorry the number of courses that was specified wasn't equal to the number found! Makes sure that it is equal";
                // Reset the courses for the user
                DisableSubControlGroup(catogorizationGroup);
                ResetCoursesControls();
                return;
            }
            tblockResult.Text = "";

            int i = 0;
            foreach (var line in regexResultList)
            {
                foreach (var text in line)
                {
                    var trimedTxt = text.Trim();
                    if (!string.IsNullOrEmpty(trimedTxt))
                    {
                        ++i;
                        //regexResult.Add(trimedTxt);
                        //lboxSorterCategories.Add(0); // add the indx of the first (default) cateory as this items category (should be none)
                        sortList.Add(new KeyValuePair<string, int>(trimedTxt, 0));
                        tblockResult.Text += trimedTxt + " ";
                    }
                }
                regexResultCourseIndicator.Add(i);
                i = 0;
                tblockResult.Text += "\n";

            }

            courseInfo.Clear();
            for (int j = 0; j < numberOfCourses; j++) 
            {
                courseInfo.Add(new Dictionary<string, string>());
            }


            

        }


        private void btnRemoveCategory_Click(object sender, RoutedEventArgs e)
        {
            ResetInstructions();

            var category = lboxCategory.SelectedIndex;
            if (category < numberOfDefaultCategories) //trying to remove a default category, not allowed
            {
                tblockInstructions.Text = "Cannot remove one of the default categories.";
                return;
            }
            if(category >= 0)
            RemoveItemFromScrollList<string>(category, scrollCategory, lboxCategory, categoryList);

            
        }

        private void btnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            ResetInstructions();
            var category = tbCategory.Text.Trim();
            if (string.IsNullOrEmpty(category))
            {
                return;
            }
            AddItemToScrollList<string>(category, scrollCategory, lboxCategory, categoryList);
            tbCategory.Clear();
        }

        private void btnSetCategory_Click(object sender, RoutedEventArgs e)
        {
            ResetInstructions();
            var index = lboxSorter.SelectedIndex;
            var category = lboxCategory.SelectedIndex;

            if (!WithinBoundsOfItems(index, sortList))
            {
                tblockInstructions.Text = "Make sure to select an item to categorize in the list.";
                return;
            }

            if (!WithinBoundsOfItems(category, categoryList))
            {
                tblockInstructions.Text = "Make sure to select an item to categorize in the list.";
                return;
            }

            //lboxSorterCategories[index] = category;
            sortList[index] = new KeyValuePair<string, int>(sortList[index].Key,category);

            tblockInstructions.Text = "Success!";

            lboxSorter.SelectedIndex += 1;
            scrollSorter.UpdateLayout();
            lboxSorter.UpdateLayout();
            lboxSorter.ScrollIntoView(lboxSorter.SelectedItem);
            
            if (IntelligentMatch) 
            {
                int course = FindCorrespondingCourse(index); // returns -1 if the index does't corespond to a course
                if (course >= 0) 
                {
                    string regexString = RegexBuilder.GenerateRegex(sortList[index].Key);
                    Regex regex = new Regex(regexString);
                    //find the index as if it were to be in the first course, (sort of normalized)
                    index = index - regexResultCourseIndicator.Where((e, ind) => ind < course).Sum();

                    

                    // regex...Indeicator holds the count of the number of 'text' in each course
                    // its size = number of courses, with index 0 being first, 1 being second, etc...
                    for (int i = 0; i < regexResultCourseIndicator.Count; i++)  
                    {
                        // if the course is the one already set by this method skip it
                        if (i == course) 
                        {
                            continue;
                        }


                        int sumTillCourse = regexResultCourseIndicator.Where((e, ind) => ind <= i).Sum();
                        int newIndex;
                        // for both courses that have indexes below/above
                        if (index < 0) 
                        {
                            newIndex = regexResultCourseIndicator.Where((e, ind) => ind <= i).Sum() + index;
                        }
                        else
                        {
                            newIndex = regexResultCourseIndicator.Where((e, ind) => ind < i).Sum() + index;
                        }
                        
                        if (newIndex < sumTillCourse && newIndex >= 0) 
                        {
                            string key = sortList[newIndex].Key;
                            if (regex.IsMatch(key))
                            {
                                sortList[newIndex] = new KeyValuePair<string, int>(key, category);
                                tblockInstructions.Text += "\nAuto Match Performed!";
                            }
                        }
                    
                    }
                }
            }

        }

 
        private void btnGetCategory_Click(object sender, RoutedEventArgs e)
        {
            ResetInstructions();
            lblGetCategory.Content = "";

            var index = lboxSorter.SelectedIndex;
            

            if (!WithinBoundsOfItems(index, sortList))
            {
                tblockInstructions.Text = "Make sure to select the item to retrieve the category of in the list.";
                return;
            }
            //var category = lboxSorterCategories[index];
            var category = sortList[index].Value;
            if (!WithinBoundsOfItems(category, categoryList))
            {
                tblockInstructions.Text = "Something went wrong retrieveing the saved category.";
                //lboxSorterCategories[index] = 0;
                sortList[index] = new KeyValuePair<string, int>(sortList[index].Key, 0);

                return;
            }

            tblockInstructions.Text = "Success!";
            lblGetCategory.Content = categoryList[category];
            lboxCategory.SelectedIndex = category;
            lboxCategory.Items.Refresh();
            lboxCategory.UpdateLayout();
            lboxCategory.ScrollIntoView(lboxCategory.SelectedItem);

            lboxSorter.Items.Refresh();
            lboxSorter.UpdateLayout();
            lboxSorter.ScrollIntoView(lboxCategory.SelectedItem);

        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ResetInstructions();
            courseInfo.Clear();


            int index;
            int courseIndex = 0;
            int indicatorIndex = 0;
            int sum = regexResultCourseIndicator[indicatorIndex];
            courseInfo.Add(new Dictionary<string, string>()); // first course
            for (int i = 0; i < sortList.Count; i++)
            {
                if (i >= sum) {
                    // new course
                    courseInfo.Add(new Dictionary<string, string>());
                    sum += regexResultCourseIndicator[++indicatorIndex];

                }
                var item = sortList[i].Key;
                var category = categoryList[sortList[i].Value];
                if (item != null && category != null)
                {
                    if (category != categoryList[0]) // ignore the none default category
                    {
                        string prevVal;
                        if (courseInfo[indicatorIndex].TryGetValue(category, out prevVal))
                        {
                            courseInfo[indicatorIndex].Remove(category);
                            courseInfo[indicatorIndex].Add(category, prevVal + " " + item);
                        }
                        else
                        {

                            courseInfo[indicatorIndex].Add(category, item);
                        }
                    }
                }
            }
            tblockResult.Text = "";
            int indx = 1;
            foreach (var courseDict in courseInfo) 
            {
                tblockResult.Text += "For course ";
                string value;
                if (courseDict.TryGetValue("CourseCode", out value))
                {
                    tblockResult.Text += value +":\n";
                }
                else 
                {
                    tblockResult.Text += "# " + (indx++) + ":\n";
                }
                
                
                foreach (var entry in courseDict) 
                {
                    tblockResult.Text += entry.Key + " = " + entry.Value + "\n";
                }

                tblockResult.Text += "\n";
            }

            // Create the calenders and save to a files


            List<GCalenderEntry> gCalenders = new List<GCalenderEntry>();
            
            foreach(var courseDict in courseInfo) 
            {
                gCalenders.Add(new GCalenderEntry(courseDict));

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
                for (k = 0; k < table[0].Count; k++) {
                    string value = course.GetField(table[0][k]);
                    if (value == null)
                    {
                        value = GCalenderEntry.MissingFieldIndecator;
                    }
                    table[j].Add(value); 
                    // or table[table.Count - 1].Add(value);
                }
                j++; // increment the row to the next course
            }

            tblockResult.Text = "";
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

            tblockResult.Text += "What will be saved to file: \n" + content;

            
            var file = tbFileName.Text.Trim();
            var path = tbDirectoryName.Text.Trim();
            if (string.IsNullOrEmpty(file) || string.IsNullOrEmpty(path))
            {
                tblockInstructions.Text = "You must enter the file name and directory name.";
                return;
            }
            var fileName = Path.GetFileName(file);
            var pathIncomplete = Path.GetFullPath(path);
            var success = false;
            try
            {
                success = GCalenderEntry.GenerateCalenderFile(pathIncomplete, fileName, content, true);
            }
            catch (Exception ex) 
            {
                tblockInstructions.Text = ex.Message;
            }

            if (success) 
            {
                tblockInstructions.Text = "Successfully saved it to file.";
            }
        }

        private void cbAutoMatch_Checked(object sender, RoutedEventArgs e)
        {
            IntelligentMatch = true;
        }

        private void cbAutoMatch_Unchecked(object sender, RoutedEventArgs e)
        {
            IntelligentMatch = false;
        }

    }
}
