using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace IATK
{
    class IATKSerialiser
    {
        // We are trying to disable file mode for serialization activities.
        public static bool FileMode = false;

        public static bool Exists(string fileName)
        {
            if(FileMode)
            {
                return File.Exists(fileName);
            }
            else
            {
                return IATKSceneSerialiser.GetSceneSerialiser().Exists(fileName);
            }
        }

        public static void Write(string fileName, string data)
        {
            if(FileMode)
            {
                File.WriteAllText(fileName, data);
            }
            else
            {
                IATKSceneSerialiser.GetSceneSerialiser().Write(fileName, data);
            }
        }

        public static string Read(string fileName)
        {
            if (FileMode)
            {
                return File.ReadAllText(fileName);
            }
            else
            {
                return IATKSceneSerialiser.GetSceneSerialiser().Read(fileName);
            }
        }
    }
}
