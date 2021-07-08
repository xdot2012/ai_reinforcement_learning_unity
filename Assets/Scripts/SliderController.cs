using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{

    Slider slider;
    GameManager gameManager;

    private void Start()
    {
        slider = transform.GetComponent<Slider>();
        gameManager = FindObjectOfType<GameManager>();
    }
    void Update()
    {
        float value = slider.value;
        gameManager.SetSpeed(5 + value * 10);
    }
}
