using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSummoning : MonoBehaviour
{
    public GameObject toolWheelUI;
    public List<ToolUIElement> toolUIElements;
    [SerializeField] private float zOffsetToSpawnTool = 5f;
    [SerializeField] private float yOffsetToSpawnTool = 5f;

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
        Cursor.visible = toggle;
    }

    private void SetToolToSummon(GameObject toolPrefab)
    {
        currentToolToSummon = toolPrefab;
    }

    private void SummonTool()
    {
        if (currentToolToSummon == null) return;

        var offset = new Vector3(0, yOffsetToSpawnTool, zOffsetToSpawnTool);
        var summonPos = transform.position + offset;
        Instantiate(currentToolToSummon, summonPos, Quaternion.identity);
    }
}
