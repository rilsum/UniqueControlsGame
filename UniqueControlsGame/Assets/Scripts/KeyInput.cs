using UnityEngine;
using UnityEngine.UI;

[System.Serializable] public class KeyObject
{
    public KeyCode key;
    public Image KeyImg;
    public AudioSource KeyAud;
    public Color updateColour = Color.red;
    public Color revertColour = Color.white;
}
public class KeyInput : MonoBehaviour
{
    public KeyObject[] keys;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyObject k in keys)
        {
            if (Input.GetKeyDown(k.key))
            {
            Debug.Log(k.key + " Sound!");
            k.KeyImg.color = k.updateColour;
            k.KeyAud.Play();
            }
            if (Input.GetKeyUp(k.key))
            {
                k.KeyImg.color = k.revertColour;
            }
        }
    }
}
