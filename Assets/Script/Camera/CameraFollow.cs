using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    [Range(1,10)]
    public float smoothFactor;
    public Vector3 minValue,maxValue;
    
    void FixedUpdate()
    {
        follow();
    }

    void follow()
    {
        Vector3 targetPosition=target.position+offset;
        Vector3 boundPosition=new Vector3(Mathf.Clamp(targetPosition.x,minValue.x,maxValue.x),Mathf.Clamp(targetPosition.y,minValue.y,maxValue.y),Mathf.Clamp(targetPosition.z,minValue.z,maxValue.z));
        Vector3 smoothPosition=Vector3.Lerp(transform.position,boundPosition,smoothFactor*Time.fixedDeltaTime);
        transform.position=smoothPosition;
    }
}
