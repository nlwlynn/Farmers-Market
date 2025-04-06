using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;
    [SerializeField]
    private Camera buildCamera;

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayermask;

    public event Action OnClicked, OnExit;

    private EventSystem eventSystem;
    public PlacementSystem placementSystem;
    public ShopManager shopManager;


    void Awake()
    {
        eventSystem = EventSystem.current;
        if (buildCamera != null)
            buildCamera.gameObject.SetActive(false);

        if (shopManager == null)
        {
            Debug.LogError("shopManager reference is missing in InputManager!");
        }
    }

    private void Update()
    {
        if (placementSystem.isBuilding)
        {
            SwitchCameraOn();
        }
        if (!placementSystem.isBuilding && shopManager.closeBuild)
        {
            Debug.Log("input test");
            SwitchCameraOff();
        }
        if (Input.GetMouseButtonDown(0))
            OnClicked?.Invoke();
        if (Input.GetKeyDown(KeyCode.Escape))
            OnExit?.Invoke();

    }
    public static bool ignoreNextUIInteraction = false;

    public bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        if (ignoreNextUIInteraction)
        {
            Debug.Log("Ignoring UI click (temporary override)");
            ignoreNextUIInteraction = false; // Reset after bypassing UI for one click
            return false;
        }

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        bool isOverUI = results.Count > 0;
        Debug.Log($"Pointer Over UI: {isOverUI}");
        return isOverUI;
    }



    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = buildCamera.nearClipPlane;
        Ray ray = buildCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, placementLayermask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }

    public void SwitchCameraOn()
    {
        if (sceneCamera != null && buildCamera != null)
        {
            sceneCamera.gameObject.SetActive(false);
            buildCamera.gameObject.SetActive(true);
        }
    }

    public void SwitchCameraOff()
    {
        if (sceneCamera != null && buildCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
            buildCamera.gameObject.SetActive(false);

            if (shopManager != null)
            {
                shopManager.closeBuild = false;
            }
        }
    }

}