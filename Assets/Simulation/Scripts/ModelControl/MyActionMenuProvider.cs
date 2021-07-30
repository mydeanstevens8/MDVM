using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MDVM
{

    public class MyActionMenuProvider : MonoBehaviour
    {
        public List<MyActionMenu.ActionMenuAction> actions;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void LaunchActionMenu()
        {
            MyActionMenu actionMenu = GameObject.Find("ActionMenuCanvas").GetComponent<MyActionMenu>();

            actionMenu.AssignActions(actions);
            actionMenu.ActivateActionMenu(transform.position);
        }
    }

}
