using UnityEngine;

public class TreeNode {
    public TreeNode[] children;
    public Vector3 position;
    public TreeNode parent;
    public float radius;
    public int vertexIndex = -1;
    
    public TreeNode(Vector3 position, TreeNode parent, float radius) {
        this.position = position;
        this.parent = parent;
        this.radius = radius;
    }
    
    public void AddChild(TreeNode child) {
        if (children == null) {
            children = new TreeNode[1];
            children[0] = child;
        } else {
            TreeNode[] newChildren = new TreeNode[children.Length + 1];
            for (int i = 0; i < children.Length; i++) {
                newChildren[i] = children[i];
            }
            newChildren[^1] = child;
            children = newChildren;
        }
    }
}
