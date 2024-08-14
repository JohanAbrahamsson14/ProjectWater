using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public void Interact(Ray ray, float distance, FirstPersonController player)
    {
        if (!Physics.Raycast(ray, out RaycastHit hitData, distance, LayerMask.GetMask("Interactable"))) return;
        if (hitData.collider.gameObject.TryGetComponent(out IInteractable interactable)) interactable.Action(player);
    }
}
