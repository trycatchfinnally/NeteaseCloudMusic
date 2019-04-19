using NeteaseCloudMusic.Services.NetWork;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NeteaseCloudMusic.Wpf.ViewModel.IndirectView
{
    public class UserZoneViewModel : IndirectViewModelBase
    {
        private readonly INetWorkServices _netWorkServices;
        private readonly IRegionManager _navigationService;
        private Global.Model.User _innerUser = new Global.Model.User();
        //public event EventHandler RefreshCompleated;

        public UserZoneViewModel(INetWorkServices netWorkServices, IRegionManager navigationService)
        {
            _netWorkServices = netWorkServices;
            this._navigationService = navigationService;
            UserPlayListCommand = new DelegateCommand<Global.Model.PlayList>(UserPlayListCommandExecute);
        }

        private void UserPlayListCommandExecute(Global.Model.PlayList model)
        {

            if (model != null)
            {
                var parmater = new NavigationParameters();
                parmater.Add(NavigationIdParmmeterName, model.Id);
                this._navigationService.RequestNavigate(Context.RegionName, nameof(View.IndirectView.PlayListDetailView), parmater);
            }

        }
        #region 接口实现
        



         

        protected override async void SetById(long id)
        {
            var detailTask = _netWorkServices.GetAsync<Global.Model.User>("User", "GetUserById", new { id });
            var playListTask = _netWorkServices.GetAsync<Global.Model.PlayList[]>("User", "GetUserPlayList", new { id });
            await Task.WhenAll(detailTask, playListTask);
            if (detailTask.Result.Successed&&playListTask.Result.Successed)
            {
                _innerUser = detailTask.Result.Data;
                var playList = playListTask.Result.Data;
                this.UserCreatedPlayLists.Clear();
                UserCreatedPlayLists.AddRange(playList.Where(x => x.CreateUser?.UserId == id));
                this.UserCollectionPlayLists.Clear();
                UserCollectionPlayLists.AddRange(playList.Where(x => x.CreateUser?.UserId != id));
                RaiseAllPropertyChanged(); 
            }
            else
            {
                //todo 网络连接失败
            }
        }
        #endregion
        /// <summary>
        /// 用户头像
        /// </summary>
        public string UserImage => _innerUser.UserImage;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName => _innerUser.UserName;
        public string Place => $"{_innerUser.Province} {_innerUser.City}";
        /// <summary>
        /// 关注数量
        /// </summary>
        public int Follows => _innerUser.Follows;
        /// <summary>
        /// 粉丝数量
        /// </summary>
        public int Followeds => _innerUser.Followeds;
        public string DetailDescription => _innerUser.DetailDescription;
        public int EventCount => _innerUser.EventCount;
        /// <summary>
        /// 用户创建的歌单
        /// </summary>
        public ObservableCollection<Global.Model.PlayList> UserCreatedPlayLists { get; } = new ObservableCollection<Global.Model.PlayList>();
        /// <summary>
        /// 用户收藏的歌单
        /// </summary>
        public ObservableCollection<Global.Model.PlayList> UserCollectionPlayLists { get; } = new ObservableCollection<Global.Model.PlayList>();
        /// <summary>
        /// 点击用户收藏的歌单执行的命令
        /// </summary>
        public ICommand UserPlayListCommand { get; }

    }
}
