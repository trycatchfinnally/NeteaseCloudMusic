using NeteaseCloudMusic.Wpf.ViewModel.IndirectView;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace NeteaseCloudMusic.Wpf.View.IndirectView
{
    /// <summary>
    /// PlayPanelView.xaml 的交互逻辑
    /// </summary>
    public partial class PlayPanelView
    {
        private readonly Services.AudioDecode.IAudioPlayableServices _audioPlayableServices;
        private DoubleAnimation _diskRotate;
        private DoubleAnimation _diskControlRotate;
        public PlayPanelView(ViewModel.IndirectView.PlayPanelViewModel viewModel, Services.AudioDecode.IAudioPlayableServices audioPlayableServices)
        {
            DataContext = viewModel;
            this._audioPlayableServices = audioPlayableServices;
            InitializeComponent(); Loaded += PlayPanelView_Loaded;
        }



        private void PlayPanelView_Loaded(object sender, RoutedEventArgs e)
        {
            if (this._diskRotate == null)
            {
                this._diskRotate = new DoubleAnimation(0, 360, TimeSpan.FromSeconds(5)) { RepeatBehavior = RepeatBehavior.Forever };
                RotateTransform rotateTransform = new RotateTransform();

                this.grdImage.RenderTransform = rotateTransform;
                rotateTransform.BeginAnimation(RotateTransform.AngleProperty, this._diskRotate);
            }

        }

        private PlayPanelViewModel ViewModel => DataContext as PlayPanelViewModel;
        private void ContentControl_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshLyric();
        }

        /// <summary>
        /// 刷新歌词的同时可以获取到播放状态
        /// </summary>
        private async void RefreshLyric()
        {
            while (true)
            {
                if (this._audioPlayableServices != null && (ViewModel?.Lryics?.Count).GetValueOrDefault() > 0)
                {
                    var playTime = this._audioPlayableServices.Position;
                    var lyrics = ViewModel.Lryics;
                    var minTime = lyrics.Min(x => Math.Abs(x.Time.TotalMilliseconds - playTime.TotalMilliseconds));
                    if (minTime <= 100)
                    {
                        var item = lyrics.First(x => Math.Abs(x.Time.TotalMilliseconds - playTime.TotalMilliseconds) == minTime);
                        this.lstLryics.SelectedItem = item;
                        if (this.rootScroll.VerticalOffset <= this.lrcPanelPart.Height.Value)
                            this.lstLryics.ScrollIntoView(item);
                    }
                }
                SetDiskControl(this._audioPlayableServices.PlayState == Services.AudioDecode.PlayState.Playing);
                await Task.Delay(100);
            }
        }
        private void SetDiskControl(bool isPlaying)
        {
            if (this._diskControlRotate == null)
            {
                this._diskControlRotate = new DoubleAnimation(0, -40, TimeSpan.FromSeconds(0.5));
                this._diskControlRotate.FillBehavior = FillBehavior.HoldEnd;
                this._diskControlRotate.EasingFunction = new BackEase { Amplitude = 0.3, EasingMode = EasingMode.EaseOut };
            }

            if (isPlaying)
            {
                var rt = new RotateTransform(0);
                this.imgDiskControl.RenderTransform = rt;
                // _diskControlRotate.
            }
            else
            {
                var rt = new RotateTransform(-40);
                this.imgDiskControl.RenderTransform = rt;
                // rt.BeginAnimation(RotateTransform.AngleProperty, _diskControlRotate);
            }
        }
        private void LrcButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic item = (e.Source as Button).Content;
            this._audioPlayableServices.Position = item.Time;
        }

        private void BtnReplied_Click(object sender, RoutedEventArgs e)
        {
            this.txtInput.Text = string.Empty;
            var btn = e.Source as Button;
            string userName = btn?.Tag?.ToString();
            this.txtInput.Text = "@" + userName;
        }
    }

}
