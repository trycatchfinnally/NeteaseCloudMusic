using NeteaseCloudMusic.Services.NetWork;
using NeteaseCloudMusic.Wpf.Extensions;
using NeteaseCloudMusic.Wpf.Model;
using Newtonsoft.Json;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    public class AnchorRadioViewModel : BindableBase
    {
        private readonly INetWorkServices _netWorkServices;
        private readonly IRegionManager _navigationService;
        private bool _dataHasInit;
        public AnchorRadioViewModel(INetWorkServices netWorkServices, IRegionManager navigationService)
        {
            this._netWorkServices = netWorkServices;
            this._navigationService = navigationService;

            //for (int i = 0; i < 20; i++)
            //{
            //    AnchorRadioCatalogues.Add(new AnchorRadioCatalogueModel { CatalogueTitle = i.ToString() });
            //    WonderfulRadioRecommends.Add(new RadioModel { Title = "sahsdhadsh", Description = i.ToString() });
            //    RadioList.Add(new RadioModel { Title = "yiyidgsdg" });
            //    HotRadioList.Add(new RadioModel { AnchorName = "hot hot hot ", SubCount = i * 45486 });

            //}
            InitData();
        }
        private async void InitData()
        {
            if (_dataHasInit)
            {
                return;
            }
            var task1 = _netWorkServices.GetAsync("FindMusic", "GetAnchorRadioCategories");
            var task2= _netWorkServices.GetAsync("FindMusic", "GetRecommendProgram");
            await Task.WhenAll(task1,task2);
            await
               Task.WhenAll( AnchorRadioCatalogues.AddRangeAsync(JsonConvert.DeserializeObject<Global.Model.AnchorRadioCatalogue[]>(task1.Result)),
               WonderfulRadioRecommends.AddRangeAsync(JsonConvert.DeserializeObject<Global.Model.Program[]>(task2.Result)));
        }
        /// <summary>
        /// 电台目录的集合数据
        /// </summary>
        public ObservableCollection<Global.Model.AnchorRadioCatalogue> AnchorRadioCatalogues { get; } = new ObservableCollection<Global.Model.AnchorRadioCatalogue>();
        /// <summary>
        /// 精彩节目推荐的集合数据
        /// </summary>
        public ObservableCollection<Global.Model.Program> WonderfulRadioRecommends { get; } = new ObservableCollection<Global.Model.Program>();  


        /// <summary>
        /// 订阅电台对应的集合
        /// </summary>
        public ObservableCollection<RadioModel> RadioList { get; } = new ObservableCollection<RadioModel>();


        /// <summary>
        /// 热门电台的集合
        /// </summary>
        public ObservableCollection<RadioModel> HotRadioList { get; } = new ObservableCollection<RadioModel>();


    }
}
