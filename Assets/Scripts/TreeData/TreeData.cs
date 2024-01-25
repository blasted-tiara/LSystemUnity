using System;
using System.Collections.Generic;
using UnityEngine;

public class TreeData {
    public TreeNode root;
    private readonly float stepSize;
    private readonly float angle;
    private readonly float initRadius;
    private readonly float radiusDecay;
    private const int numberOfSides = 6;

    private Stack<TurtleState> turtleStateStack = new();
    
    public TreeData(float stepSize, float angle) {
        this.stepSize = stepSize;
        this.angle = angle;
    }

    public TreeData(float stepSize, float angle, float initRadius, float radiusDecay) {
        this.stepSize = stepSize;
        this.angle = angle;
        this.initRadius = initRadius;
        this.radiusDecay = radiusDecay;
    }

    public void CreateTreeDataFromString(string LSystemString) {
        TreeNode currentNode = root = new(Vector3.zero, null, initRadius);

        TurtleState turtleState = new() {
            position = Vector3.zero,
            radius = initRadius
        };
        foreach (char c in LSystemString) {
            switch (c) {
                case 'F':
                    TreeNode newNode = new(GetForwardPosition(currentNode, turtleState), currentNode, turtleState.radius);
                    currentNode.AddChild(newNode);
                    currentNode = newNode;
                    break;
                case '*':
                    currentNode.hasLeaf = true;
                    break;
                case '+':
                    turtleState.direction.z += angle;
                    break;
                case '-':
                    turtleState.direction.z -= angle;
                    break;
                case '\\':
                    turtleState.direction.x += angle;
                    break;
                case '/':
                    turtleState.direction.x -= angle;
                    break;
                case '&':
                    turtleState.direction.y += angle;
                    break;
                case '^':
                    turtleState.direction.y -= angle;
                    break;
                case '[':
                    TurtleState newTurtleState = turtleState;
                    newTurtleState.currentNode = currentNode;
                    turtleStateStack.Push(newTurtleState);
                    break;
                case ']':
                    turtleState = turtleStateStack.Pop();
                    currentNode = turtleState.currentNode;
                    break;
                case '!':
                    turtleState.radius = Math.Max(turtleState.radius *= radiusDecay, 2f);
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
    
    public List<LeafData> GetLeafPositions() {
        List<LeafData> leafPositions = new();
        TreeNode currentNode;
        Queue<TreeNode> nodesToProcess = new();
        nodesToProcess.Enqueue(root);
        while (nodesToProcess.Count > 0) {
            currentNode = nodesToProcess.Dequeue();
            if (currentNode.hasLeaf) {
                LeafData newLeaf = new() {
                    position = currentNode.position,
                    direction = currentNode.position - currentNode.parent.position,
                    radius = currentNode.radius,
                };

                leafPositions.Add(newLeaf);
            }
            if (currentNode.children != null) {
                foreach (TreeNode child in currentNode.children) {
                    nodesToProcess.Enqueue(child);
                }
            }
        }

        return leafPositions;
    }

    public Mesh GenerateMesh() {
        int vertexCount = GetVertexCount(root, numberOfSides);
        Debug.Log(vertexCount);
        int triangleCount = GetTriangleCount(root, numberOfSides);
        Debug.Log(triangleCount);

        Vector3[] vertices = new Vector3[vertexCount];
        Vector3[] normals = new Vector3[vertexCount];
        Vector4[] tangents = new Vector4[vertexCount];
        CreateVertexData(vertices, normals, tangents, root, numberOfSides);
        
        int[] triangles = new int[triangleCount * 3];
        CreateTriangles(triangles, root, numberOfSides);
        
        Vector2[] uvs = new Vector2[vertexCount];
        CreateUVs(uvs, root, numberOfSides);

        Mesh mesh = new() {
            name = "Tree Mesh",
            vertices = vertices,
            triangles = triangles,
            normals = normals,
            tangents = tangents,
            uv = uvs
        };
        
        return mesh;
    }
    
    /*
    public Mesh GenerateDecimatedMesh() {
        Mesh mesh = GenerateMesh();
        float quality = 0.3f;
        
        var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
        meshSimplifier.Initialize(mesh);
        meshSimplifier.SimplifyMesh(quality);

        return meshSimplifier.ToMesh();
    }
    */

    private void CreateVertexData(Vector3[] vertices, Vector3[] normals, Vector4[] tangents, TreeNode rootNode, int sides) {
        Queue<TreeNode> nodesToCreate = new();
        nodesToCreate.Enqueue(rootNode);
        
        int currentIndex = 0;
        while (nodesToCreate.Count > 0) {
            TreeNode currentNode = nodesToCreate.Dequeue();
            currentNode.vertexIndex = currentIndex;
            if (currentNode.children != null && currentNode.children.Length > 0) {
                var (sliceVertices, sliceNormals, sliceTangents) = TreeSlice.GenerateSliceData(currentNode, sides);
                for (int i = 0; i < sliceVertices.Length; i++) {
                    vertices[currentIndex] = sliceVertices[i];
                    normals[currentIndex] = sliceNormals[i];
                    tangents[currentIndex] = new Vector4(sliceTangents[i].x, sliceTangents[i].y, sliceTangents[i].z, -1f);
                    currentIndex++;
                }

                foreach (TreeNode child in currentNode.children) {
                    nodesToCreate.Enqueue(child);
                }
            } else {
                vertices[currentIndex] = currentNode.position;
                normals[currentIndex] = Vector3.Normalize(currentNode.position - currentNode.parent.position);
                currentIndex++;
            }
        }
    }
    
    private void CreateUVs(Vector2[] uvs, TreeNode rootNode, int sides) {
        Queue<(TreeNode, float)> nodesToCreate = new();

        nodesToCreate.Enqueue((rootNode, 0));

        const float TEXTURE_SCALE = 80;
        
        while (nodesToCreate.Count > 0) {
            var (currentNode, currentLength) = nodesToCreate.Dequeue();
            float newLength = currentNode.parent == null ? 0 : currentLength + Vector3.Distance(currentNode.position, currentNode.parent.position) / TEXTURE_SCALE;

            if (currentNode.children != null && currentNode.children.Length > 0) {
                for (int childIndex = 0; childIndex < currentNode.children.Length; childIndex++) {
                    for (int i = 0; i <= sides; i++) {
                        uvs[currentNode.vertexIndex + childIndex * sides + i] = new Vector2(i / (float) sides, newLength);
                    }

                    nodesToCreate.Enqueue((currentNode.children[childIndex], newLength));
                }
            } else {
                uvs[currentNode.vertexIndex] = new Vector2(0.5f, 1);
            }
        }
    }
    
    private void CreateTrianglesForApex(int[] triangles, int currentTriangleIndex, TreeNode currentNode, int sides) {
        int parentVertexIndexStart = currentNode.parent.vertexIndex;
        int childOrder = currentNode.GetChildOrder();
        int apexVertexIndex = currentNode.vertexIndex;
        for (int i = 0; i < sides; i++) {
            triangles[currentTriangleIndex + 3 * i] = parentVertexIndexStart + i + childOrder * sides;
            triangles[currentTriangleIndex + 3 * i + 1] = parentVertexIndexStart + i + 1 + childOrder * sides;
            triangles[currentTriangleIndex + 3 * i + 2] = apexVertexIndex; 
        }
    }

    private void CreateTrianglesForIntermediateNode(int[] triangles, int currentTriangleIndex, TreeNode currentNode, int sides) {
        int childOrder = currentNode.GetChildOrder();
        int mostColinearChildIndex = currentNode.GetMostColinearChild();
        int parentVertexIndexStart = currentNode.parent.vertexIndex;
        for (int i = 0; i < sides; i++) {
            triangles[currentTriangleIndex + 6 * i] = parentVertexIndexStart + i + childOrder * sides;
            triangles[currentTriangleIndex + 6 * i + 1] = parentVertexIndexStart + i + 1 + childOrder * sides;
            triangles[currentTriangleIndex + 6 * i + 2] = currentNode.vertexIndex + i + mostColinearChildIndex * sides;

            triangles[currentTriangleIndex + 6 * i + 3] = parentVertexIndexStart + i + 1 + childOrder * sides;
            triangles[currentTriangleIndex + 6 * i + 4] = currentNode.vertexIndex + i + 1 + mostColinearChildIndex * sides;
            triangles[currentTriangleIndex + 6 * i + 5] = currentNode.vertexIndex + i + mostColinearChildIndex * sides;
        }
    }
    
    private void CreateTriangles(int[] triangles, TreeNode rootNode, int sides) {
        Queue<TreeNode> nodesToCreate = new();
        nodesToCreate.Enqueue(rootNode);
        
        int currentTriangleIndex = 0;
        while (nodesToCreate.Count > 0) {
            TreeNode currentNode = nodesToCreate.Dequeue();
            if (currentNode.children == null) {
                // generate n triangles for apex
                CreateTrianglesForApex(triangles, currentTriangleIndex, currentNode, sides);
                currentTriangleIndex += 3 * sides;
            } else {
                if (currentNode.children.Length > 0) {
                    foreach (TreeNode child in currentNode.children) {
                        nodesToCreate.Enqueue(child);
                    }
                    if (currentNode.parent != null) {
                        // generate 2n triangles for intermediate node
                        CreateTrianglesForIntermediateNode(triangles, currentTriangleIndex, currentNode, sides);
                        currentTriangleIndex += 6 * sides;
                    }
                }
            }
        }
    }
    
    private int GetVertexCount(TreeNode rootNode, int sides) {
        int count = 0;
        Queue<TreeNode> nodesToCount = new();
        nodesToCount.Enqueue(rootNode);

        while (nodesToCount.Count > 0) {
            TreeNode currentNode = nodesToCount.Dequeue();
            
            if (currentNode.children != null && currentNode.children.Length > 0) {
                count += (sides + 1) * currentNode.children.Length;
                foreach (TreeNode child in currentNode.children) {
                    nodesToCount.Enqueue(child);
                }
            } else {
                count++;
            }
        }

        return count;
    }
    
    private int GetTriangleCount(TreeNode rootNode, int sides) {
        int count = 0;
        Queue<TreeNode> nodesToCount = new();
        
        if (rootNode.children == null || rootNode.children.Length == 0) {
            return 0;
        }

        foreach (TreeNode child in rootNode.children) {
            nodesToCount.Enqueue(child);
        }
        
        while (nodesToCount.Count > 0) {
            TreeNode currentNode = nodesToCount.Dequeue();
            
            if (currentNode.children != null && currentNode.children.Length > 0) {
                count += sides * 2;
                
                foreach (TreeNode child in currentNode.children) {
                    nodesToCount.Enqueue(child);
                }
            } else {
                count += sides;
            }
        }
        
        return count;
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
