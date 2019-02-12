using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Wpf.Extensions
{
    internal static class ObservableCollectionExtensions
    {
        /// <summary>
        /// 每次插入的数量
        /// </summary>
        private const int CountPerTime = 30;
        /// <summary>
        /// 每一次插入后的间隔
        /// </summary>
        private const int AwaitMillSeconds = 100;
        /// <summary>
        /// 批量插入对象到集合中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="addItems"></param>
        /// <param name="callBack">添加完成后需要执行的回调函数</param>
        public static async  Task  AddRangeAsync<T>(this ObservableCollection<T> source,IEnumerable<T> addItems,Action<ObservableCollection<T>> callBack=null )
        {
            if (source ==null )
            {
                throw new ArgumentNullException(nameof(source));
            }
            source.Clear();
            if (addItems!=null )
            {
                if (!(addItems is System.Collections.IList || addItems is Array))
                    addItems = addItems.ToArray();
                int pageCount = (int)Math.Ceiling(addItems.Count() / (double)CountPerTime);
                for (int i = 0; i < pageCount; i++)
                {
                    source.AddRange(addItems .Skip(i * CountPerTime).Take(CountPerTime));
                    await Task.Delay(AwaitMillSeconds);
                }
            }
            callBack?.Invoke(source);
        }
    }
}
