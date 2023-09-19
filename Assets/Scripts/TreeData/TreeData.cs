using System.Collections.Generic;
using UnityEngine;

public class TreeData {
    public TreeNode root;
    private float stepSize;
    private float angle;
    private Stack<TurtleState> turtleStateStack = new();
    
    public TreeData(float stepSize, float angle) {
        this.stepSize = stepSize;
        this.angle = angle;
    }
    
    public void CreateTreeDataFromString(string LSystemString) {
        TreeNode currentNode = root = new(Vector3.zero);
        
        TurtleState turtleState = new();
        foreach (char c in LSystemString) {
            switch (c) {
                case 'F':
                    TreeNode newNode = new(GetForwardPosition(currentNode, turtleState));
                    currentNode.AddChild(newNode);
                    currentNode = newNode;
                    break;
                case '+':
                    turtleState.direction.z += angle;
                    break;
                case '-':
                    turtleState.direction.z -= angle;
                    break;
                case '[':
                    TurtleState newTurtleState = new() {
                        position = turtleState.position,
                        direction = turtleState.direction,
                        currentNode = currentNode
                    };
                    turtleStateStack.Push(newTurtleState);
                    break;
                case ']':
                    turtleState = turtleStateStack.Pop();
                    currentNode = turtleState.currentNode;
                    break;
            }
        }
    }
    
    public void VisualiseTreeStructure() {
        TreeNode currentNode;
        Queue<TreeNode> nodesToDraw = new();
        nodesToDraw.Enqueue(root);
        while (nodesToDraw.Count > 0) {
            currentNode = nodesToDraw.Dequeue();
            DrawLinesToChildren(currentNode);
            if (currentNode.children != null) {
                foreach (TreeNode child in currentNode.children) {
                    nodesToDraw.Enqueue(child);
                }
            }
        }
    }
    
    private void DrawLinesToChildren(TreeNode currentNode) {
        if (currentNode.children != null) {
            foreach (TreeNode child in currentNode.children) {
                DrawLine(currentNode.position, child.position);
            }
        }
    }
    
    private void DrawLine(Vector3 start, Vector3 end) {
        Debug.DrawLine(start, end, Color.white, 10000f);
    }
    
    private Vector3 GetForwardPosition(TreeNode currentNode, TurtleState turtleState) {
        Vector3 newPosition = currentNode.position + Quaternion.Euler(turtleState.direction) * Vector3.up * stepSize;

        return newPosition;
    }
}
