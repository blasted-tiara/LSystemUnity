using UnityEngine;

public class Turtle : MonoBehaviour {
    [SerializeField]
    private LSystem lSystem;
    // Start is called before the first frame update

    void Start() {
        string generationResult = lSystem.Generate();
        Debug.Log(generationResult);
    }

}
