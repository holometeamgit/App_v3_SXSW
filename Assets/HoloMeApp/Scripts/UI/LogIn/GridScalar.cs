using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class GridScalar : UIBehaviour {

    public enum Fit {
        Horizontal
    }

    public Fit FitBy;


    private GridLayoutGroup grid;
    private RectOffset gridPadding;
    private RectTransform parent;

    Vector2 lastSize;

    protected override void OnRectTransformDimensionsChange() {
        UpdateSize();
    }

    private void UpdateSize() {
        grid = GetComponent<GridLayoutGroup>();
        parent = GetComponent<RectTransform>();
        gridPadding = grid.padding;
        lastSize = Vector2.zero;
        if (lastSize == parent.rect.size) {
            return;
        }
        switch (FitBy) {
        case Fit.Horizontal:
            var paddingX = gridPadding.left + gridPadding.right;
//            Debug.Log("name " + parent.gameObject.name + " " + parent.rect.width);
            var cellSize = Mathf.Round((parent.rect.width - paddingX - (grid.spacing.x * (grid.constraintCount - 1))) / grid.constraintCount);
            grid.cellSize = new Vector2(cellSize, grid.cellSize.y);
            break;
        }
    }
}