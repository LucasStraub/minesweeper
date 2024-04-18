using UnityEngine;
using UnityEngine.Tilemaps;
using Tile = TileController.TileType;

public class TilemapController : MonoBehaviour
{
    [SerializeField] private Tilemap _base;
    [SerializeField] private Tilemap _top;
    [SerializeField] private TileController _controller;

    public void SetBaseTile(Vector3Int position, Tile tileType)
    {
        if (_controller == null || _base == null)
            return;

        var tile = _controller.GetTile(tileType);
        _base.SetTile(position * 2, tile);
        _base.SetTile(position * 2 + new Vector3Int(1, 0, 0), tile);
        _base.SetTile(position * 2 + new Vector3Int(0, 1, 0), tile);
        _base.SetTile(position * 2 + new Vector3Int(1, 1, 0), tile);
    }

    public void SetTopTile(Vector3Int position, Tile tileType)
    {
        if (_controller == null || _top == null)
            return;

        var tile = _controller.GetTile(tileType);
        _top.SetTile(position, tile);
    }
}
