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

        private int stepsDeviation;
        private readonly int defaultStepsDeviation = 20;
        private readonly int defaultMinStepsDeviation = 5;
        private readonly int defaultMaxStepsDeviation = 95;

        public MainWindow()
        {
            InitializeComponent();

            SetStepsDeviationValue(defaultStepsDeviation);

            viewModel = new ViewModel();




        }

        class S
        {
            public string User { get; set; }
            public string AverageSteps { get; set; }
            public string MaxSteps { get; set; }
            public string MinSteps { get; set; }
        }


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

                    UserListListBox.ItemsSource = usersData;

                    //  Create list events and graph 

                }

                //MessageBox.Show(viewModel.ShowInputAllDaysData());                          // Debug
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
                SetStepsDeviationValue(stepsDeviation);
            }

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
            stepsDeviation = value;
            DeviationTextBox.Text = stepsDeviation.ToString();
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
    }
}
