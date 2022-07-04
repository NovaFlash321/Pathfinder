using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class GeneratorTimer : MonoBehaviour
{
    int roomCount = 0;
    private float startTime, endTime, lastTime;
    List<float> times = new List<float>();

    [SerializeField] string textSave = "";
    public static string directory = "/GeneratorTimeData";
    public static string fileName = "/GeneratorData.csv";
    public class TimeClock 
    {
        public string name;
        public float time;
        public string date;
    }

    private void Start()
    {
        //times = new Dictionary<string, float>(); //Name will be unique key as each method is run once
    }

    public void Export(string _name, float _time)
    {
        TimeClock _clock = new TimeClock();
        _clock.name = _name;
        _clock.time = _time;
        _clock.date = System.DateTime.Now.ToString();
        //SaveToFile(_clock);
    }

    private void SaveToFile()
    {
        string dir = Application.dataPath + directory;
        if (!Directory.Exists(dir + fileName))
        {
            Debug.Log("File does not exist");
            TextWriter tw = new StreamWriter(dir + fileName, false);
            tw.WriteLine(", Main Path Generation, Branching Path Generation, Combine Rooms, Get Room Directions, Remove Duplicates, Spawn Rooms, Align Rooms");
            tw.Close();

        }

        TextWriter _tw = new StreamWriter(dir + fileName, true);

        string toFile = $"{System.DateTime.Now.ToString("dd-MM-yy HH-mm-ss")}, ";
        
        for(int i = 1; i < times.Count; i++)
        {
            toFile += (times[i] - times[i - 1]);
            if(i != times.Count - 1)
            {
                toFile += ", ";
            }
        }
        //Debug.Log(toFile);
        _tw.WriteLine(toFile);

        


        _tw.Close();
        Debug.Log($"Updated/Saved file at {dir}");

        //Debug.Log(dir);
        //TextWriter tw = new StreamWriter(dir + fileName, false);
        //if(!Directory.Exists(dir))
        //{
        //    Directory.CreateDirectory(dir);
        //    tw.WriteLine("Method Name, Time to Load, Date");

        //}
        //File.Open(dir + fileName, FileMode.Open);
        //tw = new StreamWriter(dir + fileName, true);
        //tw.WriteLine($"{_clock.name}, {_clock.time}, {_clock.date}");
        //tw.Close();

    }


    public void AddToDic(float _time)
    {
        times.Add(_time); //Init time will always be 0 relative
    }
                        
    public void GetEnd()
    {
        endTime = Time.realtimeSinceStartup;
        SaveToFile();
    }

   

    public void SetCount(int _count)
    {
        
        roomCount = _count;
    }

    public void DecreaseCount()
    {
        roomCount -= 1;
    }


    void Update()
    {
        if(roomCount == 0 && endTime == 0)
        {
            Debug.Log("Rooms are aligned");
            AddToDic(Time.realtimeSinceStartup);

            GetEnd();
        }
    }
}
