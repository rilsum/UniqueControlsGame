using UnityEngine;
using UnityEngine.UI;

[System.Serializable] public class KeyObject
{
    public KeyCode key;
    public Image KeyImg;
    public AudioSource KeyAud;
    public Color updateColour = Color.red;
    public Color revertColour = Color.white;
    public GameObject effectPrefab;
    public Transform spawnEffect;
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
            //initiating sound and color chane of key
            if (Input.GetKeyDown(k.key))
            {
            Debug.Log(k.key + " Sound!");
            k.KeyImg.color = k.updateColour;
            k.KeyAud.Play();
                        // adding sprite visual effects when key is pressed
            if (k.effectPrefab != null && k.spawnEffect != null){
                GameObject effect = Instantiate (
                    k.effectPrefab,
                    k.spawnEffect.position,
                    Quaternion.identity,
                    k.spawnEffect
                );
                Destroy(effect, 1.5f);
            }
            }


            if (Input.GetKeyUp(k.key))
            {
                k.KeyImg.color = k.revertColour;
            }
        }
    }
}
