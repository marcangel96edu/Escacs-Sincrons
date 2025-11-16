using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {
    private Manager manager;
    private GameObject moveSelector;
    private bool eliminated;

    private string type = "none";
    private bool initialMove = true;
    private string[] piecesEnumerate = {"playerKing", "playerQueen", "playerRook", "playerBishop", "playerKnight", "playerPawn", 
                                        "enemyKing", "enemyQueen", "enemyRook", "enemyBishop", "enemyKnight", "enemyPawn"};
    private Vector2 position = new Vector2(-1f, -1f);
    private List<Vector2> possibleMoves = new List<Vector2>();
    private List<Vector2> threads = new List<Vector2>();

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        
    }

    void OnMouseDown() {
        foreach (GameObject dot in GameObject.FindGameObjectsWithTag("moveSelector")){
            Destroy(dot);
        }
        foreach(Vector2 move in possibleMoves) {
            GameObject moveSelectorInstance = Instantiate(moveSelector, manager.traduceSquare(move), Quaternion.identity);
            MoveSelector moveSelectorScript = moveSelectorInstance.GetComponent<MoveSelector>();
            moveSelectorScript.setManager(manager);
            moveSelectorScript.setPosition(move);
            moveSelectorScript.setPieceToMove(gameObject);
        }
    }

    public void setPossibleMoves() {
        possibleMoves = new List<Vector2>();
        threads = new List<Vector2>();
        if (!eliminated) {
            switch(type) {
                case "playerKing":
                    setPlayerKingPossiblesMoves();
                    break;
                case "playerQueen":
                    setRecursivePossiblesMoves("player", position, 0f, 1f);
                    setRecursivePossiblesMoves("player", position, 1f, 0f);
                    setRecursivePossiblesMoves("player", position, 0f, -1f);
                    setRecursivePossiblesMoves("player", position, -1f, 0f);

                    setRecursivePossiblesMoves("player", position, 1f, 1f);
                    setRecursivePossiblesMoves("player", position, 1f, -1f);
                    setRecursivePossiblesMoves("player", position, -1f, 1f);
                    setRecursivePossiblesMoves("player", position, -1f, -1f);
                    break;
                case "playerRook":
                    setRecursivePossiblesMoves("player", position, 0f, 1f);
                    setRecursivePossiblesMoves("player", position, 1f, 0f);
                    setRecursivePossiblesMoves("player", position, 0f, -1f);
                    setRecursivePossiblesMoves("player", position, -1f, 0f);
                    break;
                case "playerBishop":
                    setRecursivePossiblesMoves("player", position, 1f, 1f);
                    setRecursivePossiblesMoves("player", position, 1f, -1f);
                    setRecursivePossiblesMoves("player", position, -1f, 1f);
                    setRecursivePossiblesMoves("player", position, -1f, -1f);
                    break;
                case "playerKnight":
                    setKnightPossiblesMoves("player");
                    break;
                case "playerPawn":
                    setPlayerPawnPossiblesMoves();
                    break;
                case "enemyKing":
                    setEnemyKingPossiblesMoves();
                    break;
                case "enemyQueen":
                    setRecursivePossiblesMoves("enemy", position, 0f, 1f);
                    setRecursivePossiblesMoves("enemy", position, 1f, 0f);
                    setRecursivePossiblesMoves("enemy", position, 0f, -1f);
                    setRecursivePossiblesMoves("enemy", position, -1f, 0f);

                    setRecursivePossiblesMoves("enemy", position, 1f, 1f);
                    setRecursivePossiblesMoves("enemy", position, 1f, -1f);
                    setRecursivePossiblesMoves("enemy", position, -1f, 1f);
                    setRecursivePossiblesMoves("enemy", position, 1f, -1f);
                    break;
                case "enemyRook":
                    setRecursivePossiblesMoves("enemy", position, 0f, 1f);
                    setRecursivePossiblesMoves("enemy", position, 1f, 0f);
                    setRecursivePossiblesMoves("enemy", position, 0f, -1f);
                    setRecursivePossiblesMoves("enemy", position, -1f, 0f);
                    break;
                case "enemyBishop":
                    setRecursivePossiblesMoves("enemy", position, 1f, 1f);
                    setRecursivePossiblesMoves("enemy", position, 1f, -1f);
                    setRecursivePossiblesMoves("enemy", position, -1f, 1f);
                    setRecursivePossiblesMoves("enemy", position, -1f, -1f);
                    break;
                case "enemyKnight":
                    setKnightPossiblesMoves("enemy");
                    break;
                case "enemyPawn":
                    setEnemyPawnPossiblesMoves();
                    break;

                default:
                    break;
            }
        }
    }

    private void setPlayerKingPossiblesMoves() {
        Vector2 newPossiblePosition = new Vector2(position.x, position.y + 1);
        List<Vector2> enemyThreads = manager.getAllThreads("enemy");
        
        if (newPossiblePosition.y < 8f) {
            threads.Add(newPossiblePosition);

            if (!manager.getTileStatus(newPossiblePosition).Contains("player") && !enemyThreads.Contains(newPossiblePosition)) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x + 1, position.y + 1);
        if (newPossiblePosition.x < 8f && newPossiblePosition.y < 8f) {
            threads.Add(newPossiblePosition);

            if (!manager.getTileStatus(newPossiblePosition).Contains("player") && !enemyThreads.Contains(newPossiblePosition)) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x + 1, position.y);
        if (newPossiblePosition.x < 8f) {
            threads.Add(newPossiblePosition);

            if (!manager.getTileStatus(newPossiblePosition).Contains("player") && !enemyThreads.Contains(newPossiblePosition)) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x + 1, position.y - 1);
        if (newPossiblePosition.x < 8f && newPossiblePosition.y > -1f) {
            threads.Add(newPossiblePosition);

            if (!manager.getTileStatus(newPossiblePosition).Contains("player") && !enemyThreads.Contains(newPossiblePosition)) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x, position.y - 1);
        if (newPossiblePosition.y > -1f) {
            threads.Add(newPossiblePosition);

            if (!manager.getTileStatus(newPossiblePosition).Contains("player") && !enemyThreads.Contains(newPossiblePosition)) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x - 1, position.y - 1);
        if (newPossiblePosition.x > -1f && newPossiblePosition.y > -1f) {
            threads.Add(newPossiblePosition);

            if (!manager.getTileStatus(newPossiblePosition).Contains("player") && !enemyThreads.Contains(newPossiblePosition)) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x - 1, position.y);
        if (newPossiblePosition.x > -1f) {
            threads.Add(newPossiblePosition);

            if (!manager.getTileStatus(newPossiblePosition).Contains("player") && !enemyThreads.Contains(newPossiblePosition)) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x - 1, position.y + 1);
        if (newPossiblePosition.x > -1f && newPossiblePosition.y < 8f) {
            threads.Add(newPossiblePosition);

            if (!manager.getTileStatus(newPossiblePosition).Contains("player") && !enemyThreads.Contains(newPossiblePosition)) {
                possibleMoves.Add(newPossiblePosition);
            }
        }
    }

    private void setRecursivePossiblesMoves(string pieceType, Vector2 lastPosition, float incX, float incY) {
        Vector2 newPossiblePosition = new Vector2(lastPosition.x + incX, lastPosition.y + incY);
        if (newPossiblePosition.x < 8f && newPossiblePosition.x > -1f
            && newPossiblePosition.y < 8f && newPossiblePosition.y > -1f) {
            threads.Add(newPossiblePosition);

            if (!manager.getTileStatus(newPossiblePosition).Contains(pieceType)) {
                possibleMoves.Add(newPossiblePosition);
            }
            if (manager.getTileStatus(newPossiblePosition).Contains("empty")) {
                setRecursivePossiblesMoves(pieceType, newPossiblePosition, incX, incY);
            }
        }

    }

    private void setKnightPossiblesMoves(string pieceType) {
        Vector2 newPossiblePosition = new Vector2(position.x + 1, position.y + 2);
        if (newPossiblePosition.x < 8f && newPossiblePosition.y < 8f) {
            threads.Add(newPossiblePosition);

            if (!manager.getTileStatus(newPossiblePosition).Contains(pieceType)) {
                possibleMoves.Add(newPossiblePosition);   
            }
        }

        newPossiblePosition = new Vector2(position.x + 2, position.y + 1);
        if (newPossiblePosition.x < 8f && newPossiblePosition.y < 8f) {
            threads.Add(newPossiblePosition);

            if (!manager.getTileStatus(newPossiblePosition).Contains(pieceType)) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x + 2, position.y - 1);
        if (newPossiblePosition.x < 8f && newPossiblePosition.y > -1f ) {
            threads.Add(newPossiblePosition);

            if (!manager.getTileStatus(newPossiblePosition).Contains(pieceType)) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x + 1, position.y - 2);
        if (newPossiblePosition.x < 8f && newPossiblePosition.y > -1f ) {
            threads.Add(newPossiblePosition);

            if (!manager.getTileStatus(newPossiblePosition).Contains(pieceType)) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x - 1, position.y - 2);
        if (newPossiblePosition.x > -1f  && newPossiblePosition.y > -1f) {
            threads.Add(newPossiblePosition);

            if (!manager.getTileStatus(newPossiblePosition).Contains(pieceType)) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x - 2, position.y - 1);
        if (newPossiblePosition.x > -1f && newPossiblePosition.y > -1f) {
            threads.Add(newPossiblePosition);

            if (!manager.getTileStatus(newPossiblePosition).Contains(pieceType)) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x - 2, position.y + 1);
        if (newPossiblePosition.x > -1f && newPossiblePosition.y < 8f) {
            threads.Add(newPossiblePosition);
            if (!manager.getTileStatus(newPossiblePosition).Contains(pieceType)) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x - 1, position.y + 2);
        if (newPossiblePosition.x > -1f && newPossiblePosition.y < 8f) {
            threads.Add(newPossiblePosition);

            if (!manager.getTileStatus(newPossiblePosition).Contains(pieceType)) {
                possibleMoves.Add(newPossiblePosition);
            }
        }
    }

    private void setPlayerPawnPossiblesMoves() {
        Vector2 newPossiblePosition = new Vector2(position.x, position.y + 1);
        if (newPossiblePosition.y < 8f && manager.getTileStatus(newPossiblePosition) == "empty") {
            possibleMoves.Add(newPossiblePosition);
        }

        newPossiblePosition = new Vector2(position.x + 1, position.y + 1);
        if (newPossiblePosition.x < 8f && newPossiblePosition.y < 8f) {
            threads.Add(newPossiblePosition);

            if (manager.getTileStatus(newPossiblePosition).Contains("enemy")) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x - 1, position.y + 1);
        if (newPossiblePosition.x > -1f  && newPossiblePosition.y < 8f) {
            threads.Add(newPossiblePosition);

            if (manager.getTileStatus(newPossiblePosition).Contains("enemy")) {
                possibleMoves.Add(newPossiblePosition);
            }
        }
        
        newPossiblePosition = new Vector2(position.x, position.y + 2);
        if (initialMove && manager.getTileStatus(new Vector2(position.x, position.y + 1)) == "empty"
        && manager.getTileStatus(newPossiblePosition) == "empty") {
            possibleMoves.Add(newPossiblePosition);
        }
    }

    private void setEnemyKingPossiblesMoves() {
        Vector2 newPossiblePosition = new Vector2(position.x, position.y + 1);
        List<Vector2> playerThreads = manager.getAllThreads("player");
        if (newPossiblePosition.y < 8f) {
            threads.Add(newPossiblePosition);

            if (!manager.getTileStatus(newPossiblePosition).Contains("enemy") && !playerThreads.Contains(newPossiblePosition)) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x + 1, position.y + 1);
        if (newPossiblePosition.x < 8f && newPossiblePosition.y < 8f) {
            threads.Add(newPossiblePosition);

            if (!manager.getTileStatus(newPossiblePosition).Contains("enemy") && !playerThreads.Contains(newPossiblePosition)) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x + 1, position.y);
        if (newPossiblePosition.x < 8f) {
            threads.Add(newPossiblePosition);

            if (!manager.getTileStatus(newPossiblePosition).Contains("enemy") && !playerThreads.Contains(newPossiblePosition)) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x + 1, position.y - 1);
        if (newPossiblePosition.x < 8f && newPossiblePosition.y > -1f) {
            threads.Add(newPossiblePosition);

            if (!manager.getTileStatus(newPossiblePosition).Contains("enemy") && !playerThreads.Contains(newPossiblePosition)) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x, position.y - 1);
        if (newPossiblePosition.y > -1f) {
            threads.Add(newPossiblePosition);

            if (!manager.getTileStatus(newPossiblePosition).Contains("enemy") && !playerThreads.Contains(newPossiblePosition)) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x - 1, position.y - 1);
        if (newPossiblePosition.x > -1f && newPossiblePosition.y > -1f) {
            threads.Add(newPossiblePosition);

            if (!manager.getTileStatus(newPossiblePosition).Contains("enemy") && !playerThreads.Contains(newPossiblePosition)) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x - 1, position.y);
        if (newPossiblePosition.x > -1f) {
            threads.Add(newPossiblePosition);

            if (!manager.getTileStatus(newPossiblePosition).Contains("enemy") && !playerThreads.Contains(newPossiblePosition)) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x - 1, position.y + 1);
        if (newPossiblePosition.x > -1f && newPossiblePosition.y < 8f) {
            threads.Add(newPossiblePosition);

            if (!manager.getTileStatus(newPossiblePosition).Contains("enemy") && !playerThreads.Contains(newPossiblePosition)) {
                possibleMoves.Add(newPossiblePosition);
            }
        }
    }

    private void setEnemyPawnPossiblesMoves() {
        Vector2 newPossiblePosition = new Vector2(position.x, position.y - 1);
        if (newPossiblePosition.y > -1f && manager.getTileStatus(newPossiblePosition) == "empty") {
            possibleMoves.Add(newPossiblePosition);
        }

        newPossiblePosition = new Vector2(position.x + 1, position.y - 1);
        if (newPossiblePosition.x < 8f && newPossiblePosition.y > -1f) {
            threads.Add(newPossiblePosition);

            if (manager.getTileStatus(newPossiblePosition).Contains("player")) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x - 1, position.y - 1);
        if (newPossiblePosition.x > -1f  && newPossiblePosition.y > -1f) {
            threads.Add(newPossiblePosition);

            if (manager.getTileStatus(newPossiblePosition).Contains("player")) {
                possibleMoves.Add(newPossiblePosition);
            }
        }
        
        newPossiblePosition = new Vector2(position.x, position.y - 2);
        if (initialMove && manager.getTileStatus(new Vector2(position.x, position.y - 1)) == "empty"
        && manager.getTileStatus(newPossiblePosition) == "empty") {
            possibleMoves.Add(newPossiblePosition);
        }
    }

    public void setPosition(Vector2 newPosition) {
        position = newPosition;
    }

    public Vector2 getPosition() {
        return position;
    }

    public void setType(string newType) {
        type = newType;
    }

    public string getType() {
        return type;
    }

    public List<Vector2> getThreads() {
        return threads;
    }

    public void setManager(Manager newManager) {
        manager = newManager;
    }

    public void setMoveSelector(GameObject newMoveSelector) {
        moveSelector = newMoveSelector;
    }

    public void setMoved() {
        initialMove = false;
    }

    public void setEliminated() {
        eliminated = true;
        manager.placePiece(gameObject, new Vector2(-1f, -1f));
    }

    public bool getEliminated() {
        return eliminated;
    }
}
