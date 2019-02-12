using NeteaseCloudMusic.Global.Model;
using NeteaseCloudMusic.Services.NetWork;
using NeteaseCloudMusic.Wpf.Extensions;
using NeteaseCloudMusic.Wpf.Model;
using NeteaseCloudMusic.Wpf.View.IndirectView;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    public class PersonalityRecommendViewModel :BindableBase
    {
        private readonly INetWorkServices _netWorkServices;
        private readonly IRegionManager _navigationService;

        public PersonalityRecommendViewModel(INetWorkServices netWorkServices, IRegionManager navigationService)
        {

            _netWorkServices = netWorkServices;
            this._navigationService = navigationService;
            InitData();
            MoreCommand = new DelegateCommand<string>(MoreCommandExecute);
            RecommendPlayListCommend = new DelegateCommand<PlayListModel>(RecommendPlayListCommendExecute);
            PrivateContentCommand = new DelegateCommand<PictureListBoxItemModel>(PrivateContentCommandExecute);
            NewMusicCommand = new DelegateCommand<Global.Model.Music>(NewMusicCommandExecute);
            NewMusicMvCommand = new DelegateCommand<long?>(NewMusicMvCommandExecute);
            NewMvCommand = new DelegateCommand<Global.Model.Mv>(NewMvCommandExecute);
            EveryDayMusicRecommendCommand = new DelegateCommand(EveryDayMusicRecommendCommandExecute);
            BillBoardCommand = new DelegateCommand(BillBoardCommandExecute);
        }

        private void BillBoardCommandExecute()
        {
            this._navigationService.RequestNavigate(Context.RegionName, nameof(View.IndirectView.BillBoardView));
        }

        private void NewMvCommandExecute(Mv mv)
        {
            if ((mv?.Id).HasValue)
            {
                NewMusicMvCommandExecute(mv.Id);
            }
        }
        private void EveryDayMusicRecommendCommandExecute()
        {
            this._navigationService.RequestNavigate(Context.RegionName, nameof(View.IndirectView.EveryDayMusicRecommendView));

        }
        private void NewMusicMvCommandExecute(long? mvId)
        {
            if (mvId.HasValue)
            {
                var parmater = new NavigationParameters();
                parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName, mvId.Value);
                this._navigationService.RequestNavigate(Context.RegionName, nameof(MvPlayView), parmater);
            }
        }

        private void NewMusicCommandExecute(Global.Model.Music music)
        {
            if (music != null)
            {
                Context.PlayCommand.Execute(music);
            }
        }

        private void MoreCommandExecute(string arg)
        {
            switch (arg)
            {
                case "1":
                    //推荐歌单实现
                    break;
                case "2":
                    //独家放送实现
                    break;
                case "3":
                    //最新音乐实现
                    break;
                case "4":
                    //推荐MV实现
                    break;
                case "5":
                    //主播电台实现
                    break;
            }

        }
        private void RecommendPlayListCommendExecute(PlayListModel playListModel)
        {
            var parmater = new NavigationParameters();
            parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName, playListModel.Id);
            this._navigationService.RequestNavigate(Context.RegionName, nameof(PlayListDetailView), parmater);
            // this._navigationService.RequestNavigate(Context.RegionName, nameof(PlayListDetailView)+ $"?PlayListId={playListModel.Id}");
        }
        private void PrivateContentCommandExecute(PictureListBoxItemModel pictureListBoxItem)
        {
            if (!string.IsNullOrEmpty(pictureListBoxItem?.RealUrl))
                System.Diagnostics.Process.Start(pictureListBoxItem.RealUrl);
        }
        /// <summary>
        /// 加载的时候获取数据
        /// </summary>
        private async void InitData()
        {
            var json = await _netWorkServices.GetAsync("FindMusic", "PersonalityRecommend", new { limit = 10 });
            var temp = JsonConvert.DeserializeObject<Global.Model.PersonalityRecommend>(json);
            if (temp?.RecommendList != null)
            {
                RecommendList.Clear();
                RecommendList.AddRange(temp.RecommendList.Select(x => new PlayListModel(x)));
            }
            if (temp?.AnchorRadioList != null)
            {
                AnchorRadioList.Clear();
                AnchorRadioList.AddRange(temp.AnchorRadioList.Select(x => new RadioModel(x)));
            }
            if (temp?.RecommendMvList != null)
            {

                await RecommendMvList.AddRangeAsync(temp.RecommendMvList );
            }
            if (temp?.NewMusicList != null)
            {
                NewMusicList.Clear();
                NewMusicList.AddRange(temp.NewMusicList);
            }
            if (temp?.PrivateContentList != null)
            {
                ExclusiveDeliveryList.Clear();
                ExclusiveDeliveryList.AddRange(temp.PrivateContentList.Select(x => new PictureListBoxItemModel(x)
                 ));
            }
            if (temp?.BannerList != null)
            {
                BannerList.Clear();
                BannerList.AddRange(temp.BannerList.Select(x => new BannerModel { Image = x.Pic }));
            }
        }

        

        public ObservableCollection<PlayListModel> RecommendList
        {
            get;
        } = new ObservableCollection<PlayListModel>();

        public int Date { get; } = DateTime.Now.Day;

        public ObservableCollection<PictureListBoxItemModel> ExclusiveDeliveryList
        {
            get;
        } = new ObservableCollection<PictureListBoxItemModel>();

        public ObservableCollection<Global.Model.Music> NewMusicList
        {
            get;
        } = new ObservableCollection<Global.Model.Music>();

        public ObservableCollection<Global.Model.Mv> RecommendMvList
        {
            get;
        } = new ObservableCollection<Global.Model.Mv>();
        /// <summary>
        /// 主播电台的列表
        /// </summary>
        public ObservableCollection<RadioModel> AnchorRadioList
        {
            get;
        } = new ObservableCollection<RadioModel>();
        /// <summary>
        /// 轮播图列表
        /// </summary>
        public ObservableCollection<BannerModel> BannerList
        {
            get;
        } = new ObservableCollection<BannerModel>();
        /// <summary>
        /// 更多对应的命令
        /// </summary>
        public ICommand MoreCommand { get; }
        /// <summary>
        /// 选择推荐歌单的时候执行的命令
        /// </summary>
        public ICommand RecommendPlayListCommend { get; }
        /// <summary>
        /// 独家放送的点击命令
        /// </summary>
        public ICommand PrivateContentCommand { get; }
        /// <summary>
        /// 最新音乐的点击命令
        /// </summary>
        public ICommand NewMusicCommand { get; }
        /// <summary>
        /// 最新音乐mv的命令
        /// </summary>
        public ICommand NewMusicMvCommand { get; }
        /// <summary>
        /// 最新MV的命令
        /// </summary>
        public ICommand NewMvCommand { get; }
        public ICommand EveryDayMusicRecommendCommand { get; }
        public ICommand BillBoardCommand { get; }
    }
}
