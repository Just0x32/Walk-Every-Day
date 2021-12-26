using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Walk_Every_Day.DataTypes;

namespace Walk_Every_Day
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel viewModel;

        private List<OutputUserDataItem> usersData;
        List<ExtendedItem> extendedList;

        private readonly Color defaultDeviationAtMaxColor = Colors.Green;
        private readonly Color defaultDeviationAtMinColor = Colors.Red;
        private readonly Color defaultDeviationAtMaxAndMinColor = Colors.Blue;
        private readonly Color defaultDeviationAtNoneColor = Colors.Transparent;

        private int stepsDeviationInProcents;
        private readonly int defaultStepsDeviation = 20;
        private readonly int defaultMinStepsDeviation = 5;
        private readonly int defaultMaxStepsDeviation = 95;

        public MainWindow()
        {
            InitializeComponent();

            SetStepsDeviationValue(defaultStepsDeviation);

            viewModel = new ViewModel();

            UserListListBox.SelectionChanged += UserListListBox_SelectionChanged;


        }

        private int UserListSelectedIndex { get => UserListListBox.SelectedIndex; }

        private void HandleError()
        {
            if (viewModel.IsFileReadingError)
                MessageBox.Show("File reading error!");

            if (viewModel.IsInputDataWrong)
                MessageBox.Show("Input data is wrong!");

            if (viewModel.IsDayParsingError)
                MessageBox.Show("Day parsing error!");
        }

        private void SendFilePaths(string[] filePaths) => viewModel.SendFilePaths(filePaths);

        private void LoadDataButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON files (*.json)|*.json";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                SendFilePaths(openFileDialog.FileNames);

                usersData = viewModel.GetData();

                if (viewModel.IsError)
                {
                    HandleError();
                }
                else if (usersData != null)
                {
                    //MessageBox.Show(ShowUsersData());               // Debug

                    UserListListBox.ItemsSource = MakeUserList(usersData);


                    //  Create list events and graph 

                }

                //MessageBox.Show(viewModel.ShowInputAllDaysData());                          // Debug
            }
        }

        private List<ExtendedItem> MakeUserList(List<OutputUserDataItem> usersData)
        {
            if (usersData != null)
            {
                extendedList = new List<ExtendedItem>();

                foreach (var item in usersData)
                {
                    extendedList.Add(new ExtendedItem
                    {
                        User = item.User,
                        AverageSteps = item.AverageSteps,
                        MaxSteps = item.MaxSteps,
                        MinSteps = item.MinSteps,
                        UserDayDataList = item.UserDayDataList,
                        BackgroundColor = GetBackgroundColor(item.AverageSteps, item.MaxSteps, item.MinSteps)
                    });
                }
            }

            return extendedList;

            string GetBackgroundColor(int averageSteps, int maxSteps, int minSteps)
            {
                bool isMaxStepsDeviation = false;
                bool isMinStepsDeviation = false;

                int absoluteDeviation = (int)((float)averageSteps * (float)stepsDeviationInProcents / 100f);

                if ((maxSteps - averageSteps) > absoluteDeviation)
                    isMaxStepsDeviation = true;

                if ((averageSteps - minSteps) > absoluteDeviation)
                    isMinStepsDeviation = true;

                if (isMaxStepsDeviation && isMinStepsDeviation)
                {
                    return defaultDeviationAtMaxAndMinColor.ToString();
                }
                else if (isMaxStepsDeviation)
                {
                    return defaultDeviationAtMaxColor.ToString();
                }
                else if (isMinStepsDeviation)
                {
                    return defaultDeviationAtMinColor.ToString();
                }
                else
                {
                    return defaultDeviationAtNoneColor.ToString();
                }
            }
        }

        private void UserListListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (UserListSelectedIndex >= 0)
            {
                //GraphTextBox.Text = UserListSelectedIndex.ToString();                                           // Debug

                // Create graph
            }
            else
            {
                // Clear graph
            }

        }

        private void ExportDataButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeviationSetButton_Click(object sender, RoutedEventArgs e)
        {
            int textBoxValue;

            if (Int32.TryParse(DeviationTextBox.Text, out textBoxValue))
            {
                SetStepsDeviationValue(ReturnValidStepsDeviationValue(textBoxValue));
            }
            else
            {
                SetStepsDeviationValue(stepsDeviationInProcents);
            }

            UserListListBox.ItemsSource = MakeUserList(usersData);

            int ReturnValidStepsDeviationValue(int value)
            {
                if (value < defaultMinStepsDeviation)
                {
                    return defaultMinStepsDeviation;
                }
                else if (value > defaultMaxStepsDeviation)
                {
                    return defaultMaxStepsDeviation;
                }
                else
                {
                    return value;
                }
            }
        }

        private void SetStepsDeviationValue(int value)
        {
            stepsDeviationInProcents = value;
            DeviationTextBox.Text = stepsDeviationInProcents.ToString();
        }

        private string ShowUsersData()             // Debug
        {
            StringBuilder outputData = new StringBuilder();

            outputData.AppendLine("Output data");

            for (int i = 0; i < 7/*OutputAllUsersData.Count*/; i++)
            {
                outputData.Append(Environment.NewLine + Environment.NewLine + "User: " + usersData[i].User);
                outputData.Append(Environment.NewLine + "AverageSteps: " + usersData[i].AverageSteps);
                outputData.Append(Environment.NewLine + "MaxSteps: " + usersData[i].MaxSteps);
                outputData.Append(Environment.NewLine + "MinSteps: " + usersData[i].MinSteps);

                for (int j = 0; j < usersData[i].UserDayDataList.Count; j++)
                {
                    outputData.Append(Environment.NewLine + "Day: " + usersData[i].UserDayDataList[j].Day);
                    outputData.Append("     Rank: " + usersData[i].UserDayDataList[j].Rank);
                    outputData.Append("     Status: " + usersData[i].UserDayDataList[j].Status);
                    outputData.Append("     Steps: " + usersData[i].UserDayDataList[j].Steps);
                }
            }

            return outputData.ToString();
        }

        public class ExtendedItem : OutputUserDataItem
        {
            public string BackgroundColor { get; set; }
        }
    }
}
