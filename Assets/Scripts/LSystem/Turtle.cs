using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Turtle : MonoBehaviour {
    private string treeDescriptorString;

    [SerializeField]
    private GameObject leafModel;
    
    [SerializeField]
    private float angle;
    
    [SerializeField]
    private float stepSize;

    
    [SerializeField]
    private float initRadius;
    
    [SerializeField]
    private float radiusDecay;

    [SerializeField]
    private LSystem lSystem;
    
    void InstantiateLeaf(LeafData leafData) {
        for (int i = 0; i < 4; i++) {
            GameObject leaf = Instantiate(leafModel, leafData.position, Quaternion.LookRotation(leafData.direction));
            float scale = leafData.radius * Random.Range(0.8f, 2f);
            leaf.transform.localScale = new Vector3(scale, scale, scale);
            leaf.transform.Rotate(0, 0, Random.Range(i * 90, (i + 1) * 90));
            leaf.transform.parent = transform;
        }

    }
    
    void Start() {
        treeDescriptorString = lSystem.Generate();
        TreeData treeData = new(stepSize, angle, initRadius, radiusDecay);
        treeData.CreateTreeDataFromString(treeDescriptorString);

        Mesh mesh = treeData.GenerateMesh();
        GetComponent<MeshFilter>().mesh = mesh;

        List<LeafData> leafPositions = treeData.GetLeafPositions();
        foreach (LeafData leafData in leafPositions) {
            InstantiateLeaf(leafData);
        }
    }
}
