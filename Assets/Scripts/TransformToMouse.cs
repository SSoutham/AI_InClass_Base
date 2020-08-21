using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformToMouse : MonoBehaviour
{
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray,out hit, 100.0f))
            {
                transform.position = hit.point;
            }
        }
    }
}
