using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyEditorDebugOnly : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(!Application.isEditor)
        {
            gameObject.SetActive(false);
        }
    }
}
