using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapController : MonoBehaviour,IOnEventCallback
{

    public GameObject CellPrefab;
    private List<PlayerControls> players = new List<PlayerControls>();

    private double lastTickTime;

    private GameObject[,] cells;

    public void AddPlayer(PlayerControls player)
    {
        players.Add(player);

        cells[player.GamePostiton.x, player.GamePostiton.y].SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        cells = new GameObject[1200,100];
        int y = 30;
        for (int x = 0; x < 1200; x += 40)
        {
            cells[x,y] = Instantiate(CellPrefab, new Vector3(x,y), Quaternion.identity, transform);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (PhotonNetwork.Time > lastTickTime + 1 && PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Vector2Int[] directions = players.OrderBy(p => p.photonView.Owner.ActorNumber)
                .Select(p => p.Direction)
                .ToArray();
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            //Событие шага
            PhotonNetwork.RaiseEvent(42, directions, options, sendOptions);
            //шаг
            PerformTick(directions);
        }
    }

    private void PerformTick(Vector2Int[] directions)
    {
        if (players.Count != directions.Length) return;

        int i = 0;
        foreach (var playr in players.OrderBy(p=>p.photonView.Owner.ActorNumber))
        {
            playr.Direction = directions[i++];
            playr.GamePostiton += playr.Direction;

            if (playr.GamePostiton.x < 0) playr.GamePostiton.x = 0;
            if (playr.GamePostiton.y < 0) playr.GamePostiton.y = 0;
            if (playr.GamePostiton.x >= cells.GetLength(0)) playr.GamePostiton.x = cells.GetLength(0) - 1;
            if (playr.GamePostiton.y >= cells.GetLength(0)) playr.GamePostiton.y = cells.GetLength(1) - 1;
        }
        lastTickTime = PhotonNetwork.Time;
    }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case 42:
                Vector2Int[] directions = (Vector2Int[])photonEvent.CustomData;
                PerformTick(directions);
                break;
        }
    }

    public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
}
