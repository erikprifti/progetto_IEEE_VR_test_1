using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class socketCollision : MonoBehaviour
{
    private bool _isActive = false;
    public void OnTriggerEnter(Collider other)
    {
        _isActive = true;
        Debug.Log("AAA socket attivo");
    }

    public void OnTriggerExit(Collider other)
    {
        _isActive=false;
        Debug.Log("AAA socket uscito");

    }

    public bool isActive()
    {
        return _isActive;
    }
}
