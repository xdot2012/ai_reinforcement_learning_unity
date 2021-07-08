using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Zone {
    public int id;
    public Vector2Int position;
    public string type;
    private HashSet<Agent> agents;
    public GameObject gameObject;
    public bool full;

    public Zone(int id, Vector2Int position, string type) {
        this.id = id;
        this.position = position;
        this.type = type;
        this.agents = new HashSet<Agent>();
        if (type == "TREE") this.gameObject = GameObject.Instantiate(Storage.GetTreePrefab(), GetWorldPosition(position), Quaternion.identity);
        else if (type == "BUSH") this.gameObject = GameObject.Instantiate(Storage.GetBushPrefab(), GetWorldPosition(position), Quaternion.identity);
        else this.gameObject = GameObject.Instantiate(Storage.GetTallTreePrefab(), GetWorldPosition(position), Quaternion.identity);
    }
    public void RedoSetup() {
        this.agents = new HashSet<Agent>();
        this.UpdateWorldPosition(this.position);
    }
    public bool isAgentSafe(string rivalType) {
        if (type == "TREE") { if (rivalType == "SNAKE") return true;}
        else if (type == "TALL_TREE") { if (rivalType == "LEOPARD") return true;}
        else if (type == "BUSH") { if (rivalType == "HAWK") return true;}
        return false;
    }
    public void AddAgent(Agent agent) { this.agents.Add(agent); }
    public void RemoveAgent(Agent agent) => this.agents.Remove(agent);
    public Vector3 GetWorldPosition(Vector2Int position) => new Vector3(position.x, position.y, 1);
    public void UpdateWorldPosition(Vector2Int position) { this.gameObject.transform.position = GetWorldPosition(position); }
    public State GetSafeState() {
        if (this.type == "TREE") return new OnTree();
        else if (this.type == "BUSH") return new OnBush();
        else if (this.type == "TALL_TREE") return new OnTallTree();
        else return null;
    }
}
