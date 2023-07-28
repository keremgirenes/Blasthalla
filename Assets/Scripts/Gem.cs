using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    private Board board;
    private FindMatches findMatches;

    [Header("Board Variables")]
    public int column;
    public int row;
    public int prevColumn;
    public int prevRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    private GameObject otherGem;

    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPos;
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();

        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //column = targetX;
        //row = targetY;
        //prevColumn = column;
        //prevRow = row;
    }

    // Update is called once per frame
    void Update()
    {
        //FindMatches();

        if (isMatched)
        {
            SpriteRenderer objectSprite = GetComponent<SpriteRenderer>();
            objectSprite.color = new Color(0.5f, 0.5f, 0.5f, .5f);
        }

        targetX = column;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            // Move towards the target
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, .015f);
            if (board.allGems[column, row] != this.gameObject)
            {
                board.allGems[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            // Directly set the position
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            // Move towards the target
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, .015f);
            board.allGems[column, row] = this.gameObject;
            if (board.allGems[column, row] != this.gameObject)
            {
                board.allGems[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            // Directly set the position
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
        }
    }

    private void OnMouseDown()
    {
        if (board.currentState == GameState.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }
    
    void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MovePieces();
            board.currentState = GameState.wait;
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {
            // Right Swipe
            otherGem = board.allGems[column + 1, row];
            prevColumn = column;
            prevRow = row;
            otherGem.GetComponent<Gem>().column -= 1;
            column += 1;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            // Up Swipe
            otherGem = board.allGems[column, row + 1];
            prevColumn = column;
            prevRow = row;
            otherGem.GetComponent<Gem>().row -= 1;
            row += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            // Left Swipe
            otherGem = board.allGems[column - 1, row];
            prevColumn = column;
            prevRow = row;
            otherGem.GetComponent<Gem>().column += 1;
            column -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            // Down Swipe
            otherGem = board.allGems[column, row - 1];
            prevColumn = column;
            prevRow = row;
            otherGem.GetComponent<Gem>().row += 1;
            row -= 1;
        }
        StartCoroutine(CheckMoveCo());
    }

    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.5f);
        if (otherGem != null)
        {
            if(!isMatched && !otherGem.GetComponent<Gem>().isMatched)
            {
                otherGem.GetComponent<Gem>().column = column;
                otherGem.GetComponent<Gem>().row = row;
                column = prevColumn;
                row = prevRow;
                yield return new WaitForSeconds(.5f);
                board.currentState = GameState.move;
            } 
            else
            {
                board.DestroyMatches();
            }
            otherGem = null;
        }
    }

    void FindMatches()
    {
        if (column > 0 && column < board.width - 1)
        {
            GameObject leftGem1 = board.allGems[column - 1, row];
            GameObject rightGem1 = board.allGems[column + 1, row];
            if (leftGem1 != null && rightGem1 != null)
            {
                if (leftGem1.tag == this.gameObject.tag && rightGem1.tag == this.gameObject.tag)
                {
                    leftGem1.GetComponent<Gem>().isMatched = true;
                    rightGem1.GetComponent<Gem>().isMatched = true;
                    isMatched = true;
                }
            }
        }
        if (row > 0 && row < board.height - 1)
        {
            GameObject upGem1 = board.allGems[column, row + 1];
            GameObject downGem1 = board.allGems[column, row - 1];
            if (upGem1 != null && downGem1 != null)
            {
                if (upGem1.tag == this.gameObject.tag && downGem1.tag == this.gameObject.tag)
                {
                    upGem1.GetComponent<Gem>().isMatched = true;
                    downGem1.GetComponent<Gem>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }
}
