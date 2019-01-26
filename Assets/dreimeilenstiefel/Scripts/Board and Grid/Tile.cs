/*
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

public class Tile : MonoBehaviour
{
    private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
    private static Tile previousSelected = null;

    public SpriteRenderer spriteRenderer;
    private bool isSelected = false;

    private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Select()
    {
        isSelected = true;
        spriteRenderer.color = selectedColor;
        previousSelected = gameObject.GetComponent<Tile>();
        SFXManager.instance.PlaySFX(Clip.Select);
    }

    private void Deselect()
    {
        isSelected = false;
        spriteRenderer.color = Color.white;
        previousSelected = null;
    }

    void OnMouseDown()
    {
        if (BoardManager.instance.IsPlayerMoving)
        {
            print(spriteRenderer.sprite.name);
            if (spriteRenderer.sprite == BoardManager.instance.emptySprite)
            {
                BoardManager.instance.MovingPlayerTarget = this;
            }
            return;
        }

        // Not Selectable conditions
        if (spriteRenderer.sprite == BoardManager.instance.emptySprite || this == BoardManager.instance.playerTile || BoardManager.instance.IsShifting)
        {
            return;
        }

        if (isSelected)
        { // Is it already selected?
            Deselect();
        }
        else
        {
            if (previousSelected == null)
            { // Is it the first tile selected?
                Select();
            }
            else
            {
                if (GetAllAdjacentTiles().Contains(previousSelected)) // Is it an adjacent tile?
                {
                    SFXManager.instance.PlaySFX(Clip.Swap);
                    GUIManager.instance.MoveCounter--; // Add this line here

                    BoardManager.instance.SwapTiles(this, previousSelected);
                    previousSelected.ClearAllMatches();
                    previousSelected.Deselect();
                    ClearAllMatches();

                }
                else
                {
                    previousSelected.GetComponent<Tile>().Deselect();
                    Select();
                }
            }
        }
    }

    private Tile GetAdjacent(Vector2 castDir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        if (hit.collider != null)
        {
            return hit.collider.gameObject.GetComponent<Tile>();
        }
        return null;
    }

    public List<Tile> GetAllAdjacentTiles()
    {
        List<Tile> adjacentTiles = new List<Tile>();
        for (int i = 0; i < adjacentDirections.Length; i++)
        {
            adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
        }
        return adjacentTiles;
    }

    private List<GameObject> FindMatch(Vector2 castDir)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == spriteRenderer.sprite)
        {
            matchingTiles.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
        }
        return matchingTiles;
    }

    private void ClearMatch(Vector2[] paths)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        for (int i = 0; i < paths.Length; i++) { matchingTiles.AddRange(FindMatch(paths[i])); }
        if (matchingTiles.Count >= 2)
        {
            for (int i = 0; i < matchingTiles.Count; i++)
            {
                matchingTiles[i].GetComponent<SpriteRenderer>().sprite = BoardManager.instance.emptySprite;
            }
            matchFound = true;
        }
    }

    private bool matchFound = false;
    public void ClearAllMatches()
    {
        if (spriteRenderer.sprite == BoardManager.instance.emptySprite)
            return;

        ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
        ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
        if (matchFound)
        {
            spriteRenderer.sprite = BoardManager.instance.emptySprite;
            matchFound = false;
            StopCoroutine(BoardManager.instance.FindNullTiles()); //Add this line
            StartCoroutine(BoardManager.instance.FindNullTiles()); //Add this line
            SFXManager.instance.PlaySFX(Clip.Clear);
        }
    }

}