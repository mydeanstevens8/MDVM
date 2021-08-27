using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using MDVM.UI;

namespace MDVM.Test
{
    public class ModalDisplayTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.J))
            {
                DisplayModalTestWindow();
            }
        }

        void DisplayModalTestWindow()
        {
            GameObject sublayer = new GameObject();

            TextMeshProUGUI tmPro = sublayer.AddComponent<TextMeshProUGUI>();

            tmPro.text = "This is a test.";
            tmPro.color = Color.black;
            tmPro.fontSize = 72;
            tmPro.alignment = TextAlignmentOptions.Center;

            ModalDisplay.Instance.ShowModal(sublayer, "Test Window", true);
        }
    }
}

