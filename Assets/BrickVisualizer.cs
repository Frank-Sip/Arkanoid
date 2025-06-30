using UnityEngine;
using UnityEditor;

public class BrickVisualizer : MonoBehaviour
{
    [SerializeField] private BrickController brickControllerSO;
    [SerializeField] private Color gizmoColor = new Color(0.5f, 0.8f, 1f, 0.4f);
    [SerializeField] private bool showWireframe = true;

    private void OnDrawGizmos()
    {
        if (brickControllerSO == null)
            return;

        // Obtener las dimensiones del ladrillo desde el SO
        Vector2 brickSize = GetBrickSize();

        // Configurar color del gizmo
        Gizmos.color = gizmoColor;

        // Dibujar un cubo para cada hijo
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                Vector3 position = child.position;
                Vector3 size = new Vector3(brickSize.x, brickSize.y, 0.1f);

                if (showWireframe)
                    Gizmos.DrawWireCube(position, size);
                else
                    Gizmos.DrawCube(position, size);

#if UNITY_EDITOR
                Handles.Label(position, child.name);
#endif
            }
        }
    }

    private Vector2 GetBrickSize()
    {
        // Acceder a brickConfig (el BrickSO) desde el BrickController
        if (brickControllerSO != null && brickControllerSO.brickConfig != null)
        {
            // BrickSO tiene width y height como propiedades
            return new Vector2(
                brickControllerSO.brickConfig.width,
                brickControllerSO.brickConfig.height
            );
        }

        // Valor predeterminado si no se puede determinar
        return new Vector2(1f, 0.5f);
    }
}