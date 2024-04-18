using UnityEngine;

public class Cell
{
    public int X;
    public int Y;
    public bool Covered;
    public bool Bomb;
    public bool Flag;
    public int BombsCount;
    public Vector2Int Position => new(X, Y);
}