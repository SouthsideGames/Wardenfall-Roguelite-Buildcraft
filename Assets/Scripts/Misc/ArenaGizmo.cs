using UnityEngine;

public class ArenaGizmo : MonoBehaviour
{
    // Set Gizmo color
    public Color gizmoColor = Color.green;

    private void OnDrawGizmos()
    {
        // Set Gizmo color
        Gizmos.color = gizmoColor;

        // Calculate the center and size from Constants
        Vector3 center = new Vector3(Constants.arenaSize.x / 2, Constants.arenaSize.y / 2, 0);
        Vector3 size = new Vector3(Constants.arenaSize.x, Constants.arenaSize.y, 0);

        // Draw the arena as a wireframe cube
        Gizmos.DrawWireCube(center, size);
    }
}
