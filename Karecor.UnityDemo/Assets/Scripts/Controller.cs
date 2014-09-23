using System.Collections.Generic;
using Karcero.Engine;
using Karcero.Engine.Models;
using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour
{
    private DungeonGenerator<Cell> mGenerator;
    private Vector3 mScaleVector;
    public GameObject DoorV;
    public GameObject DoorH;
    public GameObject E;
    public GameObject EW;
    public GameObject N;
    public GameObject NE;
    public GameObject NES;
    public GameObject NEW;
    public GameObject NEWS;
    public GameObject NS;
    public GameObject NSW;
    public GameObject NW;
    public GameObject OPEN;
    public GameObject S;
    public GameObject SE;
    public GameObject SEW;
    public GameObject SW;
    public GameObject W;
    private const short NORTH = 1;
    private const short EAST = 2;
    private const short SOUTH = 4;
    private const short WEST = 8;
    private const float SPRITE_SIZE = 0.16f;
    private Dictionary<short, GameObject> mPrefabByWalls = new Dictionary<short, GameObject>();
    private List<GameObject> mLiveObjects = new List<GameObject>();
    private float mCamHalfHeight;
    private float mCamHalfWidth;
    private Vector3 mTopLeftPosition;
    private float mMargin = 0.1f;
    // Use this for initialization
    void Start()
    {
        mGenerator = new DungeonGenerator<Cell>();
        mPrefabByWalls[EAST] = E;
        mPrefabByWalls[EAST | WEST] = EW;
        mPrefabByWalls[NORTH] = N;
        mPrefabByWalls[NORTH | EAST] = NE;
        mPrefabByWalls[NORTH | EAST | WEST] = NEW;
        mPrefabByWalls[NORTH | EAST | SOUTH] = NES;
        mPrefabByWalls[NORTH | EAST | WEST | SOUTH] = NEWS;
        mPrefabByWalls[NORTH | SOUTH] = NS;
        mPrefabByWalls[NORTH | SOUTH | WEST] = NSW;
        mPrefabByWalls[NORTH | WEST] = NW;
        mPrefabByWalls[0] = OPEN;
        mPrefabByWalls[SOUTH] = S;
        mPrefabByWalls[SOUTH | EAST] = SE;
        mPrefabByWalls[SOUTH | EAST | WEST] = SEW;
        mPrefabByWalls[SOUTH | WEST] = SW;
        mPrefabByWalls[WEST] = W;

        mCamHalfHeight = Camera.main.orthographicSize;
        mCamHalfWidth = Camera.main.aspect * mCamHalfHeight;
        mTopLeftPosition = new Vector3(-mCamHalfWidth + mMargin, mCamHalfHeight + mMargin, 0) + Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            foreach (var obj in mLiveObjects)
            {
                DestroyObject(obj);
            }
            var map = mGenerator.GenerateA()
                .MediumDungeon()
                .ABitRandom()
                .SomewhatSparse()
                .WithMediumChanceToRemoveDeadEnds()
                .WithMediumSizeRooms()
                .WithLargeNumberOfRooms()
                .Now();
                      //.AndTellMeWhenItsDone(map =>
               //{
                   //Debug.Log("Worked! room count - " + map.Rooms.Count);

               //});

            var scaleW = (mCamHalfWidth * 2 - mMargin*2) / (SPRITE_SIZE * map.Width);
            var scaleH = (mCamHalfHeight*2 - mMargin*2) /(SPRITE_SIZE * map.Height);
            mScaleVector = new Vector3(scaleW, scaleH);
            foreach (var cell in map.AllCells)
            {
                var prefab = GetPrefab(cell, map);
                if (prefab == null) continue;

                prefab.transform.position = new Vector3(mTopLeftPosition.x + SPRITE_SIZE * scaleW * cell.Column,
                                                        mCamHalfHeight * 2 - (mTopLeftPosition.y + SPRITE_SIZE * scaleH * cell.Row),
                                                        cell.Terrain == TerrainType.Door ? 1 : 0);
                
                prefab.transform.localScale = mScaleVector;
                mLiveObjects.Add(prefab);
            }
        }
    }

    private GameObject GetPrefab(Cell cell, Map<Cell> map)
    {
        GameObject obj = null;
        if (cell.Terrain == TerrainType.Door)
        {
            if (cell.Row > 0 && map.GetAdjacentCell(cell, Direction.North).Terrain == TerrainType.Floor)
                obj = Instantiate(DoorV) as GameObject;
            else
                obj = Instantiate(DoorH) as GameObject;
        }
        else if (cell.Terrain == TerrainType.Floor)
        {
            short sum = 0;
            if (IsWall(map,Direction.North, cell)) sum += NORTH;
            if (IsWall(map, Direction.South, cell)) sum += SOUTH;
            if (IsWall(map, Direction.West, cell)) sum += WEST;
            if (IsWall(map, Direction.East, cell)) sum += EAST;
            if (mPrefabByWalls.ContainsKey(sum))
                obj = Instantiate(mPrefabByWalls[sum]) as GameObject;
        }
        
        return obj;
    }

    private bool IsWall(Map<Cell> map, Direction direction, Cell cell)
    {
        Cell adjacentCell;
        return !map.TryGetAdjacentCell(cell, direction, out adjacentCell) || adjacentCell.Terrain == TerrainType.Rock;
    }
}
