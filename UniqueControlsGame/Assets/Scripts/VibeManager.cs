using UnityEngine;
using TMPro;

public class VibeManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI genreText;
    private string[] genres = {"Joyfull", "Sad", "Brainrotttt", "DANGERRRRRR"};
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string randomizeVibe = genres[Random.Range(0, genres.Length)];
        genreText.text = "Current Vibe: " + randomizeVibe;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
