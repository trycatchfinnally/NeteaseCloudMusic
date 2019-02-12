using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using System.Windows.Input;
using Prism.Commands;
using System.Threading;
using NeteaseCloudMusic.Services.NetWork;
using System.Text.RegularExpressions;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    public class SearchViewModel : BindableBase
    {
        private string _keyWord;
        private CancellationTokenSource _searchResultCancel;
        private readonly INetWorkServices _netWorkServices;
        public SearchViewModel(INetWorkServices netWorkServvices)
        {
            this._netWorkServices = netWorkServvices;
            // SearchRecomend.AddRange(new[] { "陈奕迅", "明年今日", "起风了", "陈奕迅", "明年今日", "起风了", "陈奕迅", "明年今日", "起风了" });
            SearchHistory.AddRange(new[] { "习近平", "江泽民", "哈哈", "上台拿衣服", "三个戴表", "苟利国家生死以" });
            DeleteHistoryCommand = new DelegateCommand<string>(DeleteHistoryCommandImpl);
            SelectedSuggestCommand = new DelegateCommand<dynamic>(SelectedSuggestCommandImpl);
            KeyWordComplete = new DelegateCommand(KeyWordCompleteImpl);
            Init();


        }
        private async void Init()
        {
            var json = await _netWorkServices.GetAsync("search", "hot");
            var xdoc = _netWorkServices.Json2Xml(json);
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
            KeyWord = itemKeys[0];
            KeyWordCompleteImpl();
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
                _searchResultCancel?.Cancel();
                var newCancel = new CancellationTokenSource();
                _searchResultCancel = newCancel;
                try
                {
                    var result = await SearhResultAsync(_searchResultCancel.Token);
                    SearchResult.AddRange(result);
                    //从其他页面导航回来时显示popup
                    RaisePropertyChanged(nameof(KeyWord));
                    Console.WriteLine(string.Join(";", result));
                }
                catch (OperationCanceledException)
                {

                }
                catch (AggregateException e)
                {
                    Console.WriteLine(e.InnerException);
                }
                if (_searchResultCancel == newCancel)
                    _searchResultCancel = null;
            }
        }
        /// <summary>
        /// 搜索关键字
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async Task<List<string>> SearhResultAsync(CancellationToken ct)
        {

            //var json =  ( await _netWorkServices.GetAsync("search", "suggest", new { KeyWord }, ct));
            return await _netWorkServices.GetAsync("search", "suggest", new { KeyWord }, ct).ContinueWith<List<string>>( tempJson =>
            {
                string regex = $"{KeyWord}";
                if (tempJson.IsCanceled||string.IsNullOrWhiteSpace(regex)) return new List<string>();
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
            // string regex = $"{KeyWord}";
            // var tempIndex = Regex.Matches(json, regex).Cast<Match>().Select(x => x.Index);
            // var result = new List<string>();
            // foreach (var item in tempIndex)
            // {

            //     var startIndex = item;
            //     var endIndex = item+KeyWord.Length;
            //     while (startIndex>0)
            //     {
            //         if (json[startIndex] != '"')
            //             startIndex--;
            //         else break;
            //     }
            //     while (endIndex< json.Length)
            //     {
            //         if (json[endIndex] != '"')
            //             endIndex++;
            //         else break;
            //     }
            //     result.Add(json.Substring(startIndex+1, endIndex-startIndex-1));
            // }
            //// var temp = Regex.Matches(tmpXml, regex).Cast<Match>().Select(x=>x.Value ).ToList();
            // ct.ThrowIfCancellationRequested();
            // return result;
            //return new List<string>(new[] { "陈奕迅", "明年今日", "起风了", "陈奕迅", "明年今日", "起风了", "陈奕迅", "明年今日", "起风了", KeyWord });
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
        /// 输入的关键字
        /// </summary>
        public string KeyWord
        {
            get
            {
                return _keyWord;
            }

            set
            {
                if (_keyWord == value) return;
                SetProperty(ref _keyWord, value);
                KeyWordCompleteImpl();
            }
        }

        /// <summary>
        /// 搜索出来的建议结果
        /// </summary>
        public ObservableCollection<string> SearchResult { get; } = new ObservableCollection<string>();

    }
}
