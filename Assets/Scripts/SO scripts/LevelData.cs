using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BrickPositions
{
    public List<Vector3> positions = new List<Vector3>();
}

[CreateAssetMenu(fileName = "Level", menuName = "Levels/Level")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public int levelNumber;
    public List<GameObject> levelPrefabs = new List<GameObject>();
}