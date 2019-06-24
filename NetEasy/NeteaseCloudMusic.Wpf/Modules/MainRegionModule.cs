using NeteaseCloudMusic.Wpf.Properties;
using NeteaseCloudMusic.Wpf.View;
using NeteaseCloudMusic.Wpf.View.IndirectView;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace NeteaseCloudMusic.Wpf.Modules
{
    public class MainRegionModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {


            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(Settings.Default.RegionName, typeof(FindMusicView));
            regionManager.RegisterViewWithRegion(Settings.Default.RegionName, typeof(SearchView));
            regionManager.RegisterViewWithRegion(Settings.Default.RegionName, typeof(MvView));
            regionManager.RegisterViewWithRegion(Settings.Default.RegionName, typeof(FriendView));
            regionManager.RegisterViewWithRegion(Settings.Default.RegionName, typeof(LocalMusicView));
            regionManager.RegisterViewWithRegion(Settings.Default.RegionName, typeof(DownloadManagerView));
            regionManager.RegisterViewWithRegion(Settings.Default.RegionName, typeof(RecentlyPlayView));
            regionManager.RegisterViewWithRegion(Settings.Default.RegionName, typeof(PlayListDetailView));
            regionManager.RegisterViewWithRegion(Settings.Default.RegionName, typeof(MvPlayView));
            regionManager.RegisterViewWithRegion(Settings.Default.RegionName, typeof(UserZoneView));
            regionManager.RegisterViewWithRegion(Settings.Default.RegionName, typeof(PlayPanelView));
            regionManager.RegisterViewWithRegion(Settings.Default.RegionName, typeof(AlbumView));
            regionManager.RegisterViewWithRegion(Settings.Default.RegionName, typeof(ArtistDetailView));
            regionManager.RegisterViewWithRegion(Settings.Default.RegionName, typeof(EveryDayMusicRecommendView));
            regionManager.RegisterViewWithRegion(Settings.Default.RegionName, typeof(BillBoardView));
            regionManager.RegisterViewWithRegion(Settings.Default.RegionName, typeof(TopArtistsBillBoardView));
            regionManager.RegisterViewWithRegion(Settings.Default.RegionName, typeof(View.Cloud.CloudMusicView));

            //  regionManager.Regions[Settings.Default.RegionName].Add(containerProvider.Resolve<PlayListDetailView>());
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

            //throw new NotImplementedException();
        }
    }
}
