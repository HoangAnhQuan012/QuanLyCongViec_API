using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quanLyCongViec.Global
{
    public class GlobalModel
    {
        public static SortedList<int, string> ProjectStatusSorted = new SortedList<int, string>
        {
            { 0, "Not Start" },
            { 1, "In Progress" },
            { 2, "Completed" },
        };

        public static SortedList<int, string> KindOfJob = new SortedList<int, string>
        {
            {1, "Q&A" },
            {2, "DEV" },
            {3, "Fix bug" },
            {4, "Unit Test" },
            {5, "Fix bug UT" },
            {6, "Meeting" },
            {7, "Waiting for work" },
            {8, "Trực hệ thống" },
            {9, "Write BRD" },
            {10, "Merge build code" }
        };

        public static SortedList<int, string> Type = new SortedList<int, string>
        {
            {1, "Normal" },
            {2, "OT" },
            {3, "Outsource" }
        };

        public static SortedList<int, string> WorkReportStatus = new SortedList<int, string>
        {
            { 0, "Wait for approval" },
            { 1, "Approved" },
            { 2, "Rejected" },
        };
    }
}
