using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineDemo.Models
{
    public class IncidentReports
    {
        public string ID { get; set; }
        public int CaseNumber { get; set; }
        public DateTime DateReported { get; set; }
        public string OffenseCode { get; set; }
        public string Offense { get; set; }
        public string SubDiv { get; set; }
        public int Zone { get; set; }
        public string Status { get; set; }
    }

    public class ForChartDesc
    {
        public string ID { get; set; }
        public string Offense { get; set; }
    }

    public class ForPie
    {
        public string ID { get; set; }
        public string SubDivForPie { get; set; }
    }

    public class ForStatus
    {
        public string ID { get; set; }
        public string Status { get; set; }
    }
}