using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    [SerializeField] string sceneName;
    public GameObject loadingScreen;
    public Slider slider;
    float fillAmount=0;
    [SerializeField] float units=25;
    [SerializeField] Animator sceneAnimator;

    void Start()
    {
        StartCoroutine(fillingBar());
    }

    void Update()
    {
        UpdateBar();
    }

    IEnumerator fillingBar()
    {
        for(int i=0;i<=units;i++)
        {
            fillAmount=i/units;
            yield return null;
        }
        //Done Loading
        SceneManager.LoadSceneAsync(sceneName);
    }

    void UpdateBar()
    {
        slider.value=fillAmount;
    }
}
