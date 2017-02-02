using DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MongoDriver
{
    internal class Program2
    {
        private static MongoDBHelper mongodbHelper = new MongoDBHelper();
        private static Random random = new Random();
        private static readonly string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private static void Main(string[] args)
        {
            for (int index = 1; index <= 100; index++)
            {
                //somebody from above built this resource contract
                ReportDataContract reportDataContract = GenerateReportDataContract();
                reportDataContract.SortableValues = new object[] { new DateTime(1980, 05, 13), new DateTime(2011, 01, 03), 1547896, "My Name" };

                //here is the resource code V1
                object[] sortableValues = reportDataContract.SortableValues;
                Type[] sortablePropTypes = sortableValues.Select(val => val.GetType()).ToArray();

                //TODO : Try to implement Caching for below method
                MethodInfo createSortableReportData = typeof(Program2)
                                .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                                .Where(method => method.IsGenericMethod && method.Name == "CreateSortableReportData" && method.GetGenericArguments().Length == sortablePropTypes.Length)
                                .First()
                                .MakeGenericMethod(sortablePropTypes);

                object[] parameters = new object[sortableValues.Length + 1];
                parameters[0] = reportDataContract;
                Array.Copy(sortableValues, 0, parameters, 1, sortableValues.Length);

                object sortableReportData = createSortableReportData.Invoke(null, parameters);

                InsertReportData(sortableReportData);
            }
        }

        private static ReportDataContract GenerateReportDataContract()
        {
            List<string> items = new List<string>();
            int numberOfDataItems = random.Next(100);

            for (int itemIndex = 0; itemIndex < numberOfDataItems; itemIndex++)
            {
                items.Add(new string(Enumerable.Repeat(chars, random.Next(1000)).Select(s => s[random.Next(s.Length)]).ToArray()));
            }

            ReportDataContract reportData = new ReportDataContract
            {
                ID = Guid.NewGuid(),
                Data = items.ToArray(),
            };

            return reportData;
        }

        #region CreateSortableReportData

        private static ReportData CreateSortableReportData<TProp1>(ReportDataContract reportData, TProp1 prop1)
        {
            SortableReportData<TProp1> sortableReportData = new SortableReportData<TProp1>();

            AssignReportData(reportData, sortableReportData);
            sortableReportData.Property1 = prop1;

            return sortableReportData;
        }

        private static ReportData CreateSortableReportData<TProp1, TProp2>(ReportDataContract reportData, TProp1 prop1, TProp2 prop2)
        {
            SortableReportData<TProp1, TProp2> sortableReportData = new SortableReportData<TProp1, TProp2>();

            AssignReportData(reportData, sortableReportData);

            sortableReportData.Property1 = prop1;
            sortableReportData.Property2 = prop2;

            return sortableReportData;
        }

        private static ReportData CreateSortableReportData<TProp1, TProp2, TProp3>(ReportDataContract reportData, TProp1 prop1, TProp2 prop2, TProp3 prop3)
        {
            SortableReportData<TProp1, TProp2, TProp3> sortableReportData = new SortableReportData<TProp1, TProp2, TProp3>();

            AssignReportData(reportData, sortableReportData);

            sortableReportData.Property1 = prop1;
            sortableReportData.Property2 = prop2;
            sortableReportData.Property3 = prop3;

            return sortableReportData;
        }

        private static ReportData CreateSortableReportData<TProp1, TProp2, TProp3, TProp4>(ReportDataContract reportData, TProp1 prop1, TProp2 prop2, TProp3 prop3, TProp4 prop4)
        {
            SortableReportData<TProp1, TProp2, TProp3, TProp4> sortableReportData = new SortableReportData<TProp1, TProp2, TProp3, TProp4>();

            AssignReportData(reportData, sortableReportData);

            sortableReportData.Property1 = prop1;
            sortableReportData.Property2 = prop2;
            sortableReportData.Property3 = prop3;
            sortableReportData.Property4 = prop4;

            return sortableReportData;
        }

        private static void AssignReportData(ReportDataContract source, ReportData destination)
        {
            destination.ID = source.ID;
            destination.Data = source.Data;
        }

        #endregion CreateSortableReportData

        private static void InsertReportData(object sortableReportData)
        {
            mongodbHelper.Insert<object>("Local-TestDBSif", "ReportDataSif_Generic", new object[] { sortableReportData });
        }
    }


}
