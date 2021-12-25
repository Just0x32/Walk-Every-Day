using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Walk_Every_Day.DataTypes;

namespace Walk_Every_Day
{
    class ViewModel
    {
        Model model;

        public ViewModel() => model = new Model();

        public List<List<InputDayDataItem>> InputAllDaysData { get => model.InputAllDaysData; }     // Debug

        public bool IsError { get => model.IsError(); }

        public bool IsFileReadingError { get => model.IsFileReadingError; }

        public bool IsInputDataWrong { get=> model.IsInputDataWrong ; }

        public bool IsDayParsingError { get => model.IsDayParsingError; }

        public List<OutputUserDataItem> GetData() => model.GetData();

        public void SendFilePaths(string[] filePaths) => model.GetFilePaths(filePaths);

        public string ShowInputAllDaysData()             // Debug
        {
            StringBuilder inputData = new StringBuilder();

            inputData.AppendLine("Input data");
            inputData.AppendLine("IsFileReadingError: " + IsFileReadingError.ToString());
            inputData.AppendLine("IsInputDataWrong: " + IsInputDataWrong.ToString());
            inputData.AppendLine("IsDayParsingError: " + IsDayParsingError.ToString());

            for (int i = 0; i < InputAllDaysData.Count; i++)
            {
                inputData.Append(Environment.NewLine + "Element " + i);

                for (int j = 0; j < 7/*AllDaysData[i].Count*/; j++)
                {
                    inputData.Append(Environment.NewLine + "Rank: " + InputAllDaysData[i][j].Rank);
                    inputData.Append("      User: " + InputAllDaysData[i][j].User);
                    inputData.Append("      Status: " + InputAllDaysData[i][j].Status);
                    inputData.Append("      Steps: " + InputAllDaysData[i][j].Steps);
                }
            }

            return inputData.ToString();
        }
    }
}
