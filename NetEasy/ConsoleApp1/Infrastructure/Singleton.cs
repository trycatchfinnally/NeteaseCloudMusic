using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Core.Infrastructure
{
 public    class Singleton
    {
        static readonly IDictionary<Type, object> allSingletons;
        public static IDictionary<Type, object> AllSingletons => allSingletons;
        static Singleton()
        {
            allSingletons = new Dictionary<Type, object>();

        }

    }
    public class Singleton<T> : Singleton
    {
        static T instance;

        public static T Instance { get { return instance; } set { instance = value; AllSingletons[typeof(T)] = value; } }
    }

}
