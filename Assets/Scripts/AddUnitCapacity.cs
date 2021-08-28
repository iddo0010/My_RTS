using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddUnitCapacity : MonoBehaviour
{
    [SerializeField] int tentCap;
    void Awake()
    {
        UnitAllowance.instance.AddUnitCapacity(tentCap);
    }
}
