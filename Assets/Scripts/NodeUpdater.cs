using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NodeUpdater : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other);
    }
}