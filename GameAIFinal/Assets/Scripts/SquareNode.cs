using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareNode
{
    Square square; // which Square this node represents
    public Square Square { get { return square; } }
    Square target; // which Square we're trying to get to (may not need to store this)
    SquareNode previous; // which SquareNode this SquareNode came from
    public SquareNode Previous { get { return previous; } set { updatePrevious(value); } }

    int fCost;
    public int FCost { get { return fCost; } }
    int gCost;
    public int GCost { get { return gCost; } }
    int hCost;

    public SquareNode(Square square, Square target)
    { // this constructor is only used for the origin
        this.square = square;
        this.target = target;
        previous = null;

        gCost = 0;
        hCost = Mathf.RoundToInt(Square.getDistanceBetween(square, target) * 10); // Distance to target in grid space

        fCost = gCost + hCost;
    }

    public SquareNode(Square square, SquareNode previous, Square target)
    { // this constructor is used for anything that's not the origin
        this.square = square;
        this.previous = previous;
        this.target = target;

        gCost = getNewGCost(previous, this);
        hCost = Mathf.RoundToInt(Square.getDistanceBetween(square, target) * 10);

        fCost = gCost + hCost;
    }

    public override bool Equals(object otherObj)
    {
        if (otherObj == null || !GetType().Equals(otherObj.GetType()))
        {
            return false;
        }
        else
        { // we know it's actually a SquareNode
            SquareNode other = (SquareNode)otherObj;
            return square.Equals(other.square);
        }
    }

    public static int getNewGCost(SquareNode first, SquareNode second)
    {
        if (first.square.GridPosition.x != second.square.GridPosition.x && first.square.GridPosition.y != second.square.GridPosition.y)
        { // diagonal move
            return first.gCost + 14;
        }
        else
        { // horizontal or vertical move
            return first.gCost + 10;
        }
    }


    public SquareNode[] getNeighborNodes()
    {
        SquareNode[] results = new SquareNode[4]; // was 8 before
        Square[] neighbors = square.getNeighbors();

        for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i] != null)
            {
                results[i] = new SquareNode(neighbors[i], this, target);
            }
            else
            {
                results[i] = null;
            }
        }

        return results;
    }

    void updatePrevious(SquareNode previous)
    {
        this.previous = previous;
        gCost = getNewGCost(this.previous, this);
        fCost = gCost + hCost;
    }
}