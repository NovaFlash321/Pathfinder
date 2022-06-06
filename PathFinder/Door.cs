using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Transform DoorLocation;
    [SerializeField]DoorDirection direction;

    //private void Start()
    //{
    //    DoorLocation = this.transform;
    //}

    public DoorDirection GetDirection()
    {
        return direction;
    }
    public Transform GetLocation()
    {
        return DoorLocation;
    }

    public void SetDirection(DoorDirection _direction)
    {
        direction = _direction;
    }

    private void Update()
    {
        DoorLocation.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

}

public enum DoorDirection { North, South, East, West };