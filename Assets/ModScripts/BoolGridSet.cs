using System;
using UnityEngine;

public class BoolGridSet : MonoBehaviour
{
    [SerializeField] private GameObject Indicator1;
    [SerializeField] private GameObject Indicator2;

    internal byte Value { get; private set; }

    internal void Reset(bool value, bool i1)
    {
        Value = (byte)(value ? 1 : 0);
        Indicator1.SetActive(value && i1);
        Indicator2.SetActive(value && !i1);
    }
}
