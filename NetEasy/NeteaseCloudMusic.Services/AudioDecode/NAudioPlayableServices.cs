 
using NAudio.Wave;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Services.AudioDecode
{
    public class NAudioPlayableServices : IAudioPlayableServices
    {
        private readonly ILoggerFacade _logger;
        private WaveOutEvent outputDevice;
        private MediaFoundationReader mfr;
        private float _volumn=50;
        public NAudioPlayableServices(ILoggerFacade logger )
        {
            this._logger = logger;
        }
        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (e.Exception !=null )
            {
                //_logger.Exception ("播放出现错误！",e.Exception);
            }
            outputDevice?.Dispose();
            outputDevice = null;
            mfr?.Dispose();
            mfr = null;
        }
        public float Volumn
        {
            get
            {

                return _volumn;
            }
            set
            {
               
                 _volumn = value;
                if (outputDevice != null)
                    outputDevice.Volume = value / 100;
            }
        }
        public TimeSpan Position
        {
            get {
                if (mfr == null)
                    return TimeSpan.Zero;
               
                return mfr.CurrentTime;
            }
            set
            {
               
                if (mfr != null)
                    mfr.CurrentTime = value;
            }
        }
        public TimeSpan Length
        {
            get {

                if (mfr == null)
                    return TimeSpan.Zero;
                
                return mfr.TotalTime;
            }
        }
        public PlayState PlayState
        {
            get
            {
                if (outputDevice == null)
                    return PlayState.UnKnown;
                return (PlayState)Enum.Parse(typeof(PlayState), outputDevice.PlaybackState.ToString());
            }
        }
        public void Play(string url)
        {
           // _logger.Info($"准备播放的url为{url}");
            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent();
                outputDevice.PlaybackStopped += OnPlaybackStopped;
            }
            if (mfr == null)
            {
                mfr = new MediaFoundationReader(url);
                outputDevice.Init(mfr);
                outputDevice.Volume = Volumn / 100;
            
            }
            outputDevice.Play();
           // _logger.Info($"文件{url}开始播放");
        }
        public void Resume()
        {
            if (this.PlayState!=PlayState.Paused)
            {
                throw new ArgumentException("只能从暂停状态恢复播放");
            }
            outputDevice?.Play();
        }
        public void Stop() => outputDevice?.Stop();
        public void Pause()
        {
            outputDevice?.Pause(); 

        }

    }
}
