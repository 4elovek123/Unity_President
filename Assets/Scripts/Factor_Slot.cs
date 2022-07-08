using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.UI;

public class Factor_Slot : MonoBehaviour, IDropHandler
{
    private string DragItemName;
    public JSONController jSONController; 
    public bool _targetDragIsTrue;

    public void OnDrop(PointerEventData eventData) // при перетаскивании Фактора
    {
        _targetDragIsTrue = true; 
    } 
}