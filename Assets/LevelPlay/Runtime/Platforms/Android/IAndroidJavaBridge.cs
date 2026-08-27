#if UNITY_ANDROID
namespace Unity.Services.LevelPlay
{
    /// <summary>
    /// Internal interface for abstracting AndroidJavaObject bridge calls.
    /// </summary>
    internal interface IAndroidNativeBridge
    {
        void Call(string methodName, params object[] args);
        T Call<T>(string methodName, params object[] args);
    }

    /// <summary>
    /// Production implementation that wraps AndroidJavaObject.
    /// </summary>
    internal class AndroidNativeBridge : IAndroidNativeBridge
    {
        private readonly UnityEngine.AndroidJavaObject m_JavaObject;

        public AndroidNativeBridge(UnityEngine.AndroidJavaObject javaObject)
        {
            m_JavaObject = javaObject;
        }

        public void Call(string methodName, params object[] args)
        {
            m_JavaObject.Call(methodName, args);
        }

        public T Call<T>(string methodName, params object[] args)
        {
            return m_JavaObject.Call<T>(methodName, args);
        }
    }
}
#endif
