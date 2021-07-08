using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    float timer = 0.0f;
    public Context context;
    public Camera mainCamera;
    public float speed = 10f;
    public TextMeshPro textMesh;

    void Start() {
        this.context = new Context(10, 10, 1, 50, 1, 1);
        textMesh = gameObject.AddComponent<TextMeshPro>();
        textMesh.fontSize = 3;
        textMesh.rectTransform.position = new Vector3(2.25f, 10.5f, 10f);
    }

    void FixedUpdate() {
        timer += this.speed * Time.deltaTime;
        int seconds = (int)timer % 60;
        if (seconds > 1) {
            timer = 0.0f;
            this.context.Iterate();
        }
        textMesh.text = this.context.GetSymbolUsage() ;
    }

    public void SpeedUP() {
        if(this.speed < 10)
            this.speed = this.speed*2;
    }

    public void SpeedDown()
    {
        if (this.speed > 0.5f)
            this.speed = this.speed/2;
    }

    public float GetSpeed() => this.speed;
    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
}
