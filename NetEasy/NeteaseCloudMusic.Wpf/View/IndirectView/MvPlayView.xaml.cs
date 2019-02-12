using NeteaseCloudMusic.Wpf.ViewModel.IndirectView;
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

namespace NeteaseCloudMusic.Wpf.View.IndirectView
{
    /// <summary>
    /// MvPlayView.xaml 的交互逻辑
    /// </summary>
    public partial class MvPlayView
    {

        public MvPlayView(MvPlayViewModel viewModel)
        {
            this.DataContext = viewModel;
            viewModel.RefreshCompleated += ViewModel_RefreshCompleated;
            InitializeComponent();
        }

        private void ViewModel_RefreshCompleated(object sender, EventArgs e)
        {
            this.supportPiex.SelectedIndex = 0;
            this.playButton.IsChecked = true;
            PlayButton_Click(null, null);
            RefreshTime();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (playButton.IsChecked == true)
            {
                this.mvPlayer.Play();
                Context.PauseCommand.Execute(null);
            }
            else { this.mvPlayer.Pause(); }
        }
        /// <summary>
        /// 刷新事件
        /// </summary>
        private async void RefreshTime()
        {
            while (true)
            {
                if (mvPlayer.NaturalDuration.HasTimeSpan)
                {
                    this.txtDuration.Text = mvPlayer.Position.ToString(@"hh\:mm\:ss") + "/" + mvPlayer.NaturalDuration.TimeSpan
               .ToString(@"hh\:mm\:ss");
                }
                else this.txtDuration.Text = mvPlayer.Position.ToString(@"hh\:mm\:ss") + "/00:00:00";
                this.positionSlider.Value = mvPlayer.Position.TotalMilliseconds;
                await Task.Delay(100);
            }
        }



        private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var skipTime = e.NewValue - e.OldValue;
            if (TimeSpan.FromMilliseconds(Math.Abs(skipTime)) > TimeSpan.FromSeconds(2))
            {
                var temp = this.mvPlayer.Position;
                this.mvPlayer.Position = temp.Add(TimeSpan.FromMilliseconds(skipTime));
            }
        }

        private void SupportPiex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.mvPlayer.Stop();
            if (supportPiex.SelectedValue == null) return;
            this.mvPlayer.Source = new Uri(supportPiex.SelectedValue.ToString());
            if (playButton.IsChecked == true)
            {
                this.mvPlayer.Play();
                Context.PauseCommand.Execute(null);
            }
            this.tgVideoPiex.Content = ((KeyValuePair<int, string>)e.AddedItems[0]).Key;
        }

        private void ContentControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.mvPlayer.Stop();
        }
    }
}
