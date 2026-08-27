#if LEVELPLAY_DEPENDENCIES_INSTALLED

using System;
using System.IO;
using Newtonsoft.Json;

namespace Unity.Services.LevelPlay.Editor
{
    sealed class JsonSettingsStorage<T> : IStorage<T> where T : class, new()
    {
        readonly string m_FilePath;
        readonly ILevelPlayLogger m_Logger;

        public JsonSettingsStorage(string filePath, ILevelPlayLogger logger)
        {
            m_FilePath = filePath;
            m_Logger = logger;
        }

        public T Load()
        {
            if (!File.Exists(m_FilePath)) return new T();

            try
            {
                var json = File.ReadAllText(m_FilePath);
                var data = JsonConvert.DeserializeObject<T>(json);

                return data ?? throw new Exception("The settings file could not be read.");
            }
            catch (Exception e)
            {
                m_Logger.LogError("Error while reading settings file: " + e.Message);
                return new T();
            }
        }

        public void Save(T data)
        {
            try
            {
                var folder = Path.GetDirectoryName(m_FilePath);
                if (!string.IsNullOrEmpty(folder)) Directory.CreateDirectory(folder);

                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(m_FilePath, json);
            }
            catch (Exception e)
            {
                m_Logger.LogError($"Error while writing settings file to {m_FilePath}: " + e.Message);
            }
        }
    }
}

#endif
