using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyLevel", menuName = "ScriptableObjects/DifficultyLevel", order = 1)]
public class DifficultyLevel : ScriptableObject
{
    public string Name;
    public Vector2Int Size;
    public int Mines;
    public int Width => Size.x;
    public int Height => Size.y;
}
