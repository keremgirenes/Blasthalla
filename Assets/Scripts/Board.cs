using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour
{
    public FindMatches findMatches;

    public GameState currentState = GameState.move;

    public int width;
    public int height;
    public int offset;

    public GameObject tilePrefab;
    public GameObject[] gems;

    private BackgroundTile[,] allTiles;
    public GameObject[,] allGems;

    public GameObject gemExplosion;

    // Start is called before the first frame update
    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        allTiles = new BackgroundTile[width, height];
        allGems = new GameObject[width, height];
        SetUp();
    }

    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPos = new Vector2(i, j + offset);
                Vector2 tilePos = new Vector2(i, j);
                GameObject backgroundTile = Instantiate(tilePrefab, tilePos, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "(" + i + ", " + j + ")";

                int gemToUse = Random.Range(0, gems.Length);
                int iteration = 0; 
                while (MatchesAt(i, j, gems[gemToUse]) && iteration < 10)
                {
                    gemToUse = Random.Range(0, gems.Length);
                    iteration++;
                }

                GameObject gem = Instantiate(gems[gemToUse], tempPos, Quaternion.identity);
                gem.GetComponent<Gem>().column = i;
                gem.GetComponent<Gem>().row = j;
                gem.transform.parent = this.transform;
                gem.name = "(" + i + ", " + j + ")";
                allGems[i, j] = gem;
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject gem)
    {
        if (column > 1 && row > 1)
        {
            if (allGems[column - 1, row].tag == gem.tag && allGems[column - 2,row].tag == gem.tag)
            {
                return true;
            }
            if (allGems[column, row - 1].tag == gem.tag && allGems[column, row - 2].tag == gem.tag)
            {
                return true;
            }
        } else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allGems[column, row - 1].tag == gem.tag && allGems[column, row - 2].tag == gem.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allGems[column - 1, row].tag == gem.tag && allGems[column - 2, row].tag == gem.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void DestroyGemAt(int column, int row)
    {
        if(allGems[column, row].GetComponent<Gem>().isMatched)
        {
            findMatches.currentMatches.Remove(allGems[column, row]);
            GameObject explosionParticle = Instantiate(gemExplosion, allGems[column, row].transform.position, Quaternion.identity);
            Destroy(explosionParticle, 5f);
            Destroy(allGems[column, row]);
            allGems[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allGems[i, j] != null)
                {
                    DestroyGemAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    allGems[i, j].GetComponent<Gem>().row -= nullCount;
                    allGems[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i, j] == null)
                {
                    Vector2 tempPos = new Vector2(i, j + offset);
                    int gemToUse = Random.Range(0, gems.Length);
                    GameObject gem = Instantiate(gems[gemToUse], tempPos, Quaternion.identity);
                    allGems[i, j] = gem;
                    gem.GetComponent<Gem>().column = i;
                    gem.GetComponent<Gem>().row = j;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGems[i, j] != null)
                {
                    if (allGems[i, j].GetComponent<Gem>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        yield return new WaitForSeconds(.5f);
        currentState = GameState.move;
    }
}
