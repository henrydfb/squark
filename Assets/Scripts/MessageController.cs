using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MessageController : MonoBehaviour {

    private Text component;

    public void Show(string message)
    {
        component = GetComponent<Text>();
        component.color = new Color(1, 1, 1, 1);
        component.text = message;
        //component.transform.position = new Vector3(0.5f,0.5f) - Camera.main.ScreenToViewportPoint(new Vector3(component.GetScreenRect().width, component.GetScreenRect().height) / 2);
    }

    public void Hide()
    {
        component.color = new Color(1, 1, 1, 0);
    }
}
