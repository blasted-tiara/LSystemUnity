using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Turtle : MonoBehaviour {
    private string treeDescriptorString;

    [SerializeField]
    private float angle;
    
    [SerializeField]
    private float stepSize;

    [SerializeField]
    private LSystem lSystem;
    // Start is called before the first frame update
    
    [SerializeField]
    private float initRadius;
    
    [SerializeField]
    private float radiusDecay;
    
    void Start() {
        treeDescriptorString = lSystem.Generate();
        TreeData treeData = new(stepSize, angle, initRadius, radiusDecay);
        treeData.CreateTreeDataFromString(treeDescriptorString);
        treeData.VisualiseTreeStructure();

        Mesh mesh = treeData.GenerateMesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
