using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private Transform _target;
    private Vector3 _offset;

    void Awake(){
        RectTransform rectTransform = GetComponent<RectTransform>();
        _offset = rectTransform.position - _target.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.InGame) return;
        transform.position = _target.position + _offset;    
    }
}
