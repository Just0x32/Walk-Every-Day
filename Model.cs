using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using Walk_Every_Day.DataTypes;

namespace Walk_Every_Day
{
    class Model
    {
        private List<string> filePaths;

        private List<List<InputDayDataItem>> inputAllDaysData;
        private List<OutputUserDataItem> outputAllUsersData;

        public Model() { }

        public bool IsError() => IsFileReadingError | IsInputDataWrong | IsDayParsingError | IsFileWritingError;

        public bool IsFileReadingError { get; private set; } = false;

        public bool IsFileWritingError { get; private set; } = false;

        public bool IsInputDataWrong { get; private set; } = false;

        public bool IsDayParsingError { get; private set; } = false;

        private void ResetAllDataStructures()
        {
            filePaths = new List<string>();
            inputAllDaysData = new List<List<InputDayDataItem>>();
            outputAllUsersData = new List<OutputUserDataItem>();
        }

        public void GetFilePaths(string[] filePaths)
        {
            if (filePaths != null)
            {
                ResetAllDataStructures();

                foreach (var item in filePaths)
                {
                    this.filePaths.Add(item);
                }
            }
        }

        public List<OutputUserDataItem> GetData()
        {
            ResetErrors();

            if (filePaths != null && filePaths.Count > 0)
            {
                ReadDataFiles();
                ConvertDataStructure();
                CalculateAverageMaxAndMinSteps();
            }
            else
            {
                IsFileReadingError = true;
            }

            return outputAllUsersData;
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
                outputAllUsersData[lastIndex].UserDayDataList = new List<OutputUserDataItem.UserDayData>();

                return lastIndex;
            }

            void AddOutputUserDayData(int outputIndexOfUser, int dayNumberm, int inputRank, string inputStatus, int inputSteps)
            {
                outputAllUsersData[outputIndexOfUser].UserDayDataList.Add(new OutputUserDataItem.UserDayData());
                int outputLastIndex = outputAllUsersData[outputIndexOfUser].UserDayDataList.Count - 1;

                outputAllUsersData[outputIndexOfUser].UserDayDataList[outputLastIndex].Day = dayNumber;
                outputAllUsersData[outputIndexOfUser].UserDayDataList[outputLastIndex].Rank = inputRank;
                outputAllUsersData[outputIndexOfUser].UserDayDataList[outputLastIndex].Status = inputStatus;
                outputAllUsersData[outputIndexOfUser].UserDayDataList[outputLastIndex].Steps = inputSteps;
            }
        }

        private void CalculateAverageMaxAndMinSteps()
        {
            int sumOfSteps;
            int dayCount;
            int maxSteps;
            int minSteps;

            for (int i = 0; i < outputAllUsersData.Count; i++)
            {
                ResetStepsVariables();

                foreach (var userDayData in outputAllUsersData[i].UserDayDataList)
                {
                    if (userDayData.Steps > maxSteps)
                        maxSteps = userDayData.Steps;

                    if (userDayData.Steps < minSteps)
                        minSteps = userDayData.Steps;

                    sumOfSteps += userDayData.Steps;
                    dayCount++;
                }

                outputAllUsersData[i].AverageSteps = sumOfSteps / dayCount;
                outputAllUsersData[i].MaxSteps = maxSteps;
                outputAllUsersData[i].MinSteps = minSteps;
            }

            void ResetStepsVariables()
            {
                sumOfSteps = 0;
                dayCount = 0;
                maxSteps = Int32.MinValue;
                minSteps = Int32.MaxValue;
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

        public void SaveCurrentUserData(int userIndex, string filePath)
        {
            ResetErrors();

            if (filePath != null)
            {
                SaveUserDataFile(outputAllUsersData[userIndex], filePath);
            }
            else
            {
                IsFileWritingError = true;
            }
        }

        private void SaveUserDataFile(OutputUserDataItem userData, string filePath)
        {
            StreamWriter streamWriter = null;

            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                };

                streamWriter = new StreamWriter(filePath);
                string jsonText = JsonSerializer.Serialize(userData, options);

                streamWriter.Write(jsonText);
            }
            catch (IOException)
            {
                IsFileWritingError = true;
            }
            finally
            {
                streamWriter?.Dispose();
            }
        }

        private void ResetErrors()
        {
            IsFileReadingError = false;
            IsFileWritingError = false;
            IsInputDataWrong = false;
            IsDayParsingError = false;
        }
    }
}
