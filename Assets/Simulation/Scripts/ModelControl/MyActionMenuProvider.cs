namespace MDVM
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class MyActionMenuProvider : MonoBehaviour
    {
        Dictionary<string, UnityEvent> Actions;

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
            actionMenu.ActivateActionMenu();
        }
    }

}
