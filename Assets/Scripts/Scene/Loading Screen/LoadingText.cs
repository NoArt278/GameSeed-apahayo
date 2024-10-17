using TMPro;
using UnityEngine;

public class LoadingText : MonoBehaviour
{
    [SerializeField] private float _loopDuration = 3f;
    private TextMeshProUGUI text;

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (Time.time % _loopDuration < _loopDuration / 4)
        {
            text.text = "Loading";
        }
        else if (Time.time % _loopDuration < _loopDuration / 4 * 2)
        {
            text.text = "Loading .";
        } 
        else if (Time.time % _loopDuration < _loopDuration / 4 * 3)
        {
            text.text = "Loading . .";
        }
        else
        {
            text.text = "Loading . . .";
        }
    }
}