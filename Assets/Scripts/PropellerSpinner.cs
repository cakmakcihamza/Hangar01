using UnityEngine;

public class PropellerSpinner : MonoBehaviour
{
    public float rotationSpeed = 1200f;

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
