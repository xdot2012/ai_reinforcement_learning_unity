using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Agent : Subject, IObserver
{
    public List<Node> currentPath = new List<Node>();
    [SerializeField]
    public string alertCause;
    public Context context;

    public GameObject gameObject; 
    public int id;
    public Vector2Int position;
    public int visionRange = 3;
    public int hearRange = 6;
    public int alertRange = 6;
    public int symbolToRepeat;
    public bool notified = false;

    public string type;
    public bool rival;

    public Vector2Int lastPosition;
    private State state;
    public Zone zone;
    public float[,] symbols;

    public void SetupSymbols() {
        this.symbols = new float[10, 3];
        for(int x = 0; x < 10; x++) {
            for(int y=0; y < 3; y++) {
                symbols[x, y] = Random.Range(0f, 1f);
                //Debug.LogWarning(symbols[x, y]);
            }
        }
    }
    public void UpdateSymbol(int symbolIndex, string rivalSeen) {
        int rivalIndex = this.context.rivalindex[rivalSeen];
        this.symbols[symbolIndex, rivalIndex] += 0.1f;
    }

    public string SymbolToRival(int symbolIndex) {
        float largerNumber = 0;
        int rivalIndex = 0;
        for(int y = 0; y<3; y++) {
            if(symbols[symbolIndex, y] > largerNumber) {
                largerNumber = symbols[symbolIndex, y];
                rivalIndex = y;
            }
        }
        return this.context.rivalTypes[rivalIndex];
    }

    public int RivalToSymbol(string rivalType) {
        int rivalIndex = this.context.rivalindex[rivalType];
        float largerNumber = 0f;
        int symbolIndex = 0;
        for(int x= 0; x < 10; x++) {
            if(symbols[x, rivalIndex] > largerNumber) {
                largerNumber = symbols[x, rivalIndex];
                symbolIndex = x;
            }
        }
        return symbolIndex;
    }

    public void RedoSetup() {
        this.lastPosition = this.position;
        this.UpdateWorldPosition(this.position);
        this.TransitionTo(new Idle());
        this.Clear();
        this.symbolToRepeat = 0;
        this.notified = false;
        this.alertCause = null;
        if(this.rival) {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = Storage.GetSprite(this.type);
        }
    }
    public Agent(int id, Vector2Int position, string type, Context context, bool rival=false) {
        this.id = id;
        this.type = type;
        this.context = context;
        this.rival = rival;
        this.position = position;
        this.lastPosition = position;

        if (!rival) this.gameObject = GameObject.Instantiate(Storage.GetAgentPrefab(), GetWorldPosition(position), Quaternion.identity);
        else this.gameObject = GameObject.Instantiate(Storage.GetRivalPrefab(type), GetWorldPosition(position), Quaternion.identity);
        this.gameObject.GetComponent<AgentController>().agent = this;
        this.ChangeState(new Idle());
        this.SetupSymbols();
    }
    public void Move() {
        //Debug.LogFormat("{0}[{1}] Agent Destination: {2}", subject.position, subject.id, subject.GetDestination());
        this.FindPath();
        if(this.currentPath.Count > 0) { 
            if(this.currentPath.Count == 1) {
                if (!this.rival) {
                    Zone currentZone = this.context.GetZone(this.position);
                    if(currentZone != null) this.ChangeState(currentZone.GetSafeState());
                }
            }
            else {
                this.currentPath.RemoveAt(0);
                this.lastPosition = this.position;
                this.position = new Vector2Int(this.currentPath[0].x, this.currentPath[0].y);
                this.UpdateWorldPosition(this.position);
                this.ChangeDirectionFacing(this.position - this.lastPosition);
            }
        }
    }
    private void FindPath() { this.state.FindPath(); }
    public bool CanbeTargeted(string rivalType) { return this.rival || this.state.CanbeTargeted(rivalType); }
    public bool CanbeNotified() { return this.state.CanbeNotified(); }
    public bool isFinal() { return this.state.isFinal(); }
    private void ChangeState(State state) { if(this.state == null || !this.state.isFinal()) this.TransitionTo(state); }
    private void TransitionTo(State state)
    {
        //Debug.Log($"Context: {this.type} -{this.id}: Transition to {state.GetType().Name}.");
        this.state = state;
        this.state.SetContext(this);
        this.state.ChangeColor();
        this.state.FindPath();
    }
    public Vector3 GetWorldPosition(Vector2Int position) => new Vector3(position.x, position.y, 1);
    public void UpdateWorldPosition(Vector2Int position) { this.gameObject.transform.position = GetWorldPosition(position); }
    public void ChangeDirectionFacing(Vector2Int lastMove) {
        this.gameObject.transform.GetChild(0).transform.rotation = new Quaternion();
        this.gameObject.transform.GetChild(0).transform.Rotate(Vector3.forward * this.context.helperRotation[directionFaciong[lastMove]]);
    }

    public void LookForRivals() {
        if(!this.isFinal())
        {
            this.Clear();
            foreach(Agent observer in this.context.agentsList) {
                if(Vector2Int.Distance(this.position, observer.position) <= observer.visionRange && observer.CanbeNotified()) {
                    this.Attach(observer);
                }
            }
            foreach (Agent observer in this.context.rivalsList) {
                if (Vector2Int.Distance(this.position, observer.position) <= observer.visionRange) {
                    this.Scream(observer.type);
                    return;
                }
            }
        }

    }
    public void LookForAgents() {
        if(!this.isFinal()) {
            foreach (Agent agent in this.context.agentsList) {
                if (Vector2Int.Distance(this.position, agent.position) <= agent.visionRange + 2) {
                    this.alertCause = agent.type;
                    this.ChangeState(new Alert());
                    return;
                }
            }
        }
    }

    public void Attack() => this.state.Attack();
    public void Die() {
        this.TransitionTo(new Dead());
    }

    public static Dictionary<Vector2Int, string> directionFaciong = new Dictionary<Vector2Int, string>() {
        {new Vector2Int(0,  1), "NORTH"},
        {new Vector2Int(0, -1), "SOUTH"},
        {new Vector2Int( 1, 0), "EAST"},
        {new Vector2Int(-1, 0), "WEST"},
        {new Vector2Int(0,  0), "NORTH"}
    };

    public void Scream(string alertCause) {
        int symbol;
        if (this.notified) symbol = this.symbolToRepeat;
        else {
            symbol = this.RivalToSymbol(alertCause);
            //Debug.Log($"Context: {this.type} -{this.id}: Translated Rival to Symbol: {symbol}.");
            symbolToRepeat = symbol;
            this.notified = true;
        }
        this.UpdateSymbol(symbol, alertCause);
        this.Notify(symbol);
    }
    public void Update(int symbol) {
        if (this.CanbeNotified() && !this.isFinal()) {
            //Debug.Log($"Context: {this.type} -{this.id}: Received a Symbol {symbol}.");

            alertCause = this.SymbolToRival(symbol);
            this.symbolToRepeat = symbol;
            this.notified = true;
            this.ChangeState(new Alert());
        }
    }
    
}
