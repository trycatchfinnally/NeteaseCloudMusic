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
using NeteaseCloudMusic.Wpf.ViewModel.IndirectView;

namespace NeteaseCloudMusic.Wpf.View.IndirectView
{
    /// <summary>
    /// PlayPanelView.xaml 的交互逻辑
    /// </summary>
    public partial class PlayPanelView  
    {
        private readonly Services.AudioDecode.IAudioPlayableServices _audioPlayableServices;
        public PlayPanelView(ViewModel.IndirectView.PlayPanelViewModel viewModel,Services.AudioDecode.IAudioPlayableServices audioPlayableServices)
        {
            this.DataContext = viewModel;
            _audioPlayableServices = audioPlayableServices;
            InitializeComponent();
        }
        private PlayPanelViewModel ViewModel => this.DataContext as PlayPanelViewModel;
        private void ContentControl_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshLyric();
        }
        private async void RefreshLyric(  )
        {
            while (true )
            {
                if (_audioPlayableServices!=null &&(ViewModel?.Lryics?.Count).GetValueOrDefault()>0)
                {
                    var playTime = _audioPlayableServices.Position;
                    var lyrics = ViewModel.Lryics;
                  var minTime=  lyrics.Min(x => Math.Abs(x.Time.TotalMilliseconds - playTime.TotalMilliseconds));
                    if (minTime<=100)
                    {
                        var item = lyrics.First(x => Math.Abs(x.Time.TotalMilliseconds - playTime.TotalMilliseconds) == minTime);
                        this.lstLryics.SelectedItem = item;
                        lstLryics.ScrollIntoView(item);
                    }

                }
                await Task.Delay(100);
            }
        }

        private void LrcButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic item = (e.Source as Button).Content;
            _audioPlayableServices.Position = item.Time;
        }
    }
    
}
