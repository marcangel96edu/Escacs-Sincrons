using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromoSelector : MonoBehaviour {
    [SerializeField]
    private Manager manager;
    [SerializeField]
    private int pieceToPromote;
    [SerializeField]
    private MoveSelector moveSelector;
    [SerializeField]
    private GameObject pieceToMove;
    [SerializeField]
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

    public void setMoveSelector(MoveSelector newMoveSelector) {
        moveSelector = newMoveSelector;
    }

    public void setPieceToPromote(int newPieceToPromote) {
        pieceToPromote = newPieceToPromote;
    }

    public void setPieceToMove(GameObject newPieceToMove) {
        pieceToMove = newPieceToMove;
    }

    public void setPosition(Vector2 newPosition) {
        position = newPosition;
    }

    public void setPromoSelector(Manager newManager, MoveSelector newMoveSelector, GameObject newPieceToMove, Vector2 newPosition) {
        setManager(newManager);
        setMoveSelector(newMoveSelector);
        setPieceToMove(newPieceToMove);
        setPosition(newPosition);
    }

    void OnMouseDown() {
        manager.preMovePiece(pieceToMove, position, "player", pieceToPromote);
        foreach (GameObject dot in GameObject.FindGameObjectsWithTag("moveSelector")){
            Destroy(dot);
        }
        foreach (GameObject dot in GameObject.FindGameObjectsWithTag("promoSelector")){
            Destroy(dot);
        }
    }
}
