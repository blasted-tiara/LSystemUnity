using UnityEngine;

public class TreeSlice {
    public static Vector3[] GenerateSlice(TreeNode center, int n) {
        Vector3 normal = GetRingNormal(center);
        Vector3[] vertices = new Vector3[n];
        float angleIncrement = 360f / n;
        Vector3 InitialVertex = Vector3.Normalize(Vector3.Cross(normal, Vector3.forward) ) * center.radius;
        for (int i = 0; i < n; i++) {
            if (normal != Vector3.zero) {
                vertices[i] = center.position + Quaternion.AngleAxis(angleIncrement * i, normal) * InitialVertex;
            } else {
                vertices[i] = center.position;
            }
        }

        return vertices;
    }
    
    private static Vector3 GetRingNormal(TreeNode center) {
        if (center.parent == null) {
            // root node
            if (center.children.Length == 1) {
                return center.children[0].position - center.position;
            } else {
                return Vector3.zero;
            }
        } else {
            if (center.children == null || center.children.Length == 0) {
                // apex node
                return Vector3.zero;
            } else if (center.children.Length == 1) {
                // intermediate node
                return center.children[0].position - center.parent.position;
            } else {
                return center.position - center.parent.position;
            }
        }
    }
}