using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Storage
{
    private static bool hasLoaded = false;
    private static GameObject agentPrefab = Resources.Load<GameObject>("agentPrefab");
    private static GameObject rivalPrefab = Resources.Load<GameObject>("rivalPrefab");
    private static GameObject snakePrefab = Resources.Load<GameObject>("snakeRivalPrefab");
    private static GameObject leopardPrefab = Resources.Load<GameObject>("leopardRivalPrefab");
    private static GameObject hawkPrefab = Resources.Load<GameObject>("hawkRivalPrefab");

    private static Sprite snake= Resources.Load<Sprite>("sprites/png/snake_sprite");
    private static Sprite leopard = Resources.Load<Sprite>("sprites/png/leopard_sprite");
    private static Sprite hawk = Resources.Load<Sprite>("sprites/png/hawk_sprite");

    private static GameObject treePrefab = Resources.Load<GameObject>("treePrefab");
    private static GameObject bushPrefab = Resources.Load<GameObject>("bushPrefab");
    private static GameObject tallTreePrefab = Resources.Load<GameObject>("tallTreePrefab");

    private static void Load() {
        if (!hasLoaded) {
            hasLoaded = true;
        }
    }

    public static GameObject GetAgentPrefab() {
        if (!hasLoaded) Load();

        return agentPrefab;
    }
    public static GameObject GetRivalPrefab(string type) {
        if (!hasLoaded) Load();
        if (type == "LEOPARD") return leopardPrefab;
        else if (type == "SNAKE") return snakePrefab;
        else if (type == "HAWK") return hawkPrefab;
        return rivalPrefab;
    }

    public static Sprite GetSprite(string type) {
        if (!hasLoaded) Load();
        if (type == "LEOPARD") return leopard;
        else if (type == "SNAKE") return snake;
        else return hawk;
    }

    public static GameObject GetTreePrefab() {
        if (!hasLoaded) Load();
        return treePrefab;
    }

    public static GameObject GetBushPrefab() {
        if (!hasLoaded) Load();
        return bushPrefab;
    }

    public static GameObject GetTallTreePrefab() {
        if (!hasLoaded) Load();
        return tallTreePrefab;
    }
}
