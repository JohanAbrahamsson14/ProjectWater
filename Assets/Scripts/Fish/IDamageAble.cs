using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageAble
{
    public float maxHealth { get; set; }
    public float currentHealth  { get; set; }

    public void GetDamaged(float value);
    
    public void Death();
}
