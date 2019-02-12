using System;
using Prism.Mvvm;
using NeteaseCloudMusic.Global.Enums;
using NeteaseCloudMusic.Global.Model;
using System.Linq;

namespace NeteaseCloudMusic.Wpf.Model
{
    /// <summary>
    /// 代表音乐的绑定Model
    /// </summary>
    public class MusicModel : NotifyPropertyChangedModelBase<Music>
    {


        public MusicModel():base()
        {

        }
        public MusicModel(Music music):base(music)
        {

        }

        /// <summary>
        /// 对应的图片
        /// </summary>
        public string Picture
        {
            get
            {
                return innerModel.PicUrl;
            }

            set {
                
                innerModel.PicUrl = value;
                
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 歌曲的标题
        /// </summary>
        public string Title
        {
            get
            {
                return innerModel.Name;
            }

            set { innerModel.Name = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 音乐的质量级别
        /// </summary>
        public MusicQualityLevel MusicQuality
        {
            get
            {
                
                return innerModel.MusicQuality;
            }

           
        }
        /// <summary>
        /// 是否应该显示sq hq等
        /// </summary>
        public bool IsHigh => MusicQuality != MusicQualityLevel.Low;
        /// <summary>
        /// 表示歌手名
        /// </summary>
        public string ArtistName
        {
            get
            {
               // return string.Join(",", innerModel.Artists.Select(x => x.Name));
               return innerModel.ArtistName;
            }

            set//后续注释
            {
              
               // innerModel.ArtistName = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 歌曲是否有MV
        /// </summary>
        public bool HasMv
        {
            get
            {
                return innerModel.HasMv;
            }

            
        }
        /// <summary>
        /// mv id
        /// </summary>
        public long MvId
        {
            get
            {
                return innerModel.MvId;
            }

             
        }
        /// <summary>
        /// 歌曲的持续时间
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                return  ( innerModel.Duration);
            }

            set {
                
                    
                innerModel.Duration =value ;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 是否喜欢歌曲
        /// </summary>
        public bool IsLike
        {
            get
            {
                return innerModel.IsLike;
            }

            set { innerModel.IsLike = value;RaisePropertyChanged(); }
        }
        /// <summary>
        /// 专辑名
        /// </summary>
        public string AlbumName
        {
            get
            {
                return innerModel.AlbumName;
            }

            set { //innerModel.AlbumName=value ;
                RaisePropertyChanged();
            }
        }
        public long Id => innerModel.Id;
        public string Url
        {
            get { return innerModel.Url; }
            set { innerModel.Url = value; }
        }

    }
}
