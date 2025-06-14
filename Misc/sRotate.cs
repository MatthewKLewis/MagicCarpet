using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sRotate : MonoBehaviour
{
    [SerializeField] private float speed;

    void Update()
    {
        transform.Rotate(new Vector3(0, speed, 0));
    }
}
