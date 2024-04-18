using UnityEngine;

public class Board
{
    private readonly Cell[,] _cells;
    private Vector2Int _size;
    public Vector2Int Size => _size;
    public int Width => _size.x;
    public int Height => _size.y;

    private int _mines;
    public int Mines => _mines;

    public Board(Vector2Int size, int mines)
    {
        _size = size;
        _cells = new Cell[size.x, size.y];
        _mines = mines;
    }

    public Cell this[int i, int j]
    {
        get { return _cells[i, j]; }
        set { _cells[i, j] = value; }
    }
}
