using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private Transform target;
    private Vector3 offset;

    void Awake(){
        RectTransform rectTransform = GetComponent<RectTransform>();
        offset = rectTransform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.InGame) return;
        transform.position = target.position + offset;    
    }
}
