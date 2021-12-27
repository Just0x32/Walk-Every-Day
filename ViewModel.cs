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

        public bool IsError { get => model.IsError(); }

        public bool IsFileReadingError { get => model.IsFileReadingError; }

        public bool IsFileWritingError { get => model.IsFileWritingError; }

        public bool IsInputDataWrong { get=> model.IsInputDataWrong ; }

        public bool IsDayParsingError { get => model.IsDayParsingError; }

        public List<OutputUserDataItem> GetData() => model.GetData();

        public void SendFilePaths(string[] filePaths) => model.GetFilePaths(filePaths);

        public void SaveCurrentUserData(int userIndex, string filePath) => model.SaveCurrentUserData(userIndex, filePath);
    }
}
