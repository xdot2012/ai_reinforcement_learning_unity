using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AgentController : MonoBehaviour
{
    [SerializeField]
    public Agent agent;
    public bool active;

    // Start is called before the first frame update
    void Start() {
        this.active = false;
    }

    public void Update() {
        if (this.active) {
            this.DrawPath();
        }
    }
    // Update is called once per frame
    public void OnMouseDown(){
        this.active = true;
    }

    public void DrawPath() {
        if (agent.currentPath.Count > 0){
            int currentNode = 0;
            Vector3 initStart = agent.GetWorldPosition(new Vector2Int(agent.position.x, agent.position.y)) +
                new Vector3(0, agent.context.cellSize / 2);
            Vector3 initEnd = agent.GetWorldPosition(new Vector2Int(agent.currentPath[currentNode].x, agent.currentPath[currentNode ].y)) +
                new Vector3(0, agent.context.cellSize / 2, 0); ;
            Debug.DrawLine(initStart, initEnd, Color.red);

            while (currentNode < agent.currentPath.Count - 1) {
                Vector3 start = agent.GetWorldPosition(new Vector2Int(agent.currentPath[currentNode].x, agent.currentPath[currentNode].y)) +
                    new Vector3(0, agent.context.cellSize / 2);
                Vector3 end = agent.GetWorldPosition(new Vector2Int(agent.currentPath[currentNode + 1].x, agent.currentPath[currentNode + 1].y)) +
                    new Vector3(0, agent.context.cellSize / 2, 0); ;
                Debug.DrawLine(start, end, Color.red);
                currentNode++;
            }
        }
    }
}
