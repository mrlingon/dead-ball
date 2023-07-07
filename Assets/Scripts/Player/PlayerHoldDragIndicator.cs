using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHoldDragIndicator : MonoBehaviour
{
    [NaughtyAttributes.Required]
    public PlayerHoldDrag HoldDrag;

    [NaughtyAttributes.Required]
    public SpriteRenderer DragStartIndicator;

    [NaughtyAttributes.Required]
    public SpriteRenderer DragEndIndicator;

    private LineRenderer LineRenderer;

    void Awake()
    {
        LineRenderer = GetComponentInChildren<LineRenderer>();
        LineRenderer.Reset();

        HoldDrag.StartDrag += () =>
        {
            LineRenderer.positionCount = 2;
            DragStartIndicator.gameObject.SetActive(true);
            DragEndIndicator.gameObject.SetActive(true);
        };

        HoldDrag.Released += (d) =>
        {
            DragStartIndicator.gameObject.SetActive(false);
            DragEndIndicator.gameObject.SetActive(false);
            LineRenderer.Reset();
        };
    }

    void Update()
    {
        if (!HoldDrag.IsDragging) return;

        LineRenderer.SetPosition(0, HoldDrag.DragPointsInWorld.Origin);
        LineRenderer.SetPosition(1, HoldDrag.DragPointsInWorld.Current);

        DragStartIndicator.transform.position = HoldDrag.DragPointsInWorld.Origin;
        DragEndIndicator.transform.position = HoldDrag.DragPointsInWorld.Current;
    }
}
