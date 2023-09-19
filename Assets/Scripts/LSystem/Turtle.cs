using UnityEngine;

public class Turtle : MonoBehaviour {
    private string treeDescriptorString;

    [SerializeField]
    private LSystem lSystem;
    // Start is called before the first frame update

    void Start() {
        treeDescriptorString = lSystem.Generate();
        TreeData treeData = new(1, Mathf.PI / 7.0f);
        treeData.CreateTreeDataFromString(treeDescriptorString);
        treeData.VisualiseTreeStructure();
    }
    
    // generate a mesh from the LSystem
    private Mesh generateMeshFromString(string treeMeshString) {
        Mesh mesh = new();
        

        // mesh.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 1, 0) };
        // mesh.triangles = new int[] { 0, 1 };
        return mesh;
    }

}
