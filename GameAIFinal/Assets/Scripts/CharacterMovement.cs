using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterMovement : GridAligned {
    private Vector2Int direction = Vector2Int.right;
    public Vector2Int Direction { get => direction; set => direction = value; }
    private Vector2Int targetGridPosition;
    [SerializeField] private float moveSpeed;

    public Square square { get => Square.getSquareAt(transform.position); }

    public UnityEvent onTargetReached;
    [System.Serializable] public class TargetUpdatedEvent : UnityEvent<Vector3> { }
    public TargetUpdatedEvent onTargetUpdated;
    public UnityEvent onNextUnreachable;

    private void Start() {
        targetGridPosition = gridPosition + direction;
        setNextTarget();
    }

    private void Update() {
        if(lerp()) {
            onTargetReached.Invoke();
            gridPosition = targetGridPosition;
            setNextTarget();
        } else {
            setNextTarget();
        }
    }

    private void setNextTarget() {
        Vector2Int nextTarget = gridPosition + direction;
        Square nextSquare = Square.getSquareAt(LevelGrid.CellToWorld(new Vector3Int(nextTarget.x, nextTarget.y, 0)));
        if(nextSquare == null) {
            return;
        }

        if(!canMoveIntoSquare(nextSquare)) {
            onNextUnreachable.Invoke();
            return;
        }

        targetGridPosition = nextTarget;
        onTargetUpdated.Invoke(nextSquare.transform.position);
    }

    private bool lerp() {
        Vector3 targetPosition = LevelGrid.CellToWorld(new Vector3Int(targetGridPosition.x, targetGridPosition.y, 0));

        if(!canMoveIntoSquare(Square.getSquareAt(targetPosition))) {
            return false;
        }

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        if(transform.position == targetPosition) {
            return true;
        }

        return false;
    }

    public bool canMoveIntoSquare(Square square) {
        if(square == null) {
            return false;
        }

        if(direction == Vector2Int.up) {
            return !square.SouthWall;
        }
        if(direction == Vector2Int.right) {
            return !square.WestWall;
        }
        if(direction == Vector2Int.down) {
            return !square.NorthWall;
        }
        if(direction == Vector2Int.left) {
            return !square.EastWall;
        }

        return false;
    }
    
    public bool canMoveIntoSquare(Vector2Int direction) {
        Vector2Int nextGridPosition = gridPosition + direction;
        Square square = Square.getSquareAt(LevelGrid.CellToWorld(new Vector3Int(nextGridPosition.x, nextGridPosition.y, 0)));

        if(square == null) {
            return false;
        }

        if(direction == Vector2Int.up) {
            return !square.SouthWall;
        }
        if(direction == Vector2Int.right) {
            return !square.WestWall;
        }
        if(direction == Vector2Int.down) {
            return !square.NorthWall;
        }
        if(direction == Vector2Int.left) {
            return !square.EastWall;
        }

        return false;
    }

    public static bool canMoveIntoSquare(Square square, Vector2Int direction) {
        if(square == null) {
            return false;
        }

        if(direction == Vector2Int.up) {
            return !square.SouthWall;
        }
        if(direction == Vector2Int.right) {
            return !square.WestWall;
        }
        if(direction == Vector2Int.down) {
            return !square.NorthWall;
        }
        if(direction == Vector2Int.left) {
            return !square.EastWall;
        }

        return false;
    }

    public static bool canMoveIntoSquare(Square current, Square next) {
        if(current == null || next == null) {
            return false;
        }

        Vector2Int delta = next.GridPosition - current.GridPosition;

        if(delta == Vector2Int.up) {
            return !next.SouthWall;
        }
        if(delta == Vector2Int.right) {
            return !next.WestWall;
        }
        if(delta == Vector2Int.down) {
            return !next.NorthWall;
        }
        if(delta == Vector2Int.left) {
            return !next.EastWall;
        }

        return false;
    }

    public void setRandomDirection() {
        List<Square> possibleMoves = new List<Square>();
        foreach(Square square in square.getNeighbors()) {
            if(square != null) {
                if(canMoveIntoSquare(square)) {
                    possibleMoves.Add(square);
                }
            }
        }

        int roll = Random.Range(0, possibleMoves.Count - 1);
        Square newTarget = possibleMoves[roll];
        Vector2Int delta = newTarget.GridPosition - square.GridPosition;
        direction = delta;
    }
}
