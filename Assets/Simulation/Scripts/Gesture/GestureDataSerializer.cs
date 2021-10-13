/* * *
Copyright (c) 2021 Dean Stevens and affiliates.

Permission is hereby granted, free of charge, to any person obtaining a copy 
of this software and associated documentation files ("MDVM"), to deal in 
MDVM without restriction, including without limitation the rights to use, 
copy, modify, merge, publish, distribute, sublicense, and/or sell copies of 
MDVM, and to permit persons to whom MDVM is furnished to do so, subject to 
the following conditions:

The above copyright notice and this permission notice shall be included in 
all copies or substantial portions of MDVM.

MDVM IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR 
A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL DEAN STEVENS, 
OTHER AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH MDVM OR THE USE OR OTHER DEALINGS IN MDVM.

If you reside in Australia, the above disclaimer does not affect any consumer 
guarantees automatically given to you under the Competition and Consumer Act 
2010 (Commonwealth) and the Australian Consumer Law.
 * * */

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MDVM.Gesture
{
    class GestureDataSerializer
    {
        public static GestureDataSerializer Instance
        {
            get;
            private set;
        }

        static GestureDataSerializer()
        {
            Instance = new GestureDataSerializer();
        }

        private GestureDataSerializer()
        {

        }

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
