using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipObjectUpdater : MonoBehaviour
{
    [SerializeField] private TableChipHandler chipHandler;
    
    public int chips;
    public int idx;
    public TableChipHandler.ContentType contentType;

    public void UpdateChipContents()
    {
        chipHandler.UpdateChips(contentType, idx, chips);
    }
}
