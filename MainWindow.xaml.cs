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
using Walk_Every_Day.GraphTypes;

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

        private readonly Color defaultDeviationAtMaxColor = Colors.LightGreen;
        private readonly Color defaultDeviationAtMinColor = Colors.PaleVioletRed;
        private readonly Color defaultDeviationAtMaxAndMinColor = Colors.PowderBlue;
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

            if (viewModel.IsFileWritingError)
                MessageBox.Show("File writing error!");

            if (viewModel.IsInputDataWrong)
                MessageBox.Show("Input data is wrong!");

            if (viewModel.IsDayParsingError)
                MessageBox.Show("Day parsing error!");
        }

        private void SendInputFilePaths(string[] filePaths) => viewModel.SendFilePaths(filePaths);

        private void LoadDataButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON files (*.json)|*.json";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                SendInputFilePaths(openFileDialog.FileNames);

                usersData = viewModel.GetData();

                if (viewModel.IsError)
                {
                    HandleError();
                }
                else if (usersData != null)
                {
                    UserListListBox.ItemsSource = MakeUserList(usersData);
                }
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
            DeleteGraph();

            if (UserListSelectedIndex >= 0 && usersData != null)
            {
                int[] graphBorders = GraphBorders(UserListSelectedIndex, usersData);
                CreateGraph(graphBorders[0], graphBorders[1], 2, graphBorders[2], graphBorders[3], 5000, Points(UserListSelectedIndex, usersData));
            }

            int[] GraphBorders(int userIndex, List<OutputUserDataItem> usersData)
            {
                int daysQuantity = usersData[userIndex].UserDayDataList.Count;

                int firstDay = usersData[userIndex].UserDayDataList[0].Day;

                int lastDay;
                if (daysQuantity == 1)
                {
                    lastDay = firstDay + 1;
                }
                else
                {
                    lastDay = usersData[userIndex].UserDayDataList[daysQuantity - 1].Day;
                }

                int minSteps = usersData[userIndex].MinSteps;
                int maxSteps = usersData[userIndex].MaxSteps;

                return new int[4] { firstDay, lastDay, minSteps, maxSteps };
            }

            List<IntegerPoint> Points(int userIndex, List<OutputUserDataItem> usersData)
            {
                List<IntegerPoint> points = new List<IntegerPoint>();

                int inListDaysQuantity = usersData[userIndex].UserDayDataList.Count;

                int firstDay = usersData[userIndex].UserDayDataList[0].Day;

                if (inListDaysQuantity == 1)
                {
                    int steps = usersData[userIndex].UserDayDataList[0].Steps;
                    points.Add(new IntegerPoint(firstDay, steps));
                    points.Add(new IntegerPoint(firstDay + 1, 0));
                }
                else
                {
                    int lastDay = usersData[userIndex].UserDayDataList[inListDaysQuantity - 1].Day;

                    int inListCounter = 0;
                    int currentDay = firstDay;

                    while (currentDay <= lastDay && inListCounter < inListDaysQuantity)
                    {
                        if (currentDay == usersData[userIndex].UserDayDataList[inListCounter].Day)
                        {
                            int steps = usersData[userIndex].UserDayDataList[inListCounter].Steps;

                            points.Add(new IntegerPoint(currentDay, steps));
                            inListCounter++;
                        }
                        else
                        {
                            points.Add(new IntegerPoint(currentDay, 0));
                        }

                        currentDay++;
                    }
                }

                return points;
            }
        }

        private void CreateGraph(int leftX, int rightX, int stepX, int bottomY, int topY, int stepY, List<IntegerPoint> points)
        {
            GraphDrawingImage graph = new GraphDrawingImage(
                leftX, rightX, stepX, 0, topY, stepY,
                points,
                Brushes.Black, Brushes.LightGray, Brushes.LightSalmon,
                3);

            CreateColumnValuesGrid();
            CreateRowValuesGrid();

            GraphGrid.Children.Add(graph.GridLines);
            GraphGrid.Children.Add(graph.ValueLines);
            GraphGrid.Children.Add(graph.CoordinateLines);
            GraphGrid.Children.Add(graph.MaxValuePoint);
            GraphGrid.Children.Add(graph.MinValuePoint);

            void CreateColumnValuesGrid()
            {
                TextBlock textBlock;

                int count = 0;

                for (int value = graph.TopY; value >= graph.BottomY; value -= graph.StepY)
                {
                    ColumnValuesGrid.RowDefinitions.Add(new RowDefinition());

                    textBlock = new TextBlock();
                    textBlock.Text = value.ToString();
                    Grid.SetRow(textBlock, count);
                    ColumnValuesGrid.Children.Add(textBlock);
                    count++;
                }
            }

            void CreateRowValuesGrid()
            {
                TextBlock textBlock;

                int count = 0;

                for (int value = graph.LeftX; value <= graph.RightX; value += graph.StepX)
                {
                    RowValuesGrid.ColumnDefinitions.Add(new ColumnDefinition());

                    textBlock = new TextBlock();
                    textBlock.Text = value.ToString();
                    Grid.SetColumn(textBlock, count);
                    textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                    RowValuesGrid.Children.Add(textBlock);
                    count++;
                }
            }
        }

        private void DeleteGraph()
        {
            GraphGrid.Children.Clear();
            ColumnValuesGrid.Children.Clear();
            ColumnValuesGrid.RowDefinitions.Clear();
            RowValuesGrid.Children.Clear();
            RowValuesGrid.ColumnDefinitions.Clear();
        }

        private void ExportDataButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserListSelectedIndex >= 0 && usersData != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "JSON files (*.json)|*.json";
                saveFileDialog.FileName = "Export.json";

                if (saveFileDialog.ShowDialog() == true)
                {
                    ExportCurrentUserData(UserListSelectedIndex, saveFileDialog.FileName);

                    if (viewModel.IsError)
                        HandleError();
                }
            }
        }

        private void ExportCurrentUserData(int userIndex, string filePath) => viewModel.SaveCurrentUserData(userIndex, filePath);

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

            DeleteGraph();

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

        public class ExtendedItem : OutputUserDataItem
        {
            public string BackgroundColor { get; set; }
        }
    }
}
