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
                    turtleState.radius *= radiusDecay;
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

    public Mesh GenerateMesh() {
        int vertexCount = GetVertexCount(root, numberOfSides);
        Debug.Log(vertexCount);
        int triangleCount = GetTriangleCount(root, numberOfSides);
        Debug.Log(triangleCount);

        Vector3[] vertices = new Vector3[vertexCount];
        CreateVertices(vertices, root, numberOfSides);
        
        int[] triangles = new int[triangleCount * 3];
        CreateTriangles(triangles, root, numberOfSides);

        Mesh mesh = new() {
            name = "Tree Mesh",
            vertices = vertices,
            triangles = triangles
        };

        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        return mesh;
    }
    
    public Mesh GenerateDecimatedMesh() {
        Mesh mesh = GenerateMesh();
        float quality = 0.3f;
        
        var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
        meshSimplifier.Initialize(mesh);
        meshSimplifier.SimplifyMesh(quality);

        return meshSimplifier.ToMesh();
    }
    
    private void CreateVertices(Vector3[] vertices, TreeNode rootNode, int sides) {
        Queue<TreeNode> nodesToCreate = new();
        nodesToCreate.Enqueue(rootNode);
        
        int currentIndex = 0;
        while (nodesToCreate.Count > 0) {
            TreeNode currentNode = nodesToCreate.Dequeue();
            currentNode.vertexIndex = currentIndex;
            if (currentNode.children != null && currentNode.children.Length > 0) {
                foreach (TreeNode child in currentNode.children) {
                    nodesToCreate.Enqueue(child);
                }
                Vector3[] sliceVertices = TreeSlice.GenerateSlice(currentNode, sides);
                foreach (Vector3 vertex in sliceVertices) {
                    vertices[currentIndex] = vertex;
                    currentIndex++;
                }
            } else {
                vertices[currentIndex] = currentNode.position;
                currentIndex++;
            }
        }
    }
    
    private void CreateTrianglesForApex(int[] triangles, int currentTriangleIndex, TreeNode currentNode, int sides) {
        int parentVertexIndexStart = currentNode.parent.vertexIndex;
        int apexVertexIndex = currentNode.vertexIndex;
        for (int i = 0; i < sides; i++) {
            triangles[currentTriangleIndex + 3 * i] = parentVertexIndexStart + i;
            triangles[currentTriangleIndex + 3 * i + 1] = parentVertexIndexStart + (i + 1) % sides;
            triangles[currentTriangleIndex + 3 * i + 2] = apexVertexIndex; 
        }
    }

    private void CreateTrianglesForIntermediateNode(int[] triangles, int currentTriangleIndex, TreeNode currentNode, int sides) {
        int parentVertexIndexStart = currentNode.parent.vertexIndex;
        for (int i = 0; i < sides; i++) {
            triangles[currentTriangleIndex + 6 * i] = parentVertexIndexStart + i;
            triangles[currentTriangleIndex + 6 * i + 1] = parentVertexIndexStart + (i + 1) % sides;
            triangles[currentTriangleIndex + 6 * i + 2] = currentNode.vertexIndex + i;

            triangles[currentTriangleIndex + 6 * i + 3] = parentVertexIndexStart + (i + 1) % sides;
            triangles[currentTriangleIndex + 6 * i + 4] = currentNode.vertexIndex + (i + 1) % sides;
            triangles[currentTriangleIndex + 6 * i + 5] = currentNode.vertexIndex + i;
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

        // using smooth mesh for now
        while (nodesToCount.Count > 0) {
            TreeNode currentNode = nodesToCount.Dequeue();
            
            if (currentNode.children != null && currentNode.children.Length > 0) {
                count += sides;
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
    
    private Mesh SmoothenMesh(Mesh roughMesh) {
        return roughMesh;
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
