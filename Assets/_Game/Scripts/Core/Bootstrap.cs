using UnityEngine;

namespace TowerDefense
{
    // A MonoBehaviour is a script that Unity can attach to a GameObject in a scene.
    // Unity calls Start() once, just before the first frame this object is alive.
    public class Bootstrap : MonoBehaviour
    {
        // [SerializeField] shows a private field in the Inspector so you can change
        // the value without touching code. The Inspector value wins over this default.
        [SerializeField] private string message = "Hello from the estate!";

        private void Start()
        {
            // Debug.Log prints to the Console window (Cmd+Shift+C).
            Debug.Log(message);
        }
    }
}
