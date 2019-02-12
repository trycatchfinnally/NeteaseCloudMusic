using NeteaseCloudMusic.Global.Model;

namespace NeteaseCloudMusic.Wpf.Model
{
    /// <summary>
    /// 代表歌单的绑定model
    /// </summary>
    public class PlayListModel : NotifyPropertyChangedModelBase<PlayList>
    {
        public PlayListModel() : base()
        {

        }
        public PlayListModel(PlayList innermodel) : base(innermodel)
        {

        }

        /// <summary>
        /// 歌单的图片
        /// </summary>
        public string PlayListImage
        {
            get { return innerModel.PicUrl; }
            set
            {
                innerModel.PicUrl = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 表示歌曲的id
        /// </summary>
        public long Id => innerModel.Id;
         
        /// <summary>
        /// 表示歌单标题
        /// </summary>
        public string Title
        {
            get { return innerModel.Name; }
            set
            {
                innerModel.Name = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 表示歌单创建者名称
        /// </summary>
        public string CreatedUserName
        {
            get { return innerModel.CopyWriter; }
            set
            {
                innerModel.CopyWriter = value;
                RaisePropertyChanged();
            }
        }

        public int ListenerCount
        {
            get
            {
                return (int)innerModel.PlayCount;
            }

            set
            {
                innerModel.PlayCount = value;
                RaisePropertyChanged();
            }
        }

    }
}
