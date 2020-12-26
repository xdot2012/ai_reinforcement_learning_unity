using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public HashSet<Node> neighbours; //List of Adjacent Nodes
    public Node cameFrom;

    public int x;
    public int y;

    public float gCost;
    public float hCost;
    public float fCost;

    public Node(int x, int y) {
        this.neighbours = new HashSet<Node>();
        this.cameFrom = null;
        this.x = x;
        this.y = y;
    }

    public void CalculateFCost() { fCost = gCost + hCost; }
    public Vector2Int GetPosition() => new Vector2Int(this.x, this.y);
}
