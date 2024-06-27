using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteract : MonoBehaviour, IInteractable
{
    public void Action(FirstPersonController player)
    {
        Debug.Log("hello!");
    }
}
