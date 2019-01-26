﻿/*
 * Copyright (c) 2017 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;
    public float shiftDelay = 0.1F;
    public bool cheatMode = false;
    public Tile playerTileTemplate;
    public Tile playerTile { get; set; }
    public Sprite emptySprite;
    public Sprite movableSprite;
    public List<Sprite> characters = new List<Sprite>();
    public Tile tile;
    public int xSize, ySize;

    private Tile[,] tiles;

    public bool IsShifting { get; set; }
    public bool IsPlayerMoving { get; set; }
    public Tile MovingPlayerTarget { get; set; }

    void Start()
    {
        instance = GetComponent<BoardManager>();

        Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.size;
        CreateBoard(offset.x, offset.y, new Vector2Int(xSize / 2 - 1, ySize / 2 - 1));
    }

    bool didCheckAlready = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            cheatMode = !cheatMode;
        }

        if(IsPlayerMoving || IsShifting)
        {
            didCheckAlready = false;
        }

        if (!IsPlayerMoving && !IsShifting && !didCheckAlready)
        {
            didCheckAlready = true;

            Vector2Int possibleMoveIndices;
            bool possibleMoveExists = CheckForPossibleMoves(out possibleMoveIndices);

            print(possibleMoveExists ? "found possible move at " + possibleMoveIndices : "no possible move found");
            
            if (!possibleMoveExists)
            {
                // make pop up window

                Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.size;

                for (int x = 0; x < xSize; x++)
                {
                    for (int y = 0; y < ySize; y++)
                    {
                        DestroyImmediate(tiles[x, y].gameObject);
                    }
                }

                CreateBoard(offset.x, offset.y, GetCoordinates(playerTile));
            }
        }
    }
    
    private bool CheckForPossibleMoves(out Vector2Int indices)
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                Tile currentTile = tiles[x, y];

                Sprite cachedSprite = currentTile.spriteRenderer.sprite;

                List<Tile> neighbours = currentTile.GetAllAdjacentTiles(false);

                foreach (Tile neighbour in neighbours)
                {
                    SwapTiles(currentTile, neighbour);

                    bool matchExists = currentTile.CheckIfMatchExists(Tile.MatchCheckDirection.both);

                    SwapTiles(currentTile, neighbour);

                    if (matchExists)
                    {
                        indices = new Vector2Int(x, y);

                        return true;
                    }
                }
            }
        }

        indices = new Vector2Int(-1, -1);

        return false;
    }

    private void CreateBoard(float xOffset, float yOffset, Vector2Int playerStartCoordinates)
    {
        tiles = new Tile[xSize, ySize];

        float startX = transform.position.x;
        float startY = transform.position.y;

        Sprite[] previousLeft = new Sprite[ySize]; // Add this line
        Sprite previousBelow = null; // Add this line

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                Tile newTile = Instantiate(tile, new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), tile.transform.rotation);
                tiles[x, y] = newTile;
                newTile.transform.parent = transform; // Add this line

                List<Sprite> possibleCharacters = new List<Sprite>();
                possibleCharacters.AddRange(characters);

                possibleCharacters.Remove(previousLeft[y]);
                possibleCharacters.Remove(previousBelow);

                Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)];
                newTile.GetComponent<SpriteRenderer>().sprite = newSprite;
                previousLeft[y] = newSprite;
                previousBelow = newSprite;
            }
        }

        if (playerTileTemplate != null)
        {
            int xPlayerStartPos = playerStartCoordinates.x;
            int yPlayerStartPos = playerStartCoordinates.y;

            playerTile = Instantiate(playerTileTemplate, tiles[xPlayerStartPos, yPlayerStartPos].transform.position, playerTileTemplate.transform.rotation);
            playerTile.transform.parent = transform;
            DestroyImmediate(tiles[xPlayerStartPos, yPlayerStartPos].gameObject);
            tiles[xPlayerStartPos, yPlayerStartPos] = playerTile;
        }
    }

    public void SwapTiles(Tile t1, Tile t2)
    {
        int x1 = 0, y1 = 0, x2 = 0, y2 = 0;

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (tiles[x, y] == t1)
                {
                    x1 = x;
                    y1 = y;
                }
                if (tiles[x, y] == t2)
                {
                    x2 = x;
                    y2 = y;
                }
            }
        }

        SwapTiles(x1, y1, x2, y2);
    }

    public void SwapTiles(int x1, int y1, int x2, int y2)
    {
        Vector3 posTmp = tiles[x1, y1].transform.position;
        tiles[x1, y1].transform.position = tiles[x2, y2].transform.position;
        tiles[x2, y2].transform.position = posTmp;

        Tile tmp = tiles[x1, y1];
        tiles[x1, y1] = tiles[x2, y2];
        tiles[x2, y2] = tmp;
    }

    public Vector2Int GetCoordinates(Tile tile)
    {
        Vector2Int result = Vector2Int.zero;
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (tiles[x, y] == tile)
                {
                    result = new Vector2Int(x, y);
                }
            }
        }
        return result;
    }

    public IEnumerator FindNullTiles()
    {
        // interrupt for player movement
        Vector2Int playerPos = GetCoordinates(playerTile);

        if (tiles[(int)playerPos.x, (int)playerPos.y].GetAllAdjacentTiles().Exists(t => t != null && t.spriteRenderer.sprite == emptySprite))
        {
             List<Tile> movableTiles = new List<Tile>(GetMovableTiles());

            movableTiles.ForEach(t => t.spriteRenderer.sprite = movableSprite);

            IsPlayerMoving = true;
            while (MovingPlayerTarget == null)
            {
                yield return new WaitForEndOfFrame();
            }

            Vector2Int movingplayerTargetPos = GetCoordinates(MovingPlayerTarget);
            SwapTiles(playerTile, MovingPlayerTarget);
            
            movableTiles.ForEach(t => t.spriteRenderer.sprite = emptySprite);

            IsPlayerMoving = false;
            MovingPlayerTarget = null;
        }

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (tiles[x, y].spriteRenderer.sprite == emptySprite)
                {
                    yield return StartCoroutine(ShiftTilesDown(x, y));
                    break;
                }
            }
        }

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                tiles[x, y].GetComponent<Tile>().ClearAllMatches();
            }
        }
    }

    private HashSet<Tile> GetMovableTiles()
    {
        HashSet<Tile> activeTiles = new HashSet<Tile>(playerTile.GetAllAdjacentTiles(false).FindAll(t => t.spriteRenderer.sprite == emptySprite));
        List<Tile> cachedToAdd = new List<Tile>();
        int activeTileCount = 0;

        do
        {
            cachedToAdd.Clear();
            activeTileCount = activeTiles.Count;

            foreach(Tile tile in activeTiles)
            {
                cachedToAdd.AddRange(tile.GetAllAdjacentTiles(false).FindAll(t => t.spriteRenderer.sprite == emptySprite));
            }

            cachedToAdd.ForEach(t => activeTiles.Add(t));

        } while (activeTiles.Count > activeTileCount);
        
        return activeTiles;
    }

    private IEnumerator ShiftTilesDown(int x, int yStart)
    {
        IsShifting = true;
        List<Vector2> tilePositions = new List<Vector2>();

        for (int y = yStart; y < ySize; y++)
        {
            Tile tile = this.tiles[x, y];
            tilePositions.Add(new Vector2(x, y));
        }

        int nullCount = 0;
        while (nullCount < tilePositions.Count && tiles[(int)tilePositions[nullCount].x, (int)tilePositions[nullCount].y].spriteRenderer.sprite == emptySprite)
        {
            nullCount++;
        }

        Vector2 lastTilePos = tilePositions[tilePositions.Count - 1];
        if (tiles[(int)lastTilePos.x, (int)lastTilePos.y] == playerTile)
        {
            tilePositions.RemoveAt(tilePositions.Count - 1);
        }

        for (int i = 0; i < nullCount; i++)
        {
            GUIManager.instance.Score += 50; // Add this line here
                                             //yield return new WaitForSeconds(shiftDelay);
            for (int k = 0; k < tilePositions.Count - 1; k++)
            {
                yield return new WaitForSeconds(shiftDelay);
                if (tiles[(int)tilePositions[k + 1].x, (int)tilePositions[k + 1].y] == playerTile && k + 2 < tilePositions.Count)
                {
                    SwapTiles((int)tilePositions[k].x, (int)tilePositions[k].y, (int)tilePositions[k + 2].x, (int)tilePositions[k + 2].y);
                    k++;
                }
                else if (tiles[(int)tilePositions[k].x, (int)tilePositions[k].y] == playerTile)
                {
                    // do nothing
                }
                else
                {
                    SwapTiles((int)tilePositions[k].x, (int)tilePositions[k].y, (int)tilePositions[k + 1].x, (int)tilePositions[k + 1].y);
                }
            }
            tiles[(int)tilePositions[tilePositions.Count - 1].x, (int)tilePositions[tilePositions.Count - 1].y].spriteRenderer.sprite = GetNewSprite(x, ySize - 1);
        }
        IsShifting = false;
    }

    private Sprite GetNewSprite(int x, int y)
    {
        List<Sprite> possibleCharacters = new List<Sprite>();
        possibleCharacters.AddRange(characters);

        if (x > 0)
        {
            possibleCharacters.Remove(tiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (x < xSize - 1)
        {
            possibleCharacters.Remove(tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (y > 0)
        {
            possibleCharacters.Remove(tiles[x, y - 1].GetComponent<SpriteRenderer>().sprite);
        }

        return possibleCharacters[Random.Range(0, possibleCharacters.Count)];
    }

}
