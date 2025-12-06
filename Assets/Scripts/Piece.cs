using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {
    private Manager manager;
    private GameObject moveSelector;
    [SerializeField]
    private GameObject auxPiece;
    [SerializeField]
    private bool eliminated;
    [SerializeField]
    private List<Sprite> promoSprites;
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private string type = "none";
    private string[] piecesEnumerate = {"playerKing", "playerQueen", "playerRook", "playerBishop", "playerKnight", "playerPawn", 
                                        "enemyKing", "enemyQueen", "enemyRook", "enemyBishop", "enemyKnight", "enemyPawn"};
    [SerializeField]
    private Vector2 position = new Vector2(-1f, -1f);
    [SerializeField]
    private List<Vector2> possibleMoves = new List<Vector2>();
    [SerializeField]
    private List<Vector2> threads = new List<Vector2>();

    [SerializeField]
    private int lastMoveTempo = -1;

    private BoxCollider2D pieceCollider;

    // Start is called before the first frame update
    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        
    }

    void OnMouseDown() {
        if (type.Contains("player")) {
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
                    setRecursivePossiblesMoves("player", position, -1f, -1f);
                    setRecursivePossiblesMoves("player", position, -1f, 1f);
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
                    setRecursivePossiblesMoves("player", position, -1f, -1f);
                    setRecursivePossiblesMoves("player", position, -1f, 1f);
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
                    setRecursivePossiblesMoves("enemy", position, -1f, -1f);
                    setRecursivePossiblesMoves("enemy", position, -1f, 1f);
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
                    setRecursivePossiblesMoves("enemy", position, -1f, -1f);
                    setRecursivePossiblesMoves("enemy", position, -1f, 1f);
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
        
        if (newPossiblePosition.y < 8f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, "player");
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains("player") && !manager.isAuxMate("player")) {
                    possibleMoves.Add(newPossiblePosition);
                }
                manager.resetAuxPieces();
            } else if (!manager.getAuxTileStatus(newPossiblePosition).Contains("player")) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x + 1, position.y + 1);
        if (newPossiblePosition.x < 8f && newPossiblePosition.y < 8f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, "player");
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains("player") && !manager.isAuxMate("player")) {
                    possibleMoves.Add(newPossiblePosition);
                }
                manager.resetAuxPieces();
            } else if (!manager.getAuxTileStatus(newPossiblePosition).Contains("player")) {
                possibleMoves.Add(newPossiblePosition);
            } 
        }

        newPossiblePosition = new Vector2(position.x + 1, position.y);
        if (newPossiblePosition.x < 8f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, "player");
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains("player") && !manager.isAuxMate("player")) {
                    possibleMoves.Add(newPossiblePosition);
                }
                manager.resetAuxPieces();
            } else if (!manager.getAuxTileStatus(newPossiblePosition).Contains("player")) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x + 1, position.y - 1);
        if (newPossiblePosition.x < 8f && newPossiblePosition.y > -1f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, "player");
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains("player") && !manager.isAuxMate("player")) {
                    possibleMoves.Add(newPossiblePosition);
                }
                manager.resetAuxPieces();
            } else if (!manager.getAuxTileStatus(newPossiblePosition).Contains("player")) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x, position.y - 1);
        if (newPossiblePosition.y > -1f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, "player");
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains("player") && !manager.isAuxMate("player")) {
                    possibleMoves.Add(newPossiblePosition);
                }
                manager.resetAuxPieces();
            } else if (!manager.getAuxTileStatus(newPossiblePosition).Contains("player")) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x - 1, position.y - 1);
        if (newPossiblePosition.x > -1f && newPossiblePosition.y > -1f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, "player");
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains("player") && !manager.isAuxMate("player")) {
                    possibleMoves.Add(newPossiblePosition);
                }
                manager.resetAuxPieces();
            } else if (!manager.getAuxTileStatus(newPossiblePosition).Contains("player")) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x - 1, position.y);
        if (newPossiblePosition.x > -1f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, "player");
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains("player") && !manager.isAuxMate("player")) {
                    possibleMoves.Add(newPossiblePosition);
                }
                manager.resetAuxPieces();
            } else if (!manager.getAuxTileStatus(newPossiblePosition).Contains("player")) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x - 1, position.y + 1);
        if (newPossiblePosition.x > -1f && newPossiblePosition.y < 8f) {
            threads.Add(newPossiblePosition);
            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, "player");
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains("player") && !manager.isAuxMate("player")) {
                    possibleMoves.Add(newPossiblePosition);
                }
                manager.resetAuxPieces();
            } else if (!manager.getAuxTileStatus(newPossiblePosition).Contains("player")) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        if (lastMoveTempo == -1 && !manager.isMate("player")) {
            Vector2 auxPosition = new Vector2(5f, 0f);
            newPossiblePosition = new Vector2(6f, 0f);
            GameObject rook = manager.getPieceByPosition(new Vector2(7f, 0f)); 
            List<Vector2> enemyThreads = manager.getAllThreads("enemy");

            if (rook != null) {
                Piece rookScript = rook.GetComponent<Piece>();
                if (rookScript.getType() == "playerRook" && rookScript.getLastMoveTempo() == -1 &&
                    manager.getTileStatus(auxPosition) == "empty" && manager.getTileStatus(newPossiblePosition) == "empty" &&
                    !enemyThreads.Contains(auxPosition) && !enemyThreads.Contains(newPossiblePosition)) {
                    possibleMoves.Add(newPossiblePosition);
                }
            }

            auxPosition = new Vector2(3f, 0f);
            newPossiblePosition = new Vector2(2f, 0f);
            rook = manager.getPieceByPosition(new Vector2(0f, 0f)); 

            if (rook != null) {
                Piece rookScript = rook.GetComponent<Piece>();
                if (rookScript.getType() == "playerRook" && rookScript.getLastMoveTempo() == -1 &&
                    manager.getTileStatus(auxPosition) == "empty" && manager.getTileStatus(newPossiblePosition) == "empty" && manager.getTileStatus(new Vector2(1f, 0f)) == "empty" &&
                    !enemyThreads.Contains(auxPosition) && !enemyThreads.Contains(newPossiblePosition)) {
                    possibleMoves.Add(newPossiblePosition);
                }
            }
        }
    }

    private void setRecursivePossiblesMoves(string pieceType, Vector2 lastPosition, float incX, float incY) {
        Vector2 newPossiblePosition = new Vector2(lastPosition.x + incX, lastPosition.y + incY);
        if (newPossiblePosition.x < 8f && newPossiblePosition.x > -1f
            && newPossiblePosition.y < 8f && newPossiblePosition.y > -1f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, pieceType);
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains(pieceType) && !manager.isAuxMate(pieceType)) {
                    possibleMoves.Add(newPossiblePosition);
                }
                manager.resetAuxPieces();

                if (manager.getTileStatus(newPossiblePosition).Contains("empty")) {
                    setRecursivePossiblesMoves(pieceType, newPossiblePosition, incX, incY);
                }
            } else {
                if (!manager.getAuxTileStatus(newPossiblePosition).Contains(pieceType)) {
                    possibleMoves.Add(newPossiblePosition);
                }
                if (manager.getAuxTileStatus(newPossiblePosition).Contains("empty")) {
                    setRecursivePossiblesMoves(pieceType, newPossiblePosition, incX, incY);
                }
            }            
        }
    }

    private void setKnightPossiblesMoves(string pieceType) {
        Vector2 newPossiblePosition = new Vector2(position.x + 1, position.y + 2);

        if (newPossiblePosition.x < 8f && newPossiblePosition.y < 8f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, pieceType);
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains(pieceType) && !manager.isAuxMate(pieceType)) {
                    possibleMoves.Add(newPossiblePosition);   
                }
                manager.resetAuxPieces();
            } else if (!manager.getAuxTileStatus(newPossiblePosition).Contains(pieceType)) {
                possibleMoves.Add(newPossiblePosition);   
            }
            
        }

        newPossiblePosition = new Vector2(position.x + 2, position.y + 1);
        if (newPossiblePosition.x < 8f && newPossiblePosition.y < 8f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, pieceType);
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains(pieceType) && !manager.isAuxMate(pieceType)) {
                    possibleMoves.Add(newPossiblePosition);   
                }
                manager.resetAuxPieces();
            } else if (!manager.getAuxTileStatus(newPossiblePosition).Contains(pieceType)) {
                possibleMoves.Add(newPossiblePosition);   
            }
        }

        newPossiblePosition = new Vector2(position.x + 2, position.y - 1);
        if (newPossiblePosition.x < 8f && newPossiblePosition.y > -1f ) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, pieceType);
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains(pieceType) && !manager.isAuxMate(pieceType)) {
                    possibleMoves.Add(newPossiblePosition);   
                }
                manager.resetAuxPieces();
            } else if (!manager.getAuxTileStatus(newPossiblePosition).Contains(pieceType)) {
                possibleMoves.Add(newPossiblePosition);   
            }
        }

        newPossiblePosition = new Vector2(position.x + 1, position.y - 2);
        if (newPossiblePosition.x < 8f && newPossiblePosition.y > -1f ) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, pieceType);
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains(pieceType) && !manager.isAuxMate(pieceType)) {
                    possibleMoves.Add(newPossiblePosition);   
                }
                manager.resetAuxPieces();
            } else if (!manager.getAuxTileStatus(newPossiblePosition).Contains(pieceType)) {
                possibleMoves.Add(newPossiblePosition);   
            }
        }

        newPossiblePosition = new Vector2(position.x - 1, position.y - 2);
        if (newPossiblePosition.x > -1f  && newPossiblePosition.y > -1f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, pieceType);
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains(pieceType) && !manager.isAuxMate(pieceType)) {
                    possibleMoves.Add(newPossiblePosition);   
                }
                manager.resetAuxPieces();
            } else if (!manager.getAuxTileStatus(newPossiblePosition).Contains(pieceType)) {
                possibleMoves.Add(newPossiblePosition);   
            }
        }

        newPossiblePosition = new Vector2(position.x - 2, position.y - 1);
        if (newPossiblePosition.x > -1f && newPossiblePosition.y > -1f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, pieceType);
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains(pieceType) && !manager.isAuxMate(pieceType)) {
                    possibleMoves.Add(newPossiblePosition);   
                }
                manager.resetAuxPieces();
            } else if (!manager.getAuxTileStatus(newPossiblePosition).Contains(pieceType)) {
                possibleMoves.Add(newPossiblePosition);   
            }
        }

        newPossiblePosition = new Vector2(position.x - 2, position.y + 1);
        if (newPossiblePosition.x > -1f && newPossiblePosition.y < 8f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, pieceType);
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains(pieceType) && !manager.isAuxMate(pieceType)) {
                    possibleMoves.Add(newPossiblePosition);   
                }
                manager.resetAuxPieces();
            } else if (!manager.getAuxTileStatus(newPossiblePosition).Contains(pieceType)) {
                possibleMoves.Add(newPossiblePosition);   
            }
        }

        newPossiblePosition = new Vector2(position.x - 1, position.y + 2);
        if (newPossiblePosition.x > -1f && newPossiblePosition.y < 8f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, pieceType);
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains(pieceType) && !manager.isAuxMate(pieceType)) {
                    possibleMoves.Add(newPossiblePosition);   
                }
                manager.resetAuxPieces();
            } else if (!manager.getAuxTileStatus(newPossiblePosition).Contains(pieceType)) {
                possibleMoves.Add(newPossiblePosition);   
            }
        }
    }

    private void setPlayerPawnPossiblesMoves() {
        Vector2 newPossiblePosition = new Vector2(position.x, position.y + 1);

        if (auxPiece != null) {
            manager.preMoveAuxPiece(auxPiece, newPossiblePosition, "player");
            manager.setAuxMoves();
            if (newPossiblePosition.y < 8f && manager.getTileStatus(newPossiblePosition) == "empty" && !manager.isAuxMate("player")) {
                possibleMoves.Add(newPossiblePosition);   
            }
            manager.resetAuxPieces();
        } else if (newPossiblePosition.y < 8f && manager.getAuxTileStatus(newPossiblePosition) == "empty") {
            possibleMoves.Add(newPossiblePosition);
        }

        newPossiblePosition = new Vector2(position.x + 1, position.y + 1);
        if (newPossiblePosition.x < 8f && newPossiblePosition.y < 8f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, "player");
                manager.setAuxMoves();
                if (manager.getTileStatus(newPossiblePosition).Contains("enemy") && !manager.isAuxMate("player")) {
                    possibleMoves.Add(newPossiblePosition);   
                }
                manager.resetAuxPieces();
            } else if (manager.getAuxTileStatus(newPossiblePosition).Contains("enemy")) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x - 1, position.y + 1);
        if (newPossiblePosition.x > -1f  && newPossiblePosition.y < 8f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, "player");
                manager.setAuxMoves();
                if (manager.getTileStatus(newPossiblePosition).Contains("enemy") && !manager.isAuxMate("player")) {
                    possibleMoves.Add(newPossiblePosition);   
                }
                manager.resetAuxPieces();
            } else if (manager.getAuxTileStatus(newPossiblePosition).Contains("enemy")) {
                possibleMoves.Add(newPossiblePosition);
            }
        }
        
        newPossiblePosition = new Vector2(position.x, position.y + 2);
        if (auxPiece != null) {
            manager.preMoveAuxPiece(auxPiece, newPossiblePosition, "enemy");
            manager.setAuxMoves();
            if (lastMoveTempo == -1 && manager.getTileStatus(new Vector2(position.x, position.y + 1)) == "empty" && manager.getTileStatus(newPossiblePosition) == "empty" && !manager.isAuxMate("player")) {
                possibleMoves.Add(newPossiblePosition);   
            }
            manager.resetAuxPieces();
        } else if (lastMoveTempo == -1 && manager.getAuxTileStatus(new Vector2(position.x, position.y + 1)) == "empty" && manager.getAuxTileStatus(newPossiblePosition) == "empty") {
        possibleMoves.Add(newPossiblePosition);
        }
    }

    private void setEnemyKingPossiblesMoves() {
        Vector2 newPossiblePosition = new Vector2(position.x, position.y + 1);
        if (newPossiblePosition.y < 8f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, "enemy");
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains("enemy") && !manager.isAuxMate("enemy")) {
                    possibleMoves.Add(newPossiblePosition);
                }
                manager.resetAuxPieces();
            } else if (!manager.getAuxTileStatus(newPossiblePosition).Contains("enemy")) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x + 1, position.y + 1);
        if (newPossiblePosition.x < 8f && newPossiblePosition.y < 8f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {            
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, "enemy");
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains("enemy") && !manager.isAuxMate("enemy")) {
                    possibleMoves.Add(newPossiblePosition);
                }
                manager.resetAuxPieces();
            } else if (!manager.getAuxTileStatus(newPossiblePosition).Contains("enemy")) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x + 1, position.y);
        if (newPossiblePosition.x < 8f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, "enemy");
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains("enemy") && !manager.isAuxMate("enemy")) {
                    possibleMoves.Add(newPossiblePosition);
                }
                manager.resetAuxPieces();
            } else if (!manager.getAuxTileStatus(newPossiblePosition).Contains("enemy")) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x + 1, position.y - 1);
        if (newPossiblePosition.x < 8f && newPossiblePosition.y > -1f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, "enemy");
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains("enemy") && !manager.isAuxMate("enemy")) {
                    possibleMoves.Add(newPossiblePosition);
                }
                manager.resetAuxPieces();
            } else if (!manager.getAuxTileStatus(newPossiblePosition).Contains("enemy")) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x, position.y - 1);
        if (newPossiblePosition.y > -1f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, "enemy");
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains("enemy") && !manager.isAuxMate("enemy")) {
                    possibleMoves.Add(newPossiblePosition);
                }
                manager.resetAuxPieces();
            } else if (!manager.getAuxTileStatus(newPossiblePosition).Contains("enemy")) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x - 1, position.y - 1);
        if (newPossiblePosition.x > -1f && newPossiblePosition.y > -1f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, "enemy");
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains("enemy") && !manager.isAuxMate("enemy")) {
                    possibleMoves.Add(newPossiblePosition);
                }
                manager.resetAuxPieces();
            } else if (!manager.getAuxTileStatus(newPossiblePosition).Contains("enemy")) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x - 1, position.y);
        if (newPossiblePosition.x > -1f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, "enemy");
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains("enemy") && !manager.isAuxMate("enemy")) {
                    possibleMoves.Add(newPossiblePosition);
                }
                manager.resetAuxPieces();
            } else if (!manager.getAuxTileStatus(newPossiblePosition).Contains("enemy")) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x - 1, position.y + 1);
        if (newPossiblePosition.x > -1f && newPossiblePosition.y < 8f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, "enemy");
                manager.setAuxMoves();
                if (!manager.getTileStatus(newPossiblePosition).Contains("enemy") && !manager.isAuxMate("enemy")) {
                    possibleMoves.Add(newPossiblePosition);
                }
                manager.resetAuxPieces();
            } else if (!manager.getAuxTileStatus(newPossiblePosition).Contains("enemy")) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        if (lastMoveTempo == -1 && !manager.isMate("enemy")) {
            Vector2 auxPosition = new Vector2(5f, 7f);
            newPossiblePosition = new Vector2(6f, 7f);
            GameObject rook = manager.getPieceByPosition(new Vector2(7f, 7f)); 
            List<Vector2> playerThreads = manager.getAllThreads("player");

            if (rook != null) {
                Piece rookScript = rook.GetComponent<Piece>();
                if (rookScript.getType() == "enemyRook" && rookScript.getLastMoveTempo() == -1 &&
                    manager.getTileStatus(auxPosition) == "empty" && manager.getTileStatus(newPossiblePosition) == "empty" &&
                    !playerThreads.Contains(auxPosition) && !playerThreads.Contains(newPossiblePosition)) {
                    possibleMoves.Add(newPossiblePosition);
                }
            }

            auxPosition = new Vector2(3f, 7f);
            newPossiblePosition = new Vector2(2f, 7f);
            rook = manager.getPieceByPosition(new Vector2(0f, 7f)); 

            if (rook != null) {
                Piece rookScript = rook.GetComponent<Piece>();
                if (rookScript.getType() == "enemyRook" && rookScript.getLastMoveTempo() == -1 &&
                    manager.getTileStatus(auxPosition) == "empty" && manager.getTileStatus(newPossiblePosition) == "empty" && manager.getTileStatus(new Vector2(1f, 7f)) == "empty" &&
                    !playerThreads.Contains(auxPosition) && !playerThreads.Contains(newPossiblePosition)) {
                    possibleMoves.Add(newPossiblePosition);
                }
            }
        }
    }

    private void setEnemyPawnPossiblesMoves() {
        Vector2 newPossiblePosition = new Vector2(position.x, position.y - 1);

        if (auxPiece != null) {
            manager.preMoveAuxPiece(auxPiece, newPossiblePosition, "enemy");
            manager.setAuxMoves();
            if (newPossiblePosition.y > -1f && manager.getTileStatus(newPossiblePosition) == "empty" && !manager.isAuxMate("enemy")) {
                possibleMoves.Add(newPossiblePosition);   
            }
            manager.resetAuxPieces();
        } else if (newPossiblePosition.y > -1f && manager.getAuxTileStatus(newPossiblePosition) == "empty") {
            possibleMoves.Add(newPossiblePosition);
        }

        newPossiblePosition = new Vector2(position.x + 1, position.y - 1);
        if (newPossiblePosition.x < 8f && newPossiblePosition.y > -1f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, "enemy");
                manager.setAuxMoves();
                if (manager.getTileStatus(newPossiblePosition).Contains("player") && !manager.isAuxMate("enemy")) {
                    possibleMoves.Add(newPossiblePosition);   
                }
                manager.resetAuxPieces();
            } else if (manager.getAuxTileStatus(newPossiblePosition).Contains("player")) {
                possibleMoves.Add(newPossiblePosition);
            }
        }

        newPossiblePosition = new Vector2(position.x - 1, position.y - 1);
        if (newPossiblePosition.x > -1f  && newPossiblePosition.y > -1f) {
            threads.Add(newPossiblePosition);

            if (auxPiece != null) {
                manager.preMoveAuxPiece(auxPiece, newPossiblePosition, "enemy");
                manager.setAuxMoves();
                if (manager.getTileStatus(newPossiblePosition).Contains("player") && !manager.isAuxMate("enemy")) {
                    possibleMoves.Add(newPossiblePosition);   
                }
                manager.resetAuxPieces();
            } else if (manager.getAuxTileStatus(newPossiblePosition).Contains("player")) {
                possibleMoves.Add(newPossiblePosition);
            }
        }
        
        newPossiblePosition = new Vector2(position.x, position.y - 2);
        if (auxPiece != null) {
            manager.preMoveAuxPiece(auxPiece, newPossiblePosition, "enemy");
            manager.setAuxMoves();
            if (lastMoveTempo == -1 && manager.getTileStatus(new Vector2(position.x, position.y - 1)) == "empty" && manager.getTileStatus(newPossiblePosition) == "empty" && !manager.isAuxMate("enemy")) {
                possibleMoves.Add(newPossiblePosition);   
            }
            manager.resetAuxPieces();
        } else if (lastMoveTempo == -1 && manager.getAuxTileStatus(new Vector2(position.x, position.y - 1)) == "empty" && manager.getAuxTileStatus(newPossiblePosition) == "empty") {
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
        if (newType.Contains("enemy")) {
            pieceCollider = GetComponent<BoxCollider2D>();
            pieceCollider.enabled = false;
        }
    }

    public string getType() {
        return type;
    }

    public List<Vector2> getThreads() {
        return threads;
    }

    public void setThreads(List<Vector2> newThreads) {
        threads = newThreads;
    }

    public void setManager(Manager newManager) {
        manager = newManager;
    }

    public void setMoveSelector(GameObject newMoveSelector) {
        moveSelector = newMoveSelector;
    }

    public void setEliminated(bool newEliminated) {
        eliminated = newEliminated;
    }

    public bool getEliminated() {
        return eliminated;
    }

    public void setLastMoveTempo(int newLastMoveTempo) {
        lastMoveTempo = newLastMoveTempo;
    }

    public int getLastMoveTempo() {
        return lastMoveTempo;
    }

    public void enablePiece(bool enable) {
        pieceCollider = GetComponent<BoxCollider2D>();
        pieceCollider.enabled = enable;
    }

    public List<Vector2> getPossibleMoves() {
        return possibleMoves;
    }

    public void setPossibleMovesLiteral(List<Vector2> newPossibleMoves) {
        possibleMoves = newPossibleMoves;
    }

    public void setAuxPieceAsociated(GameObject newAuxPieceAsociated) {
        auxPiece = newAuxPieceAsociated;
    }

    public GameObject getAuxPiece() {
        return auxPiece;
    }

    public void setSprite(int promotionSprite) {
        spriteRenderer.sprite = promoSprites[promotionSprite];
    }
}
