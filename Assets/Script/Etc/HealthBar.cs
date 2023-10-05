using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    Vector3 enemyPosition;
    public Transform fillBar;
    [SerializeField] GameObject barSprite;

    private void Update() 
    {
        enemyPosition=enemy.transform.position;
        this.transform.position=enemyPosition+new Vector3(0.0f,0.25f,0.0f);
    }

    public void SetSize(float Xsize)
    {
        fillBar.localScale = new Vector3(Xsize,1.0f);
    }

    public void SetColor(Color color)
    {
        barSprite.GetComponent<SpriteRenderer>().color=color;
    }
}
