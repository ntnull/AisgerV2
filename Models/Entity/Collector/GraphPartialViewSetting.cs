using System.Collections.Generic;

namespace Aisger.Models.Entity.Collector
{
    public class GraphViewSetting
    {
        public string ContainerName { get; set; }
        public string Url { get; set; }
    }

    public class GraphViewParameter
    {
        public string CmdeviceCode { get; set; }
    }

    public class PieGraphParameter
    {
        public PieGraphParameter()
        {
            StatData = new List<StatData>();
            HighchartSettings = new HighchartSettings();
        }
        public List<StatData> StatData { get; set; }
        public HighchartSettings HighchartSettings { get; set; } 
    }

    public class RealtimeGraphParameter
    {
        public RealtimeGraphParameter()
        {
            HighchartSettings = new HighchartSettings();
            //AccountingActualPerLine = new AccountingActualPerLineViewModel();
        }
        public HighchartSettings HighchartSettings { get; set; }
    }

    public class ReatimeGraphData
    {
        public ReatimeGraphData()
        {
            Datasets = new List<Dataset>();
            //AccountingActualPerLine = new AccountingActualPerLineViewModel();
        }
        public List<Dataset> Datasets { get; set; }
    }

    public class StatData
    {
        public string Title { get; set; }
        public string Value { get; set; }
        public string ValuePercent { get; set; }
    }
}