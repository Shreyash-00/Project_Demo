using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Daley_Demo.API.Models
{
    public class DeviceChartConfig
    {
        // DeviceType related properties
        public int DeviceTypeID { get; set; }
        public string DeviceTypeName { get; set; }

        // ChartConfig related properties
        public int ID { get; set; }
        public string ChartHeading { get; set; }
        public string ChartType { get; set; }
        public List<string> Fields { get; set; }  // Will be serialized as JSON
        public  ChartOptions Options { get; set; }  // Will be serialized as JSON  // Will be serialized as JSON
        public int Status { get; set; }

    }
   
}