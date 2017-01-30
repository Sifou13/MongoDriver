using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace DataTypes
{    
    public class ReportData
    {
        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public Guid ID { get; set; }

        public string[] Data { get; set; }
    }

    public class SortableReportData<TSortableProperty1> : ReportData
    {
        public TSortableProperty1 SortableProperty1 { get; set; }
    }

    public class SortableReportData<TSortableProperty1, TSortableProperty2> : ReportData
    {
        public TSortableProperty1 SortableProperty1 { get; set; }

        public TSortableProperty2 SortableProperty2 { get; set; }
    }

    public class SortableReportData<TSortableProperty1, TSortableProperty2, TSortableProperty3> : ReportData
    {
        public TSortableProperty1 SortableProperty1 { get; set; }

        public TSortableProperty2 SortableProperty2 { get; set; }

        public TSortableProperty3 SortableProperty3 { get; set; }
    }
    
    public static class SortableReportDataExtensions
    {
        public static void SetSortableProperties<TProp1, TProp2, TProp3>(this SortableReportData<TProp1, TProp2, TProp3> reportData, TProp1 prop1, TProp2 prop2, TProp3 prop3)
        {
            reportData.SortableProperty1 = prop1;
            reportData.SortableProperty2 = prop2;
            reportData.SortableProperty3 = prop3;
        }
    }
}
