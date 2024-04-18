using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Tile = TileController.TileType;

public class GameManager : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] private UIController _uiController;
    [SerializeField] private TilemapController _tilemapController;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private ParticleController _particleController;
    [SerializeField] private AudioController _audioController;

    [Header("Settings")]
    [SerializeField] private DifficultyLevel[] _difficultyLevels;
    private enum GameState
    {
        Prepared,
        Running,
        Ended,
    }
    private GameState _currentState;

    private Board _board;
    private Vector3Int _boardOffset;
    private DifficultyLevel _currentDifficultyLevel;

    private readonly List<Cell> _holdingCells = new();

    private int _flags;
    private float _timer;
    private int _minesFound;

    private float _lastClick;

    private void OnEnable()
    {
        UIController.OnButtonClick += PrepareGame;
        UIController.OnDropDownValueChanged += SetDifficulty;

        InputController.OnRightMouseButtonDown += RightMouseButtonDown;
        InputController.OnLeftMouseButton += LeftMouseButton;
        InputController.OnLeftMouseButtonUp += LeftMouseButtonUp;
    }



    private void OnDisable()
    {
        UIController.OnButtonClick -= PrepareGame;
        UIController.OnDropDownValueChanged -= SetDifficulty;

        InputController.OnRightMouseButtonDown -= RightMouseButtonDown;
        InputController.OnLeftMouseButton -= LeftMouseButton;
        InputController.OnLeftMouseButtonUp -= LeftMouseButtonUp;
    }

    private void OnDrawGizmos()
    {
        if (_board == null)
            return;

        Gizmos.color = Color.green;
        var size = new Vector3(_board.Width, _board.Height);
        Gizmos.DrawWireCube(Vector3.zero, size);

        Gizmos.color = Color.red;
        for (int x = 0; x < _board.Width; x++)
        {
            for (int y = 0; y < _board.Height; y++)
            {
                var cell = _board[x, y];
                if (cell.Bomb)
                {
                    var position = BoardToWorldPosition(cell.Position) + new Vector3(0.5f, 0.5f, 0);
                    Gizmos.DrawWireSphere(position, 0.5f);
                }
            }
        }
    }

    private void Awake()
    {
        if (_uiController != null)
            _uiController.SetDropdownOptions(_difficultyLevels.Select(o => o.Name).ToList());
    }

    private void LeftMouseButtonUp(Vector3 worldMousePosition)
    {
        if (ValidateCellAndGameState(worldMousePosition, out var cell))
        {
            ClearHold();
            Reveal(cell);
        }
    }

    private void LeftMouseButton(Vector3 worldMousePosition)
    {
        if (ValidateCellAndGameState(worldMousePosition, out var cell))
        {
            ClearHold();
            Hold(cell);
        }
    }

    private void RightMouseButtonDown(Vector3 worldMousePosition)
    {
        if (ValidateCellAndGameState(worldMousePosition, out var cell))
        {
            Flag(cell);
        }
    }

    private bool ValidateCellAndGameState(Vector3 worldMousePosition, out Cell cell)
    {
        if (Time.timeSinceLevelLoad - _lastClick < 0.01f)
        {
            cell = null;
            return false;
        }

        if (TryGetCell(worldMousePosition, out cell))
        {
            if (_currentState == GameState.Prepared)
            {
                StartGame();
            }
            if (_currentState == GameState.Running)
            {
                return true;
            }
        }
        return false;
    }

    private void Start()
    {
        SetDifficulty(0);
    }

    private void SetDifficulty(int id)
    {
        if (id >= 0 && _difficultyLevels.Length > id)
        {
            _currentDifficultyLevel = _difficultyLevels[id];
            PrepareGame();
        }
    }

    private void PrepareGame()
    {
        _lastClick = Time.timeSinceLevelLoad;
        SpawnAudio(Tile.Border);

        var _size = _currentDifficultyLevel.Size;
        var _mines = _currentDifficultyLevel.Mines;

        if (_mines < 0 || _mines > _size.x * _size.y)
            return;

        StopAllCoroutines();

        // Clean last board
        if (_board != null)
        {
            for (int x = -1; x <= _board.Width; x++)
            {
                for (int y = -1; y <= _board.Height; y++)
                {
                    var position = new Vector2Int(x, y);
                    SetBaseTile(position, Tile.Covered);
                    SetTopTile(position, Tile.Empty);
                }
            }
        }

        _board = new Board(_size, _mines);
        _boardOffset = new Vector3Int((-_size.x) / 2, (-_size.y) / 2);

        if (_uiController != null)
        {
            _uiController.SetButtonSprite(0);
            _uiController.SetCanvasSize(_size);
        }

        SetTimer(0);

        _minesFound = _size.x * _size.y - _mines;
        SetFlags(_mines);

        for (int x = -1; x <= _board.Width; x++)
        {
            for (int y = -1; y <= _board.Height; y++)
            {
                var position = new Vector2Int(x, y);
                if (x == -1 || y == -1 || x == _size.x || y == _size.y) // border
                {
                    SetTopTile(position, Tile.Border);
                }
                else
                {
                    _board[x, y] = new Cell
                    {
                        X = x,
                        Y = y,
                        Covered = true,
                    };
                }
            }
        }

        _currentState = GameState.Prepared;
    }

    public void StartGame()
    {
        // Randomize Mines
        for (int i = 0; i < _board.Mines;)
        {
            var x = Random.Range(0, _board.Size.x);
            var y = Random.Range(0, _board.Size.y);

            var cell = _board[x, y];
            if (!cell.Bomb)
            {
                cell.Bomb = true;
                i++;
            }
        }

        _currentState = GameState.Running;
    }

    public void EndGame()
    {
        _currentState = GameState.Ended;
    }

    public void Win()
    {
        _uiController.SetButtonSprite(2);

        EndGame();
        StartCoroutine(WinCo());
        IEnumerator WinCo()
        {
            //Set all covered cells as flags
            for (int x = 0; x < _board.Size.x; x++)
            {
                for (int y = 0; y < _board.Size.y; y++)
                {
                    var cell = _board[x, y];
                    if (cell.Covered && !cell.Flag)
                    {
                        Flag(cell);
                        yield return new WaitForSeconds(0.01f);
                    }
                }
            }
        }
    }

    public void Lose()
    {
        _uiController.SetButtonSprite(1);

        EndGame();
        StartCoroutine(LoseCo());
        IEnumerator LoseCo()
        {
            // Show all bombs and highlights all wrong flags
            for (int x = 0; x < _board.Size.x; x++)
            {
                for (int y = 0; y < _board.Size.y; y++)
                {
                    var c = _board[x, y];
                    if (c.Covered && c.Bomb && !c.Flag)
                    {
                        SetBaseTile(c.Position, Tile.Open);
                        SetTopTile(c.Position, Tile.Bomb);
                        SpawnParticle(c.Position, Tile.Covered);
                        SpawnAudio(Tile.Bomb);
                        _cameraController.Shake();

                        yield return new WaitForSeconds(0.01f);
                    }
                    if (c.Flag && !c.Bomb)
                    {
                        SetTopTile(c.Position, Tile.Flag_Highlight);
                        yield return new WaitForSeconds(0.01f);
                    }
                }
            }
        }

    }

    public void Update()
    {
        if (_currentState == GameState.Running)
        {
            SetTimer(_timer += Time.deltaTime);
        }
    }

    public void ClearHold()
    {
        // Clear last holdingCells
        foreach (var cellAround in _holdingCells)
        {
            if (cellAround.Covered)
            {
                SetBaseTile(cellAround.Position, Tile.Covered);
            }
        }
        _holdingCells.Clear();
    }

    public void Hold(Cell cell)
    {
        _holdingCells.Clear();
        if (cell.Covered)
        {
            if (!cell.Flag)
            {
                SetBaseTile(cell.Position, Tile.Open);
                _holdingCells.Add(cell);
            }
        }
        else
        {
            var cellsAround = GetValidCellsAround(cell);
            foreach (var cellAround in cellsAround)
            {
                if (cellAround.Covered && !cellAround.Flag)
                {
                    SetBaseTile(cellAround.Position, Tile.Open);
                    _holdingCells.Add(cellAround);
                }
            }
        }
    }

    public void Flag(Cell cell)
    {
        if (cell.Flag) // Remove Flag
        {
            SetTopTile(cell.Position, Tile.Empty);
            cell.Flag = false;
            SetFlags(_flags + 1);
            SpawnParticle(cell.Position, Tile.Flag);
            ShakeCamera();
            SpawnAudio(Tile.Flag);
            return;
        }

        if (cell.Covered) // Add Flag
        {
            SetTopTile(cell.Position, Tile.Flag);
            SetFlags(_flags - 1);
            ShakeCamera();
            cell.Flag = true;
            SpawnAudio(Tile.Flag);
        }
    }

    public void Reveal(Cell cell)
    {
        if (cell.Flag) // Flag
        {
            return;
        }

        if (cell.Bomb) // Bomb
        {
            SetBaseTile(cell.Position, Tile.Open);
            SetTopTile(cell.Position, Tile.Bomb_Highlight);
            cell.Covered = false;
            SpawnAudio(Tile.Bomb);

            Lose();
            return;
        }

        if (cell.Covered) // Closed
        {
            var positionsAround = GetValidCellsAround(cell);
            var bombsCount = positionsAround.Count(o => o.Bomb);

            SetBaseTile(cell.Position, Tile.Open);
            if (bombsCount > 0)
                SetTopTile(cell.Position, (Tile)(bombsCount - 1));
            cell.BombsCount = bombsCount;
            cell.Covered = false;
            ShakeCamera();
            SpawnParticle(cell.Position, Tile.Covered);
            SpawnAudio(Tile.Open);

            _minesFound--;
            if (_minesFound <= 0)
            {
                Win();
            }

            if (bombsCount == 0)
            {
                var closedAround = positionsAround.Where(o => o.Covered);

                StartCoroutine(Reveal(closedAround));
            }
            return;
        }

        if (cell.BombsCount > 0) // reveal around if number is same as flags around
        {
            var positionsAround = GetValidCellsAround(cell);
            var flagsCount = positionsAround.Count(o => o.Flag);

            if (flagsCount == cell.BombsCount)
            {
                var closedAround = positionsAround.Where(o => o.Covered);
                foreach (var o in closedAround)
                {
                    Reveal(o);
                }
            }
        }
    }

    private IEnumerator Reveal(IEnumerable<Cell> cells)
    {
        foreach (var o in cells)
        {
            yield return new WaitForSeconds(0.01f);
            Reveal(o);
        }
    }

    private bool TryGetCell(Vector2Int position, out Cell cell)
    {
        if (IsValidPosition(position))
        {
            cell = _board[position.x, position.y];
            return true;
        }
        cell = new Cell();
        return false;
    }

    private bool TryGetCell(Vector3 worldPosition, out Cell cell)
    {
        return TryGetCell(WorldToBoardPosition(worldPosition), out cell);
    }

    private bool IsValidPosition(Vector2Int position)
    {
        return position.x >= 0 &&
            position.x < _board.Width &&
            position.y >= 0 &&
            position.y < _board.Height;
    }

    private List<Cell> GetValidCellsAround(Cell cell)
    {
        var neighborDistances = new Vector2Int[]
        {
            new(-1, -1),
            new(-1, 0),
            new(-1, 1),
            new(0, 1),
            new(1, 1),
            new(1, 0),
            new(1, -1),
            new(0, -1),
        };

        var validCellsAround = new List<Cell>();

        foreach (var neighborDistance in neighborDistances)
        {
            var position = neighborDistance + cell.Position;
            if (IsValidPosition(position))
            {
                validCellsAround.Add(_board[position.x, position.y]);
            }
        }

        return validCellsAround;
    }

    private void SetTimer(float timer)
    {
        _timer = timer;
        if (_uiController != null)
            _uiController.SetTimer(timer);
    }

    private void SetFlags(int flags)
    {
        _flags = flags;
        if (_uiController != null)
            _uiController.SetFlags(flags);
    }

    private void SetBaseTile(Vector2Int position, Tile tileType)
    {
        var worldPosition = BoardToWorldPosition(position);
        if (_tilemapController != null)
            _tilemapController.SetBaseTile(worldPosition, tileType);
    }

    private void SetTopTile(Vector2Int position, Tile tileType)
    {
        var worldPosition = BoardToWorldPosition(position);
        if (_tilemapController != null)
            _tilemapController.SetTopTile(worldPosition, tileType);
    }

    private void SpawnParticle(Vector2Int position, Tile tileType)
    {
        var worldPosition = BoardToWorldPosition(position) + new Vector3(0.5f, 0.5f, -1);
        if (_particleController != null)
            _particleController.SpawnParticle(worldPosition, tileType);
    }

    private void SpawnAudio(Tile tileType)
    {
        if (_audioController != null)
            _audioController.SpawnAudio(tileType);
    }

    private void ShakeCamera()
    {
        if (_cameraController != null)
            _cameraController.Shake();
    }

    private Vector3Int BoardToWorldPosition(Vector2Int position)
    {
        return new Vector3Int(position.x + _boardOffset.x, position.y + _boardOffset.y, 0);
    }

    private Vector2Int WorldToBoardPosition(Vector3 position)
    {
        return new Vector2Int((int)Mathf.Floor(position.x - _boardOffset.x), (int)Mathf.Floor(position.y - _boardOffset.y));
    }
}