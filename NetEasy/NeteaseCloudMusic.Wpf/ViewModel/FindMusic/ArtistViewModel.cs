using NeteaseCloudMusic.Global.Model;
using NeteaseCloudMusic.Services.NetWork;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    /// <summary>
    /// 绑定到发现音乐的歌手tab页面的viewmodel
    /// </summary>
    public class ArtistViewModel : BindableBase
    {
        private readonly INetWorkServices _netWorkServices;
        private readonly IRegionManager _navigationService;
        private bool _morePage = true;
        private int _offset = 0;
        private CancellationTokenSource _offsetCancellationToken;
        private string _selectedLanguage = "99";
        private string _selectedGender = "99";
        private string _selectedPinYin = "99";
        public ArtistViewModel(INetWorkServices netWorkServices, IRegionManager navigationService)
        {
            this._netWorkServices = netWorkServices;
            this._navigationService = navigationService;
            NextPageCommand = new DelegateCommand(NextPageCommandExecute);
            ArtistUserCommand = new DelegateCommand<long?>(ArtistUserCommandExecute);
            SelectedArtistCommand = new DelegateCommand<Global.Model.Artist>(SelectedArtistCommandExecute);
            SearchFilterChangeCommand = new DelegateCommand(SearchFilterChangeCommandExecute);
        }

        private async void SearchFilterChangeCommandExecute()
        {
            Artists.Clear();
            this._morePage = true;
            await RefreshAsync();
        }

        private void SelectedArtistCommandExecute(Artist obj)
        {
            if (obj?.Id > 0)
            {
                var parmater = new NavigationParameters();
                parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName, obj.Id);
                this._navigationService.RequestNavigate(Context.RegionName, nameof(View.IndirectView.ArtistDetailView), parmater);
            }
        }

        private void ArtistUserCommandExecute(long? id)
        {
            if (id.HasValue)
            {
                var parmater = new NavigationParameters();
                parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName, id.Value);
                this._navigationService.RequestNavigate(Context.RegionName, nameof(View.IndirectView.UserZoneView), parmater);
            }
        }

        private async void NextPageCommandExecute()
        {
            await RefreshAsync();
        }
        private async Task RefreshAsync()
        {
            this._offsetCancellationToken?.Cancel();
            var newCancel = new CancellationTokenSource();
            this._offsetCancellationToken = newCancel;
            try
            {
                if (this._morePage)
                {
                    var catString = SelectedLanguage + SelectedGender;
                    if (catString.StartsWith("99"))
                        catString = "10" + SelectedGender;
                    if (catString.EndsWith("99"))
                        catString = catString.Substring(0, 2) + "01";
                    var netWorkDataResult = await this._netWorkServices.GetAsync<KeyValuePair<bool, Global.Model.Artist[]>>("FindMusic", "ArtistsList",
                        new
                        {
                            limit = Context.LimitPerPage,
                            offset = this._offset,
                            cat = int.Parse(catString),
                            initial = SelectedPinYin == "99" ? 0 : Convert.ToChar(SelectedPinYin)
                        });
                    if (netWorkDataResult.Successed)
                    {
                        var temp = netWorkDataResult.Data;
                        this._offset++; this._morePage = temp.Key;
                        Artists.AddRange(temp.Value);
                    }
                    else
                    {
                        //todo 引发提示信息
                    }

                }
            }
            catch (OperationCanceledException)
            {


            }
            if (this._offsetCancellationToken == newCancel)
                this._offsetCancellationToken = null;
        }
        /// <summary>
        /// 根据筛选结果查询得到的集合
        /// </summary>
        public ObservableCollection<Global.Model.Artist> Artists { get; } = new ObservableCollection<Global.Model.Artist>();

        public ICommand NextPageCommand { get; }
        public ICommand ArtistUserCommand { get; }
        public ICommand SelectedArtistCommand { get; }
        /// <summary>
        /// 当条件变化的时候执行的命令
        /// </summary>
        public ICommand SearchFilterChangeCommand { get; }
        public string SelectedLanguage
        {
            get { return this._selectedLanguage; }
            set { SetProperty(ref this._selectedLanguage, value); }
        }
        public string SelectedGender
        {
            get { return this._selectedGender; }
            set { SetProperty(ref this._selectedGender, value); }
        }
        public string SelectedPinYin
        {
            get { return this._selectedPinYin; }
            set { SetProperty(ref this._selectedPinYin, value); }
        }
    }
}
