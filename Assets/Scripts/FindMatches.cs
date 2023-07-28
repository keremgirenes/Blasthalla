using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentGem = board.allGems[i, j];
                if (currentGem != null)
                {
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftGem = board.allGems[i - 1, j];
                        GameObject rightGem = board.allGems[i + 1, j];
                        if (leftGem != null && rightGem != null)
                        {
                            if (leftGem.tag == currentGem.tag && rightGem.tag == currentGem.tag)
                            {
                                if (!currentMatches.Contains(leftGem))
                                {
                                    currentMatches.Add(leftGem);
                                }
                                if (!currentMatches.Contains(rightGem))
                                {
                                    currentMatches.Add(rightGem);
                                }
                                if (!currentMatches.Contains(currentGem))
                                {
                                    currentMatches.Add(currentGem);
                                }
                                leftGem.GetComponent<Gem>().isMatched = true;
                                rightGem.GetComponent<Gem>().isMatched = true;
                                currentGem.GetComponent<Gem>().isMatched = true;
                            }
                        }
                    }
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject downGem = board.allGems[i, j - 1];
                        GameObject upGem = board.allGems[i, j + 1];
                        if (downGem != null && upGem != null)
                        {
                            if (downGem.tag == currentGem.tag && upGem.tag == currentGem.tag)
                            {
                                if (!currentMatches.Contains(upGem))
                                {
                                    currentMatches.Add(upGem);
                                }
                                if (!currentMatches.Contains(downGem))
                                {
                                    currentMatches.Add(downGem);
                                }
                                if (!currentMatches.Contains(currentGem))
                                {
                                    currentMatches.Add(currentGem);
                                }
                                downGem.GetComponent<Gem>().isMatched = true;
                                upGem.GetComponent<Gem>().isMatched = true;
                                currentGem.GetComponent<Gem>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
