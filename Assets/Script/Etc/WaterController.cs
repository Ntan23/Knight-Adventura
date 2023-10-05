using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterController : MonoBehaviour
{
    BuoyancyEffector2D buoyancy;

    private void Awake()
    {
        buoyancy = GetComponent<BuoyancyEffector2D>();
    }
    
    public void LeftDirection()
    {
        buoyancy.flowAngle = 95;
    }

    public void RightDirection()
    {
        buoyancy.flowAngle = 85;
    }
}
