using Wattle.Wild.Logging;
using Wattle.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Accessibility;

namespace Wattle.Wild.Infrastructure
{
    public class SaveSystem : Singleton<SaveSystem>
    {
        private const string CONFIG_DIRECTORY = "Config";
        private static string configPath;
        public AudioConfig AudioSettings { get; set; }

        private readonly List<IConfig> configs = new List<IConfig>();

        public override IEnumerator Initalise()
        {
            configPath = $"{Application.persistentDataPath}/{CONFIG_DIRECTORY}";

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

            yield return base.Initalise();
        }

        private async Task LoadConfigs(string configPath, Action onComplete = null)
        {
            List<Task> tasks = new List<Task>();

            if (!Directory.Exists(configPath))
            {
                LOG.Log($"Directory does not exist, creating: {configPath}", LOG.Type.SAVESYSTEM);
                Directory.CreateDirectory(configPath);
            }

            AudioSettings = new AudioConfig();
            configs.Add(AudioSettings);

            // ADD CONDIGS HERE
            foreach (IConfig config in configs)
            {
                tasks.Add(LoadConfig(config));
            }

            await Task.WhenAll(tasks);

            onComplete?.Invoke();
        }

        public static async Task LoadConfig(IConfig config)
        {
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
        }

        public static async Task SaveConfig(IConfig config)
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
            foreach (IConfig config in configs)
            {
                config.Save();
            }

            LOG.Log($"Configs saved.", LOG.Type.SAVESYSTEM);

        }
    }
}
