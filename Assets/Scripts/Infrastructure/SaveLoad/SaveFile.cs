using UnityEngine;
using Wattle.Utils;
using Wattle.Wild.Gameplay;

namespace Wattle.Wild.Infrastructure
{
    public class SaveFile : ISaveable
    {
        private struct SaveData
        {
            public SaveData(SaveFile saveFile)
            {
                playerLocation = saveFile.playerLocation.Value;
                playerPosition = saveFile.positionOffset;
            }

            public MapSectionLocation playerLocation;
            public Vector2 playerPosition;
        }
        public string FileName => "SaveFile";

        public Observable<MapSectionLocation> playerLocation = new Observable<MapSectionLocation>(MapSectionLocation.CENTER);
        public Vector2 positionOffset = new Vector2(0, 0);

        public void Deserialize(string json)
        {
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            playerLocation.Value = data.playerLocation;
            positionOffset = data.playerPosition;
        }

        public string Serialize()
        {
            string data = JsonUtility.ToJson(new SaveData(this), true);
            return data;
        }
    }
}
