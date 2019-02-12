using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeteaseCloudMusic.Global.Model;
using Prism.Mvvm;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
   public  class RecentlyPlayProgramViewModel: BindableBase
   {
       public RecentlyPlayProgramViewModel()
       {
           for (int i = 0; i < 333; i++)
           {
               RecentlyPlayProgramCollection.Add(new Global.Model.Program { });
           }
       }
       public ObservableCollection<Program> RecentlyPlayProgramCollection { get; } =
           new ObservableCollection<Program>();
   }
}
