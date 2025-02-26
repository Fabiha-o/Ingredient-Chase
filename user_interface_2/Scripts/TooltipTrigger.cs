using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea] public string tooltipMessage = "Click 'k' to collect ingredient";

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.Show(tooltipMessage);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Hide();
    }
}
