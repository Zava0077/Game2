using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static Player;
using static MathFunctions;
using UnityEngine.Tilemaps;
using static SquareCreator;
using static AnimationHelper;
using System;
using System.Security.Cryptography;

public sealed class Background : MonoBehaviour
{
    private const float BG_WIDTH = 14;
    private const float BG_HEIGHT = 6;
    private const float BG_ZDISTANCE = 18.8f;
    private static readonly Vector2Int[] positions = new Vector2Int[]
    {
        new(-1,-1),
        new(-1,0),
        new(0, 0),
        new(0,-1),
    };
    private static GameObject[] _gameObjects = new GameObject[4];
    private static Background _it;

    private Background() { }
    public static void CreateBG(TileBase bg)
    {
        OnTimeStep += UpdateBG;
        
        (var T, var G) = CreateTilemap(BG_WIDTH,BG_HEIGHT);
        T.transform.SetParent(G.transform);
        _it = G.AddComponent<Background>();

        foreach (var vec in positions)
            T.SetTile(new Vector3Int(vec.x,vec.y), bg);
        
        Vector3 pos = _it.transform.position;
        _it.transform.position = new Vector3(pos.x, pos.y, BG_ZDISTANCE);
        _it.transform.localScale *= 2f;
    }
    private static void UpdateBG() =>
        _it.StartCoroutine(_it.LerpBG());
    private IEnumerator LerpBG()
    {
        yield return LerpAnim(gameObject,Camera.main.transform.position, character.NewPos, MOVE_CD, EaseInOut, BG_ZDISTANCE, 1.15f);
    }
}
