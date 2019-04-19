using NeteaseCloudMusic.Global.Model;
using NeteaseCloudMusic.Services.NetWork;
using NeteaseCloudMusic.Wpf.Extensions;
using NeteaseCloudMusic.Wpf.View;
using NeteaseCloudMusic.Wpf.View.IndirectView;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using NeteaseCloudMusic.Services.Identity;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    public class PersonalityRecommendViewModel : NavigationViewModelBase
    {
        private readonly INetWorkServices _netWorkServices;
        private readonly IRegionManager _navigationService;
        private readonly IdentityService _dentityService;

        public PersonalityRecommendViewModel(INetWorkServices netWorkServices, 
            IRegionManager navigationService,IdentityService dentityService)
        {

            this._netWorkServices = netWorkServices;
            this._navigationService = navigationService;
            this._dentityService = dentityService;
            InitData();
            MoreCommand = new DelegateCommand<string>(MoreCommandExecute);
            RecommendPlayListCommend = new DelegateCommand<PlayList>(RecommendPlayListCommendExecute);
            PrivateContentCommand = new DelegateCommand<PictureListBoxItem>(PrivateContentCommandExecute);
            NewMusicCommand = new DelegateCommand<Global.Model.Music>(NewMusicCommandExecute);
            NewMusicMvCommand = new DelegateCommand<long?>(NewMusicMvCommandExecute);
            NewMvCommand = new DelegateCommand<Global.Model.Mv>(NewMvCommandExecute);
            EveryDayMusicRecommendCommand = new DelegateCommand(EveryDayMusicRecommendCommandExecute);
            BillBoardCommand = new DelegateCommand(BillBoardCommandExecute);
            PersonalFmCommand = new DelegateCommand(PersonalFmCommandExecute);
        }

        private async void PersonalFmCommandExecute()
        {
            if (this._dentityService.CurrentUser != null)
            {
                var netWorkDataResult= await this._netWorkServices.GetAsync< Music[]>("FindMusic", "GetPersonalFm");
                if (netWorkDataResult.Successed)
                {
                    var temp = netWorkDataResult.Data;
                    await Context.CurrentPlayMusics.AddRangeAsync(temp, x => Context.PlayCommand.Execute(x[0]));
                    var parmater = new NavigationParameters();
                    parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName, temp[0].Id);
                    this._navigationService.RequestNavigate(Context.RegionName, nameof(PlayPanelView), parmater); 
                }
                else
                {
                    //todo 提示网络
                }
            }
            else
            {
                InteractionRequests.LoginInteractionRequest.Raise(new Prism.Interactivity.InteractionRequest.Confirmation
                {
                    Title = "登陆"
                }, x =>
                {
                    if (x.Confirmed) PersonalFmCommandExecute();
                });
            }
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
            if (this._dentityService.CurrentUser!= null)
                this._navigationService.RequestNavigate(Context.RegionName, nameof(View.IndirectView.EveryDayMusicRecommendView));
            else
                InteractionRequests.LoginInteractionRequest.Raise(new Prism.Interactivity.InteractionRequest.Confirmation
                {
                    Title = "登陆"
                }, x =>
                {
                    if (x.Confirmed) EveryDayMusicRecommendCommandExecute();
                });

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
                    this._navigationService.RequestNavigate(Context.RegionName, nameof(FindMusicView) + "?TabIndex=1");
                    break;
                case "2":
                    //独家放送实现
                    break;
                case "3":
                    this._navigationService.RequestNavigate(Context.RegionName, nameof(FindMusicView) + "?TabIndex=3");

                    break;
                case "4":
                    //推荐MV实现
                    break;
                case "5":
                    //主播电台实现
                    break;
            }

        }
        private void RecommendPlayListCommendExecute(PlayList playListModel)
        {
            var parmater = new NavigationParameters();
            parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName, playListModel.Id);
            this._navigationService.RequestNavigate(Context.RegionName, nameof(PlayListDetailView), parmater);
            // this._navigationService.RequestNavigate(Context.RegionName, nameof(PlayListDetailView)+ $"?PlayListId={playListModel.Id}");
        }
        private void PrivateContentCommandExecute(PictureListBoxItem pictureListBoxItem)
        {
            if (!string.IsNullOrEmpty(pictureListBoxItem?.RealUrl))
                System.Diagnostics.Process.Start(pictureListBoxItem.RealUrl);
        }
        /// <summary>
        /// 加载的时候获取数据
        /// </summary>
        private async void InitData()
        {
            var netWorkDataResult = await this._netWorkServices.GetAsync< PersonalityRecommend>("FindMusic", "PersonalityRecommend", new { limit = 10 });
            if (!netWorkDataResult.Successed)
            {
                //todo 网络提示信息
                return;
            }
            var temp = netWorkDataResult.Data;
            if (temp?.RecommendList != null)
            {
                // RecommendList.Clear();
                //RecommendList.AddRange(temp.RecommendList.Select(x => new PlayListModel(x)));
                await RecommendList.AddRangeAsync(temp.RecommendList);
            }
            if (temp?.AnchorRadioList != null)
            {
                //AnchorRadioList.Clear();
                //AnchorRadioList.AddRange(temp.AnchorRadioList.Select(x => new RadioModel(x)));
                await AnchorRadioList.AddRangeAsync(temp.AnchorRadioList);
            }
            if (temp?.RecommendMvList != null)
            {

                await RecommendMvList.AddRangeAsync(temp.RecommendMvList);
            }
            if (temp?.NewMusicList != null)
            {
                //NewMusicList.Clear();
                //NewMusicList.AddRange(temp.NewMusicList);
                await NewMusicList.AddRangeAsync(temp.NewMusicList);
            }
            if (temp?.PrivateContentList != null)
            {
                await ExclusiveDeliveryList.AddRangeAsync(temp.PrivateContentList);
            }
            if (temp?.BannerList != null)
            {
                //BannerList.Clear();
                //BannerList.AddRange(temp.BannerList.Select(x => new BannerModel { Image = x.Pic }));
                await BannerList.AddRangeAsync(temp.BannerList);

            }
        }



        public ObservableCollection<PlayList> RecommendList
        {
            get;
        } = new ObservableCollection<PlayList>();

        public int Date { get; } = DateTime.Now.Day;

        public ObservableCollection<PictureListBoxItem> ExclusiveDeliveryList
        {
            get;
        } = new ObservableCollection<PictureListBoxItem>();

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
        public ObservableCollection<Radio> AnchorRadioList
        {
            get;
        } = new ObservableCollection<Radio>();
        /// <summary>
        /// 轮播图列表
        /// </summary>
        public ObservableCollection<Banner> BannerList
        {
            get;
        } = new ObservableCollection<Banner>();
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
        /// <summary>
        /// 私人fm对应的命令
        /// </summary>
        public ICommand PersonalFmCommand { get; }
        public ICommand BillBoardCommand { get; }
    }
}
