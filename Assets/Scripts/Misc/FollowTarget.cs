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
        transform.position = target.position + offset;    
    }
}
