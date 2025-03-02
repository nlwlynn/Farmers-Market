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

    private void Start()
    {
        StopPlacement();
    }

    public void StartPlacement(int ID)
    {
        Debug.Log($"StartPlacement called with ID: {ID}");
        StopPlacement();

        selectedObjectIndex = database.objectsData.FindIndex(database => database.ID == ID);
        if (selectedObjectIndex < 0)
        {
            Debug.LogError($"No ID found for {ID}");
            return;
        }

        // Allow one UI click bypass after selecting an object
        InputManager.ignoreNextUIInteraction = true;

        // Disable UI raycasts during placement
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

            GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
            newObject.transform.position = grid.CellToWorld(gridPosition);

            Debug.Log($"Placed object at {grid.CellToWorld(gridPosition)}");

            // Re-enable UI raycasts after placement
            SetUIRaycasts(true);
            StopPlacement();
        }
        else
        {
            Debug.Log("Pointer is over UI, skipping placement.");
        }
    }
    private void Update()
    {
      if(selectedObjectIndex < 0)
      return;
       
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        mouseIndicator.transform.position = mousePosition;
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);
    }
}
