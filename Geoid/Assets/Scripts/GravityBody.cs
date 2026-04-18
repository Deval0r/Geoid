using Unity.Physics;
using UnityEngine;

public class GravityBody : MonoBehaviour
{
Rigidbody rb;
float gravity =10f;
public Transform gravitySource;
void Start() {
    rb = GetComponent<Rigidbody>();
    rb.useGravity = false;
}
void FixedUpdate() {
if (gravitySource==null){return;}
Vector3 gravityDir = (gravitySource.position - transform.position).normalized;
rb.AddForce(gravityDir * gravity, ForceMode.Acceleration);
Vector3 up = -gravityDir;
Vector3 forward = Vector3.ProjectOnPlane(transform.forward, up).normalized;
Quaternion targetRotation = Quaternion.LookRotation(forward, up);
rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime));
}
    void OnTriggerEnter(UnityEngine.Collider other)
    {
              if (other.gameObject.TryGetComponent(out GravityZone gravityZone))
        {
            gravitySource=gravityZone.transform;
            Debug.Log("gravityzone enter");
        }  
    }
}
