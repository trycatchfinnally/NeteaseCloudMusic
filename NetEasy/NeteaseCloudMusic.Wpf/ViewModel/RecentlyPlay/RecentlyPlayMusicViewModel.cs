using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using NeteaseCloudMusic.Wpf.Model;
using System.Windows.Input;
using Prism.Commands;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
 public    class RecentlyPlayMusicViewModel: BindableBase
 {
     public RecentlyPlayMusicViewModel()
     {
         for (int i = 0; i < 235; i++)
         {
                // if ('0' + i != 'Z' && '0' + i != 'z')
             RecentlyPlayMusicCollection.Add(new Model.LocalMusicModel
             {
                 AlbumName = (char)('中' + i) + "album" + i,
                 ArtistName = (char)('狗' + i) + "Artis" + i,
                 Title = (char)('牛' + i) + "title",
                 IsLike = i % 3 == 0,
                 Duration = TimeSpan.FromMilliseconds(i * 5452),
               //  MusicQuality=(Global.Enums.MusicQualityLevel)(i%3)
             });
            }

         ClearCommand = new DelegateCommand(ClearCommandImpl);
     }

        private void ClearCommandImpl()
        {
            RecentlyPlayMusicCollection.Clear();
        }

        public ObservableCollection<MusicModel> RecentlyPlayMusicCollection { get; } =
         new ObservableCollection<MusicModel>();
     public ICommand ClearCommand { get; }
 }
}
