using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OutlineFx;

public class OutlineManager : MonoBehaviour
{
    private CapsuleCollider Collider;
    private OutlineFx.OutlineFx outlineFX;

    void Start()
    {
        Collider = GetComponent<CapsuleCollider>();

        
        outlineFX = GetComponentInChildren<OutlineFx.OutlineFx>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate(){
        CheckOutline();
    }

    private bool IsMouseHover(){

        Debug.Log("masuk");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("NPC")))
        {
            Debug.Log("Raycast hit: " + hit.collider.name);
            return hit.collider == Collider;
        }
        return false;
    }

    void CheckOutline(){
        if(IsMouseHover()){
            Debug.Log("Mouse Hovering");
            outlineFX.enabled = true;
        } else {
            outlineFX.enabled = false;
        }
    }
}   
