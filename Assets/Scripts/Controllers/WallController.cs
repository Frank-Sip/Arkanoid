using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WallController", menuName = "GameObject/WallControllerSO")]
public class WallController : ScriptableObject
{
    [SerializeField] private WallSO wallConfig;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private AtlasApplier atlasApplier;

    private List<GameObject> wallInstances = new List<GameObject>();

    public void Init(List<Transform> wallParents)
    {
        if (wallParents == null || wallParents.Count == 0) return;

        foreach (var wall in wallInstances)
        {
            if (wall != null)
            {
                Destroy(wall);
            }
        }
        wallInstances.Clear();

        foreach (Transform wallParent in wallParents)
        {
            if (wallParent == null) continue;

            GameObject wallGO = Instantiate(wallPrefab, wallParent.position, wallParent.rotation, wallParent);
            wallGO.SetActive(false);

            Transform visual = wallGO.transform.GetChild(0);
            WallPhysics.Initiate(wallGO.transform, visual, wallConfig);

            if (atlasApplier != null)
            {
                atlasApplier.ApplyAtlas(visual.gameObject);
            }

            wallInstances.Add(wallGO);
        }
    }

    public void Activate()
    {
        foreach (var wall in wallInstances)
        {
            if (wall != null)
            {
                wall.SetActive(true);
            }
        }
    }

    public void Deactivate()
    {
        foreach (var wall in wallInstances)
        {
            if (wall != null)
            {
                wall.SetActive(false);
            }
        }
    }
}