using UnityEngine;

public class Interraction : MonoBehaviour
{
    private Tile _tileA;
    private Tile _tileB;

    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private Material _selectedMaterial;
    [SerializeField] private TileManager _tileManager;
    [SerializeField] private Camera _camera;
    private Vector3 _startPosition;
    private Vector3 _offsetPosition;
    private float _positionForSwipe = 0.05f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_tileManager.IsCalculating)
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.GetComponent<Tile>() is Tile tile)
                {
                    _startPosition = new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, 0);
                    SelectTile(tile);
                }
            }
        }

        if (Input.GetMouseButton(0) && _tileA)
        {
            _offsetPosition = new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, 0);

            if (_offsetPosition.x - _startPosition.x > _positionForSwipe)
            {
                GetTileWithDictionary(new Vector2Int(_tileA.X + 1, _tileA.Y));
            }
            else if (_offsetPosition.x - _startPosition.x < -_positionForSwipe)
            {
                GetTileWithDictionary(new Vector2Int(_tileA.X - 1, _tileA.Y));
            }
            else if (_offsetPosition.y - _startPosition.y > _positionForSwipe)
            {
                GetTileWithDictionary(new Vector2Int(_tileA.X, _tileA.Y + 1));
            }
            else if (_offsetPosition.y - _startPosition.y < -_positionForSwipe)
            {
                GetTileWithDictionary(new Vector2Int(_tileA.X, _tileA.Y - 1));
            }
        }
    }

    private void GetTileWithDictionary(Vector2Int xy)
    {
        if (_tileManager.TilesDictionary.ContainsKey(xy))
        {
            Tile tile = _tileManager.TilesDictionary[xy];
            if (tile)
            {
                SelectTile(tile);
            }
        }
    }

    private void SelectTile(Tile tile)
    {
        if (_tileA == null)
        {
            _tileA = tile;
            _tileA.GetComponent<Renderer>().material = _selectedMaterial;
        }
        else if (_tileB == null)
        {
            _tileB = tile;

            if (_tileA == _tileB)
            {
                ResetTiles();
                return;
            }

            if (Vector3.Distance(_tileA.transform.position, _tileB.transform.position) > 1f)
            {
                ResetTiles();
                _tileA = tile;
                _tileA.GetComponent<Renderer>().material = _selectedMaterial;
                return;
            }

            Candy candyA = _tileA.Candy;
            Candy candyB = _tileB.Candy;

            candyA.SetTile(_tileB);
            candyB.SetTile(_tileA);

            if (_tileManager.Check())
            {
                _tileManager.Calculate();
            }
            else
            {
                candyA.SetTile(_tileA);
                candyB.SetTile(_tileB);
            }

            ResetTiles();
        }
    }

    private void ResetTiles()
    {
        _tileA.GetComponent<Renderer>().material = _defaultMaterial;

        _tileA = null;
        _tileB = null;
    }
}
