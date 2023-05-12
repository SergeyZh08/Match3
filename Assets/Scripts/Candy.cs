using System.Collections;
using UnityEngine;

public class Candy : MonoBehaviour
{
    [field: SerializeField] public int Index { get; private set; }
    public Tile Tile { get; private set; }
    private Vector3 _targetPosition;
    

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * 7.5f);
    }

    public void SetTile(Tile tile)
    {
        _targetPosition = tile.transform.position;
        if (Tile && Tile.Candy == this)
        {
            Tile.SetCandy(null);
        }

        tile.SetCandy(this);
        Tile = tile;
    }

    public void Die()
    {
        StartCoroutine(DieProcess());
    }

    private IEnumerator DieProcess()
    {
        for (float t = transform.localScale.x; t >= 0; t -= Time.deltaTime * 4f)
        {
            transform.localScale = Vector3.one * t;
            yield return null;
        }

        Destroy(gameObject);
    }
}
