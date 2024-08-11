using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderToRectTransform : MonoBehaviour
{
    private void Awake()
    {
        gameObject.GetComponent<BoxCollider2D>().size = new Vector2 (
            gameObject.GetComponent<RectTransform>().sizeDelta.x,
            gameObject.GetComponent<RectTransform>().sizeDelta.y
        );
    }
}
