using UnityEngine;

[ExecuteAlways]
public class ArenaGizmo : MonoBehaviour
{
    public Vector2 arenaSize = new Vector2(50, 40);
    public Color gizmoColor = Color.green;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Vector3 size = new Vector3(arenaSize.x, arenaSize.y, 0.1f);
        Gizmos.DrawWireCube(Vector3.zero, size);
    }
}
