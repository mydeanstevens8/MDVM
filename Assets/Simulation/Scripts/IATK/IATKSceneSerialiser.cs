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