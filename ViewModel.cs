using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Walk_Every_Day
{
    class ViewModel
    {
        Model model;

        public ViewModel() => model = new Model();

        public List<List<Model.InputDayDataItem>> InputAllDaysData { get => model.InputAllDaysData; }

        public List<Model.OutputUserDataItem> OutputAllUsersData { get => model.OutputAllUsersData; }

        public bool IsError { get => model.IsError(); }

        public bool IsFileReadingError { get => model.IsFileReadingError; }

        public bool IsInputDataWrong { get=> model.IsInputDataWrong ; }

        public bool IsDayParsingError { get => model.IsDayParsingError; }

        public void GetData() => model.GetData();

        public string ShowInputAllDaysData()             // Debug
        {
            StringBuilder inputData = new StringBuilder();

            inputData.AppendLine("Input data");
            inputData.AppendLine("IsFileReadingError: " + IsFileReadingError.ToString());
            inputData.AppendLine("IsInputDataWrong: " + IsInputDataWrong.ToString());

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

        public string ShowOutputAllUsersData()             // Debug
        {
            StringBuilder outputData = new StringBuilder();

            outputData.AppendLine("Output data");

            for (int i = 0; i < 7/*OutputAllUsersData.Count*/; i++)
            {
                outputData.Append(Environment.NewLine + Environment.NewLine + "User: " + OutputAllUsersData[i].User);
                outputData.Append(Environment.NewLine + "AverageSteps: " + OutputAllUsersData[i].AverageSteps);
                outputData.Append(Environment.NewLine + "MaxSteps: " + OutputAllUsersData[i].MaxSteps);
                outputData.Append(Environment.NewLine + "MinSteps: " + OutputAllUsersData[i].MinSteps);

                for (int j = 0; j < OutputAllUsersData[i].UserDayDataList.Count; j++)
                {
                    outputData.Append(Environment.NewLine + "Day: " + OutputAllUsersData[i].UserDayDataList[j].Day);
                    outputData.Append("     Rank: " + OutputAllUsersData[i].UserDayDataList[j].Rank);
                    outputData.Append("     Status: " + OutputAllUsersData[i].UserDayDataList[j].Status);
                    outputData.Append("     Steps: " + OutputAllUsersData[i].UserDayDataList[j].Steps);
                }
            }

            return outputData.ToString();
        }
    }
}
