using UnityEngine;

public class TargetPosition : MonoBehaviour
{
    void Update()
    {
        Vector3 temp = Input.mousePosition;
        temp.z = 19;
        this.gameObject.transform.position = Camera.main.ScreenToWorldPoint(temp);
    }
}
