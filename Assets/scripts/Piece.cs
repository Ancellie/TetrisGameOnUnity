using UnityEngine;
using System.Collections;

public class Piece : MonoBehaviour
{
    private float fallTime = 1f;
    private float fastFallTime = 0.1f;
    private float nextFallTime;
    private float moveDelay = 0.1f;
    private float nextMoveTime;
    private bool isFastDropping = false;
    private bool isMovingLeft = false;
    private bool isMovingRight = false;
    private Coroutine fastDropCoroutine;
    
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int position { get; private set; }
    public Vector3Int[] cells { get; private set; }
    
    
    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;

        if (this.cells == null)
        {
            this.cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }
    
    private void Start()
    {
        nextFallTime = Time.time + fallTime;
    }
    
    private void Update()
    {
        this.board.Clear(this);

        if (Time.time >= nextFallTime)
        {
            Move(Vector2Int.down);
            nextFallTime = Time.time + (isFastDropping ? fastFallTime : fallTime);
        }

        if (Time.time >= nextMoveTime)
        {
            if (isMovingLeft)
            {
                Move(Vector2Int.left);
                nextMoveTime = Time.time + moveDelay;
            }
            else if (isMovingRight)
            {
                Move(Vector2Int.right);
                nextMoveTime = Time.time + moveDelay;
            }
        }

        this.board.Set(this);
    }
    
    public void MoveLeft()
    {
        this.board.Clear(this);
        isMovingLeft = true;
        isMovingRight = false;
        Move(Vector2Int.left);
        nextMoveTime = Time.time + moveDelay;
        this.board.Set(this);
    }

    public void MoveRight()
    {
        this.board.Clear(this);
        isMovingRight = true;
        isMovingLeft = false;
        Move(Vector2Int.right);
        nextMoveTime = Time.time + moveDelay;
        this.board.Set(this);
    }

    public void StopMoving()
    {
        isMovingLeft = false;
        isMovingRight = false;
    }
    
    public void StartFastDrop()
    {
        if (fastDropCoroutine != null)
        {
            StopCoroutine(fastDropCoroutine);
        }
        fastDropCoroutine = StartCoroutine(FastDropCoroutine());
    }

    public void StopFastDrop()
    {
        if (fastDropCoroutine != null)
        {
            StopCoroutine(fastDropCoroutine);
            fastDropCoroutine = null;
        }
        isFastDropping = false;
        nextFallTime = Time.time + fallTime;
    }

    private IEnumerator FastDropCoroutine()
    {
        isFastDropping = true;
        while (true)
        {
            this.board.Clear(this);
            bool moved = Move(Vector2Int.down);
            this.board.Set(this);

            if (!moved)
            {
                break;
            }

            yield return new WaitForSeconds(fastFallTime);
        }
        isFastDropping = false;
        nextFallTime = Time.time + fallTime;
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = this.board.IsValidPosition(this, newPosition);

        if (valid)
        {
            this.position = newPosition;
        }

        return valid;
    }
    
}
