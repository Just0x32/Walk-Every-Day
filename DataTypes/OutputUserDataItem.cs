using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Walk_Every_Day.DataTypes
{
    public class OutputUserDataItem
    {
        public string User { get; set; }

        public int AverageSteps { get; set; }

        public int MaxSteps { get; set; }

        public int MinSteps { get; set; }

        public List<UserDayData> UserDayDataList { get; set; }

        public class UserDayData
        {
            public int Day { get; set; }

            public int Rank { get; set; }

            public string Status { get; set; }

            public int Steps { get; set; }
        }
    }
}
