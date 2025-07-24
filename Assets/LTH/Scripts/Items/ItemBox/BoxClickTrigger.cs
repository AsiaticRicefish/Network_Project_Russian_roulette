using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxClickTrigger : MonoBehaviour
{
    private void OnMouseDown()
    {
        ItemBoxManager.Instance.OnBoxClicked();
    }
}