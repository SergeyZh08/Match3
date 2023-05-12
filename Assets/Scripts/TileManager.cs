using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public bool IsCalculating { get; private set; } = true;
    public Dictionary<Vector2Int, Tile> TilesDictionary { get; private set; } = new Dictionary<Vector2Int, Tile>();
    [SerializeField] private Candy[] _candies;
    [SerializeField] private Vector3Int _offsetPosition;
    [SerializeField] private float _timeOffset = 0.25f;
    private int _xLength = 0;
    private int _yLength = 0;
    private Tile[] _allTiles;

    private void Start()
    {
        _allTiles = GetComponentsInChildren<Tile>();
        GetFieldLength();
        SetUpTilesDictionary();
        CreateCandies();
        Calculate();
    }

    private void GetFieldLength()
    {
        for (int i = 0; i < _allTiles.Length; i++)
        {
            int posX = Mathf.RoundToInt(_allTiles[i].transform.position.x);
            int posY = Mathf.RoundToInt(_allTiles[i].transform.position.y);

            if (_xLength < posX)
            {
                _xLength = posX;
            }

            if (_yLength < posY)
            {
                _yLength = posY;
            }
        }
        _xLength++;
        _yLength++;
    }

    private void SetUpTilesDictionary()
    {
        for (int x = 0; x < _xLength; x++) 
        {
            for (int y = 0; y < _yLength; y++)
            {
                Vector2Int xy = new Vector2Int(x, y);
                TilesDictionary.Add(xy, null);
            }
        }

        for (int i = 0; i < _allTiles.Length; i++)
        {
            int posX = Mathf.RoundToInt(_allTiles[i].transform.position.x);
            int posY = Mathf.RoundToInt(_allTiles[i].transform.position.y);
            _allTiles[i].SetCoordinates(posX, posY);

            Vector2Int xy = new Vector2Int(posX, posY);
            TilesDictionary[xy] = _allTiles[i];
        }
    }

    private void CreateCandies()
    {
        for (int i = 0; i < _allTiles.Length; i++)
        {
            if (_allTiles[i].Candy == null)
            {
                int randomIndex = Random.Range(0, _candies.Length);
                Vector3 position = new Vector3(_allTiles[i].X, _allTiles[i].Y, 0f);
                Candy newCandy = Instantiate(_candies[randomIndex], position + _offsetPosition, Quaternion.identity);
                newCandy.SetTile(_allTiles[i]);
            }
        }
    }

    public void Calculate()
    {
        StartCoroutine(CalculateProcess());
    }

    private IEnumerator CalculateProcess()
    {
        IsCalculating = true;

        Check();
        yield return new WaitForSeconds(_timeOffset);
        DestroyCandies();
        yield return new WaitForSeconds(_timeOffset);
        MoveDown();
        yield return new WaitForSeconds(_timeOffset);
        CreateCandies();
        yield return new WaitForSeconds(_timeOffset);
        if (Check())
        {
            Calculate();
        }
        else
        {
            IsCalculating = false;
        }
    }

    public bool Check()
    {
        bool result = false;
        List<Tile> identicalVertical = new List<Tile>();
        for (int x = 0; x < _xLength; x++)
        {
            for (int y = 0; y < _yLength; y++)
            {
                if (CheckByXY(x, y, identicalVertical))
                {
                    result = true;
                }
            }
            identicalVertical.Clear();
        }

        List<Tile> identicalHorizontal = new List<Tile>();
        for (int y = 0; y < _yLength; y++)
        {
            for (int x = 0; x < _xLength; x++)
            {
                if (CheckByXY(x, y, identicalHorizontal))
                {
                    result = true;
                }
            }
            identicalHorizontal.Clear();
        }

        return result;
    }

    private bool CheckByXY(int x, int y, List<Tile> identicalTiles)
    {
        Vector2Int xy = new Vector2Int(x, y);
        Tile tile = TilesDictionary[xy];

        if (tile == null)
        {
            identicalTiles.Clear();
            return false;
        }

        if (identicalTiles.Count > 0)
        {
            Tile previousTile = identicalTiles[identicalTiles.Count - 1];
            if (tile.Candy.Index != previousTile.Candy.Index)
            {
                identicalTiles.Clear();
            }
        }

        identicalTiles.Add(tile);

        if (identicalTiles.Count >= 3)
        {
            foreach (var item in identicalTiles)
            {
                item.SetDestroyFlag(true);
            }
            return true;
        }

        return false;
    }

    private void MoveDown()
    {
        for (int x = 0; x < _xLength; x++)
        {
            int n = 0;
            for (int y = 0; y < _yLength; y++)
            {
                Vector2Int xy = new Vector2Int(x, y);
                if (TilesDictionary[xy] == null)
                {
                    n = 0;
                    continue;
                }
                if (TilesDictionary[xy].Candy == null)
                {
                    n++;
                }
                else
                {
                    Vector2Int tileIndex = xy - new Vector2Int(0, n);
                    TilesDictionary[xy].Candy.SetTile(TilesDictionary[tileIndex]);
                }
            }
        }
    }

    private void DestroyCandies()
    {
        foreach (var kvp in TilesDictionary)
        {
            if (kvp.Value)
            {
                kvp.Value.CheckFlag();
            }
        }
    }
}
