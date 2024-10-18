using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    private CapsuleCollider _collider;
    private OutlineFx.OutlineFx _outlineFX;

    void Start()
    {
        _collider = GetComponent<CapsuleCollider>();
        _outlineFX = GetComponentInChildren<OutlineFx.OutlineFx>();
    }

    void FixedUpdate(){
        CheckOutline();
    }

    private bool IsMouseHover(){

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("NPC")))
        {
            return hit.collider == _collider;
        }
        return false;
    }

    void CheckOutline(){
        if(IsMouseHover()){
            _outlineFX.enabled = true;
        } else {
            _outlineFX.enabled = false;
        }
    }
}   
