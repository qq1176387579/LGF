
namespace LGF
{
    /// <summary>
    /// 非继承MonoBehaver单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonBase<T> where T : SingletonBase<T>, new()
    {
        private static T instance = default(T);
        protected static T _Instance => Instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                    instance.OnNew(); //周期回调
                }
                return instance;
            }
        }


        protected SingletonBase() { }

        public static T GetSingleton()
        {
            return Instance;
        }

        public virtual void Init()
        {



        }

        protected virtual void OnNew()
        {

        }

        public static bool CheckInstance()
        {
            return instance != null;
        }

    }

}



