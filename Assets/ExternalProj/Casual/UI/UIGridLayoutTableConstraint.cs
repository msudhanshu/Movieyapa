//
//  UIGridLayoutTableConstraint.cs
//
//  Author:
//       Manjeet <msudhanshu@kiwiup.com>
//
using UnityEngine;

public class UIGridLayoutTableConstraint : MonoBehaviour
{
	public void Constraint() {
		/*
		RectTransform t = gridLayoutGroup.GetComponent<RectTransform>();
		if(horizontal) {
			scrollRect.horizontal = true;
			scrollRect.vertical = false;
			gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Vertical;
			t.sizeDelta = new Vector2( (float)( Mathf.Ceil((float)i/(float)rowCount) )* (gridLayoutGroup.cellSize.x+gridLayoutGroup.spacing.x), 
			                          rowCount*(gridLayoutGroup.cellSize.y+gridLayoutGroup.spacing.y) );
		} else {
			scrollRect.horizontal = false;
			scrollRect.vertical = true;
			gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
			t.sizeDelta = new Vector2(colCount * (gridLayoutGroup.cellSize.x+gridLayoutGroup.spacing.x), (float)( Mathf.Ceil((float)i/(float)colCount) )*(gridLayoutGroup.cellSize.y+gridLayoutGroup.spacing.y));  
        }
        //TODO : CASE WHEN ONE WANTS TO SCROLL BOTH THE WAY....
        */
	}
    
}
