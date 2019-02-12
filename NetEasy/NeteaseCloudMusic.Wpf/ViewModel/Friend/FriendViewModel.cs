using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeteaseCloudMusic.Wpf.View;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
  public   class FriendViewModel:BindableBase
  {
      public FriendViewModel()
      {
          Items.AddRange(CreateItems(new[] { "找","栋","sadg","发给","sdaghfdh","□"}));
      }
      public ObservableCollection<TestItems> Items { get; } = new ObservableCollection<TestItems>();

      private IEnumerable<TestItems> CreateItems(IEnumerable<string> data)
      {
          foreach (var item in data)
          {
              yield return new ViewModel.TestItems {Text = item};
          }
      }
  }

    public class TestItems : IComparable
    {
        public string  Text { get; set; }
        public override string ToString()
        {
            return Text;
        }
        public int Compare(object x, object y)
        {
            var temp1 = (char)(SortHelper.SortByPinYinConverter.Convert(x, null, null, null)??' ');
            var temp2 = (char)(SortHelper.SortByPinYinConverter.Convert(y, null, null, null)??' ');
            return temp1.CompareTo(temp2);
        }
       

        public int CompareTo(object obj)
        {
            return Compare(this, obj);
        }
    }
}
