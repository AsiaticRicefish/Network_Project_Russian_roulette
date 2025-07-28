using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxClickTrigger : MonoBehaviour
{
    private void OnMouseDown()
    {
        var itemSync = FindObjectOfType<ItemSync>();
        itemSync.BoxOpen();
    }
}