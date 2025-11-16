using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelector : MonoBehaviour {
    private Manager manager;
    private GameObject pieceToMove;
    private Vector2 position;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void setManager(Manager newManager) {
        manager = newManager;
    }

    public void setPieceToMove(GameObject newPieceToMove) {
        pieceToMove = newPieceToMove;
    }

    public void setPosition(Vector2 newPosition) {
        position = newPosition;
    }

    void OnMouseDown() {
        manager.placePiece(pieceToMove, position);
        foreach (GameObject dot in GameObject.FindGameObjectsWithTag("moveSelector")){
            Destroy(dot);
        }
    }
}
