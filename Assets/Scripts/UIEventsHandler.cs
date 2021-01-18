using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEventsHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons) {
            button.onClick.AddListener(HandleButton1Click);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleButton1Click()
    {
        Debug.Log("button on click");
    }
}
