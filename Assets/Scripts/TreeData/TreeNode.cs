using UnityEngine;

public class TreeNode {
    public TreeNode[] children;
    public Vector3 position;
    public TreeNode parent;
    public float radius;
    public int vertexIndex = -1;
    public bool hasLeaf = false;
    
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
    
    /**
     * Returns the index of this node in its parent's children array.
     * Returns -1 if this node has no parent.
     */
    public int GetChildOrder() {
        if (parent != null) {
            
            if (parent.children == null) {
                return -1;
            }

            for (int i = 0; i < parent.children.Length; i++) {
                if (this == parent.children[i]) {
                    return i;
                }
            }
        }

        return -1;
    }
    
    /**
     * Returns index of the most colinear child of this node.
     */
    public int GetMostColinearChild() {
        if (children == null || children.Length == 0 || parent == null) {
            return -1;
        } else if (children.Length == 1) {
            return 0;
        } else {
            int mostColinearChildIndex = -1;
            float mostColinearAngle = float.MaxValue;
            for (int i = 0; i < children.Length; i++) {
                float angle = Vector3.Angle(position - parent.position, children[i].position - position);
                if (angle < mostColinearAngle) {
                    mostColinearAngle = angle;
                    mostColinearChildIndex = i;
                }
            }

            return mostColinearChildIndex;
        }
    }
}
