using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using DG.Tweening;

public class ControllerScript : MonoBehaviour
{
    /*
     * References:
     * https://docs.unity3d.com/ScriptReference/Video.VideoPlayer.html
     * https://docs.unity3d.com/Manual/VideoPlayer.html
     * https://www.youtube.com/watch?v=V8rwCWiRLWI
     * https://www.youtube.com/watch?v=xKso4D6T8Sk
     * 
     */

    // Prefabs
    public GameObject buttonPrefab;
    public GameObject videoPanelPrefab;
    public GameObject objectPanelPrefab;
    public GameObject textPrefab;
    public GameObject cube;

    public VideoPlayer videoPlayer;
    public Button fButton;

    public Transform panelETransform;
    public Transform panelGTransform;


    private GameObject[] uiElements;
    private Color[] colors;

    //Used to loop over the color array
    private int count;

    //Track if Button F has been clicked
    private bool clicked;

    //Cube y rotation speed
    private float speed;


    // Start is called before the first frame update
    void Start()
    {
        clicked = false;
        fButton.onClick.AddListener(FButtonOnClick);

        //Create a color array. Set it to hold Red, Green and Blue.
        colors = new Color[3];
        colors[0] = new Color(1.0f, 0.0f, 0.0f);
        colors[1] = new Color(0.0f, 1.0f, 0.0f);
        colors[2] = new Color(0.0f, 0.0f, 1.0f);

        //Arbitrary value for cube rotation speed
        speed = 100f;

        //Default cube color to red.
        count = 0;
        cube.GetComponent<Renderer>().material.color = colors[count];
        count++;

        videoPlayer.loopPointReached += EndReached;
    }

    //Animate cube rotation on Y axis
    void Update()
    {
        cube.transform.Rotate(Vector3.up, speed * Time.deltaTime);
    }

    //Event which fires everytime the video player hits the end of the video.
    void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        //Set color to yellow.
        cube.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 0.0f);
    }

    private void FButtonOnClick()
    {
        //If not clicked, instantiate UI elements onclick.
        if (!clicked)
        {
            //Localpostion, parent transform, buttonText
            GameObject cButton = CreateButton(new Vector2(0, 75), panelETransform, "C");
            GameObject dButton = CreateButton(new Vector2(0, 0), panelETransform, "D");

            cButton.GetComponent<Button>().onClick.AddListener(CButtonOnClick);
            dButton.GetComponent<Button>().onClick.AddListener(DButtonOnClick);

            //Instantiate the text.
            GameObject playText = Instantiate(textPrefab);
            playText.GetComponent<RectTransform>().localPosition = new Vector2(0, 40);
            playText.transform.SetParent(panelETransform, false);
            playText.GetComponent<Text>().text = "Play / Pause";

            GameObject colorText = Instantiate(textPrefab);
            colorText.GetComponent<RectTransform>().localPosition = new Vector2(0, -35);
            colorText.transform.SetParent(panelETransform, false);
            colorText.GetComponent<Text>().text = "Next Color";

            //Instantiate the Video Panel.
            CreatePanel(new Vector2(0, 90), panelGTransform, videoPanelPrefab);

            //Instantiate the Object Panel.
            CreatePanel(new Vector2(0, -90), panelGTransform, objectPanelPrefab);

            //Set Panel E size to 150px
            panelETransform.gameObject.GetComponent<RectTransform>().DOSizeDelta(new Vector2(150, 0), 0.5f, false);

            //Ease button F anchor point
            fButton.gameObject.GetComponent<RectTransform>().DOAnchorMin(new Vector2(1, 0), 0.5f, false);
            fButton.gameObject.GetComponent<RectTransform>().DOAnchorMax(new Vector2(1, 0), 0.5f, false);
            fButton.gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-40, 20), 0.5f, false);

        }
        else
        {
            //Set Panel E back to original size.
            panelETransform.gameObject.GetComponent<RectTransform>().DOSizeDelta(new Vector2(60, 0), 0.5f, false);

            //Ease button F anchor point back to original location
            fButton.gameObject.GetComponent<RectTransform>().DOAnchorMin(new Vector2(0.5f, 0), 0.5f, false);
            fButton.gameObject.GetComponent<RectTransform>().DOAnchorMax(new Vector2(0.5f, 0), 0.5f, false);
            fButton.gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 20), 0.5f, false);

            //Destroy all created objects with UIPanel tag.
            uiElements = GameObject.FindGameObjectsWithTag("UIPanel");

            foreach (GameObject uiElement in uiElements)
            {
                Destroy(uiElement);
            }
        }

        clicked = !clicked;
    }


    private void CButtonOnClick()
    {
        if (videoPlayer.isPaused)
        {
            videoPlayer.Play();
        }
        else
        {
            videoPlayer.Pause();
        }
    }

    private void DButtonOnClick()
    {
        // use % to loop the color array
        cube.GetComponent<Renderer>().material.color = colors[count % colors.Length];
        count++;
    }

    private GameObject CreateButton(Vector2 localPos, Transform parentTransform, string buttonText)
    {
        //Instantiate button prefab within parent object
        GameObject button = Instantiate(buttonPrefab);
        button.GetComponent<RectTransform>().localPosition = localPos;
        button.transform.SetParent(parentTransform, false);

        //Set the instantiated button's text component.
        GameObject textComponent = button.transform.GetChild(0).gameObject;
        textComponent.GetComponent<Text>().text = buttonText;

        return button;
    }

    private void CreatePanel(Vector2 localPos, Transform parentTransform, GameObject prefab)
    {
        GameObject panel = Instantiate(prefab);
        panel.GetComponent<RectTransform>().localPosition = localPos;
        panel.transform.SetParent(panelGTransform, false);
    }
}
