using UnityEngine;

public class Tile : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public bool DestroyFlag { get; private set; }
    public Candy Candy { get; private set; }

    public void SetCoordinates(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void SetCandy(Candy candy)
    {
        Candy = candy;
    }

    public void SetDestroyFlag(bool destroyFlag)
    {
        DestroyFlag = destroyFlag;
    }

    public void CheckFlag()
    {
        if (DestroyFlag)
        {
            DestroyFruit();
        }
    }

    private void DestroyFruit()
    {
        Candy.Die();
        Candy = null;
        DestroyFlag = false;
    }
}
