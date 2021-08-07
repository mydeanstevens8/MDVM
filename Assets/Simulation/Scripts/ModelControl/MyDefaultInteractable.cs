using MDVM.UI;
using System;
using System.Collections;
using System.Collections.Generic;
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