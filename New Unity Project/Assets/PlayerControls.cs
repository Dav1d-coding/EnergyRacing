using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour, IPunObservable
{
    public PhotonView photonView;
    private SpriteRenderer spriteRenderer;
    public Vector2Int GamePostiton;
    public Vector2Int Direction;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Direction);
        }
        else
        {
            Direction = (Vector2Int)stream.ReceiveNext();
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GamePostiton = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        FindObjectOfType<MapController>().AddPlayer(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Direction = Vector2Int.left;
                transform.Translate(-Time.deltaTime * 40, 0, 0);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                Direction = Vector2Int.right;
                transform.Translate(Time.deltaTime * 40, 0, 0);
            }
            if (Input.GetKey(KeyCode.UpArrow)) Direction = Vector2Int.up;
            if (Input.GetKey(KeyCode.DownArrow)) Direction = Vector2Int.down;

        }

        if (Direction == Vector2Int.right) spriteRenderer.flipX = false;
        if (Direction == Vector2Int.left) spriteRenderer.flipX = true;

        //transform.position = Vector3.Lerp(transform.position, (Vector2)GamePostiton, Time.deltaTime * 3);
    }
}
