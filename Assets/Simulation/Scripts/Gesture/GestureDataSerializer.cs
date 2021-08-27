using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MDVM.Gesture
{
    class GestureDataSerializer
    {
        public string GestureStorageDirectory()
        {
            string localPath = "GestureData";
#if UNITY_ANDROID && !UNITY_EDITOR
            return Path.Combine(Application.persistentDataPath, localPath);
#elif UNITY_EDITOR
            return Path.Combine(Application.streamingAssetsPath, localPath);
#endif
        }

        void CreateGestureStorageIfNeeded()
        {
            string gestureStorageDirectory = GestureStorageDirectory();
            if (!Directory.Exists(gestureStorageDirectory))
            {
                Directory.CreateDirectory(gestureStorageDirectory);
            }
        }

        public string GestureStorageFilePath(string name)
        {
            return Path.Combine(GestureStorageDirectory(), name + ".json");
        }

        public void Serialize(HandData data)
        {
            string jsonedData = JsonUtility.ToJson(data);
            CreateGestureStorageIfNeeded();
            string filePath = GestureStorageFilePath(data.Name);

            File.WriteAllText(filePath, jsonedData);
        }

        public HandData Deserialize(string name)
        {
            string jsonedData = File.ReadAllText(GestureStorageFilePath(name));
            return JsonUtility.FromJson<HandData>(jsonedData);
        }

        public string GetJSONRepresentation(HandData data)
        {
            return JsonUtility.ToJson(data);
        }

        public void PrintJSONRepresentation(HandData data)
        {
            Debug.Log(GetJSONRepresentation(data));
        }
    }
}
