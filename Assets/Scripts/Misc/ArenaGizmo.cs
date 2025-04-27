using UnityEngine;

[ExecuteAlways]
public class ArenaGizmo : MonoBehaviour
{
    public Color gizmoColor = Color.green;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        
        Vector3 center = Vector3.zero;
        
        Vector3 size = new Vector3(Constants.arenaSize.x, Constants.arenaSize.y, 0.1f);

        Gizmos.DrawWireCube(center, size);
    }
}