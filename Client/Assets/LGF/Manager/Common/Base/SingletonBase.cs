
namespace LGF
{
    /// <summary>
    /// 非继承MonoBehaver单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonBase<T> where T : SingletonBase<T>, new()
    {
        private static T _instance = default(T);
        protected static T _Instance => _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                    _instance.OnNew(); //周期回调
                }
                return _instance;
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
            return _instance != null;
        }

        public static void SingletonClear()
        {
            _instance = null;
        }
    }

}



