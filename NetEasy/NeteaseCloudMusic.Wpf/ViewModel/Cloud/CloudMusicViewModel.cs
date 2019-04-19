using NeteaseCloudMusic.Global.Model;
using NeteaseCloudMusic.Services.NetWork;
using NeteaseCloudMusic.Wpf.Extensions;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace NeteaseCloudMusic.Wpf.ViewModel.Cloud
{
    public class CloudMusicViewModel : NavigationViewModelBase
    {
        private Global.Model.CloudDisk _innerModel = new Global.Model.CloudDisk();
        private bool _dataIsInit = false;
        private readonly INetWorkServices _netWorkServices;
        private readonly IRegionManager _navigationService;
        public CloudMusicViewModel(INetWorkServices netWorkServvices, IRegionManager navigationService)
        {
            this._netWorkServices = netWorkServvices;
            this._navigationService = navigationService;
            PlayAllCloudMusicCommand = new DelegateCommand(PlayAllCloudMusicCommandExecute);
            AlbumCommand = new DelegateCommand<long?>(AlbumCommandExecute);
            ArtistCommand = new DelegateCommand<long?>(ArtistCommandExecute);
            SelectedCommand = new DelegateCommand<IEnumerable>(SelectedCommandExecute);
        }

        private void SelectedCommandExecute(IEnumerable obj)
        {
            if (obj == null) return;
            var tmp = obj.Cast<CloudMusic>().ToArray();
            if (tmp.Length == 1)
            {
                Context.PlayCommand.Execute(new Music
                {
                    Id = tmp[0].Id,
                    Name = tmp[0].Name,
                    Duration = tmp[0].SimpleMusic?.Duration ?? TimeSpan.FromSeconds(120)
                });
            }
        }

        private void ArtistCommandExecute(long? obj)
        {
            if (obj > 0)
            {
                var parmater = new NavigationParameters();
                parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName, obj.Value);
                this._navigationService.RequestNavigate(Context.RegionName, nameof(View.IndirectView.ArtistDetailView), parmater);
            }
        }

        private void AlbumCommandExecute(long? obj)
        {
            if (obj > 0)
            {
                var parmater = new NavigationParameters();
                parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName, obj.Value);
                this._navigationService.RequestNavigate(Context.RegionName, nameof(View.IndirectView.AlbumView), parmater);
            }
        }

        private async void PlayAllCloudMusicCommandExecute()
        {
            if (CloudMusics.Count > 0)
            {
                await Context.CurrentPlayMusics.AddRangeAsync(CloudMusics.Select(x => new Music
                {
                    Id = x.Id,
                    Name = x.Name,
                    Duration = x.SimpleMusic?.Duration ?? TimeSpan.Zero,

                }), x => Context.PlayCommand.Execute(x[0]));
            }
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (!this._dataIsInit)
            {
                //var json = await this._netWorkServices.GetAsync("User", "UserCloud", new { limit = 200, offset = 0 });
                //this._innerModel = JsonConvert.DeserializeAnonymousType(json, this._innerModel);
                var networkResult = await this._netWorkServices.GetAnonymousTypeAsync("User", "UserCloud", new { limit = 200, offset = 0 }, this._innerModel);
                if (networkResult.Successed)
                {
                    this._innerModel = networkResult.Data;
                    await CloudMusics.AddRangeAsync(this._innerModel.CloudMusics);
                    RaisePropertyChanged(nameof(Size));
                    RaisePropertyChanged(nameof(MaxSize));
                    this._dataIsInit = true;
                }
                else
                {
                   //显示网络连接失败的提示信息
                }

            }
            base.OnNavigatedTo(navigationContext);
        }

        public FileSize Size => this._innerModel?.Size ?? FileSize.EmptyFileSize;
        public FileSize MaxSize => this._innerModel?.MaxSize ?? FileSize.EmptyFileSize;
        /// <summary>
        /// 表示云盘音乐
        /// </summary>
        public ObservableCollection<CloudMusic> CloudMusics { get; } = new ObservableCollection<CloudMusic>();

        /// <summary>
        /// 播放全部对应的命令
        /// </summary>
        public ICommand PlayAllCloudMusicCommand { get; }
        /// <summary>
        /// 专辑对应的命令
        /// </summary>
        public ICommand AlbumCommand { get; }
        /// <summary>
        /// 歌手对应的命令
        /// </summary>
        public ICommand ArtistCommand { get; }
        /// <summary>
        /// 选择某项的命令
        /// </summary>
        public ICommand SelectedCommand { get; }
    }
}
