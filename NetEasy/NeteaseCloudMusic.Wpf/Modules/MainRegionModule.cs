using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Ioc;
using Prism.Regions;
using NeteaseCloudMusic.Wpf.View;
using NeteaseCloudMusic.Wpf.View.IndirectView;

namespace NeteaseCloudMusic.Wpf.Modules
{
    public class MainRegionModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
           

            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(Context.RegionName, typeof(FindMusicView));
            regionManager.RegisterViewWithRegion(Context.RegionName, typeof(SearchView));
            regionManager.RegisterViewWithRegion(Context.RegionName, typeof(MvView));
            regionManager.RegisterViewWithRegion(Context.RegionName, typeof(FriendView));
            regionManager.RegisterViewWithRegion(Context.RegionName, typeof(LocalMusicView));
            regionManager.RegisterViewWithRegion(Context.RegionName, typeof(DownloadManagerView));
            regionManager.RegisterViewWithRegion(Context.RegionName, typeof(RecentlyPlayView));
           regionManager.RegisterViewWithRegion(Context.RegionName, typeof(PlayListDetailView));
            regionManager.RegisterViewWithRegion(Context.RegionName, typeof(MvPlayView));
            regionManager.RegisterViewWithRegion(Context.RegionName, typeof(UserZoneView));
            regionManager.RegisterViewWithRegion(Context.RegionName, typeof(PlayPanelView));
            regionManager.RegisterViewWithRegion(Context.RegionName, typeof(AlbumView));
            regionManager.RegisterViewWithRegion(Context.RegionName, typeof(ArtistDetailView));
            regionManager.RegisterViewWithRegion(Context.RegionName, typeof(EveryDayMusicRecommendView));
            regionManager.RegisterViewWithRegion(Context.RegionName, typeof(BillBoardView));
            regionManager.RegisterViewWithRegion(Context.RegionName, typeof(TopArtistsBillBoardView));

            //  regionManager.Regions[Context.RegionName].Add(containerProvider.Resolve<PlayListDetailView>());
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
            //throw new NotImplementedException();
        }
    }
}
