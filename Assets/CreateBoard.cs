using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateBoard : MonoBehaviour
{
    public GameObject[] tilePrefabs;
    public GameObject housePrefab;

    public GameObject threePrefab;

    public Text score;
    GameObject[] tiles;
    long dirtBB = 0;
    long desertBB = 0;
    long threeBB = 0;
    long playerBB = 0;

    // Start is called before the first frame update
    void Start()
    {
        tiles = new GameObject[64];
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                int randomTile = UnityEngine.Random.Range(0, tilePrefabs.Length);
                Vector3 pos = new Vector3(c, 0, r);
                GameObject tile = Instantiate(tilePrefabs[randomTile], pos, Quaternion.identity);
                tiles[r * 8 + c] = tile;
                if (tile.tag == "Dirt")
                {
                    dirtBB = SetCellState(dirtBB, r, c);
                }
                if (tile.tag == "Desert")
                {
                    desertBB = SetCellState(desertBB, r, c);
                }
                tile.name = tile.tag + "_" + r + "_" + c;
            }
        }
        Debug.Log("Dirt cells = " + CellCount(dirtBB));
        InvokeRepeating("PlantTree", 0.1f, 0.1f);
        // PrintBB("dirt", dirtBB);
    }

    void PlantTree()
    {
        int rr = UnityEngine.Random.Range(0, 8);
        int rc = UnityEngine.Random.Range(0, 8);
        if (GetCellState(dirtBB & ~playerBB, rr, rc))
        {
            GameObject three = Instantiate(threePrefab);
            three.transform.parent = tiles[rr * 8 + rc].transform;
            three.transform.localPosition = Vector3.zero;
            threeBB = SetCellState(threeBB, rr, rc);
        }
    }
    bool GetCellState(long bitboard, int row, int col)
    {
        long mask = 1L << (row * 8 + col);
        return (bitboard & mask) != 0;
    }
    void PrintBB(string name, long bitboard)
    {
        Debug.Log(name + ";" + Convert.ToString(bitboard, 2).PadLeft(64, '0'));
    }
    long SetCellState(long bitboard, int row, int col)
    {
        long newBit = 1L << (row * 8 + col);
        return (bitboard |= newBit);
    }

    int CellCount(long bitboard)
    {
        int count = 0;
        long bb = bitboard;
        while (bb != 0)
        {
            bb &= bb - 1;
            count++;
        }
        return count;
    }
    void CalculateScore()
    {
        score.text = "Score: " + ((CellCount(playerBB & dirtBB) * 10) + (CellCount(playerBB & desertBB) * 2));
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                var r = (int)hit.collider.gameObject.transform.position.z;
                var c = (int)hit.collider.gameObject.transform.position.x;
                if (GetCellState(~threeBB & (dirtBB | desertBB), r, c))
                {
                    GameObject house = Instantiate(housePrefab);
                    house.transform.parent = hit.collider.gameObject.transform;
                    house.transform.localPosition = Vector3.zero;
                    playerBB = SetCellState(playerBB, r, c);
                    CalculateScore();
                }
            }
        }
    }
}
