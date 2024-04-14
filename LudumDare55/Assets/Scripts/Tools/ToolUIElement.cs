using UnityEngine;
using TMPro;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolUIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject toolPrefab;
    public TMP_Text textArea;
    public string toolName = "Default";
    public Image uiImage;
    public bool useSprites = false;
    public Sprite onHoverEnterSprite;
    public Sprite onHoverExitSprite;
    [Tooltip("This is the text that shows up on the textArea of the for the tool when you highlight it")]
    public Action<GameObject> OnHover;


    private void OnDisable()
    {
        Highlight(false);
    }

    public void Highlight(bool toggle)
    {
        if (toggle)
        {
            if (uiImage != null)
            {
                if (useSprites)
                {
                    uiImage.sprite = onHoverEnterSprite;
                }
                else
                {
                    uiImage.color = Color.red;
                }
            }

            textArea.SetText(toolName);

            OnHover?.Invoke(toolPrefab);

        }
        else
        {
            if (uiImage != null)
            {
                if (useSprites)
                {
                    uiImage.sprite = onHoverExitSprite;
                }
                else
                {
                    uiImage.color = Color.white;
                }
            }

            textArea.SetText("");

            OnHover?.Invoke(null);
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        Highlight(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Highlight(false);
    }
}
