using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Wattle.Utils;
using Wattle.Wild.Logging;

namespace Wattle.Wild.Infrastructure
{
    public class SaveSystem : Singleton<SaveSystem>
    {
        private const string CONFIG_DIRECTORY = "Data";
        private static string configPath;
        public AudioConfig AudioSettings { get; set; } = new AudioConfig();
        public SaveFile SaveFile { get; set; }

        private readonly List<ISaveable> configs = new List<ISaveable>();

        public override IEnumerator Initalise()
        {
            configPath = $"{Application.persistentDataPath}/{CONFIG_DIRECTORY}";
            initialised = true;

#if !UNITY_WEBGL
            LOG.Log($"SAVE PATH {configPath}");

            try
            {
                Task.Run(() => LoadConfigs(configPath, () =>
                {
                    initialised = true;
                }));
            }
            catch (Exception e)
            {
                LOG.LogError($"Exception thrown: {e}", LOG.Type.SAVESYSTEM);
            }
#else
            AudioSettings = new AudioConfig();
#endif

            yield return base.Initalise();
        }

        private async Task LoadConfigs(string configPath, Action onComplete = null)
        {
            List<Task> tasks = new List<Task>();

            AudioSettings = new AudioConfig();
            configs.Add(AudioSettings);

#if !UNITY_WEBGL
            if (!Directory.Exists(configPath))
            {
                LOG.Log($"Directory does not exist, creating: {configPath}", LOG.Type.SAVESYSTEM);
                Directory.CreateDirectory(configPath);
            }

            // ADD CONDIGS HERE
            foreach (ISaveable config in configs)
            {
                tasks.Add(LoadConfig(config));
            }
#endif
            await Task.WhenAll(tasks);

            onComplete?.Invoke();
        }

        public static async Task LoadConfig(ISaveable config)
        {
            await Task.Yield();
#if !UNITY_WEBGL
            string filePath = $"{configPath}/{config.FileName}.json";

            if (File.Exists(filePath))
            {
                string json = await File.ReadAllTextAsync(filePath);
                config.Deserialize(json);

                LOG.Log($"CONFIG: [{config.FileName}] Loaded from {filePath}", LOG.Type.SAVESYSTEM);
            }
            else
            {
                await SaveConfig(config);

                string json = await File.ReadAllTextAsync(filePath);
                config.Deserialize(json);

                LOG.Log($"CONFIG: {config.FileName} Created and Loaded from {filePath}", LOG.Type.SAVESYSTEM);
            }
#endif
        }

        public static async Task SaveConfig(ISaveable config)
        {
            string filePath = $"{configPath}/{config.FileName}.json";

            string json = config.Serialize();
            await File.WriteAllTextAsync(filePath, json);

            LOG.Log($"CONFIG: {config.FileName} Saved - At: {filePath}", LOG.Type.SAVESYSTEM);
        }

        public void ResetConfigs()
        {
            AudioSettings = new AudioConfig();
            AudioSettings.Save();

            LOG.Log($"Configs reset to defaults", LOG.Type.SAVESYSTEM);
        }

        public void SaveConfigs()
        {
            foreach (ISaveable config in configs)
            {
                config.Save();
            }

            LOG.Log($"Configs saved.", LOG.Type.SAVESYSTEM);

        }
    }
}
