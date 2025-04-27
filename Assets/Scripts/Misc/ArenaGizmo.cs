using UnityEngine;

[ExecuteAlways]
public class ArenaGizmo : MonoBehaviour
{
    public Color gizmoColor = Color.green;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        
        // Center of arena at (0,0)
        Vector3 center = Vector3.zero;
        
        // Adjust the size: give Z at least a small value like 0.1f so it shows properly
        Vector3 size = new Vector3(Constants.arenaSize.x, Constants.arenaSize.y, 0.1f);

        // Draw the arena boundary
        Gizmos.DrawWireCube(center, size);
    }
}