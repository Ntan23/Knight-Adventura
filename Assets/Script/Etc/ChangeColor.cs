using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColor : MonoBehaviour
{
    public void change()
    {
        GetComponent<Image>().color=new Color32(195,195,195,255);
    }

    public void unchange()
    {
        GetComponent<Image>().color=new Color32(255,255,255,255);
    }
}
