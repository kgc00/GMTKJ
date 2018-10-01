using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaysAndColliders : MonoBehaviour
{
    [SerializeField]
    private int currentRaycasts = 50;
    [SerializeField]
    private int currentColliders = 50;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("asdkf");
        for (var i = 0; i < currentRaycasts; i++)
        {
            Physics.Raycast(transform.position, UnityEngine.Random.onUnitSphere, 10);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CreateMoreRaycasts();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CreateMoreColliders();
        }
    }

    private void CreateMoreColliders()
    {
        Debug.Log("2");
        for (int i = 0; i < currentColliders; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);
            go.AddComponent<MeshCollider>();
            go.transform.rotation = UnityEngine.Random.rotation;
            go.transform.position = (UnityEngine.Random.insideUnitSphere * 100);
        }
        currentColliders *= 2;
    }

    private void CreateMoreRaycasts()
    {
        Debug.Log("1");
        currentRaycasts *= 2;
    }
}
