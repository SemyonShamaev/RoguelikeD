using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ButtonCreator : MonoBehaviour
{
    public Transform canvas;
    public Font font;

	void Start ()
    {
    	CreateButton();
	}
	
	void Update ()
    {
	
	}

    public void CreateButton ()
    {
        GameObject newButton = new GameObject("New button", typeof(Image), typeof(Button), typeof(LayoutElement));
        newButton.transform.SetParent(canvas);

        newButton.GetComponent<LayoutElement>().minHeight = 35;
        newButton.transform.localPosition = Vector3.zero;
        GameObject newText = new GameObject("New text", typeof(Text));
        newText.transform.SetParent(newButton.transform);
        newText.GetComponent<Text>().text = "New button";
        newText.GetComponent<Text>().font = font;
        RectTransform rt = newText.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.anchoredPosition = new Vector2(0, 0);
        rt.sizeDelta = new Vector2(0, 0);
		
        newText.GetComponent<Text>().color = new Color(0, 0, 0);
        newText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        newButton.GetComponent<Button>().onClick.AddListener(delegate { press(); });
    	
		
    }

    public void press ()
    {
 
    }
}
