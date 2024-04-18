using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileController", menuName = "ScriptableObjects/TileController", order = 1)]
public class TileController : ScriptableObject
{
    public TileBase[] Numbers;
    public TileBase Open;
    public TileBase Covered;
    public TileBase Bomb;
    public TileBase Bomb_Highlight;
    public TileBase Flag;
    public TileBase Flag_Highlight;
    public TileBase Border;

    public enum TileType
    {
        One, Two, Three, Four, Five, Six, Seven, Eight,
        Open, Covered,
        Bomb, Bomb_Highlight,
        Flag, Flag_Highlight,
        Border,
        Empty,
    }

    public TileBase GetTile(TileType tileType) => tileType switch
    {
        TileType.One => Numbers[0],
        TileType.Two => Numbers[1],
        TileType.Three => Numbers[2],
        TileType.Four => Numbers[3],
        TileType.Five => Numbers[4],
        TileType.Six => Numbers[5],
        TileType.Seven => Numbers[6],
        TileType.Eight => Numbers[7],
        TileType.Bomb => Bomb,
        TileType.Bomb_Highlight => Bomb_Highlight,
        TileType.Flag => Flag,
        TileType.Flag_Highlight => Flag_Highlight,
        TileType.Open => Open,
        TileType.Covered => Covered,
        TileType.Border => Border,
        _ => null,
    };
}