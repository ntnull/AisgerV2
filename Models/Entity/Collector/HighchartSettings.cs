using System;
using System.Collections.Generic;

namespace Aisger.Models.Entity.Collector
{
    public class HighchartSettings
    {
        //public string chartType { get; set; }
        //public string chartTitle { get; set; }
        // public string chartSubtitle { get; set; }
        public string yTitle { get; set; }
        public object[] categories { get; set; }
        public Series[] series { get; set; }
        public Dataset[] datasets { get; set; }
        public string valueSuffix { get; set; }

        // Additional Fields
        public string additionalValue { get; set; }
    }


    public class Series
    {
        public Series()
        {
            data = new List<Object>();
            colorByPoint = true;
        }
        public string name { get; set; }
        public bool colorByPoint { get; set; }
        public List<object> data { get; set; }
        public double y { get; set; }

        // additional data
        public string unit { get; set; }
    }

    public class DataObject
    {
        public string name { get; set; }
        public double y { get; set; }
        public string color { get; set; }
    }

    public class Dataset
    {
        // public string code { get; set; }
        public string name { get; set; }
        public object xdata { get; set; }
        public object ydata { get; set; }
        public string unit { get; set; }
    }
}