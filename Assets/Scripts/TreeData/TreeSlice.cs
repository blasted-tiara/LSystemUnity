using UnityEngine;

public class TreeSlice {
    public static (Vector3[], Vector3[]) GenerateSlice(TreeNode center, int n) {
        int numberOfChildren = center.children == null ? 0 : center.children.Length;
        float angleIncrement = 360f / n;

        Vector3[] vertices = new Vector3[(n + 1) * numberOfChildren];
        Vector3[] normals = new Vector3[(n + 1) * numberOfChildren];
        
        for (int childIndex = 0; childIndex < numberOfChildren; childIndex++) {
            float radius = numberOfChildren > 1 ? (center.radius + center.children[childIndex].radius) / 2f : center.radius;

            Vector3 normal = GetRingNormal(center, childIndex);
            Vector3 InitialVertex = Vector3.Normalize(Vector3.Cross(normal, Vector3.forward) ) * radius;
            for (int vertexIndex = 0; vertexIndex <= n; vertexIndex++) {
                if (normal != Vector3.zero) {
                    vertices[childIndex * n + vertexIndex] = center.position + Quaternion.AngleAxis(angleIncrement * vertexIndex, normal) * InitialVertex;
                    normals[childIndex * n + vertexIndex] = Vector3.Normalize(vertices[childIndex * n + vertexIndex] - center.position);
                } else {
                    vertices[vertexIndex] = center.position;
                    normals[vertexIndex] = Vector3.Normalize(center.position - center.parent.position);
                }
            }

        }

        return (vertices, normals);
    }
    
    private static Vector3 GetRingNormal(TreeNode center, int n) {
        if (center.parent == null) {
            // root node
            if (center.children.Length == 1) {
                return center.children[n].position - center.position;
            } else {
                return Vector3.zero;
            }
        } else {
            if (center.children == null || center.children.Length == 0) {
                // apex node
                return Vector3.zero;
            } else {
                // intermediate node
                return center.children[n].position - center.position;
            }
        }
    }
}