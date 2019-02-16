using NeteaseCloudMusic.Services.NetWork;
using NeteaseCloudMusic.Wpf.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace NeteaseCloudMusic.Wpf.ViewModel.IndirectView
{
    public class EveryDayMusicRecommendViewModel : Prism.Mvvm.BindableBase
    {
        private readonly INetWorkServices _netWorkServices;
        private bool _isSelectedModel;
        //是否已经加载好数据，如果已经加载，重新导航到当前页面时不执行联网操作
        //private bool _dataHasComplete = false;
        public EveryDayMusicRecommendViewModel(INetWorkServices netWorkServices)
        {
            this._netWorkServices = netWorkServices;

        }
        private async void InitData()
        {
            var json =await _netWorkServices.GetAsync("FindMusic", "GetEveryDayMusicRecommend");
            var temp = JsonConvert.DeserializeObject<List<Global.Model.Music>>(json);
            await RecommendMusics.AddRangeAsync(temp);
        }
        /// <summary>
        /// 每日推荐对应的日
        /// </summary>
        public int Day => DateTime.Now.Day;
        public bool IsSelectedModel
        {
            get { return _isSelectedModel; }
            set { SetProperty(ref _isSelectedModel, value); }
        }
        public SelectionMode SelectionMode => IsSelectedModel ? SelectionMode.Multiple : SelectionMode.Single;
        /// <summary>
        /// 推荐歌曲如果有MV，对应mv的命令
        /// </summary>
        public ICommand RecomendMvCommand { get; }
        public ICommand PlayAllCommand { get;   }
        public ObservableCollection<Global.Model.Music> RecommendMusics { get; } = new ObservableCollection<Global.Model.Music>();
    }
}
