using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> pieces;
    [SerializeField]
    private List<Vector2> piecesInitialPositions;
    [SerializeField]
    private List<string> pieceTypes;
    [SerializeField]
    private GameObject moveSelector;

    // Start is called before the first frame update
    void Start()  {
        InstantiateInitialPositions();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void eliminatePiece(Vector2 position) {
        Debug.Log("mao entra 3");
        foreach (GameObject piece in pieces) {
            Debug.Log("mao entra bucle");
            Piece pieceScript = piece.GetComponent<Piece>();
            if (pieceScript.getPosition() == position) {
                Debug.Log("mao entra eliminated");
                pieceScript.setEliminated();
            }
        }
    }

    public void placePiece(GameObject piece, Vector2 position) {
        Debug.Log("mao entra 1");
        Piece pieceScript = piece.GetComponent<Piece>();
        Debug.Log("mao entra 2");
        if (!pieceScript.getEliminated() && getTileStatus(position) != "empty") eliminatePiece(position);
        pieceScript.setPosition(position);
        piece.transform.position = traduceSquare(position);
        pieceScript.setMoved();
        resetAllPossibleMoves();
    }

    private void initiatePiece(GameObject piece, Vector2 position, string type) {
        Piece pieceScript = piece.GetComponent<Piece>();
        pieceScript.setType(type);
        pieceScript.setManager(this);
        pieceScript.setMoveSelector(moveSelector);
        pieceScript.setPosition(position);
        piece.transform.position = traduceSquare(position);
        
    }


    public Vector3 traduceSquare(Vector2 position) {
        return new Vector3(position.x * 0.7875f, position.y * 0.7875f, 0f);
    }

    private void InstantiateInitialPositions() {
        for (int i = 0; i < 32; ++i) {
            GameObject pieceInstance = Instantiate(pieces[i]);
            pieces[i] = pieceInstance;
            initiatePiece(pieceInstance, piecesInitialPositions[i], pieceTypes[i]); 
        }
        resetAllPossibleMoves();
    }

    private void resetAllPossibleMoves() {
        foreach(GameObject piece in pieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            pieceScript.setPossibleMoves();
        }
    }

    public string getTileStatus(Vector2 position) {
        foreach (GameObject piece in pieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            if (pieceScript.getPosition() == position) return pieceScript.getType();
        }
        return "empty";
    }

    public GameObject getPieceByPosition(Vector2 position) {
        foreach (GameObject piece in pieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            if (pieceScript.getPosition() == position) return piece;
        }
        return null;
    }

    public List<Vector2> getAllThreads(string playerType) {
        List<Vector2> allThreads = new List<Vector2>();

        foreach(GameObject piece in pieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            if (pieceScript.getType().Contains(playerType)) {
                List<Vector2> pieceThreads = pieceScript.getThreads();

                foreach(Vector2 thread in pieceThreads) {
                    if (!allThreads.Contains(thread)) {
                        allThreads.Add(thread);
                    }
                }
            }
        }
        return allThreads;
    }
}
