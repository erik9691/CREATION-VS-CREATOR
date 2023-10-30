using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField]
    float _speed = 5f, rotationSpeed = 0.3f, changeSpeed = 0.5f;

    [SerializeField] Transform tempTarget;
    Transform target, playerTarget;
    bool isGrabbed = false;

    private void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Overlord Head").transform;
        target = tempTarget;
        Invoke("ChangeTarget", changeSpeed);
    }

    void Update()
    {
        if (isGrabbed) return;
        Vector3 lookDirection = target.position - transform.position;
        lookDirection.Normalize();

        Quaternion lookdir = Quaternion.LookRotation(lookDirection);
        lookdir = Quaternion.Euler(new Vector3(lookdir.eulerAngles.x, lookdir.eulerAngles.y, 0));
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookdir, rotationSpeed * Time.deltaTime);

        transform.position += (target.position - transform.position).normalized * _speed * Time.deltaTime;
    }

    public void ChangeTarget()
    {
        if (target != playerTarget)
        {
            target = playerTarget;
        }
        else
        {
            target = tempTarget;
            isGrabbed = false;
        }
    }

    public void GotGrabbed()
    {
        isGrabbed = true;
    }
}
