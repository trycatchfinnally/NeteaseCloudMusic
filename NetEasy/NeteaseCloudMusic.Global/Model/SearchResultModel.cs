using System;
using System.Collections.Generic;

namespace NeteaseCloudMusic.Global.Model
{
    public class SearchResultModel
    {
         
        /// <summary>
        /// 搜索返回的音乐，包括总的数量和当前页的结果
        /// </summary>
        public KeyValuePair<int, Music[]> Musics { get; set; }
        /// <summary>
        /// 搜索返回的歌手，包括总的数量和当前页的结果
        /// </summary>
        public KeyValuePair<int, Artist[]> Artists { get; set; }
        /// <summary>
        /// 搜索返回的专辑，包括总的数量和当前页的结果
        /// </summary>
        public KeyValuePair<int, Album[]> Albums { get; set; }
        /// <summary>
        /// 搜索返回的歌单，包括总的数量和当前页的结果
        /// </summary>
        public KeyValuePair<int, PlayList[]> PlayLists { get; set; }
        /// <summary>
        /// 搜索返回的用户，包括总的数量和当前页的结果
        /// </summary>
        public KeyValuePair<int, User[]> Users { get; set; }
        /// <summary>
        /// 搜索返回的mv，包括总的数量和当前页的结果
        /// </summary>
        public KeyValuePair<int, Mv[]> Mvs { get; set; }
        public KeyValuePair<int, Radio[]> Radios { get; set; }

    }
}
