using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class Room : MonoBehaviour
{
    [SerializeField] Door[] doors;
    List<string> DoorLocations = new List<string>();
    [SerializeField]List<string> myDirections = new List<string>();
    List<string> allDirections = new List<string> { "North", "South", "East", "West"};
    PathFinder pathFinder;
    GeneratorTimer _timer;
    // Start is called before the first frame update
    void Start()
    {
        _timer = GameObject.Find("Timer").GetComponent<GeneratorTimer>();
        pathFinder = GameObject.Find("Generator").GetComponent<PathFinder>();
        DoorLocations = pathFinder.GetDirections()[this.transform.position];


        SetDirections();
    }
    private void Update()
    {
        Profiler.BeginSample("Room Rotation");
        myDirections = new List<string>();
        bool[] isAligned = new bool[DoorLocations.Count];
        SetDirections();
        PrepareIndex(isAligned);
        RotateRoom(isAligned);
        CheckAlignment(isAligned);
        Profiler.EndSample();
    }
    private void PrepareIndex(bool[] alignment)
    {
        foreach (Door door in doors)
        {
            myDirections.Add(door.GetDirection().ToString());
        }

        int index = 0;


        foreach (string direction in myDirections)
        {
            foreach (string check in DoorLocations)
            {

                if (check == direction)
                {
                    alignment[index] = true;
                    break;
                }
                else
                {
                    alignment[index] = false;
                }
            }
            index++;
        }
    }
    private void SetDirections()
    {
        for (int i = 0; i < doors.Length; i++)
        {
            Transform roomTrans, doorTrans;
            roomTrans = this.transform;
            doorTrans = doors[i].GetLocation();
            Vector3 Location = roomTrans.transform.position - doorTrans.transform.position;

            if (Location.x < 0 && Location.z <= 0.1f && Location.z >= -0.1f) doors[i].SetDirection(DoorDirection.East);
            if (Location.x > 0 && Location.z <= 0.1f && Location.z >= -0.1f) doors[i].SetDirection(DoorDirection.West);
            if (Location.z < 0 && Location.x <= 0.1f && Location.x >= -0.1f) doors[i].SetDirection(DoorDirection.North);
            if (Location.z > 0 && Location.x <= 0.1f && Location.x >= -0.1f) doors[i].SetDirection(DoorDirection.South);

        }
    }
    private void RotateRoom(bool[] alignment) 
    {
        foreach (bool test in alignment)
        {
            if (!test)
            {
                this.transform.Rotate(0, 90, 0);
                break;
            }
        }
    }
    private void CheckAlignment(bool[] alignment)
    {
        for (int i = 0; i < alignment.Length; i++)
        {

            if (!alignment[i])
            {
                break;
            }

            if (i == alignment.Length - 1)
            {
                this.enabled = false;
                _timer.DecreaseCount();
            }
        }
    }


}



