using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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

    [SerializeField] private TextMeshProUGUI BackspaceMessage;
    [SerializeField] private TextMeshProUGUI EnterMessage;
    [SerializeField] private TextMeshProUGUI RecordMessage;
    [SerializeField] private TextMeshProUGUI Track1Message;
    [SerializeField] private TextMeshProUGUI Track1SavedMessage;
    [SerializeField] private TextMeshProUGUI Track2SavedMessage;
    [SerializeField] private TextMeshProUGUI Track2Message;
    [SerializeField] private TextMeshProUGUI SpacebarMessage;
    [SerializeField] private Image SpaceBar;
    [SerializeField] private Image Track1;
    [SerializeField] private float msgDuration = 1.5f;
    public KeyObject[] keys;
    public KeyCode recorder = KeyCode.Space;
    public KeyCode RemoveLastKey = KeyCode.Backspace;
    public KeyCode RemoveAllKeys = KeyCode.Return;

    private List<spaceBarRecord> recordedKeys = new List<spaceBarRecord>();
    private List<spaceBarRecord> savedTrack1 = new List<spaceBarRecord>();
    private List<spaceBarRecord> savedTrack2 = new List<spaceBarRecord>();
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
            }
        }

        if (Input.GetKeyDown(RemoveLastKey) && recordedKeys.Count > 0)
        {
            spaceBarRecord lastKey = recordedKeys[recordedKeys.Count - 1];
            foreach (KeyObject k in keys)
            {
                if (k.key == lastKey.key)
                {
                    k.KeyAud.Stop();
                }
            }
            recordedKeys.RemoveAt(recordedKeys.Count - 1);
            StartCoroutine(ShowBackSpaceMessage());
            Debug.Log("Last Recorded Key has been Removed");
        }

        if (Input.GetKeyDown(RemoveAllKeys))
        {
            StartCoroutine(ShowEnterMessage());
            RemoveAllTracks();
            recordedKeys.Clear();
            activateRecording = false;
            playRecording = false;
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

        KeyObject spaceKey = FindKeyObject(recorder);
        if (spaceKey != null)
        {
            spaceKey.KeyImg.color = Color.red;
        }
        Debug.Log("Recording Started");
        RecordMessage.gameObject.SetActive(true);
        SpacebarMessage.gameObject.SetActive(false);
    }

    void StopRecording()
    {
        activateRecording = false;
        if (savedTrack1.Count == 0)
        {
            Track1SavedMessage.gameObject.SetActive(true);
            savedTrack1 = new List<spaceBarRecord>(recordedKeys);
        }
        else
        {
            Track2SavedMessage.gameObject.SetActive(true);
            savedTrack2 = new List<spaceBarRecord>(recordedKeys);
        }
        foreach (KeyObject k in keys)
        {

            k.KeyAud.Stop();
            RecordMessage.gameObject.SetActive(false);
            SpacebarMessage.gameObject.SetActive(true);
        }

        KeyObject spaceKey = FindKeyObject(recorder);
        if (spaceKey != null)
        {
            spaceKey.KeyImg.color = Color.white;
        }
        Debug.Log("Recording Stopped");
        
    }

    IEnumerator PlaybackRecording(List<spaceBarRecord> track)
    {
        if (track.Count == 0)
        {
            Debug.Log("No keys recorded");
            yield break;
        }

        playRecording = true;
        Debug.Log("Recording Playback begun");

        float lastTime = 0f;
        float longestClipLength = 0f;

        foreach (spaceBarRecord record in track)
        {
            float waitTime = record.TimePressed - lastTime;
            yield return new WaitForSeconds(waitTime);

            KeyObject keyToPlay = FindKeyObject(record.key);
            if (keyToPlay != null)
            {
                PressKey(keyToPlay);

                if (keyToPlay.KeyAud != null && keyToPlay.KeyAud.clip != null)
                {
                    longestClipLength = Mathf.Max(longestClipLength, keyToPlay.KeyAud.clip.length);
                }
                yield return new WaitForSeconds(0.1f);
                ReleaseKey(keyToPlay);
            }

            lastTime = record.TimePressed;
        }

        yield return new WaitForSeconds(longestClipLength);
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
        if (k.key == recorder && activateRecording)
        {
            return;
        }
        k.KeyImg.color = k.revertColour;
    }

    IEnumerator ShowBackSpaceMessage()
    {
        BackspaceMessage.gameObject.SetActive(true);
        yield return new WaitForSeconds(msgDuration);
        BackspaceMessage.gameObject.SetActive(false);
    }

    IEnumerator ShowEnterMessage()
    {
        EnterMessage.gameObject.SetActive(true);
        yield return new WaitForSeconds(msgDuration);
        EnterMessage.gameObject.SetActive(false);
    }


    void RemoveAllTracks()
    {
        foreach (KeyObject k in keys)
        {
            if (k.KeyAud != null && k.KeyAud.isPlaying)
            {
                k.KeyAud.Stop();
                k.KeyAud.time = 0f;
            }
        }
    }

    public void PlaySavedTrack1()
    {
        if (!playRecording && savedTrack1.Count > 0)
        {
            StartCoroutine(PlayTrack1Routine());
        }
    }

    public void PlaySavedTrack2()
    {
        if (!playRecording && savedTrack2.Count > 0)
        {
            
            StartCoroutine(PlayTrack2Routine());
           
        }
    }

    IEnumerator PlayTrack1Routine()
    {
        Track1Message.gameObject.SetActive(true);
        yield return StartCoroutine(PlaybackRecording(savedTrack1));
        Track1Message.gameObject.SetActive(false);
    }

    IEnumerator PlayTrack2Routine()
    {
        Track2Message.gameObject.SetActive(true);
        yield return StartCoroutine(PlaybackRecording(savedTrack2));
        Track2Message.gameObject.SetActive(false);
    }

    public void ReturnToMenu()
    {
        Debug.Log("Menu Successfully Loaded");
        SceneManager.LoadScene("Menu");
    }
}
