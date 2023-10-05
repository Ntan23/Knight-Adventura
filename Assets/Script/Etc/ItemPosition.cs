using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPosition : MonoBehaviour
{
    [SerializeField] Transform target;

    public void ChangePosition()
    {
        this.gameObject.transform.position = target.position;
    }
}
