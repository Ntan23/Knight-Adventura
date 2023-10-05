using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activator : MonoBehaviour
{
    [SerializeField] float scaleSpeed;
    [SerializeField] GameObject objectToActivate;
    float yScale;
    public bool isCollide;
    private void Awake()
    {
        yScale = this.gameObject.transform.localScale.y;
    }

    private void Update()
    {
        if(isCollide)
        {
            return;
        }
        else if(!isCollide)
        {
            StartCoroutine(UnpressButton());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Object") || other.name == "Crates")
        {
            isCollide = true;
            StartCoroutine(PressButton());
            objectToActivate.SetActive(true);
        }
    }

    IEnumerator PressButton()
    {
        while(this.gameObject.transform.localScale.y > 0.1f)
        {
            this.gameObject.transform.localScale -= new Vector3(0,scaleSpeed,0) * Time.deltaTime;
            if(this.gameObject.transform.localScale.y == 0)
            {
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator UnpressButton()
    {
        while(this.gameObject.transform.localScale.y < yScale)
        {
            this.gameObject.transform.localScale += new Vector3(0,scaleSpeed,0) * Time.deltaTime;
            if(this.gameObject.transform.localScale.y == yScale)
            {
                yield break;
            }
            yield return null;
        }
        objectToActivate.SetActive(false);
    }
}
