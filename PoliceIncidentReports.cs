using PagedList;
using SODA;
using OnlineDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineDemo.SodaObjects
{
    public class PoliceIncidentReports
    {
        #region Constants

        /// <summary>
        /// App Token from Socrata registered app - get yours at https://opendata.socrata.com/login
        /// </summary>
        private const string _AppToken = "BBWufDU3P6b8EP7tR2m9QDEEE";

        /// <summary>
        /// HostName of “https://data.vbgov.com/resource/v6g5-47dd.json”;
        /// </summary>
        private const string _APIEndPointHost = "data.vbgov.com";

        /// <summary>
        /// Socrata 4x4 Identifier
        /// </summary>
        private const string _APIEndPoint4x4 = "v6g5-47dd";

        private const string _APIEndPoint4x4Jan17 = "4x2a-5ujw";

        private const string _APIEndpoint4x4Both = "99wv-nxv5";

        #endregion

        #region Methods
        /// <summary>
        /// Gets sorted list of all Business Locations in the dataset by filter
        /// </summary>
        /// <param name=”SearchQuery”>Query to filter on</param>
        /// <param name=”PageNumber”>Current page number</param>
        /// <param name=”PageSize”>Number of items per page</param>
        /// <param name=”OrderBy”>Column name to sequence the list by</param>
        /// <param name=”OrderByAscDesc”>Sort direction</param>
        /// <returns>object PagedList</returns>
        public static PagedList<IncidentReports> GetIncidentReports(string SearchQuery, int PageNumber,
           int PageSize, string OrderBy, bool OrderByAscDesc)
        {
            //Create client to talk to OpenDat API Endpoint
            var client = new SodaClient(_APIEndPointHost, _AppToken);
            //get a reference to the resource itself the result (a Resouce object) is a generic type
            //the type parameter represents the underlying rows of the resource
            var dataset = client.GetResource<PagedList<IncidentReports>>(_APIEndpoint4x4Both);
            //Build the select list of columns for the SoQL call
            string[] columns = new[] { "police_case_number", "date_reported", "offense_code", "offense_description", "subdivision",
           "zone_id", "case_status" };

            //Column alias must not collide with input column name, i.e. don’t alias ‘city’ as ‘city’
            string[] aliases = new[] { "CaseNumber", "DateReported", "OffenseCode", "Offense", "SubDiv",
         "Zone", "Status"};
            //using SoQL and a fluent query building syntax
            var soql = new SoqlQuery().Select(columns)
            .As(aliases)
            .Order((OrderByAscDesc) ? SoqlOrderDirection.ASC : SoqlOrderDirection.DESC, new[] { OrderBy });
            if (!string.IsNullOrEmpty(SearchQuery))
            {
                soql = new SoqlQuery().FullTextSearch(SearchQuery);
            }
            var results = dataset.Query<IncidentReports>(soql);
            //page’em cause there might be quite a few
            PagedList<IncidentReports> pagedResults = new PagedList<IncidentReports>(results.ToList(),
               PageNumber, PageSize);
            return pagedResults;
        }

        public static List<IncidentReports> ForExcel()
        {
            //Create client to talk to OpenDat API Endpoint 
            var client = new SodaClient(_APIEndPointHost, _AppToken);

            var dataset = client.GetResource<List<IncidentReports>>(_APIEndpoint4x4Both);

            string[] columns = new[] { "police_case_number", "date_reported", "offense_code", "offense_description", "subdivision",
           "zone_id", "case_status" };

            string[] aliases = new[] { "CaseNumber", "DateReported", "OffenseCode", "Offense", "SubDiv",
         "Zone", "Status" };

            var soql = new SoqlQuery().Select(columns)
                         .As(aliases);

            var results = dataset.Query<IncidentReports>(soql);

            List<IncidentReports> excelResults = new List<IncidentReports>(results.ToList());
            return excelResults;
        }

        public static List<ForPie> SubDivPie()
        {
            //Create client to talk to OpenDat API Endpoint 
            var client = new SodaClient(_APIEndPointHost, _AppToken);

            var dataset = client.GetResource<List<ForPie>>(_APIEndpoint4x4Both);

            string[] columns = new[] { "subdivision" };

            string[] aliases = new[] { "SubDivForPie" };

            var soql = new SoqlQuery().Select(columns)
                         .As(aliases);

            var results = dataset.Query<ForPie>(soql);

            List<ForPie> chartResults = new List<ForPie>(results.ToList());
            return chartResults;
        }

        public static List<ForStatus> BarForStatus()
        {
            //Create client to talk to OpenDat API Endpoint 
            var client = new SodaClient(_APIEndPointHost, _AppToken);

            var dataset = client.GetResource<List<ForStatus>>(_APIEndpoint4x4Both);

            string[] columns = new[] { "case_status" };

            string[] aliases = new[] { "Status" };

            var soql = new SoqlQuery().Select(columns)
                         .As(aliases);

            var results = dataset.Query<ForStatus>(soql);

            List<ForStatus> chartResults = new List<ForStatus>(results.ToList());
            return chartResults;
        }

        public static List<ForChartDesc> DescriptionChart()
        {
            //Create client to talk to OpenDat API Endpoint 
            var client = new SodaClient(_APIEndPointHost, _AppToken);

            var dataset = client.GetResource<List<ForChartDesc>>(_APIEndPoint4x4);

            string[] columns = new[] { "offense_description" };

            string[] aliases = new[] { "Offense" };

            var soql = new SoqlQuery().Select(columns)
                         .As(aliases);

            var results = dataset.Query<ForChartDesc>(soql);

            List<ForChartDesc> chartResults = new List<ForChartDesc>(results.ToList());
            return chartResults;
        }

        public static List<ForChartDesc> DescriptionChart2()
        {
            //Create client to talk to OpenDat API Endpoint 
            var client = new SodaClient(_APIEndPointHost, _AppToken);

            var dataset = client.GetResource<List<ForChartDesc>>(_APIEndPoint4x4Jan17);

            string[] columns = new[] { "offense_description" };

            string[] aliases = new[] { "Offense" };

            var soql = new SoqlQuery().Select(columns)
                         .As(aliases);

            var results = dataset.Query<ForChartDesc>(soql);

            List<ForChartDesc> chartResults = new List<ForChartDesc>(results.ToList());
            return chartResults;
        }

        #endregion

    }
}