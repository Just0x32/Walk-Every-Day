using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Walk_Every_Day
{
    class Model
    {
        private List<string> filePaths = new List<string>();

        private List<List<InputDayDataItem>> inputAllDaysData = new List<List<InputDayDataItem>>();
        private List<OutputUserDataItem> outputAllUsersData = new List<OutputUserDataItem>();

        public Model()
        {
            filePaths.Add(@"E:\Developments\C#\repos\Walk-Every-Day\TestData\day1.json");           // read two files
            filePaths.Add(@"E:\Developments\C#\repos\Walk-Every-Day\TestData\day2.json");           //

        }

        public bool IsError() => IsFileReadingError | IsInputDataWrong | IsDayParsingError;

        public bool IsFileReadingError { get; private set; } = false;

        public bool IsInputDataWrong { get; private set; } = false;

        public bool IsDayParsingError { get; private set; } = false;

        public List<List<InputDayDataItem>> InputAllDaysData { get => inputAllDaysData; }     // Debug

        public List<OutputUserDataItem> OutputAllUsersData { get => outputAllUsersData; }     // Debug

        public void GetData()
        {
            ResetErrors();

            ReadDataFiles();
            ConvertDataStructure();

            void ResetErrors()
            {
                IsFileReadingError = false;
                IsInputDataWrong = false;
                IsDayParsingError = false;
            }
        }

        private void ReadDataFiles()
        {
            foreach (var item in filePaths)
            {
                List<InputDayDataItem> dayDataItems = ParseFile(item);

                if (IsFileReadingError || IsInputDataWrong)
                {
                    break;
                }
                else
                {
                    inputAllDaysData.Add(dayDataItems);
                }
            }
        }

        private void ConvertDataStructure()
        {
            int daysQuantity = inputAllDaysData.Count;
            int dayNumber;


            for (int i = 0; i < daysQuantity; i++)
            {
                dayNumber = ParseDayNumber(filePaths[i]);

                if (dayNumber < 0)
                {
                    IsDayParsingError = true;
                    break;
                }

                int outputIndexOfUser;
                string inputUser;
                int inputRank;
                string inputStatus;
                int inputSteps;

                for (int j = 0; j < inputAllDaysData[i].Count; j++)
                {
                    inputUser = inputAllDaysData[i][j].User;
                    inputRank = inputAllDaysData[i][j].Rank;
                    inputStatus = inputAllDaysData[i][j].Status;
                    inputSteps = inputAllDaysData[i][j].Steps;

                    outputIndexOfUser = OutputIndexOfUser(inputUser);

                    if (outputIndexOfUser < 0)
                        outputIndexOfUser = OutputIndexOfNewUser(inputUser);

                    AddOutputUserDayData(outputIndexOfUser, dayNumber, inputRank, inputStatus, inputSteps);
                }
            }

            int ParseDayNumber(string filePath)
            {
                int fileNameIndex = filePath.LastIndexOf(@"\") + 1;

                int dayNumber;

                if (!Int32.TryParse(filePath[(fileNameIndex + 3)..^5], out dayNumber))
                    dayNumber = -1;

                return dayNumber;
            }

            int OutputIndexOfUser(string userName)
            {
                for (int i = 0; i < outputAllUsersData.Count; i++)
                {
                    if (outputAllUsersData[i].User == userName)
                        return i;
                }

                return -1;
            }

            int OutputIndexOfNewUser(string userName)
            {
                outputAllUsersData.Add(new OutputUserDataItem());

                int lastIndex = outputAllUsersData.Count - 1;
                outputAllUsersData[lastIndex].User = userName;
                outputAllUsersData[lastIndex].UserDayDataList = new List<UserDayData>();

                return lastIndex;
            }

            void AddOutputUserDayData(int outputIndexOfUser, int dayNumberm, int inputRank, string inputStatus, int inputSteps)
            {
                outputAllUsersData[outputIndexOfUser].UserDayDataList.Add(new UserDayData());
                int outputLastIndex = outputAllUsersData[outputIndexOfUser].UserDayDataList.Count - 1;

                outputAllUsersData[outputIndexOfUser].UserDayDataList[outputLastIndex].Day = dayNumber;
                outputAllUsersData[outputIndexOfUser].UserDayDataList[outputLastIndex].Rank = inputRank;
                outputAllUsersData[outputIndexOfUser].UserDayDataList[outputLastIndex].Status = inputStatus;
                outputAllUsersData[outputIndexOfUser].UserDayDataList[outputLastIndex].Steps = inputSteps;
            }
        }

        private List<InputDayDataItem> ParseFile(string filePath)
        {
            StreamReader streamReader = null;
            List<InputDayDataItem> dayDataItems = null;

            try
            {
                streamReader = new StreamReader(filePath);

                string fileText = streamReader.ReadToEnd();
                dayDataItems = JsonSerializer.Deserialize<List<InputDayDataItem>>(fileText);
            }
            catch (IOException)
            {
                IsFileReadingError = true;
            }
            finally
            {
                streamReader?.Dispose();
            }

            if (dayDataItems is null)
            {
                IsInputDataWrong = true;
            }

            return dayDataItems;
        }

        public class InputDayDataItem
        {
            public int Rank { get; set; }

            public string User { get; set; }

            public string Status { get; set; }

            public int Steps { get; set; }
        }

        public class OutputUserDataItem
        {
            public string User { get; set; }

            public int AverageSteps { get; set; }

            public int MaxSteps { get; set; }

            public int MinSteps { get; set; }

            public List<UserDayData> UserDayDataList { get; set; }

        }

        public class UserDayData
        {
            public int Day { get; set; }

            public int Rank { get; set; }

            public string Status { get; set; }

            public int Steps { get; set; }
        }
    }
}
