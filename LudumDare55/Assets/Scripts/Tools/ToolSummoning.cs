using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSummoning : MonoBehaviour
{
    [SerializeField] private GameObject toolWheelUI;
    public List<ToolUIElement> toolUIElements;

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float distanceToSpawnTool = 1f;

    [Header("Debug")]
    [SerializeField] private GameObject currentToolToSummon;


    private void Start()
    {
        // Disable UI on start
        OpenToolWheel(false);
    }

    private void OnEnable()
    {
        foreach (var element in toolUIElements)
        {
            element.OnHover += SetToolToSummon;
        }
    }

    private void OnDisable()
    {
        foreach (var element in toolUIElements)
        {
            element.OnHover -= SetToolToSummon;
        }
    }

    private void Update()
    {
        // Can be changed to whatever later
        if (Input.GetKeyDown(KeyCode.T))
        {
            OpenToolWheel(true);
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            SummonTool();
            OpenToolWheel(false);
        }
    }

    private void OpenToolWheel(bool toggle)
    {
        toolWheelUI.SetActive(toggle);
        //Cursor.visible = toggle;
        if (toggle)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void SetToolToSummon(GameObject toolPrefab)
    {
        currentToolToSummon = toolPrefab;
    }

    private void SummonTool()
    {
        if (currentToolToSummon == null) return;

        var summonPos = cameraTransform == null ? transform.position : cameraTransform.position + cameraTransform.forward * distanceToSpawnTool;
        Instantiate(currentToolToSummon, summonPos, Quaternion.identity);
    }
}
