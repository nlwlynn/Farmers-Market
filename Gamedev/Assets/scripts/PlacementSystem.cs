using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject mouseIndicator, cellIndicator;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;
    [SerializeField]
    private ObjectsDatabaseSO database;
    private int selectedObjectIndex = -1;
    [SerializeField]
    private GameObject gridVisualization;
    [SerializeField]
    private GameObject inventoryPanel;
    public Inventory inventory; // Reference to the Inventory Manager
    public bool isBuilding = false;
    public int itemID = -1;
    private List<GameObject> scarecrowPreviewTiles = new List<GameObject>();
    [SerializeField] private GameObject scarecrowDefenseHighlightPrefab; // assign in Inspecto

    private Dictionary<Vector3Int, GameObject> placedObjects = new Dictionary<Vector3Int, GameObject>();


    private void Start()
    {
        StopPlacement();

        // preplaced carrot plot
        Vector3 preplacedWorldPos = new Vector3(142.35f, 0f, 120.98f);
        Vector3Int preplacedGridPos = grid.WorldToCell(preplacedWorldPos);

        GameObject preplacedObject = GameObject.Find("carrot-plot-perm");

        if (!placedObjects.ContainsKey(preplacedGridPos))
        {
            Debug.Log("found plot");
            placedObjects[preplacedGridPos] = preplacedObject;
        }
    }

    public void StartPlacement(int ID)
    {
        itemID = ID;
        isBuilding = true;

        // Ensure we find the correct stock index in inventory
        int itemIndex = database.objectsData.FindIndex(obj => obj.ID == ID);

        if (itemIndex < 0 || itemIndex >= inventory.stock.Length)
        {
            return;
        }

        if (inventory.stock[itemIndex] <= 0)
        {
            return;
        }

        StopPlacement();
        selectedObjectIndex = itemIndex;

        InputManager.ignoreNextUIInteraction = true;
        SetUIRaycasts(false);

        gridVisualization.SetActive(true);
        cellIndicator.SetActive(true);
        mouseIndicator.SetActive(true);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        mouseIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        ClearScarecrowPreview();
    }

    public void StopPlacementWrapper()
    {
        StopPlacement();
    }

    private void SetUIRaycasts(bool enable)
    {
        CanvasGroup canvasGroup = inventoryPanel.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = enable;
        }
    }

    private void PlaceStructure()
    {
        if (!inputManager.IsPointerOverUI())
        {
            Vector3 mousePosition = inputManager.GetSelectedMapPosition();
            Vector3Int gridPosition = grid.WorldToCell(mousePosition);

            if (IsWithinGridBoundaries(gridPosition)) // Only place if within boundaries
            {
                if (selectedObjectIndex < 0 || selectedObjectIndex >= database.objectsData.Count)
                {
                    return;
                }

                // Ensure stock is available before placing
                if (inventory.stock[selectedObjectIndex] <= 0)
                {
                    return;
                }

                // Check if spot is taken
                if (placedObjects.ContainsKey(gridPosition))
                {
                    return;
                }

                // Deduct stock
                inventory.stock[selectedObjectIndex]--;
                inventory.UpdateStockUI(); // Refresh UI immediately

                // Instantiate and track the placed object
                GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
                newObject.transform.position = grid.CellToWorld(gridPosition);
                placedObjects[gridPosition] = newObject;

                // Initialize HelperNPC if it exists on the placed object
                HelperNPC helper = newObject.GetComponent<HelperNPC>();
                if (helper != null)
                {
                    Transform spawnTransform = GameObject.Find("Helper-Origin")?.transform;
                    Face faceSO = Resources.Load<Face>("DataFace"); // Or assign from Inspector via serialized field
                    GameObject smileBodyObj = newObject.transform.Find("Slime_03")?.gameObject;
                    UIController controller = FindObjectOfType<UIController>();

                    helper.Initialize(spawnTransform, faceSO, smileBodyObj, controller);
                }
                Scarecrow scarecrow = newObject.GetComponent<Scarecrow>();
                if (scarecrow != null)
                {
                    UnityEngine.Debug.Log("Scarecrow component found, calling DefendNearbyCrops()");
                    Vector3Int scarecrowGridPos = grid.WorldToCell(newObject.transform.position);
                    scarecrow.DefendNearbyCrops(scarecrowGridPos, grid);
                }
                else
                {
                    UnityEngine.Debug.LogWarning("Scarecrow component NOT FOUND on placed object.");
                }
                // Stop placement only after a successful placement
                StopPlacement();
                isBuilding = false;
                itemID = -1;
                ClearScarecrowPreview();

            }
            else
            {
                StartPlacement(itemID);
            }
        }
    }

    public void OnRemoveButtonClicked()
    {
        if (isBuilding)
        {
            StopRemoval();
            isBuilding = false;

            if (GetComponent<InputManager>() != null)
            {
                GetComponent<InputManager>().SwitchCameraOff();
            }
            else
            {
                InputManager inputManager = FindObjectOfType<InputManager>();
                if (inputManager != null)
                {
                    inputManager.SwitchCameraOff();
                }
            }
        }
        else
        {
            InputManager.ignoreNextUIInteraction = true;
            StartRemoval();
        }
    }

    public void StartRemoval()
    {
        isBuilding = true;
        gridVisualization.SetActive(true);
        cellIndicator.SetActive(true);
        mouseIndicator.SetActive(true);

        inputManager.OnClicked += RemoveStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void StopRemoval()
    {
        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        mouseIndicator.SetActive(false);

        inputManager.OnClicked -= RemoveStructure;
        inputManager.OnExit -= StopPlacement;

        isBuilding = false;
    }

    private void RemoveStructure()
    {
        // Allow one UI click to be ignored
        /*if (InputManager.ignoreNextUIInteraction)
        {
            InputManager.ignoreNextUIInteraction = false; // Reset after one click
        }
        else if (inputManager.IsPointerOverUI())
        {
            Debug.Log("Click ignored: Pointer is over UI");
            return;
        } */

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (!IsWithinGridBoundaries(gridPosition))
        {
            return;
        }

        if (placedObjects.ContainsKey(gridPosition))
        {
            GameObject objectToRemove = placedObjects[gridPosition];
            int objectID = database.objectsData.FindIndex(obj => obj.Prefab.name == objectToRemove.name.Replace("(Clone)", "").Trim());

            if (objectID >= 0)
            {
                inventory.stock[objectID]++;
                inventory.UpdateStockUI();

                Destroy(objectToRemove);
                placedObjects.Remove(gridPosition);
                StopRemoval();
                isBuilding = false;
            }
        }
        else
        {
            Debug.Log("No object found.");
        }
    }

    private void Update()
    {
        if (!isBuilding) return; // Only update if we are in placement or removal mode

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (isBuilding && itemID == 7) // Assuming ID 7 is scarecrow
        {
            UpdateScarecrowPreview(gridPosition);
        }

        if (mouseIndicator != null)
            mouseIndicator.transform.position = mousePosition;

        if (cellIndicator != null)
        {
            cellIndicator.transform.position = grid.CellToWorld(gridPosition);
        }

        if(IsWithinGridBoundaries(gridPosition)) {
            inBounds();
        } else
        {
            outOfBounds();
        }
    }

    private bool IsWithinGridBoundaries(Vector3Int gridPosition)
    {
        // Define grid boundaries
        int minX = -3;
        int minY = -3;

        // You may want to set max boundaries too if needed
        // int maxX = someValue;
        // int maxZ = someValue;

        return gridPosition.x >= minX && gridPosition.y >= minY;
    }

    private void outOfBounds()
    {
        cellIndicator.SetActive(false);
        mouseIndicator.SetActive(false);
    }

    private void inBounds()
    {
        cellIndicator.SetActive(true);
        mouseIndicator.SetActive(true);
    }

    private void UpdateScarecrowPreview(Vector3Int centerGridPos)
    {
        ClearScarecrowPreview();

        Vector3Int[] offsets = new Vector3Int[]
        {
        new Vector3Int(-1, 1, 0), new Vector3Int(0, 1, 0), new Vector3Int(1, 1, 0),
        new Vector3Int(-1, 0, 0),                       new Vector3Int(1, 0, 0),
        new Vector3Int(-1, -1, 0), new Vector3Int(0, -1, 0), new Vector3Int(1, -1, 0),
        };

        foreach (Vector3Int offset in offsets)
        {
            Vector3Int targetPos = centerGridPos + offset;
            Vector3 worldPos = grid.CellToWorld(targetPos) + new Vector3(0.5f, 0.01f, 0.5f);

            GameObject marker = Instantiate(scarecrowDefenseHighlightPrefab, worldPos, Quaternion.identity);
            marker.transform.localScale = Vector3.one * 1f;
            scarecrowPreviewTiles.Add(marker);
        }
    }

    private void ClearScarecrowPreview()
    {
        foreach (var obj in scarecrowPreviewTiles)
        {
            if (obj != null)
                Destroy(obj);
        }
        scarecrowPreviewTiles.Clear();
    }
    public void stopAll()
    {
        StopRemoval();
        StopPlacement();
    }
}
