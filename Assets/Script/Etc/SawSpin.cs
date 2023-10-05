using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawSpin : MonoBehaviour
{
    [SerializeField] float rotateSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0,0,45)*rotateSpeed*Time.deltaTime);
    }
}
