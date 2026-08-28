using System.Collections.Generic;
using UnityEngine;

public class DescribeAllPlayers : MonoBehaviour
{
    private void Start()
    {
        DescribeAll();
    }

    public void DescribeAll()
    {
        CharacterIdentity[] allPlayers = FindObjectsByType<CharacterIdentity>();

        foreach (var player in allPlayers)
        {
            Debug.Log(GetPlayerDescription(player));
        }
    }
    
    public static string GetPlayerDescription(CharacterIdentity player)
    {
        if (player == null) return "Player is null.";
        if (player.Data == null) return $"{player.gameObject.name}: (Data not initialized yet).";

        var data = player.Data;
        string sceneName = player.gameObject.name;

        List<string> wornItems = new List<string>();

        if (data.shirt != null) wornItems.Add($"a {data.shirt.color} shirt");
        if (data.pants != null) wornItems.Add($"{data.pants.color} pants");
        if (data.shoes != null) wornItems.Add($"{data.shoes.color} shoes");
        if (data.hat != null)   wornItems.Add($"a {data.hat.color} hat");
        if (data.hair != null)  wornItems.Add($"{data.hair.color} hair");

        string outfit = wornItems.Count > 0 ? string.Join(", ", wornItems) : "nothing";

        return $"Player '{sceneName}' (ID: {data.id}) is wearing {outfit}.";
    }


    public static CharacterIdentity FindPlayerByName(string sceneObjectName)
    {
        CharacterIdentity[] allPlayers = FindObjectsByType<CharacterIdentity>();
        foreach (var player in allPlayers)
        {
            if (player.gameObject.name == sceneObjectName)
            {
                return player;
            }
        }

        Debug.LogWarning($"Could not find character named '{sceneObjectName}' in the scene!");
        return null;
    }
}