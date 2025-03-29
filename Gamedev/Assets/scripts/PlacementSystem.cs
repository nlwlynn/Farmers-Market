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

    private Dictionary<Vector3Int, GameObject> placedObjects = new Dictionary<Vector3Int, GameObject>();

    private void Start()
    {
        StopPlacement();

        // preplaced carrot plot
        Vector3 preplacedWorldPos = new Vector3(142.35f, 0f, 120.98f); 
        Vector3Int preplacedGridPos = grid.WorldToCell(preplacedWorldPos);

        GameObject preplacedObject = GameObject.Find("carrot-plot"); 

        if (!placedObjects.ContainsKey(preplacedGridPos))
        {
            placedObjects[preplacedGridPos] = preplacedObject;
            Debug.Log($"Preplaced object registered at Grid Position: {preplacedGridPos}");
        }
    }


    public void StartPlacement(int ID)
    {
        Debug.Log($"StartPlacement called with ID: {ID}");

        isBuilding = true;

        // Ensure we find the correct stock index in inventory
        int itemIndex = database.objectsData.FindIndex(obj => obj.ID == ID);

        if (itemIndex < 0 || itemIndex >= inventory.stock.Length)
        {
            Debug.LogError($"No valid inventory stock entry for ID: {ID} (Mapped Index: {itemIndex})");
            return;
        }

        Debug.Log($"Checking stock for itemIndex: {itemIndex}, Current Stock: {inventory.stock[itemIndex]}");

        if (inventory.stock[itemIndex] <= 0)
        {
            Debug.Log("Not enough stock to place this item!");
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
    }

    public void StopPlacementWrapper()
    {
        StopPlacement();
    }

    //private void PlaceStructure()
    //{

    //    if (inputManager.IsPointerOverUI())
    //    {
    //        Debug.Log("Pointer is over UI, skipping placement.");
    //        return;
    //    }
    //    Vector3 mousePosition = inputManager.GetSelectedMapPosition();

    //    //mousePosition.y = 0f;

    //    Vector3Int gridPosition = grid.WorldToCell(mousePosition);

    //    Debug.Log($"Mouse Position: {mousePosition}, Grid Position: {gridPosition}");
    //    //Vector3 worldPosition = grid.CellToWorld(gridPosition);
    //    GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
    //    //mouseIndicator.transform.position = mousePosition;
    //    //cellIndicator.transform.position = worldPosition;
    //    newObject.transform.position = grid.CellToWorld(gridPosition);
    //}

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
            Debug.Log("Placing structure, ignoring UI click blocking.");

            Vector3 mousePosition = inputManager.GetSelectedMapPosition();
            Vector3Int gridPosition = grid.WorldToCell(mousePosition);

            Debug.Log($"Mouse Position: {mousePosition}, Grid Position: {gridPosition}");

            if (selectedObjectIndex < 0 || selectedObjectIndex >= database.objectsData.Count)
            {
                Debug.LogError("Invalid object index! Cannot place structure.");
                return;
            }

            // Ensure stock is available before placing
            if (inventory.stock[selectedObjectIndex] <= 0)
            {
                return;
            }

            // checks if spot is taken
            if (placedObjects.ContainsKey(gridPosition))
            {
                return;
            }

            // Deduct stock
            inventory.stock[selectedObjectIndex]--;
            inventory.UpdateStockUI(); // Refresh UI immediately

            Debug.Log($"Placed object at {grid.CellToWorld(gridPosition)}. Remaining stock: {inventory.stock[selectedObjectIndex]}");

            // Instantiate and track the placed object
            GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
            newObject.transform.position = grid.CellToWorld(gridPosition);
            placedObjects[gridPosition] = newObject;

            // Re-enable UI raycasts after placement
            SetUIRaycasts(true);
            StopPlacement();
            isBuilding = false;
        }
    }

    public void RemoveObject()
    {
        isBuilding = true;
        // Get mouse position and convert to grid position
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        Debug.Log($"Trying to remove object at {gridPosition}");

        // Check if there is an object at the selected position
        if (placedObjects.ContainsKey(gridPosition))
        {
            GameObject objectToRemove = placedObjects[gridPosition];

            // Find which object type it is by checking the prefab database
            int objectID = database.objectsData.FindIndex(obj => obj.Prefab.name == objectToRemove.name.Replace("(Clone)", "").Trim());

            if (objectID >= 0)
            {
                Debug.Log($"Removing object: {objectToRemove.name} (ID: {objectID})");

                // Return the object to inventory
                inventory.stock[objectID]++;
                inventory.UpdateStockUI();

                // Destroy the object
                Destroy(objectToRemove);

                // Remove from tracking dictionary
                placedObjects.Remove(gridPosition);
                isBuilding = false;
            }
        }
        else
        {
            Debug.Log("No object found at this position to remove.");
        }
    }

    public void OnRemoveButtonClicked()
    {
        StartRemoval();
    }

    public void StartRemoval()
    {
        Debug.Log("StartRemoval called");

        isBuilding = true;
        gridVisualization.SetActive(true);
        cellIndicator.SetActive(true);
        mouseIndicator.SetActive(true);

        inputManager.OnClicked += RemoveStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void RemoveStructure()
    {
        if (!inputManager.IsPointerOverUI())
        {
            Vector3 mousePosition = inputManager.GetSelectedMapPosition();
            Vector3Int gridPosition = grid.WorldToCell(mousePosition);

            Debug.Log($"Mouse Position: {mousePosition}, Grid Position: {gridPosition}");

            if (placedObjects.ContainsKey(gridPosition))
            {
                GameObject objectToRemove = placedObjects[gridPosition];
                int objectID = database.objectsData.FindIndex(obj => obj.Prefab.name == objectToRemove.name.Replace("(Clone)", "").Trim());

                if (objectID >= 0)
                {
                    Debug.Log($"Removing object: {objectToRemove.name} (ID: {objectID})");

                    inventory.stock[objectID]++;
                    inventory.UpdateStockUI();

                    Destroy(objectToRemove);
                    placedObjects.Remove(gridPosition);
                }
            }
            else
            {
                Debug.Log("No object found at this position to remove.");
            }

            // Only stop removal mode if the user intends to exit
            // StopPlacement(); <- Remove or handle it elsewhere
        }
    }

    private void Update()
    {
        if (!isBuilding) return; // Only update if we are in placement or removal mode

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (mouseIndicator != null)
            mouseIndicator.transform.position = mousePosition;

        if (cellIndicator != null)
            cellIndicator.transform.position = grid.CellToWorld(gridPosition);
    }

}
