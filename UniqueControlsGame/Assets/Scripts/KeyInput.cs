using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
    [System.Serializable]
    public class spaceBarRecord
    {
        public KeyCode key;
        public float TimePressed;
    }
    public KeyObject[] keys;
    public KeyCode recorder = KeyCode.Space;

    private List<spaceBarRecord> recordedKeys = new List<spaceBarRecord>();
    private bool activateRecording = false;
    private bool playRecording = false;
    private float recordingStartTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //Toggle Recording with Space Bar
        if (Input.GetKeyDown(recorder))
        {
            if (!activateRecording)
            {
                BeginRecording();
            }
            else
            {
                StopRecording();
                StartCoroutine(PlaybackRecording());
            }
        }


        foreach (KeyObject k in keys)
        {
            //initiating sound and color chane of key
            if (Input.GetKeyDown(k.key))
            {
                PressKey(k);
                if (activateRecording && !playRecording)
                {
                    spaceBarRecord newRecord = new spaceBarRecord();
                    newRecord.key = k.key;
                    newRecord.TimePressed = Time.time - recordingStartTime;
                    recordedKeys.Add(newRecord);
                }
            }
            
            if (Input.GetKeyUp(k.key))
            {
                ReleaseKey(k);
            }
        }
    }

    void BeginRecording()
    {
        recordedKeys.Clear();
        recordingStartTime = Time.time;
        activateRecording = true;
        Debug.Log("Recording Started");
    }

    void StopRecording()
    {
        activateRecording = false;
        Debug.Log("Recording Stopped");
    }

    IEnumerator PlaybackRecording()
    {
        if (recordedKeys.Count == 0)
        {
            Debug.Log("No keys recorded");
            yield break;
        }

        playRecording = true;
        Debug.Log("Recording Playback begun");

        float lastTime = 0f;

        foreach (spaceBarRecord record in recordedKeys)
        {
            float waitTime = record.TimePressed - lastTime;
            yield return new WaitForSeconds(waitTime);

            KeyObject keyToPlay = FindKeyObject(record.key);
            if (keyToPlay != null)
            {
                PressKey(keyToPlay);
                yield return new WaitForSeconds(0.1f);
                ReleaseKey(keyToPlay);
            }

            lastTime = record.TimePressed;
        }
        playRecording = false;
        Debug.Log("Playback completed");
    }

    KeyObject FindKeyObject(KeyCode keyCode)
    {
        foreach (KeyObject k in keys)
        {
            if (k.key == keyCode)
            {
                return k;
            }
        }
        return null;
    }

    void PressKey(KeyObject k)
    {
        Debug.Log(k.key + " Sound!");
        k.KeyImg.color = k.updateColour;
        k.KeyAud.Play();
        // adding sprite visual effects when key is pressed
        if (k.effectPrefab != null && k.spawnEffect != null)
        {
            GameObject effect = Instantiate(
                k.effectPrefab,
                k.spawnEffect.position,
                Quaternion.identity,
                k.spawnEffect
            );
            Destroy(effect, 1.5f);
        }
    }

    void ReleaseKey(KeyObject k)
    {
        k.KeyImg.color = k.revertColour;
    }
}
