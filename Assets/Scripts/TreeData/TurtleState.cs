using UnityEngine;

public struct TurtleState {
    public Vector3 position;
    public Vector3 direction;
    public TreeNode currentNode;
    public float radius;
    public bool consecutiveForwardState;
    public int timesForward;
}
