using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scarecrow : MonoBehaviour
{
    public GameObject defenseHighlightPrefab; // Optional visual effect
    private List<GameObject> highlightTiles = new List<GameObject>();

    [SerializeField] private float checkInterval = 3f;
    [SerializeField] private Grid grid;

    private void Start()
    {
        if (grid == null)
            grid = FindObjectOfType<Grid>();

        Vector3Int gridPos = grid.WorldToCell(transform.position);
        StartCoroutine(PeriodicCropCheck(gridPos));
    }

    private IEnumerator PeriodicCropCheck(Vector3Int centerGridPos)
    {
        while (true)
        {
            DefendNearbyCrops(centerGridPos, grid);
            yield return new WaitForSeconds(checkInterval);
        }
    }

    public void DefendNearbyCrops(Vector3Int centerGridPos, Grid grid)
    {
        Vector3Int[] surroundingOffsets = new Vector3Int[]
        {
            new Vector3Int(-1,  1, 0), new Vector3Int(0,  1, 0), new Vector3Int(1,  1, 0),
            new Vector3Int(-1,  0, 0),                       new Vector3Int(1,  0, 0),
            new Vector3Int(-1, -1, 0), new Vector3Int(0, -1, 0), new Vector3Int(1, -1, 0),
        };

        foreach (Vector3Int offset in surroundingOffsets)
        {
            Vector3Int tile = centerGridPos + offset;
            Vector3 worldPos = grid.CellToWorld(tile) + new Vector3(0.5f, 0.01f, 0.5f); // center it

            // Detect crops
            Collider[] hits = Physics.OverlapSphere(worldPos, 0.4f);
            foreach (var hit in hits)
            {
                CropProtection crop = hit.GetComponent<CropProtection>();
                if (crop != null)
                {
                    crop.SetProtected(true);
                }
            }
        }
    }

    private void OnDestroy()
    {
        foreach (GameObject tile in highlightTiles)
        {
            if (tile != null)
                Destroy(tile);
        }
    }
}
