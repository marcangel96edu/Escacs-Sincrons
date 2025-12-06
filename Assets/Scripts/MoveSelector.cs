using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelector : MonoBehaviour {
    [SerializeField]
    private Manager manager;
    [SerializeField]
    private GameObject pieceToMove;
    [SerializeField]
    private Vector2 position;
    [SerializeField]
    private List<GameObject> promoSelectors; 

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
        Piece pieceScript = pieceToMove.GetComponent<Piece>();
        if (pieceScript.getType().Contains("Pawn") && position.y == 7f){
            Vector2 promoSelectorPosition = manager.traduceSquare(position);
            promoSelectorPosition.y += 0.75f;
            promoSelectorPosition.x -= 0.75f;
            foreach (GameObject promoSelector in promoSelectors) {
                GameObject promoSelectorInstance = Instantiate(promoSelector, promoSelectorPosition, Quaternion.identity);
                PromoSelector promoSelectorScript = promoSelectorInstance.GetComponent<PromoSelector>();
                promoSelectorScript.setPromoSelector(manager, this, pieceToMove, position);
                promoSelectorPosition.x += 0.5f;
            }
        } else {
            if (pieceScript.getType() == "playerKing" && pieceScript.getPosition() == new Vector2(4f, 0f) && (position == new Vector2(2f, 0f) || position == new Vector2(6f, 0f))) {
                if (position == new Vector2(2f, 0f)) {
                    manager.preMovePiece(pieceToMove, position, "player", -1);
                    manager.preMovePiece2(manager.getPieceByPosition(new Vector2(0f, 0f)), new Vector2(3f, 0f), "player", -1);
                } else {
                    manager.preMovePiece(pieceToMove, position, "player", -1);
                    manager.preMovePiece2(manager.getPieceByPosition(new Vector2(7f, 0f)), new Vector2(5f, 0f), "player", -1);
                }
            } else {
                manager.preMovePiece(pieceToMove, position, "player", -1);
            }
            foreach (GameObject dot in GameObject.FindGameObjectsWithTag("moveSelector")){
                Destroy(dot);
            }
        }
    }
}
