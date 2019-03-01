using UnityEngine;
using UnityEngine.SceneManagement;
public class DontDestroy : MonoBehaviour {
    [SerializeField]
    string tagToSearch;

    void Awake () {
        GameObject[] objs = GameObject.FindGameObjectsWithTag (tagToSearch);

        if (objs.Length > 1) {
            Destroy (this.gameObject);
        }

        DontDestroyOnLoad (this.gameObject);
    }
}