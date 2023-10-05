using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] GameObject door;
    [SerializeField] GameObject chest;
    public GameObject dropPosition;
    float progress;
    public float speed;
    [SerializeField] float arcHeight;
    InteractionSystem interact;
    Chest chestScript;
    private void Awake() 
    {
        interact=FindObjectOfType<InteractionSystem>();
    }

    public void change()
    {
        interact.examineText.SetActive(false);
        if(door != null)
        {
            door.GetComponent<Item>().type=Item.InteractionType.None;
            door.tag="Untagged";
        }
        chest.GetComponent<Chest>().isCollect = true;
    }

    public void CheckChestScript()
    {
        if(chestScript == null)
        {
            chestScript = GetComponentInParent<Chest>();
        }
        else
        {
            return;
        }
    }

    public void PlayAnimation()
    {
        //Increment our progress from 0 at the start, to 1 when we arrive.
        progress = Mathf.Min(progress + Time.deltaTime*chestScript.stepScale ,1.0f);

        // Turn this 0-1 value into a parabola that goes from 0 to 1, then back to 0.
        float parabola = 1.0f - 4.0f * (progress - 0.5f) * (progress - 0.5f);

        // Travel in a straight line from our start position to the target.        
        Vector3 nextPos = Vector3.Lerp(chestScript.startPosition,dropPosition.transform.position,progress);

        // Then add a vertical arc in excess of this.
        nextPos.y += parabola * arcHeight;

        transform.position = nextPos;

        // I presume you return the object so it doesn't keep arriving.
        if(progress == 1.0f)
        {
            return;
        }
    }
}
