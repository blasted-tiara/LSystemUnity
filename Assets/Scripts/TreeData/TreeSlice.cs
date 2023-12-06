using UnityEngine;

public class TreeSlice {
    /**
     * 
     */
    public static Vector3[] GenerateSlice(TreeNode center, int n) {
        int numberOfChildren = center.children == null ? 0 : center.children.Length;

        Vector3[] vertices = new Vector3[n * numberOfChildren];
        
        float smallRandomOffset = Random.Range(-0.5f, 0.5f);

        for (int childIndex = 0; childIndex < numberOfChildren; childIndex++) {
            Vector3 normal = GetRingNormal(center, childIndex);
            float angleIncrement = 360f / n;
            Vector3 InitialVertex = Vector3.Normalize(Vector3.Cross(normal, Vector3.forward) ) * center.radius;
            for (int vertexIndex = 0; vertexIndex < n; vertexIndex++) {
                if (normal != Vector3.zero) {
                    vertices[childIndex * n + vertexIndex] = center.position + Quaternion.AngleAxis(smallRandomOffset + angleIncrement * vertexIndex, normal) * InitialVertex;
                } else {
                    vertices[vertexIndex] = center.position;
                }
            }

        }

        return vertices;
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