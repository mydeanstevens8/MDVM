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

using System.IO;

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
