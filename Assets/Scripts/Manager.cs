using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Manager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> originalPieces;
    [SerializeField]
    private List<GameObject> pieces;
    [SerializeField]
    private List<Vector2> piecesInitialPositions;
    [SerializeField]
    private List<string> pieceTypes;
    [SerializeField]
    private GameObject moveSelector;
    [SerializeField]
    private float moveTime;
    [SerializeField]
    private TMP_Text moveTimerText;
    [SerializeField]
    private TMP_Text evalText;
    [SerializeField]
    private TMP_Text evalAuxText;
    [SerializeField]
    private TMP_Text livesText;
    [SerializeField]
    private int algorithmId;
    [SerializeField]
    private int miniMaxDepth;
    [SerializeField]
    private List<GameObject> auxPieces;
    [SerializeField]
    private List<string> promotion;
    [SerializeField]
    private List<float> piecesPoints;
    [SerializeField]
    private float killPoints;
    [SerializeField]
    private float matePoints;
    [SerializeField]
    private float extraMatePoints;
    [SerializeField]
    private float savePoints;
    [SerializeField]
    private float promotePoints;
    [SerializeField]
    private float castlingPoints;
    [SerializeField]
    private float demotionPoints;

    [SerializeField]
    private int maxLives;
    [SerializeField]
    private int lives;

    [SerializeField]
    private float mateEval;
    [SerializeField]
    private float aviableEval;
    [SerializeField]
    private float movesEval;
    [SerializeField]
    private float threadEval;

    [SerializeField]
    private GameObject enemyPieceToMove = null;
    [SerializeField]
    private Vector2 enemyPositionToMove = new Vector2(-1f, -1f);
    [SerializeField]
    private int enemyPieceToPromote = -1;
    [SerializeField]
    private GameObject enemyPieceToMove2 = null;
    [SerializeField]
    private Vector2 enemyPositionToMove2 = new Vector2(-1f, -1f);

    [SerializeField]
    private GameObject playerPieceToMove = null;
    [SerializeField]
    private Vector2 playerPositionToMove = new Vector2(-1f, -1f);
    [SerializeField]
    private int playerPieceToPromote = -1;
    [SerializeField]
    private GameObject playerPieceToMove2 = null;
    [SerializeField]
    private Vector2 playerPositionToMove2 = new Vector2(-1f, -1f);

    private GameObject enemyAuxPieceToMove = null;
    private Vector2 enemyAuxPositionToMove = new Vector2(-1f, -1f);
    private int enemyAuxPieceToPromote = -1;
    private GameObject enemyAuxPieceToMove2 = null;
    private Vector2 enemyAuxPositionToMove2 = new Vector2(-1f, -1f);

    private GameObject playerAuxPieceToMove = null;
    private Vector2 playerAuxPositionToMove = new Vector2(-1f, -1f);
    private int playerAuxPieceToPromote = -1;
    private GameObject playerAuxPieceToMove2 = null;
    private Vector2 playerAuxPositionToMove2 = new Vector2(-1f, -1f);

    private float moveTimer;
    [SerializeField]
    private int tempo = 0;
    [SerializeField]
    private bool playing = false;
    [SerializeField]
    private bool startPlaying = false;
    // Start is called before the first frame update
    void Start()  {
        InstantiateInitialPositions();
        setEnabledMoves(false);
        moveTimer = moveTime;
    }

    // Update is called once per frame
    void Update() {
        if (startPlaying) {
            InstantiateInitialPositions();
            //evalText.text = evaluate().ToString("0.00");
            //evalAuxText.text = evaluateAux().ToString("0.00");
            lives = maxLives;
            livesText.text = "lives: " + lives;
            moveTimer = moveTime;
            setEnabledMoves(true);
            tempo = 0;
            setPreMove();
            startPlaying = false;
            playing = true;
        }
        if (playing) advanceTimer();
    }

    public void setStartPlaying(bool newStartPlaying) {
        startPlaying = newStartPlaying;
    }

    public void setAlgorithmId(int newAlgorithmId) {
        algorithmId = newAlgorithmId;
    }

    public void exitApp() {
        Application.Quit();
    }

    private void advanceTimer() {
        moveTimer -= Time.deltaTime;
        moveTimerText.text = moveTimer.ToString("0.00");

        if (moveTimer <= 0f) {
            ++tempo;
            setMoves();
            livesText.text = "lives: " + lives;
            resetAllPossibleMoves();
            resetAuxPieces();
            //evalText.text = evaluate().ToString("0.00");
            //evalAuxText.text = evaluateAux().ToString("0.00");
            checkFinishConditions();
            if (playing) setPreMove();
            moveTimer = moveTime;
        }
    }

    private void checkFinishConditions() {
        if (lives <= 0) {
            moveTimerText.text = "You Lost!";
            playing = false;
        }
        if (checkDraw()) {
                moveTimerText.text = "Draw!";
                playing = false;
            }
            if (checkWin("player") && checkWin("enemy")) {
                moveTimerText.text = "Draw!";
                playing = false;
            }
            if (checkWin("player")) {
                moveTimerText.text = "You won!";
                playing = false;
            }
            else if (checkWin("enemy")) {
                moveTimerText.text = "You Lost!";
                playing = false;
            }
    }

    private bool checkDraw() {
        if (!isMate("enemy") && getAllMoves("enemy").Count == 0) return true;
        if (!isMate("player") && getAllMoves("player").Count == 0) return true; 

        List<GameObject> alivePieces = new List<GameObject>();
        bool enemyKingAlive = false;
        bool playerKingAlive = false;

        foreach(GameObject piece in pieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            if (!pieceScript.getEliminated()) {
                alivePieces.Add(piece);
                if (pieceScript.getType() == "playerKing") playerKingAlive = true;
                if (pieceScript.getType() == "enemyKing") enemyKingAlive = true;
            }
        }
        if (!(enemyKingAlive && playerKingAlive)) return false;

        if (alivePieces.Count == 2) return true;
        if (alivePieces.Count == 3) {
            foreach(GameObject piece in alivePieces) {
                Piece pieceScript = piece.GetComponent<Piece>();
                if (pieceScript.getType().Contains("Knight")) return true;
                if (pieceScript.getType().Contains("Bishop")) return true;
            }
        }

        if (alivePieces.Count == 4) {
            bool enemyBishopAlive = false;
            bool playerBishopAlive = false;
            foreach(GameObject piece in alivePieces) {
                Piece pieceScript = piece.GetComponent<Piece>();
                if (pieceScript.getType().Contains("enemyBishop")) enemyBishopAlive = true;
                if (pieceScript.getType().Contains("playerBishop")) playerBishopAlive = true;
            }
            if (enemyBishopAlive && playerBishopAlive) return true;
        }
        return false;
    }

    private bool checkAuxDraw() {
        if (!isAuxMate("enemy") && getAllAuxMoves("enemy").Count == 0) return true;
        if (!isAuxMate("player") && getAllAuxMoves("player").Count == 0) return true; 

        List<GameObject> alivePieces = new List<GameObject>();
        bool enemyKingAlive = false;
        bool playerKingAlive = false;

        foreach(GameObject piece in auxPieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            if (!pieceScript.getEliminated()) {
                alivePieces.Add(piece);
                if (pieceScript.getType() == "playerKing") playerKingAlive = true;
                if (pieceScript.getType() == "enemyKing") enemyKingAlive = true;
            }
        }
        if (!(enemyKingAlive && playerKingAlive)) return false;

        if (alivePieces.Count == 2) return true;
        if (alivePieces.Count == 3) {
            foreach(GameObject piece in alivePieces) {
                Piece pieceScript = piece.GetComponent<Piece>();
                if (pieceScript.getType().Contains("Knight")) return true;
                if (pieceScript.getType().Contains("Bishop")) return true;
            }
        }

        if (alivePieces.Count == 4) {
            bool enemyBishopAlive = false;
            bool playerBishopAlive = false;
            foreach(GameObject piece in alivePieces) {
                Piece pieceScript = piece.GetComponent<Piece>();
                if (pieceScript.getType().Contains("enemyBishop")) enemyBishopAlive = true;
                if (pieceScript.getType().Contains("playerBishop")) playerBishopAlive = true;
            }
            if (enemyBishopAlive && playerBishopAlive) return true;
        }
        return false;
    }

    private bool checkWin(string type) {
        type = opositeType(type);
        foreach(GameObject piece in pieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            if (pieceScript.getType() == type + "King" && pieceScript.getEliminated()) return true;
        }
        return isCheckMate(type);
    }

    private bool checkAuxWin(string type) {
        type = opositeType(type);
        foreach(GameObject piece in auxPieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            if (pieceScript.getType() == type + "King" && pieceScript.getEliminated()) return true;
        }
        return isCheckMate(type);
    }

    private bool isCheckMate(string type) {
        return isMate(type) && getAllMoves(type).Count == 0;
    }

    private bool isAuxCheckMate(string type) {
        return isAuxMate(type) && getAllAuxMoves(type).Count == 0;
    }

    private void resetPreMoves() {
        resetPlayerPreMoves();
        resetEnemyPreMoves();    
    }

    private void resetPlayerPreMoves() {
        playerPieceToMove = null;
        playerPositionToMove = new Vector2(-1f, -1f);
        playerPieceToPromote = -1;
        playerPieceToMove2 = null;
        playerPositionToMove2 = new Vector2(-1f, -1f);
    }

    private void resetEnemyPreMoves() {
        enemyPieceToMove = null;
        enemyPositionToMove = new Vector2(-1f, -1f);
        enemyPieceToPromote = -1;
        enemyPieceToMove2 = null;
        enemyPositionToMove2 = new Vector2(-1f, -1f);
    }

    private void resetAuxPreMoves() {
        resetAuxPlayerPreMoves();
        resetAuxEnemyPreMoves();
    }

    private void resetAuxPlayerPreMoves() {
        playerAuxPieceToMove = null;
        playerAuxPositionToMove = new Vector2(-1f, -1f);
        playerAuxPieceToPromote = -1;
        playerAuxPieceToMove2 = null;
        playerAuxPositionToMove2 = new Vector2(-1f, -1f);
    }

    private void resetAuxEnemyPreMoves() {
        enemyAuxPieceToMove = null;
        enemyAuxPositionToMove = new Vector2(-1f, -1f);
        enemyAuxPieceToPromote = -1;
        enemyAuxPieceToMove2 = null;
        enemyAuxPositionToMove2 = new Vector2(-1f, -1f);
    }

    private void setPreMove() {
        switch (algorithmId) {
            case 0:
                randomAlgorithm();
                break;
            case 1:
                rulesAlgorithm();
                break;
            case 2:
                rulesAlgorithm();
                break;
            case 3:
                rulesAlgorithm();
                break;
            case 4:
                minimaxAlgorithm();
                break;
            default:
                break;
        }
    }

    private void setMoves() {
        Piece playerPieceScript = null;
        Piece enemyPieceScript = null;

        bool playerMoving = false;
        bool enemyMoving = false;

        Vector2 playerTmpPosition = new Vector2(-1f, -1f);
        Vector2 enemyTmpPosition = new Vector2(-1f, -1f);

        Vector2 playerDirection = new Vector2();
        Vector2 enemyDirection = new Vector2();

        if (playerPieceToMove != null) {
            
            playerPieceScript = playerPieceToMove.GetComponent<Piece>();
            playerMoving = true;
            playerDirection = playerPositionToMove - playerPieceScript.getPosition();
            playerDirection = new Vector2(playerDirection.x == 0 ? 0 : Mathf.Sign(playerDirection.x), playerDirection.y == 0 ? 0 : Mathf.Sign(playerDirection.y));
            if (playerPositionToMove == playerPieceScript.getPosition()) {
                playerMoving = false;
                resetPlayerPreMoves();
            }
        } else {
            --lives;
        }
        if (enemyPieceToMove != null) {
            enemyPieceScript = enemyPieceToMove.GetComponent<Piece>();
            enemyMoving = true;
            enemyDirection = enemyPositionToMove - enemyPieceScript.getPosition();
            enemyDirection = new Vector2(enemyDirection.x == 0 ? 0 : Mathf.Sign(enemyDirection.x), enemyDirection.y == 0 ? 0 : Mathf.Sign(enemyDirection.y));
            if (enemyPositionToMove == enemyPieceScript.getPosition()) {
                enemyMoving = false;
                resetEnemyPreMoves();
            }
        }

        int maxIterations = 0;
        while ((playerMoving || enemyMoving) && maxIterations < 16) {
            ++maxIterations;
            if (playerMoving) {
                if (playerPieceScript.getType().Contains("Knight")) {
                    playerTmpPosition = playerPositionToMove;
                } else playerTmpPosition = playerPieceScript.getPosition() + playerDirection;

                if (getTileStatus(playerTmpPosition).Contains("enemy")) {
                    GameObject rivalPiece = getPieceByPosition(playerTmpPosition);
                    Piece rivalPieceScript = rivalPiece.GetComponent<Piece>();
                    if (rivalPieceScript.getPosition() == playerTmpPosition && rivalPiece != enemyPieceToMove) {
                        rivalPieceScript.setEliminated(true);
                        setPieceEliminated(rivalPiece);
                        playerPositionToMove = playerTmpPosition;
                    }
                }
            }
            if (enemyMoving) {
                if (enemyPieceScript.getType().Contains("Knight")) {
                    enemyTmpPosition = enemyPositionToMove;
                } else enemyTmpPosition = enemyPieceScript.getPosition() + enemyDirection;

                if (getTileStatus(enemyTmpPosition).Contains("player")) {
                    GameObject rivalPiece = getPieceByPosition(enemyTmpPosition);
                    Piece rivalPieceScript = rivalPiece.GetComponent<Piece>();
                    if (rivalPieceScript.getPosition() == enemyTmpPosition && rivalPiece != playerPieceToMove) {
                        rivalPieceScript.setEliminated(true);
                        setPieceEliminated(rivalPiece);
                        enemyPositionToMove = enemyTmpPosition;
                    }
                }
            }

            if (playerMoving && enemyMoving) {
                if (playerTmpPosition == enemyTmpPosition || (playerTmpPosition == enemyPieceScript.getPosition() && enemyTmpPosition == playerPieceScript.getPosition())) {
                    if (playerPieceScript.getType().Contains("King") && enemyPieceScript.getType().Contains("King")) {
                        playerPieceScript.setEliminated(true);
                        setPieceEliminated(playerPieceToMove);
                        playerMoving = false;

                        enemyPieceScript.setEliminated(true);
                        setPieceEliminated(enemyPieceToMove);
                        enemyMoving = false;
                    } else {
                        if (!playerPieceScript.getType().Contains("King")) {
                            playerPieceScript.setEliminated(true);
                            setPieceEliminated(playerPieceToMove);
                            playerMoving = false;
                        }
                        if (!enemyPieceScript.getType().Contains("King")) {
                            enemyPieceScript.setEliminated(true);
                            setPieceEliminated(enemyPieceToMove);
                            enemyMoving = false;
                        }
                    }
                }
            }

            if (playerMoving) {
                if (playerTmpPosition == playerPositionToMove) {
                    playerPieceScript.setLastMoveTempo(tempo);
                    playerPieceScript.setPosition(playerPositionToMove);
                    playerPieceToMove.transform.position = traduceSquare(playerPositionToMove);
                    if (playerPieceToPromote != -1) {
                        playerPieceScript.setType("player" + promotion[playerPieceToPromote]);
                        playerPieceScript.setSprite(playerPieceToPromote);
                    }
                    if (playerPieceToMove2 != null) {
                        Piece rookScript = playerPieceToMove2.GetComponent<Piece>();
                        rookScript.setLastMoveTempo(tempo);
                        rookScript.setPosition(playerPositionToMove2);
                        playerPieceToMove2.transform.position = traduceSquare(playerPositionToMove2);
                    }
                    playerMoving = false;
                    resetPlayerPreMoves();
                } else {
                    playerPieceScript.setPosition(playerTmpPosition);
                }
            }
            if (enemyMoving) {
                if (enemyTmpPosition == enemyPositionToMove) {
                    enemyPieceScript.setLastMoveTempo(tempo);
                    enemyPieceScript.setPosition(enemyPositionToMove);
                    enemyPieceToMove.transform.position = traduceSquare(enemyPositionToMove);
                    if (enemyPieceToPromote != -1) {
                        enemyPieceScript.setType("enemy" + promotion[enemyPieceToPromote]);
                        enemyPieceScript.setSprite(enemyPieceToPromote);
                    }
                    if (enemyPieceToMove2 != null) {
                        Piece rookScript = enemyPieceToMove2.GetComponent<Piece>();
                        rookScript.setLastMoveTempo(tempo);
                        rookScript.setPosition(enemyPositionToMove2);
                        enemyPieceToMove2.transform.position = traduceSquare(enemyPositionToMove2);
                    }
                    enemyMoving = false;
                    resetEnemyPreMoves();
                } else {
                    enemyPieceScript.setPosition(enemyTmpPosition);
                }
            }
        }
        if (maxIterations >= 16) Debug.LogError("max Iterations surpased");
        foreach (GameObject dot in GameObject.FindGameObjectsWithTag("moveSelector")) {
            Destroy(dot);
        }
        resetPreMoves();
    }

    public void setAuxMoves() {
        Piece playerPieceScript = null;
        Piece enemyPieceScript = null;

        bool playerMoving = false;
        bool enemyMoving = false;

        Vector2 playerTmpPosition = new Vector2(-1f, -1f);
        Vector2 enemyTmpPosition = new Vector2(-1f, -1f);

        Vector2 playerDirection = new Vector2();
        Vector2 enemyDirection = new Vector2();

        if (playerAuxPieceToMove != null) {
            playerPieceScript = playerAuxPieceToMove.GetComponent<Piece>();
            playerMoving = true;
            playerDirection = playerAuxPositionToMove - playerPieceScript.getPosition();
            playerDirection = new Vector2(playerDirection.x == 0 ? 0 : Mathf.Sign(playerDirection.x), playerDirection.y == 0 ? 0 : Mathf.Sign(playerDirection.y));
            Vector2 ps = playerPieceScript.getPosition();
        }
        if (enemyAuxPieceToMove != null) {
            enemyPieceScript = enemyAuxPieceToMove.GetComponent<Piece>();
            enemyMoving = true;
            enemyDirection = enemyAuxPositionToMove - enemyPieceScript.getPosition();
            enemyDirection = new Vector2(enemyDirection.x == 0 ? 0 : Mathf.Sign(enemyDirection.x), enemyDirection.y == 0 ? 0 : Mathf.Sign(enemyDirection.y));
        }

        int maxIterations = 0;
        while ((playerMoving || enemyMoving) && maxIterations < 16) {
            ++maxIterations;

            if (playerMoving) {
                if (playerPieceScript.getType().Contains("Knight")) {
                    playerTmpPosition = playerAuxPositionToMove;
                } else playerTmpPosition = playerPieceScript.getPosition() + playerDirection;

                if (getAuxTileStatus(playerTmpPosition).Contains("enemy")) {
                    GameObject rivalPiece = getAuxPieceByPosition(playerTmpPosition);
                    Piece rivalPieceScript = rivalPiece.GetComponent<Piece>();
                    if (rivalPieceScript.getPosition() == playerTmpPosition && rivalPiece != enemyAuxPieceToMove) {
                        rivalPieceScript.setEliminated(true);
                        setPieceEliminated(rivalPiece);
                        playerAuxPositionToMove = playerTmpPosition;
                    }
                }
            }

            if (enemyMoving) {
                if (enemyPieceScript.getType().Contains("Knight")) {
                    enemyTmpPosition = enemyAuxPositionToMove;
                } else enemyTmpPosition = enemyPieceScript.getPosition() + enemyDirection;

                if (getAuxTileStatus(enemyTmpPosition).Contains("player")) {
                    GameObject rivalPiece = getAuxPieceByPosition(enemyTmpPosition);
                    Piece rivalPieceScript = rivalPiece.GetComponent<Piece>();
                    if (rivalPieceScript.getPosition() == enemyTmpPosition && rivalPiece != playerAuxPieceToMove) {
                        rivalPieceScript.setEliminated(true);
                        setPieceEliminated(rivalPiece);
                        enemyAuxPositionToMove = enemyTmpPosition;
                    }
                }
            }

            if (playerMoving && enemyMoving) {

                if (playerTmpPosition == enemyTmpPosition || (playerTmpPosition == enemyPieceScript.getPosition() && enemyTmpPosition == playerPieceScript.getPosition())) {
                    if (playerPieceScript.getType().Contains("King") && enemyPieceScript.getType().Contains("King")) {
                        playerPieceScript.setEliminated(true);
                        setPieceEliminated(playerAuxPieceToMove);
                        playerMoving = false;

                        enemyPieceScript.setEliminated(true);
                        setPieceEliminated(enemyAuxPieceToMove);
                        enemyMoving = false;
                    } else {
                        if (!playerPieceScript.getType().Contains("King")) {
                            playerPieceScript.setEliminated(true);
                            setPieceEliminated(playerAuxPieceToMove);
                            playerMoving = false;
                        }
                        if (!enemyPieceScript.getType().Contains("King")) {
                            enemyPieceScript.setEliminated(true);
                            setPieceEliminated(enemyAuxPieceToMove);
                            enemyMoving = false;
                        }
                    }
                }
            }

            if (playerMoving) { 
                if (playerTmpPosition == playerAuxPositionToMove) {
                    playerPieceScript.setLastMoveTempo(tempo);
                    playerPieceScript.setPosition(playerAuxPositionToMove);
                    playerAuxPieceToMove.transform.position = traduceSquare(playerAuxPositionToMove);
                    if (playerAuxPieceToPromote != -1) {
                        playerPieceScript.setType("player" + promotion[playerAuxPieceToPromote]);
                        playerPieceScript.setSprite(playerAuxPieceToPromote);
                    }
                    if (playerAuxPieceToMove2 != null) {
                        Piece rookScript = playerAuxPieceToMove2.GetComponent<Piece>();
                        rookScript.setLastMoveTempo(tempo);
                        rookScript.setPosition(playerAuxPositionToMove2);
                        playerAuxPieceToMove2.transform.position = traduceSquare(playerAuxPositionToMove2);
                    }
                    playerMoving = false;
                    resetAuxPlayerPreMoves();
                } else {
                    playerPieceScript.setPosition(playerTmpPosition);
                }
            }
            if (enemyMoving) {
                if (enemyTmpPosition == enemyAuxPositionToMove) {
                    enemyPieceScript.setLastMoveTempo(tempo);
                    enemyPieceScript.setPosition(enemyAuxPositionToMove);
                    enemyAuxPieceToMove.transform.position = traduceSquare(enemyAuxPositionToMove);
                    if (enemyAuxPieceToPromote != -1) {
                        enemyPieceScript.setType("enemy" + promotion[enemyAuxPieceToPromote]);
                        enemyPieceScript.setSprite(enemyAuxPieceToPromote);
                    }
                    if (enemyAuxPieceToMove2 != null) {
                        Piece rookScript = enemyAuxPieceToMove2.GetComponent<Piece>();
                        rookScript.setLastMoveTempo(tempo);
                        rookScript.setPosition(enemyAuxPositionToMove2);
                        enemyAuxPieceToMove2.transform.position = traduceSquare(enemyAuxPositionToMove2);
                    }
                    enemyMoving = false;
                    resetAuxEnemyPreMoves();
                } else {
                    enemyPieceScript.setPosition(enemyTmpPosition);
                }
            }
        }
        if (maxIterations >= 16) {
            Debug.LogError("max Iterations surpased (AUX)");
        }
        resetAllAuxPossibleMoves();
        resetAuxPreMoves();
    }

    public void preMovePiece(GameObject piece, Vector2 position, string type, int pieceToPromote) {
        if (type == "player") {
            playerPieceToMove = piece;
            playerPositionToMove = position;
            playerPieceToPromote = pieceToPromote;
        } else if (type == "enemy") {
            enemyPieceToMove = piece;
            enemyPositionToMove = position;
            enemyPieceToPromote = pieceToPromote;
        }
    }

    public void preMovePiece2(GameObject piece, Vector2 position, string type, int pieceToPromote) {
        if (type == "player") {
            playerPieceToMove2 = piece;
            playerPositionToMove2 = position;
        } else if (type == "enemy") {
            enemyPieceToMove2 = piece;
            enemyPositionToMove2 = position;
        }
    }

    public void preMoveAuxPiece(GameObject piece, Vector2 position, string type) {
        if (type == "player") {
            playerAuxPieceToMove = piece;
            playerAuxPositionToMove = position;
        } else if (type == "enemy") {
            enemyAuxPieceToMove = piece;
            enemyAuxPositionToMove = position;
        }
    }

    public void setPieceEliminated(GameObject piece) {
        piece.GetComponent<Piece>().setPosition(new Vector2(-1f, -1f));
        piece.transform.position = traduceSquare(new Vector2(-1f, -1f));
    }

    private void initiatePiece(GameObject piece, Vector2 position, string type, GameObject auxPiece) {
        Piece pieceScript = piece.GetComponent<Piece>();
        pieceScript.setType(type);
        pieceScript.setManager(this);
        pieceScript.setMoveSelector(moveSelector);
        pieceScript.setPosition(position);
        pieceScript.setAuxPieceAsociated(auxPiece);
        piece.transform.position = traduceSquare(position);
        
    }

    private void initiateAuxPiece(GameObject piece, Vector2 position, string type) {
        Piece pieceScript = piece.GetComponent<Piece>();
        pieceScript.setType(type);
        pieceScript.setManager(this);
        pieceScript.setMoveSelector(moveSelector);
        pieceScript.setPosition(position);
        piece.transform.position = auxTraduceSquare(position);
        
    }

    public bool isKingDead(string type) {
        foreach (GameObject piece in pieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            if (pieceScript.getType() == type + "king") {
                return pieceScript.getEliminated();
            }
        }
        return false;
    }

    public bool isAuxKingDead(string type) {
        foreach (GameObject piece in auxPieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            if (pieceScript.getType() == type + "king") {
                return pieceScript.getEliminated();
            }
        }
        return false;
    }

    private string opositeType(string type) {
        if (type == "enemy") return "player";
        else return "enemy";
    }

    public bool isAuxMate(string type) {
        foreach (GameObject piece in auxPieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            if (pieceScript.getType() == type + "King") {
                return getAllAuxThreads(opositeType(type)).Contains(pieceScript.getPosition());
            }
        }
        return false;
    }

    public bool isMate(string type) {
        foreach (GameObject piece in pieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            if (pieceScript.getType() == type + "King") {
                return getAllThreads(opositeType(type)).Contains(pieceScript.getPosition());
            }
        }
        return false;
    }

    public void resetAuxPieces() {
        resetAuxPreMoves();
        for (int i = 0; i < 32; ++i) {
            Piece auxPieceScript = auxPieces[i].GetComponent<Piece>();
            Piece pieceScript = pieces[i].GetComponent<Piece>();

            auxPieceScript.setPosition(pieceScript.getPosition());
            auxPieceScript.setEliminated(pieceScript.getEliminated());
            auxPieces[i].transform.position = auxTraduceSquare(auxPieceScript.getPosition());
            
            auxPieceScript.setLastMoveTempo(pieceScript.getLastMoveTempo());
            auxPieceScript.setPossibleMovesLiteral(pieceScript.getPossibleMoves());
            auxPieceScript.setThreads(pieceScript.getThreads());
            auxPieceScript.setType(pieceScript.getType());
        }
    }

    private void copyAuxPieces(List<GameObject> auxPiecesCopy) {
        resetAuxPreMoves();
        for (int i = 0; i < 32; ++i) {
            Piece auxPieceScript = auxPieces[i].GetComponent<Piece>();
            Piece pieceScript = auxPiecesCopy[i].GetComponent<Piece>();

            auxPieceScript.setPosition(pieceScript.getPosition());
            auxPieceScript.setEliminated(pieceScript.getEliminated());
            auxPieces[i].transform.position = auxTraduceSquare(auxPieceScript.getPosition());
            
            auxPieceScript.setLastMoveTempo(pieceScript.getLastMoveTempo());
            auxPieceScript.setPossibleMovesLiteral(pieceScript.getPossibleMoves());
            auxPieceScript.setThreads(pieceScript.getThreads());
            auxPieceScript.setType(pieceScript.getType());
        }
    }

    private void randomAlgorithm() {
        List<GameObject> aviablePieces = new List<GameObject>();
        foreach(GameObject piece in pieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            if (pieceScript.getType().Contains("enemy") && pieceScript.getPossibleMoves().Count > 0) {
                aviablePieces.Add(piece);
            }
        }

        System.Random random = new System.Random();
        int i = random.Next(0, aviablePieces.Count);
        enemyPieceToMove = aviablePieces[i];
        Piece enemyPieceToMoveScript = enemyPieceToMove.GetComponent<Piece>();
        List<Vector2> possibleMoves = enemyPieceToMoveScript.getPossibleMoves();
        int j = random.Next(0, possibleMoves.Count);
        enemyPositionToMove = possibleMoves[j];

        if (enemyPieceToMoveScript.getType().Contains("Pawn") && enemyPositionToMove.y == 0f && enemyPieceToMoveScript.getPosition().y == 1f) {
            enemyPieceToPromote = random.Next(0, promotion.Count - 1);
        } else enemyPieceToPromote = -1;

        if (enemyPieceToMoveScript.getType() == "enemyKing" && enemyPieceToMoveScript.getPosition() == new Vector2(4f, 7f)) {
            if (enemyPositionToMove == new Vector2(2f, 7f)) {
                enemyPieceToMove2 = getPieceByPosition(new Vector2(0f, 7f));
                enemyPositionToMove2 = new Vector2(3f, 7f);
            } else if (enemyPositionToMove == new Vector2(6f, 7f)) {
                enemyPieceToMove2 = getPieceByPosition(new Vector2(7f, 7f));
                enemyPositionToMove2 = new Vector2(5f, 7f);
            } else {
                enemyPieceToMove2 = null;
                enemyPositionToMove2 = new Vector2(-1f, -1f);
            }
        } else {
            enemyPieceToMove2 = null;
            enemyPositionToMove2 = new Vector2(-1f, -1f);
        }
    }

    private void playerRandomAlgorithm() {
        List<GameObject> aviablePieces = new List<GameObject>();
        foreach(GameObject piece in pieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            if (pieceScript.getType().Contains("player") && pieceScript.getPossibleMoves().Count > 0) {
                aviablePieces.Add(piece);
            }
        }

        System.Random random = new System.Random();
        int i = random.Next(0, aviablePieces.Count);
        playerPieceToMove = aviablePieces[i];
        Piece playerPieceToMoveScript = playerPieceToMove.GetComponent<Piece>();
        List<Vector2> possibleMoves = playerPieceToMoveScript.getPossibleMoves();
        int j = random.Next(0, possibleMoves.Count);
        playerPositionToMove = possibleMoves[j];

        if (playerPieceToMoveScript.getType().Contains("Pawn") && playerPositionToMove.y == 0f && playerPieceToMoveScript.getPosition().y == 1f) {
            playerPieceToPromote = random.Next(0, promotion.Count - 1);
        } else playerPieceToPromote = -1;

        if (playerPieceToMoveScript.getType() == "playerKing" && playerPieceToMoveScript.getPosition() == new Vector2(4f, 7f)) {
            if (playerPositionToMove == new Vector2(2f, 7f)) {
                playerPieceToMove2 = getPieceByPosition(new Vector2(0f, 7f));
                playerPositionToMove2 = new Vector2(3f, 7f);
            } else if (playerPositionToMove == new Vector2(6f, 7f)) {
                playerPieceToMove2 = getPieceByPosition(new Vector2(7f, 7f));
                playerPositionToMove2 = new Vector2(5f, 7f);
            } else {
                playerPieceToMove2 = null;
                playerPositionToMove2 = new Vector2(-1f, -1f);
            }
        } else {
            playerPieceToMove2 = null;
            playerPositionToMove2 = new Vector2(-1f, -1f);
        }
    }

    private float getPointsPerPiece(string type) {
        if (type.Contains("King")) return piecesPoints[0];
        if (type.Contains("Queen")) return piecesPoints[1];
        if (type.Contains("Rook")) return piecesPoints[2];
        if (type.Contains("Bishop")) return piecesPoints[3];
        if (type.Contains("Knight")) return piecesPoints[4];
        if (type.Contains("Pawn")) return piecesPoints[5];
        return 0f;
    }

    private void rulesAlgorithm() {
        GameObject bestPieceToMove = null;
        Vector2 bestPositionToMove = new Vector2(-1f, -1f);
        int bestNumberOfMoves = 0;
        int bestAviablePieces = 0;
        float bestPoints = -1f;

        //Debug.Log("---------------------------------------------");
        foreach(GameObject piece in pieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            if (pieceScript.getType().Contains("enemy") && pieceScript.getPossibleMoves().Count > 0) {
                foreach(Vector2 positionToMove in pieceScript.getPossibleMoves()) {
                    preMoveAuxPiece(pieceScript.getAuxPiece(), positionToMove, "enemy");
                    setAuxMoves();
                    List<Vector2> rivalThreads = getAllThreads("player");
                    
                    if (isAuxCheckMate("player")) {
                        bestPoints += 10000f;
                        bestPieceToMove = piece;
                        bestPositionToMove = positionToMove;
                    } else {
                        float actualPoints = 0f;
                        string tileStatus = getTileStatus(positionToMove);
                        if (tileStatus.Contains("player")) {
                            actualPoints += getPointsPerPiece(tileStatus) - (getPointsPerPiece(pieceScript.getType()) * demotionPoints);
                            //if (algorithmId == 1) actualPoints += killPoints;
                        } 
                        if (rivalThreads.Contains(pieceScript.getPosition())) {
                            actualPoints += getPointsPerPiece(pieceScript.getType());
                            //if (algorithmId == 2 && rivalThreads.Contains(pieceScript.getPosition())) actualPoints += savePoints;
                        }
                        if (isAuxMate("player")) {
                            actualPoints += matePoints;
                            //if (algorithmId == 3) actualPoints += extraMatePoints;
                        }
                        if (pieceScript.getType().Contains("Pawn") && positionToMove.y == 0f) actualPoints += promotePoints;
                        if (pieceScript.getType().Contains("King") && pieceScript.getPosition().x == 4f && (positionToMove.x == 2f || positionToMove.x == 6f)) actualPoints += castlingPoints;

                        List<GameObject> aviablePieces = new List<GameObject>();
                        foreach(GameObject auxPiece in auxPieces) {
                            Piece auxPieceScript = auxPiece.GetComponent<Piece>();
                            if (auxPieceScript.getType().Contains("enemy") && auxPieceScript.getPossibleMoves().Count > 0) {
                                aviablePieces.Add(auxPiece);
                            }
                        }

                        if (actualPoints > bestPoints || 
                            (actualPoints == bestPoints && aviablePieces.Count > bestAviablePieces) || 
                            (actualPoints == bestPoints && aviablePieces.Count == bestAviablePieces && getAllAuxMoves("enemy").Count > bestNumberOfMoves)) {
                            bestPoints = actualPoints;
                            bestPieceToMove = piece;
                            bestPositionToMove = positionToMove;
                            bestAviablePieces = aviablePieces.Count;
                            bestNumberOfMoves = getAllAuxMoves("enemy").Count;
                        }
                        //Debug.Log("Piece = " + pieceScript.getType() + pieceScript.getPosition() + " | to = " + positionToMove + " | points = " + actualPoints + " | newMoves = " + getAllAuxMoves("enemy").Count);
                    }

                    resetAuxPieces();
                }
            }
        }

        enemyPieceToMove = bestPieceToMove;
        enemyPositionToMove = bestPositionToMove;
        Piece enemyPieceToMoveScript = enemyPieceToMove.GetComponent<Piece>();

        if (enemyPieceToMoveScript.getType().Contains("Pawn") && enemyPositionToMove.y == 0f && enemyPieceToMoveScript.getPosition().y == 1f) {
            enemyPieceToPromote = 0;
        } else enemyPieceToPromote = -1;

        if (enemyPieceToMoveScript.getType() == "enemyKing" && enemyPieceToMoveScript.getPosition() == new Vector2(4f, 7f) && (enemyPositionToMove == new Vector2(2f, 7f) || enemyPositionToMove == new Vector2(6f, 7f))) {
            if (enemyPositionToMove == new Vector2(2f, 7f)) {
                enemyPieceToMove2 = getPieceByPosition(new Vector2(0f, 7f));
                enemyPositionToMove2 = new Vector2(3f, 7f);
            } else {
                enemyPieceToMove2 = getPieceByPosition(new Vector2(7f, 7f));
                enemyPositionToMove2 = new Vector2(5f, 7f);
            }
        } else {
            enemyPieceToMove2 = null;
            enemyPositionToMove2 = new Vector2(-1f, -1f);
        }
    }

    private void playerRulesAlgorithm() {
        GameObject bestPieceToMove = null;
        Vector2 bestPositionToMove = new Vector2(-1f, -1f);
        int bestNumberOfMoves = 0;
        int bestAviablePieces = 0;
        float bestPoints = -1f;

        //Debug.Log("---------------------------------------------");
        foreach(GameObject piece in pieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            if (pieceScript.getType().Contains("player") && pieceScript.getPossibleMoves().Count > 0) {
                foreach(Vector2 positionToMove in pieceScript.getPossibleMoves()) {
                    preMoveAuxPiece(pieceScript.getAuxPiece(), positionToMove, "player");
                    setAuxMoves();
                    List<Vector2> rivalThreads = getAllThreads("player");
                    
                    if (isAuxCheckMate("player")) {
                        bestPoints += 10000f;
                        bestPieceToMove = piece;
                        bestPositionToMove = positionToMove;
                    } else {
                        float actualPoints = 0f;
                        string tileStatus = getTileStatus(positionToMove);
                        if (tileStatus.Contains("player")) {
                            actualPoints += getPointsPerPiece(tileStatus) - (getPointsPerPiece(pieceScript.getType()) * demotionPoints);
                            if (algorithmId == 1) actualPoints += killPoints;
                        } 
                        if (rivalThreads.Contains(pieceScript.getPosition())) {
                            actualPoints += getPointsPerPiece(pieceScript.getType());
                            if (algorithmId == 2 && rivalThreads.Contains(pieceScript.getPosition())) actualPoints += savePoints;
                        }
                        if (isAuxMate("player")) {
                            actualPoints += matePoints;
                            if (algorithmId == 3) actualPoints += extraMatePoints;
                        }
                        if (pieceScript.getType().Contains("Pawn") && positionToMove.y == 0f) actualPoints += promotePoints;
                        if (pieceScript.getType().Contains("King") && pieceScript.getPosition().x == 4f && (positionToMove.x == 2f || positionToMove.x == 6f)) actualPoints += castlingPoints;

                        List<GameObject> aviablePieces = new List<GameObject>();
                        foreach(GameObject auxPiece in auxPieces) {
                            Piece auxPieceScript = auxPiece.GetComponent<Piece>();
                            if (auxPieceScript.getType().Contains("player") && auxPieceScript.getPossibleMoves().Count > 0) {
                                aviablePieces.Add(auxPiece);
                            }
                        }

                        if (actualPoints > bestPoints || 
                            (actualPoints == bestPoints && aviablePieces.Count > bestAviablePieces) || 
                            (actualPoints == bestPoints && aviablePieces.Count == bestAviablePieces && getAllAuxMoves("player").Count > bestNumberOfMoves)) {
                            bestPoints = actualPoints;
                            bestPieceToMove = piece;
                            bestPositionToMove = positionToMove;
                            bestAviablePieces = aviablePieces.Count;
                            bestNumberOfMoves = getAllAuxMoves("player").Count;
                        }
                        //Debug.Log("Piece = " + pieceScript.getType() + pieceScript.getPosition() + " | to = " + positionToMove + " | points = " + actualPoints + " | newMoves = " + getAllAuxMoves("player").Count);
                    }

                    resetAuxPieces();
                }
            }
        }

        playerPieceToMove = bestPieceToMove;
        playerPositionToMove = bestPositionToMove;
        Piece playerPieceToMoveScript = playerPieceToMove.GetComponent<Piece>();

        if (playerPieceToMoveScript.getType().Contains("Pawn") && playerPositionToMove.y == 0f && playerPieceToMoveScript.getPosition().y == 1f) {
            playerPieceToPromote = 0;
        } else playerPieceToPromote = -1;

        if (playerPieceToMoveScript.getType() == "playerKing" && playerPieceToMoveScript.getPosition() == new Vector2(4f, 7f) && (playerPositionToMove == new Vector2(2f, 7f) || playerPositionToMove == new Vector2(6f, 7f))) {
            if (playerPositionToMove == new Vector2(2f, 7f)) {
                playerPieceToMove2 = getPieceByPosition(new Vector2(0f, 7f));
                playerPositionToMove2 = new Vector2(3f, 7f);
            } else {
                playerPieceToMove2 = getPieceByPosition(new Vector2(7f, 7f));
                playerPositionToMove2 = new Vector2(5f, 7f);
            }
        } else {
            playerPieceToMove2 = null;
            playerPositionToMove2 = new Vector2(-1f, -1f);
        }
    }

    private void minimaxAlgorithm() {
        resetAuxPieces();
        resetAllAuxPossibleMoves();

        float bestEvaluation = -10001f;
        foreach(GameObject piece in pieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            if (pieceScript.getType().Contains("enemy") && !pieceScript.getEliminated()) {
                foreach (Vector2 move in pieceScript.getPossibleMoves()) {
                    float evaluation = recursiveMinimax(auxPieces, pieceScript.getAuxPiece(), move, miniMaxDepth, bestEvaluation, 10001f);

                    if (evaluation > bestEvaluation) {
                        bestEvaluation = evaluation;
                        enemyPieceToMove = piece;
                        enemyPositionToMove = move;
                    }
                    resetAuxPieces();
                    resetAllAuxPossibleMoves();
                }
            }
        }

        Piece enemyPieceToMoveScript = enemyPieceToMove.GetComponent<Piece>();
        if (enemyPieceToMoveScript.getType().Contains("Pawn") && enemyPositionToMove.y == 0f && enemyPieceToMoveScript.getPosition().y == 1f) {
            enemyPieceToPromote = 0;
        } else enemyPieceToPromote = -1;

        if (enemyPieceToMoveScript.getType() == "enemyKing" && enemyPieceToMoveScript.getPosition() == new Vector2(4f, 7f) && (enemyPositionToMove == new Vector2(2f, 7f) || enemyPositionToMove == new Vector2(6f, 7f))) {
            if (enemyPositionToMove == new Vector2(2f, 7f)) {
                enemyPieceToMove2 = getPieceByPosition(new Vector2(0f, 7f));
                enemyPositionToMove2 = new Vector2(3f, 7f);
            } else {
                enemyPieceToMove2 = getPieceByPosition(new Vector2(7f, 7f));
                enemyPositionToMove2 = new Vector2(5f, 7f);
            }
        } else {
            enemyPieceToMove2 = null;
            enemyPositionToMove2 = new Vector2(-1f, -1f);
        }
    }

    private float recursiveMinimax(List<GameObject> auxPiecesCopy, GameObject enemyPiece, Vector2 enemyMove, int depth, float alpha, float beta) {

        if (depth <= 0 || checkAuxWin("player") || checkAuxWin("enemy") || checkDraw()) {
            return evaluateAux();
        }

        if (enemyPiece == null) {

            float maxEvaluation = -10001f;
            foreach(GameObject piece in auxPieces) {
                Piece pieceScript = piece.GetComponent<Piece>();
                if (pieceScript.getType().Contains("enemy") && !pieceScript.getEliminated()) {
                    foreach(Vector2 move in pieceScript.getPossibleMoves()) {

                        float evaluation = recursiveMinimax(auxPiecesCopy, piece, move, depth, alpha, beta);
                        copyAuxPieces(auxPiecesCopy);
                        resetAllAuxPossibleMoves();

                        if (evaluation > maxEvaluation) maxEvaluation = evaluation;
                        if (evaluation > alpha) alpha = evaluation;
                        if (beta <= alpha) break;
                    }
                }
            }
            return maxEvaluation;
        } else {
            
            float minEvaluation = 10001f;
            foreach(GameObject piece in auxPieces) {
                Piece pieceScript = piece.GetComponent<Piece>();
                if (pieceScript.getType().Contains("player") && !pieceScript.getEliminated()) {
                    foreach(Vector2 move in pieceScript.getPossibleMoves()) {
                        preMoveAuxPiece(enemyPiece, enemyMove, "enemy");
                        preMoveAuxPiece(piece, move, "player");
                        setAuxMoves();

                        float evaluation = recursiveMinimax(auxPieces, null, new Vector2(-1f, -1f), depth - 1, alpha, beta);
                        copyAuxPieces(auxPiecesCopy);
                        resetAllAuxPossibleMoves();

                        if (evaluation < minEvaluation) minEvaluation = evaluation;
                        if (evaluation < beta) beta = evaluation;
                        if (beta <= alpha) break;
                    }
                }
            }
            return minEvaluation;
        } 
    }

    private void playerMinimaxAlgorithm() {
        resetAuxPieces();
        resetAllAuxPossibleMoves();

        float bestEvaluation = -10001f;
        foreach(GameObject piece in pieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            if (pieceScript.getType().Contains("player") && !pieceScript.getEliminated()) {
                foreach (Vector2 move in pieceScript.getPossibleMoves()) {
                    float evaluation = playerRecursiveMinimax(auxPieces, pieceScript.getAuxPiece(), move, miniMaxDepth, bestEvaluation, 10001f);

                    if (evaluation > bestEvaluation) {
                        bestEvaluation = evaluation;
                        playerPieceToMove = piece;
                        playerPositionToMove = move;
                    }
                    resetAuxPieces();
                    resetAllAuxPossibleMoves();
                }
            }
        }

        Piece playerPieceToMoveScript = playerPieceToMove.GetComponent<Piece>();
        if (playerPieceToMoveScript.getType().Contains("Pawn") && playerPositionToMove.y == 0f && playerPieceToMoveScript.getPosition().y == 1f) {
            playerPieceToPromote = 0;
        } else playerPieceToPromote = -1;

        if (playerPieceToMoveScript.getType() == "playerKing" && playerPieceToMoveScript.getPosition() == new Vector2(4f, 7f) && (playerPositionToMove == new Vector2(2f, 7f) || playerPositionToMove == new Vector2(6f, 7f))) {
            if (playerPositionToMove == new Vector2(2f, 7f)) {
                playerPieceToMove2 = getPieceByPosition(new Vector2(0f, 7f));
                playerPositionToMove2 = new Vector2(3f, 7f);
            } else {
                playerPieceToMove2 = getPieceByPosition(new Vector2(7f, 7f));
                playerPositionToMove2 = new Vector2(5f, 7f);
            }
        } else {
            playerPieceToMove2 = null;
            playerPositionToMove2 = new Vector2(-1f, -1f);
        }
    }

    private float playerRecursiveMinimax(List<GameObject> auxPiecesCopy, GameObject playerPiece, Vector2 playerMove, int depth, float alpha, float beta) {

        if (depth <= 0 || checkAuxWin("player") || checkAuxWin("enemy") || checkDraw()) {
            return evaluateAux();
        }

        if (playerPiece == null) {

            float minEvaluation = 10001f;
            foreach(GameObject piece in auxPieces) {
                Piece pieceScript = piece.GetComponent<Piece>();
                if (pieceScript.getType().Contains("player") && !pieceScript.getEliminated()) {
                    foreach(Vector2 move in pieceScript.getPossibleMoves()) {

                        float evaluation = recursiveMinimax(auxPiecesCopy, piece, move, depth, alpha, beta);
                        copyAuxPieces(auxPiecesCopy);
                        resetAllAuxPossibleMoves();

                        
                        if (evaluation < minEvaluation) minEvaluation = evaluation;
                        if (evaluation < beta) beta = evaluation;
                        if (beta <= alpha) break;
                    }
                }
            }
            return minEvaluation;
        } else {
            
            float maxEvaluation = -10001f;
            foreach(GameObject piece in auxPieces) {
                Piece pieceScript = piece.GetComponent<Piece>();
                if (pieceScript.getType().Contains("enemy") && !pieceScript.getEliminated()) {
                    foreach(Vector2 move in pieceScript.getPossibleMoves()) {
                        preMoveAuxPiece(playerPiece, playerMove, "player");
                        preMoveAuxPiece(piece, move, "enemy");
                        setAuxMoves();

                        float evaluation = recursiveMinimax(auxPieces, null, new Vector2(-1f, -1f), depth - 1, alpha, beta);
                        copyAuxPieces(auxPiecesCopy);
                        resetAllAuxPossibleMoves();

                        if (evaluation > maxEvaluation) maxEvaluation = evaluation;
                        if (evaluation > alpha) alpha = evaluation;
                        if (beta <= alpha) break;
                    }
                }
            }
            return maxEvaluation;
        } 
    }

    private float evaluate() {
        if (checkDraw()) return 0f;
        if (checkWin("player") && checkWin("enemy")) return 0f;
        if (checkWin("player")) return -10000f;
        if (checkWin("enemy")) return 10000f;

        float evaluation = 0f;
        if (isMate("enemy")) evaluation -= mateEval;
        if (isMate("player")) evaluation += mateEval;

        List<Vector2> playerThreads = getAllThreads("player");
        List<Vector2> enemyThreads = getAllThreads("enemy");
        foreach(GameObject piece in pieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            float sign = pieceScript.getType().Contains("enemy") ? 1f : -1f;
            if (!pieceScript.getEliminated()) {
                evaluation += sign * getPointsPerPiece(pieceScript.getType());
                List<Vector2> allMoves = pieceScript.getPossibleMoves();
                if (allMoves.Count > 0) evaluation += sign * aviableEval;
                evaluation += sign * allMoves.Count * movesEval;
                if (pieceScript.getType().Contains("enemy") && playerThreads.Contains(pieceScript.getPosition())) evaluation += -1f * threadEval * getPointsPerPiece(pieceScript.getType());
                if (pieceScript.getType().Contains("player") && enemyThreads.Contains(pieceScript.getPosition())) evaluation += threadEval * getPointsPerPiece(pieceScript.getType());
            }
        }

        return evaluation;
    }

    public float evaluateAux() {
        if (checkAuxDraw()) return 0f;
        if (checkAuxWin("player") && checkWin("enemy")) return 0f;
        if (checkAuxWin("player")) return -10000f;
        if (checkAuxWin("enemy")) return 10000f;

        float evaluation = 0f;
        if (isAuxMate("enemy")) evaluation -= mateEval;
        if (isAuxMate("player")) evaluation += mateEval;

        List<Vector2> playerThreads = getAllAuxThreads("player");
        List<Vector2> enemyThreads = getAllAuxThreads("enemy");
        foreach(GameObject piece in auxPieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            float sign = pieceScript.getType().Contains("enemy") ? 1f : -1f;
            if (!pieceScript.getEliminated()) {
                evaluation += sign * getPointsPerPiece(pieceScript.getType());
                List<Vector2> allMoves = pieceScript.getPossibleMoves();
                if (allMoves.Count > 0) evaluation += sign * aviableEval;
                evaluation += sign * allMoves.Count * movesEval;
                if (pieceScript.getType().Contains("enemy") && playerThreads.Contains(pieceScript.getPosition())) evaluation += -1f * threadEval * getPointsPerPiece(pieceScript.getType());
                if (pieceScript.getType().Contains("player") && enemyThreads.Contains(pieceScript.getPosition())) evaluation += threadEval * getPointsPerPiece(pieceScript.getType());
            }
        }

        return evaluation;
    }

    public void setEnabledMoves(bool newEnabledMoves) {
        foreach(GameObject piece in pieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            if (pieceScript.getType().Contains("player")) {
                pieceScript.enablePiece(newEnabledMoves);
            }
        }
    }

    public Vector3 traduceSquare(Vector2 position) {
        return new Vector3(position.x * 0.7875f, position.y * 0.7875f, 0f);
    }

    public Vector3 auxTraduceSquare(Vector2 position) {
        return new Vector3(position.x * 0.7875f + 20f, position.y * 0.7875f, 0f);
    }

    public int getTempo() {
        return tempo;
    }

    private void InstantiateInitialPositions() {

        foreach (GameObject dot in GameObject.FindGameObjectsWithTag("piece")){
            Destroy(dot);
        }
        pieces.Clear();
        auxPieces.Clear();
        pieces = new List<GameObject>(originalPieces);
        auxPieces = new List<GameObject>(originalPieces);

        for (int i = 0; i < 32; ++i) {
            GameObject pieceInstance = Instantiate(pieces[i]);
            pieces[i] = pieceInstance;

            GameObject auxPieceInstance = Instantiate(pieces[i]);
            auxPieces[i] = auxPieceInstance;
            initiateAuxPiece(auxPieceInstance, piecesInitialPositions[i], pieceTypes[i]);

            initiatePiece(pieceInstance, piecesInitialPositions[i], pieceTypes[i], auxPieces[i]); 
        }
        resetAllPossibleMoves();
    }

    private void resetAllPossibleMoves() {
        foreach(GameObject piece in pieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            pieceScript.setPossibleMoves();
        }
    }

    public void resetAllAuxPossibleMoves() {
        foreach(GameObject piece in auxPieces) {
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

    public string getAuxTileStatus(Vector2 position) {
        foreach (GameObject piece in auxPieces) {
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

    public GameObject getAuxPieceByPosition(Vector2 position) {
        foreach (GameObject piece in auxPieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            if (pieceScript.getPosition() == position) return piece;
        }
        return null;
    }

    public List<Vector2> getAllMoves(string playerType) {
        List<Vector2> allMoves = new List<Vector2>();

        foreach(GameObject piece in pieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            if (pieceScript.getType().Contains(playerType)) {
                List<Vector2> piecePossibleMoves = pieceScript.getPossibleMoves();

                foreach(Vector2 possibleMoves in piecePossibleMoves) {
                    if (!allMoves.Contains(possibleMoves)) {
                        allMoves.Add(possibleMoves);
                    }
                }
            }
        }
        return allMoves;
    }

    public List<Vector2> getAllAuxMoves(string playerType) {
        List<Vector2> allMoves = new List<Vector2>();

        foreach(GameObject piece in auxPieces) {
            Piece pieceScript = piece.GetComponent<Piece>();
            if (pieceScript.getType().Contains(playerType)) {
                List<Vector2> piecePossibleMoves = pieceScript.getPossibleMoves();

                foreach(Vector2 possibleMoves in piecePossibleMoves) {
                    if (!allMoves.Contains(possibleMoves)) {
                        allMoves.Add(possibleMoves);
                    }
                }
            }
        }
        return allMoves;
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

    public List<Vector2> getAllAuxThreads(string playerType) {
        List<Vector2> allThreads = new List<Vector2>();

        foreach(GameObject piece in auxPieces) {
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
