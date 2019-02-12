using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Core
{
    public static class CommonExtensions
    {
        /// <summary>
        /// 对序列中的每一个元素进行遍历操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="ac">需要执行的操作</param>
        public static void Each<T>(this IEnumerable<T> source, Action<T> ac)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (ac == null)
            {

                throw new ArgumentNullException(nameof(Action));
            }
            foreach (var item in source)
                ac.Invoke(item);

        }
        /// <summary>
        /// 遍历集合寻找是否存在满足特定条件的项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="fc">需要满足的条件</param>
        /// <returns></returns>
        public static bool HasValue<T>(this IEnumerable<T> source, Func<T, bool> fc)
        {
            foreach (var item in source)
            {
                if (fc(item))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 比较两个ienumerable的每一项否相等
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source1">需要进行比较的第一个集合</param>
        /// <param name="source2">需要进行比较的第二个集合</param>
        /// <param name="comparer">对象的相等比较。</param>
        /// <returns>比较结果</returns>
        public static bool ArraysEqual<T>(this IEnumerable<T> source1, IEnumerable<T> source2, IEqualityComparer<T> comparer)
        {
            if (ReferenceEquals(source1, source2))
                return true;
            if (source1 == null || source2 == null)
                return false;
            var arr1 = source1.ToArray();
            var arr2 = source2.ToArray();
            if (arr1.Length != arr2.Length)
                return false;
            for (int i = 0; i < arr1.Length; i++)
                if (!comparer.Equals(arr1[i], arr2[i]))
                    return false;
            return true;
        }
        /// <summary>
        /// 比较两个ienumerable的每一项否相等
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source1">需要进行比较的第一个集合</param>
        /// <param name="source2">需要进行比较的第二个集合</param>
        /// <returns>比较结果</returns>
      
        public static bool ArraysEqual<T>(this IEnumerable<T> source1, IEnumerable<T> source2) => source1.ArraysEqual(source2, EqualityComparer<T>.Default);
        /// <summary>
        /// 通过反射为对象设置属性
        /// </summary>
        /// <param name="instance">需要设置属性的对象</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="value">属性的值</param>
        public static void SetProperty(this object instance, string propertyName, object value)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException(nameof(propertyName));
            var pi = instance.GetType().GetProperty(propertyName);
            if (pi == null) throw new ArgumentException($"未找到属性名为{propertyName}的属性！");
            if (!pi.CanWrite) throw new ArgumentException($"该属性不能编辑：{propertyName}");
            if (!value.GetType().IsAssignableFrom(pi.PropertyType))
                throw new ArgumentException($"无法将{value}设置到对象上");
            pi.SetValue(instance, value);
        }

    }
}
