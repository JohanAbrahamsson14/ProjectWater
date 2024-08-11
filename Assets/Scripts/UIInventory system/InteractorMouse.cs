using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

interface IInteractableMouse
{
    public void Interact(InteractorMouse interactor, bool isPrimary);
    public void SecondaryInteract(InteractorMouse interactor);
}
interface IDragable
{
    public void OnDrag(InteractorMouse interactor);
    public void OnEndDrag(InteractorMouse interactor);
}

public class InteractorMouse : MonoBehaviour
{
    public Camera InteractorSource;
    [SerializeField] private Transform heldObj;
    private int renderIndex;
    
    public Transform HeldObj
    {
        get => heldObj;
        set
        {
            heldObj = value;
            heldObj.TryGetComponent(out IDragable tempDrag);
            iDrag = tempDrag;
        }
    }

    private IDragable iDrag;

    private void Update()
    {
        //InteractorSource.ScreenToWorldPoint(
        Vector2 screenPos = Input.mousePosition;

        Drag(screenPos);
        
        Click(screenPos);
    }

    public void Click(Vector2 screenPos)
    {
        if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1)) return;

        RaycastHit2D hitInfo = Physics2D.Raycast(screenPos, Vector2.zero);

        if (hitInfo.collider == null ||
            !hitInfo.collider.gameObject.TryGetComponent(out IInteractableMouse interactableObj)) return;
        HeldObj = hitInfo.collider.gameObject.transform;
        
        if (heldObj.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            renderIndex = spriteRenderer.rendererPriority;
            spriteRenderer.rendererPriority = 99;
        }
        
        interactableObj.Interact(this, Input.GetMouseButtonDown(0));
    }

    public void Drag(Vector2 screenPoss)
    {
        if (iDrag != null)
        {
            iDrag.OnDrag(this);
            if (Input.GetMouseButtonUp(0))
            {
                iDrag.OnEndDrag(this);
            }
        }
        if (!Input.GetMouseButtonUp(0)) return;
        if (heldObj!= null && heldObj.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            spriteRenderer.rendererPriority = renderIndex;
        }
        heldObj = null;
        iDrag = null;
    }
}