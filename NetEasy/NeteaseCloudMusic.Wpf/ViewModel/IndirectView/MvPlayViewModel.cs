using NeteaseCloudMusic.Global.Model;
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
using NeteaseCloudMusic.Wpf.Extensions;
using NeteaseCloudMusic.Wpf.Properties;

namespace NeteaseCloudMusic.Wpf.ViewModel.IndirectView
{
    public class MvPlayViewModel : IndirectViewModelBase
    {
        private readonly INetWorkServices _netWorkServices;
        private readonly IRegionManager _navigationService;
        private Global.Model.Mv _innerMv = new Global.Model.Mv();
        private Global.Model.CommentCollection _innerComment;
        private uint _currentPage;

        /// <summary>
        /// 当页面的内容刷新完毕后执行的事件
        /// </summary>
        public event EventHandler RefreshCompleated;
        public MvPlayViewModel(INetWorkServices netWorkServices, IRegionManager navigationService)
        {
            _netWorkServices = netWorkServices;
            this._navigationService = navigationService;
            NavigatedBackCommand = new DelegateCommand(() => this._navigationService.Regions[Settings.Default.RegionName].NavigationService.Journal.GoBack());
            SimiMvSelectedCommand = new DelegateCommand<IEnumerable>(SimiMvSelectedCommandExecute);
            UserCommand = new DelegateCommand<long?>(UserCommandExecute);
        }


        #region 导航接口的实现部分


        protected override async void SetById(long id)
        {

            var innerMvdataResult = await this._netWorkServices.GetAsync<Mv>("Common", "GetMvById", new { id });
            if (!innerMvdataResult.Successed)
            {
                //todo 网络连接四百
                return;
            }
            _innerMv = innerMvdataResult.Data;
            var task1 = _netWorkServices.GetAsync<CommentCollection>("Common", "GetCommentById", new { commentThreadId = _innerMv.CommendThreadId });
            var task2 = _netWorkServices.GetAsync<Mv[]>("Common", "GetSimiMv", new { id });
            await Task.WhenAll(task1, task2);
            if (task1.Result.Successed && task2.Result.Successed)
            {
                _innerComment = task1.Result.Data;
                await Task.WhenAll(
                  NewComments.AddRangeAsync(_innerComment.Comments),
                  HotComments.AddRangeAsync(_innerComment.HotComments),
                  SimiMvs.AddRangeAsync(task2.Result.Data));
                this._currentPage = 1;
                RaiseAllPropertyChanged();
                RefreshCompleated?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                //todo 网络连接四百
            }
        }
        #endregion
        private async void OnCurrentPageChanged()
        {
            var temp = await _netWorkServices.GetAsync<CommentCollection>("Common", "GetCommentById", new
            {
                commentThreadId = _innerMv.CommendThreadId,
                offset = (CurrentPage - 1) * Settings.Default.LimitPerPage,
                limit = Settings.Default.LimitPerPage
            });
            if (temp.Successed)
            {
                _innerComment = temp.Data;
                await Task.WhenAll(
                    NewComments.AddRangeAsync(_innerComment.Comments),
                    HotComments.AddRangeAsync(_innerComment.HotComments)
                    );
            }
        }
        private void SimiMvSelectedCommandExecute(IEnumerable items)
        {
            var temp = items.Cast<Mv>().ToArray();
            if (temp.Length == 0) return;
            var parmater = new NavigationParameters();
            parmater.Add(NavigationIdParmmeterName, temp[0].Id);
            this._navigationService.RequestNavigate(Settings.Default.RegionName, nameof(View.IndirectView.MvPlayView), parmater);
        }
        private void UserCommandExecute(long? id)
        {
            if (id.HasValue)
            {
                var parmater = new NavigationParameters();
                parmater.Add(NavigationIdParmmeterName, id.Value);
                this._navigationService.RequestNavigate(Settings.Default.RegionName, nameof(View.IndirectView.UserZoneView), parmater);
            }
        }
        /// <summary>
        /// 代表MV的名称
        /// </summary>
        public string Name => _innerMv.Name;

        public string ArtistName => _innerMv.ArtistName;
        /// <summary>
        /// 播放数
        /// </summary>
        public double WatchCount => _innerMv.PlayCount;
        /// <summary>
        /// Mv的地址
        /// </summary>
        public List<KeyValuePair<int, string>> MvUrls => _innerMv.Url?.Where(x => !string.IsNullOrEmpty(x.Value)).ToList();
        /// <summary>
        /// 收藏数量
        /// </summary>
        public int CollectionCount => _innerMv.SubCount;

        public uint CommentCount
        {
            get
            {
                var temp = this._innerComment?.Total;
                if (temp >= 0)
                {
                    return System.Convert.ToUInt32(temp.Value);
                }

                return 0;
            }
        }
        public uint CurrentPage
        {
            get { return this._currentPage; }
            set
            {
                SetProperty(ref this._currentPage, value);
                OnCurrentPageChanged();
            }
        }



        public TimeSpan Duration => _innerMv.Duration;
        public ICommand NavigatedBackCommand { get; }

        public ObservableCollection<Comment> NewComments { get; } = new ObservableCollection<Comment>();
        public ObservableCollection<Comment> HotComments { get; } = new ObservableCollection<Comment>();
        public bool MoreComment => _innerComment?.More ?? false;
        /// <summary>
        /// MV的发布时间
        /// </summary>
        public DateTime MvPublishTime => _innerMv.PublishTime;
        /// <summary>
        /// mv的描述
        /// </summary>
        public string MvDescation => _innerMv.Desc;
        public ObservableCollection<Mv> SimiMvs { get; } = new ObservableCollection<Mv>();
        public ICommand SimiMvSelectedCommand { get; }
        /// <summary>
        /// 点击用户名执行的命令
        /// </summary>
        public ICommand UserCommand { get; }

    }
}
