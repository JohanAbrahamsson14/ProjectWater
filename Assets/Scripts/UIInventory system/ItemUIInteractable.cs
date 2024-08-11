using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUIInteractable : MonoBehaviour, IInteractableMouse
{
    public void Interact(InteractorMouse interactor, bool isPrimary)
    {
        if (!isPrimary)
        {
            SecondaryInteract(interactor);
            return;
        }
        gameObject.transform.localScale *= 0.5f;
    }

    public void SecondaryInteract(InteractorMouse interactor)
    {
        gameObject.transform.localScale *= 2;
    }
}
