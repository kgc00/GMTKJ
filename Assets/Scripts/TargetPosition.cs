using UnityEngine;

public class TargetPosition : MonoBehaviour
{
    void Update()
    {
        Vector3 temp = Input.mousePosition;
        print(temp);
        temp.z = 19;
        this.gameObject.transform.position = Camera.main.ScreenToWorldPoint(temp);
    }
}
