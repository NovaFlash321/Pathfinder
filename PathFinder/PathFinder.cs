using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Profiling;
public class PathFinder : MonoBehaviour
{
    //Author: Nova Flash
    //Stable Build: 2 March 2022
    //Branching Rooms Added: 10 March 2022

    public GameObject debugCube, debugCube2;
    private GeneratorTimer _timer;

    public GameObject Cross, Tee, Elbow, Path, DeadEnd;



    [SerializeField, Range(2, 10)] int RoomsOnPath; //How many rooms until the exit is reached. Branching rooms can occur from tees and crosses. Spawn is not counted
    [SerializeField] int roomScale;
    [SerializeField, Range(1, 10)] int maxBranchingPathRooms;

    private Dictionary<Vector3, List<string>> RoomDirections = new Dictionary<Vector3, List<string>>(); //Used for the room prefabs
    private Dictionary<int, List<Vector3>> BranchRoomIndex = new Dictionary<int, List<Vector3>>(); //a check if a branching room is in another branching room's path line

    List<string> Directions;

    List<Vector3> allLocaions = new List<Vector3>();
    List<Vector3> allBranchLocations = new List<Vector3>();
    List<string> RoomPlacements = new List<string>();

    List<Vector3> RoomLocations = new List<Vector3>();

    List<List<Vector3>> BranchRoomLocations = new List<List<Vector3>>();

    public Dictionary<Vector3, List<string>> GetDirections()
    {
        return RoomDirections;
    }

    private void Start()
    {
        _timer = GameObject.Find("Timer").GetComponent<GeneratorTimer>();
        _timer.AddToDic(Time.realtimeSinceStartup);
        GenerateMainPath();
        GenerateBranchingPaths();

        foreach (List<Vector3> _list in BranchRoomLocations)
        {

            for (int i = 1; i < _list.Count; i++)
            {

                Debug.DrawLine(_list[i], _list[i - 1], Color.green, 10f);
            }
            //_list.Remove(_list[0]);
        }
        CombineRooms();

        //GenerateAllRooms(); //for debug
        GetRoomDirections();
        RemoveDuplicates();
        SpawnRooms();

        int allRooms = GameObject.FindGameObjectsWithTag("Room").Length;
        _timer.SetCount(allRooms);

    }
    private void RemoveDuplicates()
    {
        foreach(KeyValuePair<Vector3, List<string>> Rooms in RoomDirections)
        {
            Rooms.Value.Sort();
            foreach(string test in Rooms.Value)
            {
                //Debug.Log($"[{Rooms.Key}]: {test}");
            }    
        }
        _timer.AddToDic(Time.realtimeSinceStartup);
    }
    
    private void CombineRooms() //ONLY TO BE USED IN A CHECK
    {
        foreach(Vector3 _location in RoomLocations) 
        {
            allLocaions.Add(_location);
            
        }



        for(int i = 0; i < BranchRoomLocations.Count; i++)
        {
            List<Vector3> ph = new List<Vector3>();
            for(int j = 0; j < BranchRoomLocations[i].Count; j++)
            {
                allLocaions.Add(BranchRoomLocations[i][j]);
                ph.Add(BranchRoomLocations[i][j]);
                allBranchLocations.Add(BranchRoomLocations[i][j]);
            }
            BranchRoomIndex.Add(i, ph);
        }

        foreach(KeyValuePair<int, List<Vector3>> _branch in BranchRoomIndex)
        {
            foreach(Vector3 location in _branch.Value)
            {
                //Debug.Log($"Index: {_branch.Key}; Location: {location}");
            }
        }

        _timer.AddToDic( Time.realtimeSinceStartup);
    }

    private void GetRoomDirections() 
    {
        int index = 0;
        foreach (Vector3 _location in RoomLocations) 
        {

            foreach(KeyValuePair<int, List<Vector3>> _origin in  BranchRoomIndex)
            {

                if (_location == _origin.Value[0])
                {
                    //Debug.Log("I am the branch origin");
                    GetBranchRoomDirections(_origin.Key);

                }
                


            }
            GetDoorDirections(_location, index);
            index++;





        }

        foreach(KeyValuePair<Vector3, List<string>>  test in RoomDirections)
        {
            foreach(string _location in test.Value)
            {
                //Debug.Log($"[{test.Key}]: {_location}");
            }
        }

        _timer.AddToDic(Time.realtimeSinceStartup);
    }

    private void GetDoorDirections(Vector3 location, int index)
    {
        List<string> roomDirections = new List<string>();
        Vector3 ph = location;
       
        ph = location + new Vector3(0, 0, roomScale);
        if(location == RoomLocations[0])
        {
            if (ph == RoomLocations[index + 1])
            {
                roomDirections.Add("North");
            }

            ph = location + new Vector3(0, 0, -roomScale);
            if (ph == RoomLocations[index + 1])
            {
                roomDirections.Add("South");
            }

            ph = location + new Vector3(roomScale, 0, 0);
            if (ph == RoomLocations[index + 1])
            {
                roomDirections.Add("East");
            }

            ph = location + new Vector3(-roomScale, 0, 0);
            if (ph == RoomLocations[index + 1])
            {
                roomDirections.Add("West");
            }

            if (RoomDirections.ContainsKey(location))
            {
                foreach (string direction in roomDirections)
                {
                    if (!RoomDirections[location].Contains(direction))
                    {
                        RoomDirections[location].Add(direction);
                    }
                }
            }
            else
            {
                RoomDirections.Add(location, roomDirections);
            }
        }
        else if(location == RoomLocations[RoomLocations.Count - 1])
        {
            if (ph == RoomLocations[index - 1])
            {
                roomDirections.Add("North");
            }

            ph = location + new Vector3(0, 0, -roomScale);
            if (ph == RoomLocations[index - 1])
            {
                roomDirections.Add("South");
            }

            ph = location + new Vector3(roomScale, 0, 0);
            if (ph == RoomLocations[index - 1])
            {
                roomDirections.Add("East");
            }

            ph = location + new Vector3(-roomScale, 0, 0);
            if (ph == RoomLocations[index - 1])
            {
                roomDirections.Add("West");
            }

            if (RoomDirections.ContainsKey(location))
            {
                foreach (string direction in roomDirections)
                {
                    if (!RoomDirections[location].Contains(direction))
                    {
                        RoomDirections[location].Add(direction);
                    }
                }
            }
            else
            {
                RoomDirections.Add(location, roomDirections);
            }
        }
        else
        {
            if (ph == RoomLocations[index + 1] || ph == RoomLocations[index - 1])
            {
                roomDirections.Add("North");
            }

            ph = location + new Vector3(0, 0, -roomScale);
            if (ph == RoomLocations[index + 1] || ph == RoomLocations[index - 1])
            {
                roomDirections.Add("South");
            }

            ph = location + new Vector3(roomScale, 0, 0);
            if (ph == RoomLocations[index + 1] || ph == RoomLocations[index - 1])
            {
                roomDirections.Add("East");
            }

            ph = location + new Vector3(-roomScale, 0, 0);
            if (ph == RoomLocations[index + 1] || ph == RoomLocations[index - 1])
            {
                roomDirections.Add("West");
            }

            if (RoomDirections.ContainsKey(location))
            {
                foreach (string direction in roomDirections)
                {
                    if (!RoomDirections[location].Contains(direction))
                    {
                        RoomDirections[location].Add(direction);
                    }
                }
            }
            else
            {
                RoomDirections.Add(location, roomDirections);
            }
        }



        


    }

    private void SpawnRooms()
    {
        foreach(KeyValuePair<Vector3, List<string>> RoomDoorPair in RoomDirections)
        {
            if(RoomDoorPair.Value.Count == 1)//deadend
            {
                Instantiate(DeadEnd, RoomDoorPair.Key, Quaternion.identity);
            }
            if(RoomDoorPair.Value.Count == 2) //path or elbow
            {
                if(RoomDoorPair.Value.Contains("North") && RoomDoorPair.Value.Contains("South") || RoomDoorPair.Value.Contains("East") && RoomDoorPair.Value.Contains("West"))
                {
                    Instantiate(Path, RoomDoorPair.Key, Quaternion.identity);
                }
                else
                {
                    Instantiate(Elbow, RoomDoorPair.Key, Quaternion.identity);
                }
            }
            if(RoomDoorPair.Value.Count == 3) //tee
            {
                Instantiate(Tee, RoomDoorPair.Key, Quaternion.identity);
            }
            if(RoomDoorPair.Value.Count == 4) //cross
            {
                Instantiate(Cross, RoomDoorPair.Key, Quaternion.identity);
            }
        }

        _timer.AddToDic(Time.realtimeSinceStartup);
    }




    private void GetBranchRoomDirections(int branchKey)
    {
        foreach(Vector3 _location in BranchRoomIndex[branchKey])
        {
            List<string> roomDirections = new List<string>();
            Vector3 ph = _location;
            //going on the X and Z axis on a cartesian plane

            //first check if direction is on main path
            ph = _location + new Vector3(0, 0, roomScale);
            
            if(ph == BranchRoomIndex[branchKey][0]) 
            {
                roomDirections.Add("North");
            }
            else
            {
                foreach (Vector3 testLocation in allLocaions)
                {
                    if (ph == testLocation)
                    {
                        if (BranchRoomIndex[branchKey].Contains(testLocation))
                        {
                            roomDirections.Add("North");
                            break;
                        }
                    }


                }
            }
            

            ph = _location + new Vector3(0, 0, -roomScale);
            if (ph == BranchRoomIndex[branchKey][0])
            {
                roomDirections.Add("South");
            }
            else
            {
                foreach (Vector3 testLocation in allLocaions)
                {
                    if (ph == testLocation)
                    {
                        if (BranchRoomIndex[branchKey].Contains(testLocation))
                        {
                            roomDirections.Add("South");
                            break;
                        }
                    }


                }
            }
            

            ph = _location + new Vector3(roomScale, 0, 0);
            if (ph == BranchRoomIndex[branchKey][0])
            {
                roomDirections.Add("East");
            }
            else
            {
                foreach (Vector3 testLocation in allLocaions)
                {
                    if (ph == testLocation)
                    {
                        if (BranchRoomIndex[branchKey].Contains(testLocation))
                        {
                            roomDirections.Add("East");
                            break;
                        }
                    }


                }
            }
            

            ph = _location + new Vector3(-roomScale, 0, 0);
            if (ph == BranchRoomIndex[branchKey][0])
            {
                roomDirections.Add("West");
            }
            else
            {
                foreach (Vector3 testLocation in allLocaions)
                {
                    if (ph == testLocation)
                    {
                        if (BranchRoomIndex[branchKey].Contains(testLocation))
                        {
                            roomDirections.Add("West");
                            break;
                        }
                    }


                }
            }
            

            //second check if direction is on branch path
            RoomDirections.Add(_location, roomDirections);

            
        }
    }

    private void GenerateAllRooms()
    {
        foreach (Vector3 _location in RoomLocations)
        {
            Instantiate(debugCube, _location, Quaternion.identity);
        }

        foreach (List<Vector3> _list in BranchRoomLocations)
        {
            int index = 0;
            foreach (Vector3 _location in _list)
            {
                if (index != 0) 
                { 
                    Instantiate(debugCube2, _location, Quaternion.identity);
                    
                }
                index++;
            }
        }

    }

    private void CheckNorth(Vector3 placeholder)
    {
        for (int n = 0; n < RoomLocations.Count; n++)
        {
            if (placeholder == RoomLocations[n])
            {
                break;
            }
            else if (n == (RoomLocations.Count - 1))
            {
                Directions.Add("North");
            }
        }
    }

    private void CheckSouth(Vector3 placeholder)
    {
        for (int n = 0; n < RoomLocations.Count; n++)
        {

            if (placeholder == RoomLocations[n])
            {
                break;
            }
            else if (n == (RoomLocations.Count - 1))
            {
                Directions.Add("South");
            }
        }
    }

    private void CheckEast(Vector3 placeholder)
    {
        for (int n = 0; n < RoomLocations.Count; n++)
        {

            if (placeholder == RoomLocations[n])
            {
                break;
            }
            else if (n == (RoomLocations.Count - 1))
            {
                Directions.Add("East");
            }
        }
    }

    private void CheckWest(Vector3 placeholder)
    {
        for (int n = 0; n < RoomLocations.Count; n++)
        {

            if (placeholder == RoomLocations[n])
            {
                break;
            }
            else if (n == (RoomLocations.Count - 1))
            {
                Directions.Add("West");
            }
        }
    }

 



    private void GenerateMainPath()
    {



        RoomLocations.Add(this.transform.position);
        

        for (int i = 1; i < RoomsOnPath; i++)
        {
            RoomPlacements = new List<string>();

            if(i != 1) 
            {
                
            }


            Directions = new List<string>(); //Wipes list before starting for each room
            Vector3 placeholder = Vector3.zero;
            RoomLocations.Add(Vector3.zero);

            CheckNorth(RoomLocations[i - 1] + new Vector3(0, 0, roomScale));

            CheckSouth(RoomLocations[i - 1] + new Vector3(0, 0, -roomScale));

            CheckEast(RoomLocations[i - 1] + new Vector3(roomScale, 0, 0));

            CheckWest(RoomLocations[i - 1] + new Vector3(-roomScale, 0, 0));

           
            if(Directions.Count > 0)
            {

                switch (Directions[Random.Range(0, Directions.Count)])
                {
                    case "North":
                        RoomLocations[i] = RoomLocations[i - 1] + new Vector3(0, 0, roomScale);

                        break;
                    case "South":
                        RoomLocations[i] = RoomLocations[i - 1] + new Vector3(0, 0, -roomScale);

                        break;
                    case "East":
                        RoomLocations[i] = RoomLocations[i - 1] + new Vector3(roomScale, 0, 0);

                        break;
                    case "West":
                        RoomLocations[i] = RoomLocations[i - 1] + new Vector3(-roomScale, 0, 0);

                        break;

                };
            }
            else
            {
                Debug.LogError("Cannot move!");
                RoomLocations.Remove(RoomLocations[RoomLocations.Count - 1]);
                return;
            }


        }



        _timer.AddToDic( Time.realtimeSinceStartup);
        //NOT TO BE USED IN ACTUAL BUILD
        #region DEBUG
        //foreach (Vector3 _location in RoomLocations)
        //{

        //    Instantiate(debugCube, _location, Quaternion.identity);

        //}

        for (int i = 0; i < RoomLocations.Count - 1; i++)
        {

            Debug.DrawLine(RoomLocations[i], RoomLocations[i + 1], Color.red, 10f);

        }
        #endregion
    }

    private void GenerateBranchingPaths()
    {
        
        for (int i = 1; i < RoomLocations.Count - 1; i++) //Skips Spawn room and Exit room
        {
            //Debug.Log("In Index: " + i);
            //if (Random.Range(0, 100) <= 23 || Random.Range(0, 100) >= 91)
            if (0 < 1)
            {
                //BranchRoomLocations.Add(new List<Vector3>()) - Adds a new list of Vector3
                //BranchRoomLocations[index].Add(Vector3.zero) - Adds a new vector3
                #region Varaibles to Pass Through Function
                int Index = BranchRoomLocations.Count - 1;

                #endregion


                BranchRoomLocations.Add(new List<Vector3>());
                BranchRoomLocations[BranchRoomLocations.Count - 1].Add(RoomLocations[i]); //Make room on path new origin to list


                GenerateNewBranchingPath();
            }
        }


        _timer.AddToDic(Time.realtimeSinceStartup);
    }

    void GenerateNewBranchingPath()
    {
        //for (int b = 1; b < Random.Range(1,maxBranchingPathRooms) + 1; b++)
        for (int b = 1; b < maxBranchingPathRooms + 1; b++)
        {
            Directions = new List<string>();
            BranchRoomLocations[BranchRoomLocations.Count - 1].Add(Vector3.zero);
            Vector3 placeholder = Vector3.zero;


            #region CheckNorth
            placeholder = BranchRoomLocations[BranchRoomLocations.Count - 1][b - 1] + new Vector3(0, 0, roomScale);
            for (int n = 0; n < RoomLocations.Count; n++)
            {
                if (placeholder == RoomLocations[n])
                {
                 //   Debug.LogError("Overlap at " + placeholder + "!");

                    goto CheckSouth;
                }
                else if (n == RoomLocations.Count - 1)
                {
                    for (int l = 0; l < BranchRoomLocations.Count; l++)
                    {
                        for (int bn = 0; bn < BranchRoomLocations[l].Count; bn++)
                        {
                            if (placeholder == BranchRoomLocations[l][bn])
                            {
                              //  Debug.LogError("Overlap at " + placeholder + "!");

                                goto CheckSouth;
                            }
                            else if (l == BranchRoomLocations.Count - 1 && bn == BranchRoomLocations[l].Count - 1)
                            {
                                Directions.Add("North");
                            }
                        }
                    }


                }
            }
            #endregion


            #region CheckSouth
            CheckSouth:
            placeholder = BranchRoomLocations[BranchRoomLocations.Count - 1][b - 1] + new Vector3(0, 0, -roomScale);
            for (int n = 0; n < RoomLocations.Count; n++)
            {
                if (placeholder == RoomLocations[n])
                {
                  //  Debug.LogError("Overlap at " + placeholder + "!");

                    goto CheckEast;
                }
                else if (n == RoomLocations.Count - 1)
                {
                    for (int l = 0; l < BranchRoomLocations.Count; l++)
                    {
                        for (int bn = 0; bn < BranchRoomLocations[l].Count; bn++)
                        {
                            if (placeholder == BranchRoomLocations[l][bn])
                            {
                               // Debug.LogError("Overlap at " + placeholder + "!");

                                goto CheckEast;
                            }
                            else if (l == BranchRoomLocations.Count - 1 && bn == BranchRoomLocations[l].Count - 1)
                            {
                                Directions.Add("South");
                            }
                        }
                    }


                }
            }
        #endregion


        #region CheckEast
        CheckEast:
            placeholder = BranchRoomLocations[BranchRoomLocations.Count - 1][b - 1] + new Vector3(roomScale, 0, 0);
            for (int n = 0; n < RoomLocations.Count; n++)
            {
                if (placeholder == RoomLocations[n])
                {
                    //Debug.LogError("Overlap at " + placeholder + "!");

                    goto CheckWest;
                }
                else if (n == RoomLocations.Count - 1)
                {
                    for (int l = 0; l < BranchRoomLocations.Count; l++)
                    {
                        for (int bn = 0; bn < BranchRoomLocations[l].Count; bn++)
                        {
                            if (placeholder == BranchRoomLocations[l][bn])
                            {
                                //Debug.LogError("Overlap at " + placeholder + "!");

                                goto CheckWest;
                            }
                            else if (l == BranchRoomLocations.Count - 1 && bn == BranchRoomLocations[l].Count - 1)
                            {
                                Directions.Add("East");
                            }
                        }
                    }


                }
            }
            #endregion


            #region CheckWest
            CheckWest:
            placeholder = BranchRoomLocations[BranchRoomLocations.Count - 1][b - 1] + new Vector3(-roomScale, 0, 0);
            for (int n = 0; n < RoomLocations.Count; n++)
            {
                if (placeholder == RoomLocations[n])
                {
                    //Debug.LogError("Overlap at " + placeholder + "!");

                    goto MakeDirection;
                }
                else if (n == RoomLocations.Count - 1)
                {
                    for (int l = 0; l < BranchRoomLocations.Count; l++)
                    {
                        for (int bn = 0; bn < BranchRoomLocations[l].Count; bn++)
                        {
                            if (placeholder == BranchRoomLocations[l][bn])
                            {
                                //Debug.LogError("Overlap at " + placeholder + "!");

                                goto MakeDirection;
                            }
                            else if (l == BranchRoomLocations.Count - 1 && bn == BranchRoomLocations[l].Count - 1)
                            {
                                Directions.Add("West");
                            }
                        }
                    }


                }
            }
            #endregion





        MakeDirection:
            //Debug.Log(Directions.Count);
            if (Directions.Count != 0)
            {

                switch (Directions[Random.Range(0, Directions.Count)])
                {
                    case "North":
                        BranchRoomLocations[BranchRoomLocations.Count - 1][b] = BranchRoomLocations[BranchRoomLocations.Count - 1][b - 1] + new Vector3(0, 0, roomScale);
                        break;
                    case "South":
                        BranchRoomLocations[BranchRoomLocations.Count - 1][b] = BranchRoomLocations[BranchRoomLocations.Count - 1][b - 1] + new Vector3(0, 0, -roomScale);
                        break;
                    case "East":
                        BranchRoomLocations[BranchRoomLocations.Count - 1][b] = BranchRoomLocations[BranchRoomLocations.Count - 1][b - 1] + new Vector3(roomScale, 0, 0);
                        break;
                    case "West":
                        BranchRoomLocations[BranchRoomLocations.Count - 1][b] = BranchRoomLocations[BranchRoomLocations.Count - 1][b - 1] + new Vector3(-roomScale, 0, 0);
                        break;
                    

                };

            }
            else
            {
                //Debug.LogError("No direction could be made. Removing last location");
                BranchRoomLocations[BranchRoomLocations.Count - 1].Remove(BranchRoomLocations[BranchRoomLocations.Count - 1][BranchRoomLocations[BranchRoomLocations.Count - 1].Count - 1]);
                break;
            }


            
            
        }
                //BranchRoomLocations[BranchRoomLocations.Count - 1].Remove(BranchRoomLocations[BranchRoomLocations.Count - 1][0]);
    }


    void CheckBranchNorth(Vector3 placeholder)
    {







    }


    private void Update()
    {




    }



}





