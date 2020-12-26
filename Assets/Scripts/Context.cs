using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Context
{
    public int DIAGONAL_COST = 2;
    public int STRAIGHT_COST = 1;

    public int N_ZONES;
    public int N_AGENTS;
    public int N_RIVALS;

    public int xSize;
    public int ySize;
    public int iteration = 0;
    public Node[,] grid;
    public int cellSize = 1;

    public HashSet<Agent> agentsList;
    public HashSet<Agent> rivalsList;

    public Zone[] treeList;
    public Zone[] bushList;
    public Zone[] tallTreeList;
    
    public Dictionary<int, string> rivalTypes = new Dictionary<int, string>() { { 0, "LEOPARD" }, { 1, "SNAKE" }, { 2, "HAWK" } };
    public Dictionary<string, int> rivalindex = new Dictionary<string, int>() {{ "LEOPARD", 0 }, { "SNAKE", 1}, { "HAWK", 2 }};
    
    public Dictionary<int, string> zoneTypes = new Dictionary<int, string>() {{0, "TREE"}, {1, "BUSH"}, {2, "TALL_TREE"}};
    public Dictionary<string, float> helperRotation = new Dictionary<string, float>() {{ "NORTH" , 0 }, { "SOUTH" , 180f}, { "EAST"  , 270f}, { "WEST"  , 90f }};
    public Dictionary<string, string> causeEffect =new Dictionary<string, string>() {{ "MONKEY" ,"MONKEY"}, { "SNAKE" ,"TREE"}, { "HAWK" , "BUSH"}, { "LEOPARD" , "TALL_TREE"}};
    public Context(int xSize, int ySize, int cellSize, int nAgents, int nRivals, int nZones) {
        this.xSize = xSize;
        this.ySize = ySize;
        this.cellSize = cellSize;
        this.N_AGENTS = nAgents;
        this.N_RIVALS = nRivals;
        this.N_ZONES = nZones;
        this.agentsList = new HashSet<Agent>(); ;
        this.rivalsList= new HashSet<Agent>();

        this.treeList = new Zone[nZones];
        this.bushList = new Zone[nZones];
        this.tallTreeList = new Zone[nZones];

        this.GenerateGrid(xSize, ySize);
        this.TerraForm(nZones);
        this.Populate(nAgents, nRivals);
    }

    public void ResetSetup() {
        this.iteration = 0;
        foreach(Zone zone in treeList) {
            Vector2Int randomPosition = new Vector2Int();
            bool validPosition = false;
            while (!validPosition) {
                randomPosition = new Vector2Int((int)Random.Range(xSize / 4, xSize - xSize / 4), (int)Random.Range(ySize / 4, ySize - ySize / 4));
                //check position
                if (!PositionHasZone(randomPosition)) validPosition = true;
            }
            zone.position = randomPosition;
            zone.RedoSetup();
        }
        foreach (Zone zone in tallTreeList)
        {
            Vector2Int randomPosition = new Vector2Int();
            bool validPosition = false;
            while (!validPosition)
            {
                randomPosition = new Vector2Int((int)Random.Range(xSize / 4, xSize - xSize / 4), (int)Random.Range(ySize / 4, ySize - ySize / 4));
                //check position
                if (!PositionHasZone(randomPosition)) validPosition = true;
            }
            zone.position = randomPosition;
            zone.RedoSetup();
        }
        foreach (Zone zone in bushList)
        {
            Vector2Int randomPosition = new Vector2Int();
            bool validPosition = false;
            while (!validPosition)
            {
                randomPosition = new Vector2Int((int)Random.Range(xSize / 4, xSize - xSize / 4), (int)Random.Range(ySize / 4, ySize - ySize / 4));
                //check position
                if (!PositionHasZone(randomPosition)) validPosition = true;
            }
            zone.position = randomPosition;
            zone.RedoSetup();
        }
        foreach (Agent agent in agentsList) {
            Vector2Int randomPosition = new Vector2Int();
            bool validPosition = false;
            while (!validPosition) {
                randomPosition = new Vector2Int((int)Random.Range(0, xSize), (int)Random.Range(0, ySize));
                if (!PositionHasAgent(randomPosition)) validPosition = true;
            }
            agent.position = randomPosition;
            agent.RedoSetup();
        }
        foreach(Agent rival in rivalsList) {
            Vector2Int randomPosition = new Vector2Int();
            bool validPosition = false;
            while (!validPosition)
            {
                randomPosition = new Vector2Int((int)Random.Range(0, xSize), (int)Random.Range(0, ySize));
                int removeXorY = (int)Random.Range(0, 2);
                if (removeXorY == 0 && randomPosition.y != 0) randomPosition.x = 0;
                if (removeXorY == 1 && randomPosition.x != 0) randomPosition.y = 0;
                //check position
                if (!PositionHasAgent(randomPosition)) validPosition = true;
            }
            rival.position = randomPosition;
            rival.type = rivalTypes[(int)Random.Range(0, 3)];
            rival.RedoSetup();
        }

    }

    public void UpdateSubjectPosition(HashSet<Agent> list) {
        //Update Position
        foreach (Agent subject in list) {
            this.GenerateNeighbours();
            subject.Move();
        }
    }

    public void Iterate() {
        iteration += 1;

        UpdateSubjectPosition(agentsList);
        UpdateSubjectPosition(rivalsList);

        //Rivals Look Around
        foreach (Agent rival in rivalsList) {
            rival.LookForAgents();
            rival.Attack();
        }

        //Monkeys Look Around
        foreach (Agent agent in agentsList) {
            agent.LookForRivals();
        }
        if(this.CheckIfGameIsOVer()) {
            this.ResetSetup();
        }
    }

    private Vector2 GetWorldPosition(int x, int y) => (new Vector2(x, y) * this.cellSize) + new Vector2(-0.5f, -0.5f);

    private void TerraForm(int nZones)
    {
        GenerateZones(nZones);
    }
    private void Populate(int nAgents, int nRivals)
    {
        GenerateAgents(nAgents);
        GenerateRivals(nRivals);
    }

    private void GenerateZones(int nZones) {
        for (int i = 0; i < nZones; i++) {
            for (int j = 0; j < 3; j++) {
                Vector2Int randomPosition = new Vector2Int();
                bool validPosition = false;
                while (!validPosition) {
                    randomPosition = new Vector2Int((int)Random.Range(xSize / 4, xSize - xSize/ 4), (int)Random.Range(ySize/ 4, ySize - ySize/ 4));
                    //check position
                    if (!PositionHasZone(randomPosition)) validPosition = true;
                }
                if (j == 0) treeList[i] = new Zone(i, randomPosition, "TREE");
                else if (j == 1) bushList[i] = new Zone(i, randomPosition, "BUSH");
                else tallTreeList[i] = new Zone(i, randomPosition, "TALL_TREE");
            }
        }
    }

    public void GenerateAgents(int nAgents) {
        for (int i = 0; i < nAgents; i++) {
            Vector2Int randomPosition = new Vector2Int();
            bool validPosition = false;
            while (!validPosition)
            {
                randomPosition = new Vector2Int((int)Random.Range(0, xSize), (int)Random.Range(0, ySize));
                //check position
                if (!PositionHasAgent(randomPosition)) validPosition = true;
            }
            agentsList.Add(new Agent(i + 1, randomPosition, "MONKEY", this));
        }
    }
    private void GenerateRivals(int nRivals) {
        for (int i = 0; i < nRivals; i++)
        {
            Vector2Int randomPosition = new Vector2Int();
            bool validPosition = false;
            while (!validPosition)
            {
                randomPosition = new Vector2Int((int)Random.Range(0, xSize), (int)Random.Range(0, ySize));
                int removeXorY = (int)Random.Range(0, 2);
                if (removeXorY == 0 && randomPosition.y != 0) randomPosition.x = 0;
                if (removeXorY == 1 && randomPosition.x != 0) randomPosition.y = 0;
                //check position
                if (!PositionHasAgent(randomPosition)) validPosition = true;
            }
            rivalsList.Add( new Agent(i, randomPosition, rivalTypes[(int)Random.Range(0, 3)], this, true));
        }
    }

    private void GenerateGrid(int xSize, int ySize) {
        this.grid = new Node[xSize, ySize];
        for (int x = 0; x < grid.GetLength(0); x++) {
            for (int y = 0; y < grid.GetLength(1); y++) {
                //Instantiate Nodes
                grid[x, y] = new Node(x, y);

                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.gray, 100f);
                Debug.DrawLine(GetWorldPosition(x + 1, y), GetWorldPosition(x, y), Color.gray, 100f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, ySize), GetWorldPosition(xSize, ySize), Color.gray, 100f);
        Debug.DrawLine(GetWorldPosition(xSize, 0), GetWorldPosition(xSize, ySize), Color.gray, 100f);

        GameObject camera = GameObject.Find("Main Camera");
        Vector2 position = GetWorldPosition((int)xSize / 2, (int)ySize / 2);
        camera.transform.position = new Vector3(position.x, position.y, -10);
    }

    public void GenerateNeighbours() {
        for (int x = 0; x < xSize; x++) {
            for (int y = 0; y < ySize; y++) {
                Node node = grid[x, y];
                node.neighbours = new HashSet<Node>();
                if(x < xSize -1) {
                    //if (CanEnterPosition(grid[x + 1, y].GetPosition()))
                        node.neighbours.Add(grid[x + 1, y]);
                }
                if(x > 0) {
                    //if (CanEnterPosition(grid[x - 1, y].GetPosition()))
                        node.neighbours.Add(grid[x - 1, y]);
                }
                if(y < ySize -1) {
                    //if (CanEnterPosition(grid[x, y + 1].GetPosition()))
                        node.neighbours.Add(grid[x, y + 1]);
                }
                if(y > 0) {
                    //if (CanEnterPosition(grid[x, y - 1].GetPosition()))
                    node.neighbours.Add(grid[x, y - 1]);
                }
            }
        }
        return;
    }
    public List<Node> FindPath(Vector2Int startPosition, Vector2Int endPosition)
    {
        //Find the Nodes
        Node startNode = grid[startPosition.x, startPosition.y];
        Node endNode = grid[endPosition.x, endPosition.y];

        // Instantiate the Lists that contains the Nodes to visit and the visited ones.
        List<Node> closedList = new List<Node>();

        // Add the start node to the open list
        List<Node> openList = new List<Node>();
        openList.Add(startNode);


        // Initiate the G cost with very high values
        foreach (Node currentNode in grid)
        {
            currentNode.gCost = int.MaxValue;
            currentNode.CalculateFCost();
            currentNode.cameFrom = null;
        }

        // Calculate the start Node Values 
        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceBetweenNodes(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            // Get the Node with the lowest cost
            Node lowestCostNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < lowestCostNode.fCost) lowestCostNode = openList[i];
            }
            Node currentNode = lowestCostNode;

            if (currentNode == endNode)
            {
                // Reached goal
                List<Node> path = new List<Node>();
                path.Add(endNode);
                Node cameFromNode = endNode;

                while (cameFromNode.cameFrom != null) {
                    path.Add(cameFromNode.cameFrom);
                    cameFromNode = cameFromNode.cameFrom;
                }

                path.Reverse();
                return path;
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // Check the neighbours
            foreach (Node neighbour in currentNode.neighbours) {
                if (closedList.Contains(neighbour)) continue;
                float tentativeGCost = currentNode.gCost + CalculateDistanceBetweenNodes(currentNode, neighbour) + CalculateNodeTraverseCost(currentNode, neighbour);
                if (tentativeGCost < neighbour.gCost) {
                    neighbour.cameFrom = currentNode;
                    neighbour.gCost = tentativeGCost;
                    neighbour.hCost = CalculateDistanceBetweenNodes(neighbour, endNode);
                    neighbour.CalculateFCost();

                    if (!openList.Contains(neighbour)) {
                        openList.Add(neighbour);
                    }
                }
            }
        }
        // Out of options
        return new List<Node>();
    }
    public float CalculateNodeTraverseCost(Node startNode, Node endNode) {
        float unitTraverseCost = 0;
        float tileTraverseCost = 1;

        //if (!CanEnterPosition(new Vector2Int(endNode.x, endNode.y)))
        //    unitTraverseCost += Mathf.Infinity;

        return tileTraverseCost + unitTraverseCost;
    }
    public float CalculateDistanceBetweenNodes(Node startNode, Node endNode)
    {
        float xDistance = Mathf.Abs(startNode.x - endNode.x);
        float yDistance = Mathf.Abs(startNode.y - endNode.y);
        float remaining = Mathf.Abs(xDistance - yDistance);
        return Mathf.Min(xDistance, yDistance) + STRAIGHT_COST * remaining;
    }
    public bool CanEnterPosition(Vector2Int positionToCheck) => !PositionHasAgent(positionToCheck);
    public bool PositionHasAgent(Vector2Int positionToCheck) {
        foreach(Agent agent in agentsList) {
            if (agent != null) {
                if (agent.position == positionToCheck) return true;
            }
        }
        foreach (Agent rival in rivalsList) {
            if (rival != null) { 
                if (rival.position == positionToCheck) return true;
            }
        }
        return false;
    }
    public bool PositionHasZone(Vector2Int positionToCheck) {
        foreach (Zone tree in treeList) {
            if (tree == null) return false;
            if (tree.position == positionToCheck) return true;
        }
        foreach (Zone bush in bushList) {
            if (bush == null) return false;
            if (bush.position == positionToCheck) return true;
        }
        foreach (Zone tallTree in tallTreeList) {
            if (tallTree == null) return false;
            if (tallTree.position == positionToCheck) return true;
        }
        return false;
    }
    public Vector2Int FindClosest(Vector2Int agentPosition, string alertCausedby, string agentType, bool toKill=false){
        string whatToFind = causeEffect[alertCausedby];
        Vector2Int positionToGo = agentPosition;
        if (whatToFind == "MONKEY") {
            Agent subject = GetClosestMonkey(agentPosition, agentType);
            if (subject != null) positionToGo = subject.position;
        }
        /*
        else {
            Zone subject = FindClosestRandomZone(agentPosition);
            if (subject != null) positionToGo = subject.position;
        }
        */

        else if (whatToFind == "TREE") {
            Zone subject = GetClosestTree(agentPosition);
            if (subject != null) positionToGo = subject.position;
        }
        else if (whatToFind == "BUSH") {
            Zone subject = GetClosestBush(agentPosition);
            if (subject != null) positionToGo = subject.position;
        }
        else if (whatToFind == "TALL_TREE") {
            Zone subject = GetClosestTallTree(agentPosition);
            if (subject != null) positionToGo = subject.position;
        }
        return positionToGo;
    }

    public Zone FindClosestRandomZone(Vector2Int agentPosition) {
        string whatToFind = zoneTypes[Random.Range(0, 3)];
        Zone subject = null;
        if (whatToFind == "TREE") {
            subject = GetClosestTree(agentPosition);
        }
        else if (whatToFind == "BUSH") {
            subject = GetClosestBush(agentPosition);
        }
        else if (whatToFind == "TALL_TREE") {
            subject = GetClosestTallTree(agentPosition);
        }
        return subject;
    }

    public Zone GetClosestTallTree(Vector2Int agentPosition) {
        Zone closestOne = null;
        float minDistance = Mathf.Infinity;
        float currentDistance;
        foreach (Zone tallTree in tallTreeList) {
            currentDistance = CalculateDistanceBetweenNodes(grid[agentPosition.x, agentPosition.y], grid[tallTree.position.x, tallTree.position.y]);
            if (currentDistance < minDistance) {
                currentDistance = minDistance;
                closestOne = tallTree;
            }
        }
        return closestOne;
    }

    public Zone GetClosestBush(Vector2Int agentPosition)
    {
        Zone closestOne = null;
        float minDistance = Mathf.Infinity;
        float currentDistance;
        foreach (Zone bush in bushList)
        {
            currentDistance = CalculateDistanceBetweenNodes(grid[agentPosition.x, agentPosition.y], grid[bush.position.x, bush.position.y]);
            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                closestOne = bush;
            }
        }
        return closestOne;
    }

    public Zone GetClosestTree(Vector2Int agentPosition) {
        Zone closestOne = null;
        float minDistance = Mathf.Infinity;
        float currentDistance;
        foreach (Zone tree in treeList) {
            currentDistance = CalculateDistanceBetweenNodes(grid[agentPosition.x, agentPosition.y], grid[tree.position.x, tree.position.y]);
            if (currentDistance < minDistance) {
                currentDistance = minDistance;
                closestOne = tree;
            }
        }
        return closestOne;
    }

    public Agent GetClosestPrey(Vector2Int agentPosition, string rivalType) {
        Agent closestOne = null;
        float minDistance = Mathf.Infinity;
        float currentDistance;
        Zone[] lookToList = treeList; 
        if (rivalType == "SNAKE") { lookToList = treeList; }
        else if (rivalType == "HAWK") { lookToList = bushList; }
        else if (rivalType == "LEOPARD") { lookToList = tallTreeList; }

        foreach (Agent monkey in agentsList) {
            if (monkey.CanbeTargeted(rivalType)) { 
                currentDistance = CalculateDistanceBetweenNodes(grid[agentPosition.x, agentPosition.y], grid[monkey.position.x, monkey.position.y]);
                if (currentDistance < minDistance) {
                    minDistance = currentDistance;
                    closestOne = monkey;
                }
            }
        }
        return closestOne;
    }
    public Agent GetClosestMonkey(Vector2Int agentPosition, string rivalType) {
        Agent closestOne = null;
        float minDistance = Mathf.Infinity;
        float currentDistance;
        foreach (Agent monkey in agentsList) {
            if (monkey.CanbeTargeted(rivalType)) {
                currentDistance = CalculateDistanceBetweenNodes(grid[agentPosition.x, agentPosition.y], grid[monkey.position.x, monkey.position.y]);
                if (currentDistance < minDistance) {
                    minDistance = currentDistance;
                    closestOne = monkey;
                }
            }
        }
        return closestOne;
    }
    public bool CheckIfAgentIsSafe(Vector2Int agentPosition, string rivalType) => this.GetZone(agentPosition).isAgentSafe(rivalType);
    public Zone GetZone(Vector2Int zonePosition) {
        foreach(Zone zone in treeList) if (zone.position == zonePosition) return zone; 
        foreach(Zone zone in bushList) if (zone.position == zonePosition) return zone; 
        foreach(Zone zone in tallTreeList) if (zone.position == zonePosition) return zone;
        return null;
    }

    public bool CheckIfGameIsOVer()
    {
        foreach(Agent rival in rivalsList) {
            if (!rival.isFinal()) return false;
        }
        return true;
    }

    public string GetSymbolUsage() {
        string text = "|Agent | Leopard | Snake | Hawk |";
        text += "\n" + "--------------------------------------------------" + "\n";
        foreach (Agent agent in agentsList) {
            text += "[" + agent.id.ToString() + "]            ";
                for(int y = 0; y < 3; y++) {
                    text += "[" + agent.RivalToSymbol(this.rivalTypes[y]).ToString() +"]            ";
                }
            text += "\n" + "--------------------------------------------------" + "\n";
        }
        return text;
    }
}

