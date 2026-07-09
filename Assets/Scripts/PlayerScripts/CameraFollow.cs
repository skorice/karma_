using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; 

    public float smoothSpeed = 5f;   
    public Vector3 offset;         


    void Start()
    {
       
        if (offset == Vector3.zero)
            offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);

    }
}