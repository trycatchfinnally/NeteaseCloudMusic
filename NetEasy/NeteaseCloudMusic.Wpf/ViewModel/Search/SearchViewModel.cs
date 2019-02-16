using NeteaseCloudMusic.Global.Model;
using NeteaseCloudMusic.Services.NetWork;
using NeteaseCloudMusic.Wpf.Extensions;
using NeteaseCloudMusic.Wpf.View.IndirectView;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    public class SearchViewModel : NavigationViewModelBase
    {
        private readonly INetWorkServices _netWorkServices;
        private readonly IRegionManager _navigationService;
        #region 分页相关的字段
        /// <summary>
        /// 对应每一个标签页和其页面的偏移
        /// </summary>
        private readonly Dictionary<string, int> _tabKeyAndOffset = new Dictionary<string, int>
        {
            { "1",0},
            { "2",0},
            { "3",0},
            { "4",0},
            { "5",0},
            { "6",0},
            { "7",0},

        };
        /// <summary>
        /// 对应每一个标签页和其页面的搜索类型
        /// </summary>
        private readonly Dictionary<string, Global.Enums.SearchResultType> _tabKeyAndType = new Dictionary<string, Global.Enums.SearchResultType>
        {
            { "1",Global.Enums.SearchResultType.Music},
            { "2",Global.Enums.SearchResultType.Artist},
            { "3",Global.Enums.SearchResultType.Album},
            {"4" ,Global.Enums.SearchResultType.MV},
            {"5",Global.Enums.SearchResultType.PlayList },
            {"6",Global.Enums.SearchResultType.Radio },
            {"7",Global.Enums.SearchResultType.User }
        };
        /// <summary>
        /// 每一页和对应的总数
        /// </summary>
        private readonly Dictionary<string, int> _tabKeyAndTotal = new Dictionary<string, int>
        {
             { "1",0},
            { "2",0},
            { "3",0},
            { "4",0},
            { "5",0},
            { "6",0},
            { "7",0},
        };
        #endregion
        private string _keyWord;
        private CancellationTokenSource _searchResultCancel;
        private CancellationTokenSource _nextPageCancel;
        private System.Windows.Visibility _searchResultPanelVisiable = System.Windows.Visibility.Collapsed;
        public SearchViewModel(INetWorkServices netWorkServvices, IRegionManager navigationService)
        {
            this._netWorkServices = netWorkServvices;
            this._navigationService = navigationService;

            // SearchRecomend.AddRange(new[] { "陈奕迅", "明年今日", "起风了", "陈奕迅", "明年今日", "起风了", "陈奕迅", "明年今日", "起风了" });
            SearchHistory.AddRange(new[] { "习近平", "江泽民", "哈哈", "上台拿衣服", "三个戴表", "苟利国家生死以" });
            DeleteHistoryCommand = new DelegateCommand<string>(DeleteHistoryCommandImpl);
            SelectedSuggestCommand = new DelegateCommand<dynamic>(SelectedSuggestCommandImpl);
            SearchCommand = new DelegateCommand(SearchCommandExecute);
            MusicMvCommand = new DelegateCommand<long?>(MusicMvCommandExecute);
            MusicSelectCommand = new DelegateCommand<Music>(MusicSelectCommandExecute);
            AlbumSelectCommand = new DelegateCommand<Album>(AlbumSelectCommandExecute);
            ArtistSelectCommand = new DelegateCommand<Artist>(ArtistSelectCommandExecute);
            MvSelectCommand = new DelegateCommand<Mv>(MvSelectCommandExecute);
            PlayListSelectCommand = new DelegateCommand<PlayList>(PlayListSelectCommandExecute);
            UserSelectCommand = new DelegateCommand<User>(UserSelectCommandExecute);
            SearchResultNextPageCommand = new DelegateCommand<string>(SearchResultNextPageCommandExecute);
            Init();


        }

        private async void SearchResultNextPageCommandExecute(string tabKey)
        {
            if (this._tabKeyAndOffset.ContainsKey(tabKey))
            {
                this._nextPageCancel?.Cancel();
                var newCancel = new CancellationTokenSource();
                this._nextPageCancel = newCancel;
                if (Context.LimitPerPage*(_tabKeyAndOffset[tabKey]+1)<_tabKeyAndTotal[tabKey])
                {
                    try
                    {
                        var json = await this._netWorkServices.GetAsync("Search", "Search",
                            new
                            {
                                KeyWord,
                                SearchResultType = this._tabKeyAndType[tabKey],
                                limit = Context.LimitPerPage,
                                offset = ++this._tabKeyAndOffset[tabKey]
                            });
                        var temp = JsonConvert.DeserializeObject<SearchResultModel>(json);
                        switch (_tabKeyAndType[tabKey])
                        {
                            case Global.Enums.SearchResultType.Music:
                                this.MusicResults.AddRange(temp.Musics.Value);
                                break;
                            case Global.Enums.SearchResultType.Artist:
                                ArtistResults.AddRange(temp.Artists.Value);
                                break;
                            case Global.Enums.SearchResultType.Album:
                                AlbumResults.AddRange(temp.Albums.Value);
                                break;
                            case Global.Enums.SearchResultType.PlayList:
                                PlayListResults.AddRange(temp.PlayLists.Value);
                                break;
                            case Global.Enums.SearchResultType.User:
                                UserResults.AddRange(temp.Users.Value);
                                break;
                            case Global.Enums.SearchResultType.MV:
                                MvResults.AddRange(temp.Mvs.Value);
                                break;
                            case Global.Enums.SearchResultType.Radio:

                                break;
                            default:
                                break;
                        }
                    }
                    catch (OperationCanceledException)
                    {


                    }
                }
               
                if (newCancel == this._nextPageCancel)
                    this._nextPageCancel = null;
            }
        }

        #region 导航到其他页面
        private void UserSelectCommandExecute(User obj)
        {
            if (obj != null && obj.UserId != 0)
            {
                var parmater = new NavigationParameters();
                parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName, obj.UserId);
                this._navigationService.RequestNavigate(Context.RegionName, nameof(View.IndirectView.UserZoneView), parmater);
            }
        }

        private void PlayListSelectCommandExecute(PlayList obj)
        {
            if (obj != null && obj.Id != 0)
            {
                var parmater = new NavigationParameters();
                parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName, obj.Id);
                this._navigationService.RequestNavigate(Context.RegionName, nameof(PlayListDetailView), parmater);
            }
        }

        private void MvSelectCommandExecute(Mv obj)
        {
            if (obj?.Id > 0)
            {
                var parmater = new NavigationParameters();
                parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName, obj.Id);
                this._navigationService.RequestNavigate(Context.RegionName, nameof(MvPlayView), parmater);
            }
        }

        private void ArtistSelectCommandExecute(Artist obj)
        {
            if (obj != null && obj.Id != 0)
            {
                var parmater = new NavigationParameters();
                parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName, obj.Id);
                this._navigationService.RequestNavigate(Context.RegionName, nameof(View.IndirectView.ArtistDetailView), parmater);
            }
        }

        private void AlbumSelectCommandExecute(Album obj)
        {
            if (obj != null && obj.Id != 0)
            {
                var parmater = new NavigationParameters();
                parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName, obj.Id);
                this._navigationService.RequestNavigate(Context.RegionName, nameof(View.IndirectView.AlbumView), parmater);
            }
        }

        private void MusicSelectCommandExecute(Music obj)
        {
            if (obj != null)
            {
                Context.PlayCommand.Execute(obj);
            }
        }

        private void MusicMvCommandExecute(long? mvId)
        {
            if (mvId.GetValueOrDefault() != 0)
            {
                var parmater = new NavigationParameters();
                parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName, mvId.Value);
                this._navigationService.RequestNavigate(Context.RegionName, nameof(MvPlayView), parmater);
            }
        }
        #endregion

        private async void SearchCommandExecute()
        {
            SearchResultPanelVisiable = System.Windows.Visibility.Visible;
            await SearchAllResultAsync();
            //var json = await this._netWorkServices.GetAsync("Search", "Search", new { keyWord = KeyWord, Global.Enums.SearchResultType.All, limit = Context.LimitPerPage });
            //var temp = JsonConvert.DeserializeObject<SearchResultModel>(json);
            //await Task.WhenAll(MusicResults.AddRangeAsync(temp.Musics.Value),
            //    ArtistResults.AddRangeAsync(temp.Artists.Value),
            //    AlbumResults.AddRangeAsync(temp.Albums.Value),
            //    MvResults.AddRangeAsync(temp.Mvs.Value),
            //    UserResults.AddRangeAsync(temp.Users.Value),
            //    PlayListResults.AddRangeAsync(temp.PlayLists.Value));
        }

        private async void Init()
        {
            var json = await this._netWorkServices.GetAsync("search", "hot");
            var xdoc = this._netWorkServices.Json2Xml(json);
            var tmp = xdoc.Element("Root").Element("result").Elements("hots");
            foreach (var item in tmp)
            {
                SearchRecomend.Add(item.Element("first").Value);
            }
        }
        private void DeleteHistoryCommandImpl(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                SearchHistory.Remove(key);
            }
        }
        /// <summary>
        /// 动态一时爽，重构火葬场
        /// </summary>
        /// <param name="itemKeys"></param>
        private void SelectedSuggestCommandImpl(dynamic itemKeys)
        {
            this._keyWord = itemKeys[0];
            RaisePropertyChanged(nameof(KeyWord));
            SearchCommand.Execute(null);
        }

        private async void KeyWordCompleteImpl()
        {
            //if (!string.IsNullOrEmpty(KeyWord)&& (key==Key.Enter||!key.HasValue))
            //{
            //    Console.WriteLine("开始搜索......"+KeyWord);
            //}

            SearchResult.Clear();
            if (!string.IsNullOrWhiteSpace(KeyWord))
            {
                this._searchResultCancel?.Cancel();
                var newCancel = new CancellationTokenSource();
                this._searchResultCancel = newCancel;
                try
                {
                    if (SearchResultPanelVisiable == System.Windows.Visibility.Collapsed)
                    {
                        var result = await SearhResultAsync(this._searchResultCancel.Token);
                        SearchResult.AddRange(result);
                    }
                    //从其他页面导航回来时显示popup
                    else
                    {

                        await SearchAllResultAsync();
                    }
                    RaisePropertyChanged(nameof(KeyWord));

                }
                catch (OperationCanceledException)
                {

                }
                catch (AggregateException e)
                {
                    Console.WriteLine(e.InnerException);
                }
                if (this._searchResultCancel == newCancel)
                    this._searchResultCancel = null;
            }
        }
        private async Task SearchAllResultAsync()
        {
            var json = await this._netWorkServices.GetAsync("Search", "Search", new { keyWord = KeyWord, Global.Enums.SearchResultType.All, limit = Context.LimitPerPage });
            var temp = JsonConvert.DeserializeObject<SearchResultModel>(json);

            this._tabKeyAndTotal["1"] = temp.Musics.Key;
            this._tabKeyAndTotal["2"] = temp.Artists.Key;
            this._tabKeyAndTotal["3"] = temp.Albums.Key;
            this._tabKeyAndTotal["4"] = temp.Mvs.Key;
            this._tabKeyAndTotal["5"] = temp.PlayLists.Key;
            this._tabKeyAndTotal["6"] = temp.Radios.Key;
            this._tabKeyAndTotal["7"] = temp.Users.Key;

            await Task.WhenAll(MusicResults.AddRangeAsync(temp.Musics.Value),
                ArtistResults.AddRangeAsync(temp.Artists.Value),
                AlbumResults.AddRangeAsync(temp.Albums.Value),
                MvResults.AddRangeAsync(temp.Mvs.Value),
                UserResults.AddRangeAsync(temp.Users.Value),
                PlayListResults.AddRangeAsync(temp.PlayLists.Value));
            this._tabKeyAndOffset["1"] = 0;
            this._tabKeyAndOffset["2"] =0;
            this._tabKeyAndOffset["3"] =0;
            this._tabKeyAndOffset["4"] =0;
            this._tabKeyAndOffset["5"] =0;
            this._tabKeyAndOffset["6"] =0;
            this._tabKeyAndOffset["7"] = 0;
           
        }
        /// <summary>
        /// 搜索关键字
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async Task<List<string>> SearhResultAsync(CancellationToken ct)
        {

            //var json =  ( await _netWorkServices.GetAsync("search", "suggest", new { KeyWord }, ct));
            return await this._netWorkServices.GetAsync("search", "suggest", new { KeyWord }, ct).ContinueWith<List<string>>(tempJson =>
           {
               string regex = $"{KeyWord}";
               if (tempJson.IsCanceled || string.IsNullOrWhiteSpace(regex)) return new List<string>();
               var json = tempJson.Result;
               var tempIndex = Regex.Matches(json, regex).Cast<Match>().Select(x => x.Index);
               var result = new List<string>();
               foreach (var item in tempIndex)
               {

                   var startIndex = item;
                   var endIndex = item + KeyWord.Length;
                   while (startIndex > 0)
                   {
                       if (json[startIndex] != '"')
                           startIndex--;
                       else break;
                   }
                   while (endIndex < json.Length)
                   {
                       if (json[endIndex] != '"')
                           endIndex++;
                       else break;
                   }
                   result.Add(json.Substring(startIndex + 1, endIndex - startIndex - 1));
               }

               ct.ThrowIfCancellationRequested();
               return result.Distinct().ToList();
           }, ct);
        }

        /// <summary>
        /// 搜索历史对应的列表
        /// </summary>
        public ObservableCollection<string> SearchHistory { get; } = new ObservableCollection<string>();

        /// <summary>
        /// 搜索建议
        /// </summary>
        public ObservableCollection<string> SearchRecomend { get; } = new ObservableCollection<string>();

        /// <summary>
        /// 删除历史记录的命令
        /// </summary>
        public ICommand DeleteHistoryCommand { get; }
        /// <summary>
        /// 选中建议的搜索命令
        /// </summary>
        public ICommand SelectedSuggestCommand { get; }
        /// <summary>
        /// 关键字文本框输入了enter或者点击了搜索button执行的命令
        /// </summary>
        public ICommand KeyWordComplete { get; }
        /// <summary>
        /// 点击文本框的搜索按钮
        /// </summary>
        public ICommand SearchCommand { get; }
        /// <summary>
        /// 输入的关键字
        /// </summary>
        public string KeyWord
        {
            get
            {
                return this._keyWord;
            }

            set
            {
                if (this._keyWord == value) return;
                SetProperty(ref this._keyWord, value);
                if (string.IsNullOrEmpty(value))
                    SearchResultPanelVisiable = System.Windows.Visibility.Collapsed;
                KeyWordCompleteImpl();

            }
        }
        public System.Windows.Visibility SearchResultPanelVisiable
        {
            get { return this._searchResultPanelVisiable; }
            set { SetProperty(ref this._searchResultPanelVisiable, value); }
        }
        /// <summary>
        /// 搜索出来的建议结果
        /// </summary>
        public ObservableCollection<string> SearchResult { get; } = new ObservableCollection<string>();
        /// <summary>
        /// 搜索出来的歌曲列表
        /// </summary>
        public ObservableCollection<Music> MusicResults { get; } = new ObservableCollection<Music>();
        /// <summary>
        /// 对应的歌手结果
        /// </summary>
        public ObservableCollection<Artist> ArtistResults { get; } = new ObservableCollection<Artist>();
        /// <summary>
        /// 搜索出来的专辑列表
        /// </summary>
        public ObservableCollection<Album> AlbumResults { get; } = new ObservableCollection<Album>();
        /// <summary>
        /// 搜索出来的MV列表
        /// </summary>
        public ObservableCollection<Mv> MvResults { get; } = new ObservableCollection<Mv>();
        /// <summary>
        /// 搜索出来的歌单列表
        public ObservableCollection<PlayList> PlayListResults { get; } = new ObservableCollection<PlayList>();
        public ObservableCollection<User> UserResults { get; } = new ObservableCollection<User>();
        /// <summary>
        /// 选择音乐后面的mv符号执行的命令
        /// </summary>
        public ICommand MusicMvCommand { get; }
        /// <summary>
        /// 选择音乐执行的命令
        /// </summary>
        public ICommand MusicSelectCommand { get; }
        /// <summary>
        /// 选择专辑对应的命令
        /// </summary>
        public ICommand AlbumSelectCommand { get; }
        /// <summary>
        /// 选择歌手执行的命令
        /// </summary>
        public ICommand ArtistSelectCommand { get; }
        /// <summary>
        /// 选择mv对应的命令
        /// </summary>
        public ICommand MvSelectCommand { get; }
        /// <summary>
        /// 歌单选择的命令
        /// </summary>
        public ICommand PlayListSelectCommand { get; }
        /// <summary>
        /// 用户选择的命令
        /// </summary>
        public ICommand UserSelectCommand { get; }
        /// <summary>
        /// 用于当滚动条滑落到底部的时候执行的命令，即加载下一页
        /// </summary>
        public ICommand SearchResultNextPageCommand { get; }
    }
}
