using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

[ExecuteInEditMode]
public class ButtonAutoWidth : MonoBehaviour
{
    public TextMeshProUGUI textMeshComponent = null;
    public float padding = 40.0f;

    public float height = 128.0f;

    // Start is called before the first frame update
    void Start()
    {
        if(textMeshComponent == null)
        {
            textMeshComponent = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(textMeshComponent != null)
        {
            Vector2 preferredSize = textMeshComponent.GetPreferredValues();
            float calculatedBoxSize = padding * 2 + preferredSize.x;

            (transform as RectTransform).sizeDelta = new Vector2(calculatedBoxSize, height);
        }
    }
}
