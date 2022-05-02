using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class qkBitwiseOblivionService : MonoBehaviour
{
    internal static string[] BossModules = new string[0];

    private Coroutine FetchRoutine;
    
    void Awake()
    {
        GetComponent<KMGameInfo>().OnStateChange += StateChange;
        StateChange(KMGameInfo.State.Setup);
    }

    void StateChange(KMGameInfo.State state)
    {
        if (state == KMGameInfo.State.Setup && FetchRoutine == null)
            FetchRoutine = StartCoroutine(FetchBosses());
    }

    IEnumerator FetchBosses()
    {
        var fetch = new WWW("https://ktane.timwi.de/json/raw");
        yield return fetch;
        if (!string.IsNullOrEmpty(fetch.error))
        {
            Debug.Log("[Bitwise Oblivion] Failed to fetch boss modules: " + fetch.error);
            FetchRoutine = null;
            yield break;
        }

        BossModules =
            JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>[]>>(fetch.text)["KtaneModules"]
                .Where(m => m.ContainsKey("BossStatus"))
                .Select(m => (string)m["ModuleID"]).Except(new[] { "qkBitwiseOblivion" }).ToArray();
        Debug.LogFormat("[Bitwise Oblivion] {0} boss modules fetched", BossModules.Length);
        Debug.Log(string.Join("\n", BossModules));
        FetchRoutine = null;
    }
}
