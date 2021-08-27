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


using MDVM.UI;
using System;
using System.Reflection;
using UnityEngine;

namespace MDVM
{
    public class MyDefaultInteractable : MonoBehaviour
    {
        public bool addToParentAndDestroy = true;

        public MyGrabbable grabbableInstance;
        public MyActionMenuProvider actionMenuProvider;
        public MyPointerResponse pointerResponse;

        public OutlineOnHover visualEffectsOutline;

        // Start is called before the first frame update
        void Start()
        {
            if(addToParentAndDestroy)
            {
                AddToParentAndDestroy();
            }
        }

        void AddToParentAndDestroy()
        {
            GameObject parentOb = transform.parent.gameObject;

            if(grabbableInstance != null)
            {
                MyGrabbable newComponent = (MyGrabbable) parentOb.AddComponent(grabbableInstance.GetType());
                CopyInto(newComponent, grabbableInstance);
            }

            if (actionMenuProvider != null)
            {
                MyActionMenuProvider newComponent = (MyActionMenuProvider) parentOb.AddComponent(actionMenuProvider.GetType());
                CopyInto(newComponent, actionMenuProvider);
            }

            if (pointerResponse != null)
            {
                MyPointerResponse newComponent = (MyPointerResponse) parentOb.AddComponent(pointerResponse.GetType());
                CopyInto(newComponent, pointerResponse);
            }

            if(visualEffectsOutline != null)
            {
                OutlineOnHover newComponent = (OutlineOnHover)parentOb.AddComponent(visualEffectsOutline.GetType());
                CopyInto(newComponent, visualEffectsOutline);
            }

            // Destroys ourselves.
            Destroy(gameObject);
        }

        // Use the reflection system to copy values.
        // Based off code from https://answers.unity.com/questions/530178/how-to-get-a-component-from-an-object-and-add-it-t.html
        public static T CopyInto<T>(T comp, T other) where T : Component
        {
            Type compType = comp.GetType();
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;

            PropertyInfo[] properties = compType.GetProperties(flags);
            FieldInfo[] fields = compType.GetFields(flags);

            foreach (PropertyInfo property in properties)
            {
                if (property.CanWrite)
                {
                    try
                    {
                        property.SetValue(comp, property.GetValue(other));
                    }
                    catch (NotImplementedException) 
                    { 

                    }
                }
            }

            foreach (FieldInfo field in fields)
            {
                field.SetValue(comp, field.GetValue(other));
            }

            return comp;
        }
    }

}