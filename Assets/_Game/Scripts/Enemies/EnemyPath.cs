using UnityEngine;

namespace TowerDefense
{
    // The route enemies walk. Every child of this object is one waypoint, and
    // enemies visit them in Hierarchy order (top to bottom). Reorder by dragging.
    public class EnemyPath : MonoBehaviour
    {
        // A property: read like a field from outside, but it runs code. Read-only here.
        public int WaypointCount
        {
            get { return transform.childCount; }
        }

        // World position of the waypoint at 'index' (0 = the first one).
        public Vector3 GetWaypoint(int index)
        {
            return transform.GetChild(index).position;
        }

        // Editor-only: Unity calls this to draw helper shapes in the Scene view.
        // Nothing here exists in the real game; it only makes the path visible to us.
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < transform.childCount; i++)
            {
                Vector3 point = transform.GetChild(i).position;
                Gizmos.DrawSphere(point, 0.15f);
                if (i > 0)
                {
                    Gizmos.DrawLine(transform.GetChild(i - 1).position, point);
                }
            }
        }
    }
}
