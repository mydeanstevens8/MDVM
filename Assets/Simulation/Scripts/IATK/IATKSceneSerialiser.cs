using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IATK
{
    [ExecuteInEditMode]
    [System.Serializable]
    public class IATKSceneSerialiser : MonoBehaviour
    {
        // I should use a hashtable for this, but this is for display in the editor for debugging purposes without
        // having to use an editor script.
        [System.Serializable]
        public struct SerialStorage
        {
            public string key;
            public string value;
        }

        static IATKSceneSerialiser cachedSerial = null;

        [SerializeField]
        List<SerialStorage> serialisedValues = new List<SerialStorage>();

        public bool Exists(string path)
        {
            foreach(SerialStorage store in serialisedValues)
            {
                if(store.key == path)
                {
                    return true;
                }
            }

            Debug.Log("Path '" + path + "' not found when checking for existence.");
            return false;
        }

        public string Read(string path)
        {
            foreach (SerialStorage store in serialisedValues)
            {
                if (store.key == path)
                {
                    return store.value;
                }
            }

            Debug.LogWarning("Path '" + path + "' not found!");
            return null;
        }

        public void Write(string path, string value)
        {
            SerialStorage? toMod = null;
            foreach (SerialStorage store in serialisedValues)
            {
                if (store.key == path)
                {
                    toMod = store;
                }
            }

            if(toMod != null)
            {
                SerialStorage toModDef = (SerialStorage)toMod;
                toModDef.value = value;
            }
            else
            {
                SerialStorage newStorage = new SerialStorage();
                newStorage.key = path;
                newStorage.value = value;
                serialisedValues.Add(newStorage);
            }
        }

        public static IATKSceneSerialiser GetSceneSerialiser()
        {
            if (cachedSerial)
            {
                return cachedSerial;
            }
            else 
            {
                GameObject serialObject = GameObject.Find("IATKSerial");
                if(serialObject != null)
                {
                    cachedSerial = serialObject.GetComponent<IATKSceneSerialiser>();
                }

                if(cachedSerial == null)
                {
                    // Create the object.
                    serialObject = new GameObject("IATKSerial");
                    cachedSerial = serialObject.AddComponent<IATKSceneSerialiser>();
                }

                return cachedSerial;
            } 
        }
    }
}