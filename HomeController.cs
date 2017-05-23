using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Timers;
using OnlineDemo.SodaObjects;
using OnlineDemo.Models;

namespace OnlineDemo.Controllers
{
    [OutputCache(NoStore = true, Duration = 0)]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public static List<string> listOfCombinations = new List<string>();
        bool timedOut = false;

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Recursion(string targetSum)
        {
            string errorMsg = string.Empty;
            int sum;

            bool isNumber = Int32.TryParse(targetSum, out sum);

            if (isNumber)
            {
                ViewBag.Sum = sum;

                if(sum < 1 || sum > 50)
                {
                    errorMsg = "Enter a number between 1 and 50.";
                    ViewBag.ErrorMsg = errorMsg;
                    ViewBag.Sum = string.Empty;
                    return PartialView();
                }
            }
            else
            {
                errorMsg = "Enter a number between 1 and 50.";
                ViewBag.ErrorMsg = errorMsg;
                ViewBag.Sum = string.Empty;
                return PartialView();
            }

            ViewBag.ErrorMsg = string.Empty;

            // clear the lists for a new action
            listOfCombinations.Clear();

            List<string> ListForDistinct = new List<string>();
            List<string> ListForView = new List<string>();

            // keeping array to size 4
            int arraySize = 4;

            int[] randomArray = new int[arraySize];

            Random randy = new Random();

            for (int i = 0; i < arraySize; i++)
            {
                randomArray[i] = randy.Next(1, sum + 1);
            }

            Array.Sort(randomArray);

            // to display the integers in the view
            ViewBag.RandArrayView =
                randomArray[0] + " | " + randomArray[1] + " | " + randomArray[2] + " | " + randomArray[3];

            Timer t = new Timer(1100);
            t.Elapsed += OnTimedEvent;
            t.AutoReset = false;
            t.Start();

            try
            {
                // recursion
                for (int i = 0; i < randomArray.Length; i++)
                {
                    if (timedOut)
                    {
                        timedOut = false;
                        return null;
                    }
                    GetCombinations(randomArray, sum, 0, i, string.Empty);
                }

                if (timedOut)
                {
                    timedOut = false;
                    return null;
                }

            }
            catch
            {
                return null;
            }            

            foreach (string item in listOfCombinations)
            {
                string comboLine = "";

                // Convert the item into an array of items
                string[] SolutionArray = item.Split(',');

                List<int> SolutionList = new List<int>();

                // Convert the array into a list of ints
                foreach (string solutionItem in SolutionArray)
                {
                    SolutionList.Add(int.Parse(solutionItem));
                }

                // Order the list
                SolutionList.Sort();

                int lastIndex = 0;
                foreach (int num in SolutionList)
                {
                    lastIndex += 1;
                    comboLine += (lastIndex == SolutionList.Count) ? num + "" : num + ",";
                }

                ListForDistinct.Add(comboLine);
            }

            // ListForView = ListForDistinct;
            ViewBag.ViewOfList = ListForDistinct.Distinct().ToList();

            return PartialView();
        }

        public void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            timedOut = true;
        }

        private string AddPossible(string comboLine, int arrayIndex)
        {
            if (string.IsNullOrEmpty(comboLine))
                return arrayIndex.ToString();
            else
                return string.Format("{0},{1}", comboLine, arrayIndex.ToString());
        }

        public void GetCombinations(int[] passArray, int sum, int currentTotal, int index, string carryOver)
        {
            // add current index to total value
            currentTotal += passArray[index];

            // check if currentToal is more than target sum
            if (currentTotal > sum)
            {
                return;
            }

            // if current total matches sum then add to results
            if (currentTotal == sum)
            {
                carryOver = AddPossible(carryOver, passArray[index]);
                listOfCombinations.Add(carryOver);
                return;
            }

            // if sum has not been reached or exceeded, add carryOver and begin recursion
            if (currentTotal < sum)
            {
                // add to to the combo line (carryOver)
                carryOver = carryOver = AddPossible(carryOver, passArray[index]);

                // recursion
                for (int i = 0; i < passArray.Length; i++)
                {
                    if (timedOut)
                    {
                        return;
                    }
                    GetCombinations(passArray, sum, currentTotal, i, carryOver);
                }

                return;
            }

            return; // greater than while condition to catch error
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult SubmitForm(string message)
        {
            if (message.Length > 100)
            {
                ViewBag.FormErrorMsg = "Under 100 characters.";
                return null;
            }

            ViewBag.FormErrorMsg = string.Empty;
            ViewBag.Msg = message;

            return PartialView();

        }

        public ActionResult LoadOpenData(string SortOrder, string CurrentFilter, string SearchQuery, int? CurrentPage,
            string ListOption)
        {
            ViewBag.TypeOfReport = ListOption;

            //Handle Paging
            int pageSize = 10;
            int pageNumber = (CurrentPage ?? 1);

            //Handle Sorting by Column
            if (string.IsNullOrEmpty(SortOrder))
            {
                SortOrder = "";
            }

            ViewBag.CurrentSort = SortOrder;
            ViewBag.CaseNumSort = (SortOrder == "police_case_number") ? "police_case_number_desc" : "police_case_number";
            ViewBag.DateReportedSort = (SortOrder == "date_reported") ? "date_reported_desc" : "date_reported";
            ViewBag.DescriptionSort = (SortOrder == "offense_description") ? "offense_description_desc" : "offense_description";
            ViewBag.StatusSort = (SortOrder == "case_status") ? "case_status_desc" : "case_status";

            bool sortAsc = (!SortOrder.Contains("_desc")) ? true : false;

            //Handle Filtering
            if (!string.IsNullOrEmpty(SearchQuery))
            {
                CurrentPage = 1;
            }
            else
            {
                SearchQuery = CurrentFilter;
            }
            ViewBag.CurrentFilter = SearchQuery;
            if (!string.IsNullOrEmpty(SearchQuery))
            {
                SearchQuery = SearchQuery.ToUpper().Trim();
            }

            //Call out to OpenData API
            var bigData = PoliceIncidentReports.GetIncidentReports(SearchQuery, pageNumber, pageSize, SortOrder.
            Replace("_desc", ""), sortAsc);

            return PartialView(bigData);
        }

        public ActionResult DescicriptionChartView()
        {
            var Descriptions = PoliceIncidentReports.DescriptionChart();
            var Descriptions2 = PoliceIncidentReports.DescriptionChart2();

            int drugCount = 0, drugCount2 = 0;
            int larcenyCount = 0, larcenyCount2 = 0;
            int fruadCount = 0, fruadCount2 = 0;
            int hitCount = 0, hitCount2 = 0;
            int assaultCount = 0, assaultCount2 = 0;

            foreach (ForChartDesc item in Descriptions)
            {
                if (item.Offense.ToLower().Contains("drug"))
                {
                    drugCount += 1;
                }
                else if (item.Offense.ToLower().Contains("larceny"))
                {
                    larcenyCount += 1;
                }
                else if (item.Offense.ToLower().Contains("fraud"))
                {
                    fruadCount += 1;
                }
                else if (item.Offense.ToLower().Contains("hit"))
                {
                    hitCount += 1;
                }
                else if (item.Offense.ToLower().Contains("assault"))
                {
                    assaultCount += 1;
                }
            }

            foreach (ForChartDesc item in Descriptions2)
            {
                if (item.Offense.ToLower().Contains("drug"))
                {
                    drugCount2 += 1;
                }
                else if (item.Offense.ToLower().Contains("larceny"))
                {
                    larcenyCount2 += 1;
                }
                else if (item.Offense.ToLower().Contains("fraud"))
                {
                    fruadCount2 += 1;
                }
                else if (item.Offense.ToLower().Contains("hit"))
                {
                    hitCount2 += 1;
                }
                else if (item.Offense.ToLower().Contains("assault"))
                {
                    assaultCount2 += 1;
                }
            }

            ViewBag.DrugCount = drugCount; ViewBag.DrugCount2 = drugCount2;
            ViewBag.LarcenyCount = larcenyCount; ViewBag.LarcenyCount2 = larcenyCount2;
            ViewBag.FraudCount = fruadCount; ViewBag.FraudCount2 = fruadCount2;
            ViewBag.HitCount = hitCount; ViewBag.HitCount2 = hitCount2;
            ViewBag.AssaultCount = assaultCount; ViewBag.AssaultCount2 = assaultCount2;

            return PartialView();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}