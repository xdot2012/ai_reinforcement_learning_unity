using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State {
    protected Agent agent;
    protected Vector2Int destination;
    public void SetContext(Agent context) {
        this.agent = context;
        this.destination = agent.position;
    }
    public void SetDestination(Vector2Int destination) { this.destination = destination; }
    public abstract bool CanbeTargeted(string rivalType);
    public abstract bool CanbeNotified();
    public abstract void ChangeColor();
    public abstract void FindPath();
    public abstract bool isFinal();
    protected Vector2Int RandomDestination() => new Vector2Int(Random.Range(0, agent.context.xSize), Random.Range(0, agent.context.ySize));
    public virtual void Attack() {
        return;
    }
}

public class Idle : State {
    public override void ChangeColor() { this.agent.gameObject.GetComponent<SpriteRenderer>().color = Color.green; }
    public override bool CanbeTargeted(string rivalType) => true;
    public override bool CanbeNotified() => true;
    public override bool isFinal() => false;

    public override void FindPath() {
        if (agent.position == destination)
            this.SetDestination(RandomDestination());
        this.agent.currentPath = this.agent.context.FindPath(agent.position, destination);
    }
}

public class Alert : State {
    public override void ChangeColor() { this.agent.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow; }
    public override bool CanbeTargeted(string rivalType) => true;
    public override bool CanbeNotified() => false;
    public override bool isFinal() => false;

    public override void FindPath() {
        this.SetDestination(agent.context.FindClosest(agent.position, agent.alertCause, this.agent.type, agent.rival));
        this.agent.currentPath = this.agent.context.FindPath(this.agent.position, destination);
    }
    public override void Attack() {
        if(this.agent.rival) {
            Agent closestPrey = this.agent.context.GetClosestPrey(this.agent.position, this.agent.type);
            if (closestPrey != null) {
                if (Vector2Int.Distance(this.agent.position, closestPrey.position) <= 1) {
                    closestPrey.Die();
                    closestPrey = null;
                }
            }
            else {
                this.agent.Die();
            }
        }
    }
}

public class OnTree : State
{
    public override void ChangeColor() { this.agent.gameObject.GetComponent<SpriteRenderer>().color = Color.blue; }
    public override bool CanbeTargeted(string rivalType) {
        if (rivalType == "SNAKE") return false;
        else return true;
    }
    public override bool CanbeNotified() => false;
    public override bool isFinal() => true;
    public override void FindPath() { this.agent.currentPath = new List<Node>(); }
}

public class OnTallTree : State
{
    public override void ChangeColor() { this.agent.gameObject.GetComponent<SpriteRenderer>().color = Color.blue; }
    public override bool CanbeTargeted(string rivalType){
        if (rivalType == "LEOPARD") return false;
        else return true;
    }
    public override bool CanbeNotified() => false;
    public override bool isFinal() => true;
    public override void FindPath() { this.agent.currentPath = new List<Node>(); }
}

public class OnBush : State
{
    public override void ChangeColor() { this.agent.gameObject.GetComponent<SpriteRenderer>().color = Color.blue; }
    public override bool CanbeTargeted(string rivalType){
        if (rivalType == "HAWK") return false;
        else return true;
    }
    public override bool CanbeNotified() => false;
    public override bool isFinal() => true;
    public override void FindPath() { this.agent.currentPath = new List<Node>(); }
}

public class Dead : State {
    public override void ChangeColor() { this.agent.gameObject.GetComponent<SpriteRenderer>().color = Color.gray; }
    public override bool CanbeTargeted(string rivalType) => false;
    public override bool CanbeNotified() => false;
    public override bool isFinal() => true;
    public override void FindPath() { this.agent.currentPath = new List<Node>();}
}