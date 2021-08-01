using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace MDVM 
{
    public class MDVMDebugInfo : MonoBehaviour
    {
        TextMeshProUGUI textMesh = null;

        // Start is called before the first frame update
        void Awake()
        {
            textMesh = GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {
            float fps = 1.0f / Time.deltaTime;
            textMesh.text = Mathf.RoundToInt(fps) + " FPS";
        }
    }

}
