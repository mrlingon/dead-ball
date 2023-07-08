using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YToZAxis : MonoBehaviour
{
    public float Multiplier = 3f;

    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y * Multiplier);
    }
}
