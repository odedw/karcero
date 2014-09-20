using Karcero.Engine;
using Karcero.Engine.Models;
using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour
{

    private DungeonGenerator<Cell> mGenerator;

    // Use this for initialization
    void Start()
    {
        mGenerator = new DungeonGenerator<Cell>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mGenerator.GenerateA()
                      .MediumDungeon()
                      .ABitRandom()
                      .SomewhatSparse()
                      .WithMediumChanceToRemoveDeadEnds()
                      .WithMediumSizeRooms()
                      .WithLargeNumberOfRooms()
                      .AndTellMeWhenItsDone(map =>
               {
                   Debug.Log("Worked! room count - " + map.Rooms.Count);

               });
        }
    }

    void OnMouseDown()
    {

    }
}
