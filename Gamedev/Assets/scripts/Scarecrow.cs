using System.Collections.Generic;
using UnityEngine;

public class Scarecrow : MonoBehaviour
{
    public GameObject defenseHighlightPrefab; // assign in inspector or set via code
    private List<GameObject> highlightTiles = new List<GameObject>();

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
            Vector3 worldPos = grid.CellToWorld(tile) + new Vector3(0.5f, 0.01f, 0.5f); // centered & slightly above

            // Defend crop
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

    // Optional: remove highlight tiles if the scarecrow is destroyed
    private void OnDestroy()
    {
        foreach (GameObject tile in highlightTiles)
        {
            if (tile != null)
                Destroy(tile);
        }
    }
}
