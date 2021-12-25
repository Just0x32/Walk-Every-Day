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

namespace Walk_Every_Day
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new ViewModel();
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

                viewModel.GetData();

                if (viewModel.IsError)
                {
                    HandleError();
                }
                else
                {
                    //  Create graph and list
                }

                MessageBox.Show(viewModel.ShowInputAllDaysData());                          // Debug
                MessageBox.Show(viewModel.ShowOutputAllUsersData());                          // Debug
            }
        }

        private void ExportDataButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
