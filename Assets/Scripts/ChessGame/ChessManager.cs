using GlobalData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using static Util.Util;

public class ChessManager : MonoBehaviour, ICustomStart
{
    // First is horizontal, second vertical  -->  usage is: Board[x][y]
    public static ChessPiece[][] Board =
        {
        new ChessPiece[8],
        new ChessPiece[8],
        new ChessPiece[8],
        new ChessPiece[8],
        new ChessPiece[8],
        new ChessPiece[8],
        new ChessPiece[8],
        new ChessPiece[8]
    };

    string FEN_Notation;
    string EnPassant = "-";

    public static string AIOutput;
    public static ChessGameExec.ChessGameExecOutput UpdatedChessGameOutput;

    [SerializeField] GameObject WPawn;
    [SerializeField] GameObject WKnight;
    [SerializeField] GameObject WBishop;
    [SerializeField] GameObject WRook;
    [SerializeField] GameObject WQueen;
    [SerializeField] GameObject WKing;

    [SerializeField] GameObject BPawn;
    [SerializeField] GameObject BKnight;
    [SerializeField] GameObject BBishop;
    [SerializeField] GameObject BRook;
    [SerializeField] GameObject BQueen;
    [SerializeField] GameObject BKing;

    public GameObject TileFX;

    [SerializeField] Transform LeftBottomBoard;
    [SerializeField] Transform RightBottomBoard;
    private float BoardSize;

    private static float tileSize;
    public static Vector3 boardPos;

    //VoiceManager VoiceMgr;
    //[SerializeField] AudioController AudioMgr;

    public bool Castle_K = true;
    public bool Castle_Q = true;
    public bool Castle_k = true;
    public bool Castle_q = true;
    public static bool IsUserChecked = false;

    private void Awake()
    {
        //VoiceMgr = GetComponent<VoiceManager>();
        //AudioMgr = GetComponent<AudioController>();
    }

    public void CustomStart()
    {
        BoardSize = Mathf.Abs(LeftBottomBoard.position.x - RightBottomBoard.position.x);

        tileSize = BoardSize / 8;
        boardPos = LeftBottomBoard.position;

        SetupBoard();
        ResetGameData();

        //VoiceMgr.MoveRecognizer.Start();
    }

    private void SetupBoard()
    {
        //White pieces
        SpawnPiece(WRook, new Coord(0, 0), -90, 180);
        SpawnPiece(WKnight, new Coord(1, 0), -90, 180);
        SpawnPiece(WBishop, new Coord(2, 0), -90, 180);
        SpawnPiece(WQueen, new Coord(3, 0), -90, 180);
        SpawnPiece(WKing, new Coord(4, 0), -90, 180);
        SpawnPiece(WBishop, new Coord(5, 0), -90, 180);
        SpawnPiece(WKnight, new Coord(6, 0), -90, 180);
        SpawnPiece(WRook, new Coord(7, 0), -90, 180);
        for (int i = 0; i < 8; i++)
            SpawnPiece(WPawn, new Coord(i, 1), -90, 180);

        // Black pieces
        SpawnPiece(BRook, new Coord(0, 7), -90);
        SpawnPiece(BKnight, new Coord(1, 7), -90);
        SpawnPiece(BBishop, new Coord(2, 7), -90);
        SpawnPiece(BQueen, new Coord(3, 7), -90);
        SpawnPiece(BKing, new Coord(4, 7), -90);
        SpawnPiece(BBishop, new Coord(5, 7), -90);
        SpawnPiece(BKnight, new Coord(6, 7), -90);
        SpawnPiece(BRook, new Coord(7, 7), -90);
        for (int i = 0; i < 8; i++)
            SpawnPiece(BPawn, new Coord(i, 6), -90);
    }

    private void ResetGameData()
    {
        UpdateGameData(Util.Util.FEN_StartNotation, Util.Util.AllowedStartMoves);
    }

    void SpawnPiece(GameObject prefab, Coord coords, float rot, float yRot = 0)
    {
        ChessPiece _piece = Instantiate(prefab, GetWorldPosFromCoord(coords), Quaternion.Euler(rot, yRot, 0)).GetComponent<ChessPiece>();
        //_piece.GetComponent<Rigidbody>().isKinematic = true;
        _piece.Coords = coords;
        Board[coords.x][coords.y] = _piece;
        _piece.Lock();
    }

    public static Coord GetCoordFromWorldPos(Vector3 worldPos)
    {
        Vector3 pos = worldPos - boardPos;
        int x = Mathf.FloorToInt((pos.x / (tileSize * 8)) * 8);
        int y = Mathf.FloorToInt((pos.z / (tileSize * 8)) * 8);

        return new Coord(x, y);
    }

    public static Vector3 GetWorldPosFromCoord(Coord coord)
    {
        Vector3 pos = boardPos + (Vector3.right * (2 * coord.x + 1) * tileSize / 2) + (Vector3.forward * (2 * coord.y + 1) * tileSize / 2);
        return pos;
    }

    public void UpdatePiecePosition(Coord start, Coord end)
    {
        ChessPiece piece = Board[start.x][start.y];

        // Check for white castling
        if (piece.Type == "K")
        {
            if (start == new Coord(4, 0) && end == new Coord(6, 0))
                CastleKingWhite();
            else if (start == new Coord(4, 0) && end == new Coord(2, 0))
                CastleQueenWhite();
        }
        // Check for black castling
        else if (piece.Type == "k")
        {
            if (start == new Coord(4, 7) && end == new Coord(6, 7))
                CastleKingBlack();
            else if (start == new Coord(4, 7) && end == new Coord(2, 7))
                CastleQueenBlack();
        }

        // Check for en passant
        if (CoordToAlg(end) == EnPassant)
        {
            // White pawn
            if (piece.Type == "P")
            {
                // There is en passant
                if (start == new Coord(end.x - 1, end.y - 1) || start == new Coord(end.x + 1, end.y - 1))
                {
                    Coord blackPawnCoord = new Coord(AlgToCoord(EnPassant).x, AlgToCoord(EnPassant).y - 1);
                    ChessPiece blackPawn = Board[blackPawnCoord.x][blackPawnCoord.y];
                    blackPawn.Die();
                    Board[blackPawnCoord.x][blackPawnCoord.y] = null;
                }
            }
            // Black pawn
            else if (piece.Type == "p")
            {
                // There is en passant
                if (start == new Coord(end.x - 1, end.y + 1) || start == new Coord(end.x + 1, end.y + 1))
                {
                    Coord whitePawnCoord = new Coord(AlgToCoord(EnPassant).x, AlgToCoord(EnPassant).y + 1);
                    ChessPiece whitePawn = Board[whitePawnCoord.x][whitePawnCoord.y];
                    whitePawn.Die();
                    Board[whitePawnCoord.x][whitePawnCoord.y] = null;
                }
            }
        }

        Board[end.x][end.y] = piece;
        Board[start.x][start.y] = null;

        piece.Coords.x = end.x;
        piece.Coords.y = end.y;

        ChessDotNetManager.MakeMove(CoordToAlg(start), CoordToAlg(end));

        UpdateGameData(UpdatedChessGameOutput.FEN, UpdatedChessGameOutput.PossibleMoves);
    }

    void CastleKingWhite()
    {
        ChessPiece rook = Board[7][0];
        if (rook == null)
            Debug.LogError("rook is null");
        rook.Unlock();
        rook.transform.position = GetWorldPosFromCoord(new Coord(5, 0));
        rook.Lock();
        rook.Coords = new Coord(5, 0);

        Board[5][0] = rook;
        Board[7][0] = null;
    }

    void CastleQueenWhite()
    {
        ChessPiece rook = Board[0][0];
        if (rook == null)
            Debug.LogError("rook is null");
        rook.Unlock();
        rook.transform.position = GetWorldPosFromCoord(new Coord(3, 0));
        rook.Lock();
        rook.Coords = new Coord(3, 0);

        Board[3][0] = rook;
        Board[0][0] = null;
    }

    void CastleKingBlack()
    {
        ChessPiece rook = Board[7][7];
        if (rook == null)
            Debug.LogError("rook is null");
        rook.Unlock();
        rook.transform.position = GetWorldPosFromCoord(new Coord(5, 7));
        rook.Lock();
        rook.Coords = new Coord(5, 7);

        Board[5][7] = rook;
        Board[7][7] = null;
    }

    void CastleQueenBlack()
    {
        ChessPiece rook = Board[0][7];
        if (rook == null)
            Debug.LogError("rook is null");
        rook.Unlock();
        rook.transform.position = GetWorldPosFromCoord(new Coord(3, 7));
        rook.Lock();
        rook.Coords = new Coord(3, 7);

        Board[3][7] = rook;
        Board[0][7] = null;
    }

    IEnumerator WaitForStockFish()
    {
        // Is altered by StockFish.cs!
        AIOutput = null;

        ThreadedJob.GetMove(FEN_Notation);

        while (AIOutput == null)
        {
            yield return null;
        }

        // This is only for wizards chess
        //StartCoroutine(AutoMove(AlgToCoords(AIOutput.Substring(0, 2)), AlgToCoords(AIOutput.Substring(2, 2))));

        // This is for the chess game
        StartCoroutine(MovePiece(AlgToCoord(AIOutput.Substring(0, 2)), AlgToCoord(AIOutput.Substring(2, 2))));
    }

    //IEnumerator IUpdateChessGameExec(string codedString)
    //{
    //    //Debug.LogError("Called");
    //    //UpdatedChessGameOutput.FEN = null;
    //    //Debug.LogWarning("Encoded string sent to exec is: " + codedString);
    //    //ThreadedJob.ChessGameUpdateData(codedString);

    //    //while (UpdatedChessGameOutput.FEN == null)
    //    //{
    //    //    yield return null;
    //    //}

    //    //Debug.LogWarning("Output successfully received!");
    //    UpdateGameData(UpdatedChessGameOutput.FEN, UpdatedChessGameOutput.PossibleMoves);
    //    yield return null;
    //}

    private void UpdateGameData(string FEN, string[] possibleMoves)
    {
        FEN_Notation = FEN;
        UpdatePossibleMoves(possibleMoves);
        EnPassant = FEN.Split(' ')[3];

        if (GetTurnColorFromFEN(FEN_Notation) == "b")
            StartCoroutine(WaitForStockFish());
    }

    private void UpdatePossibleMoves(string[] moves)
    {
        // Reset possible moves
        ClearAllPossibleMoves();

        foreach (string move in moves)
        {
            Coord[] possibleMove_Coord = new Coord[] {
                AlgToCoord(move.Substring(0, 2)),
                AlgToCoord(move.Substring(2, 2))
            };

            Coord start = possibleMove_Coord[0];
            Coord end = possibleMove_Coord[1];

            Board[start.x][start.y].AddPossibleMove(end);
        }
    }

    private void ClearAllPossibleMoves()
    {
        for (int x = 0; x < Board.Length; x++)
        {
            for (int y = 0; y < Board[x].Length; y++)
            {
                ChessPiece curPiece = Board[x][y];
                if (curPiece != null)
                {
                    curPiece.ClearPossibleMoves();
                }
            }
        }
    }

    //public static bool CheckPlayerChecked()
    //{
    //    for (int x = 0; x < Board.Length; x++)
    //    {
    //        for (int y = 0; y < Board[x].Length; y++)
    //        {
    //            ChessPiece curPiece = Board[x][y];
    //            if (curPiece != null)
    //                if (IsOpponentPiece(curPiece))
    //                {
    //                    List<Coord> possibleSpots = MoveManager.GetMoveableSpots(curPiece.Coords, curPiece.Type[0], IsUserChecked);
    //                    foreach (Coord spot in possibleSpots)
    //                    {
    //                        if (Board[spot.x][spot.y] != null)
    //                        {
    //                            if (Board[spot.x][spot.y].Type == "K")
    //                            {
    //                                IsUserChecked = true;
    //                                return true;
    //                            }
    //                        }
    //                    }
    //                }
    //        }
    //    }

    //    return false;
    //}

    IEnumerator MovePiece(Coord start, Coord end)
    {
        ChessPiece piece = Board[start.x][start.y];
        if (piece == null)
            Debug.LogError("Ah shit here we go again");
        else
        {
            piece.Unlock();

            Vector3 newPos = GetWorldPosFromCoord(end);
            Vector3 initPos = piece.transform.position;

            bool isGoingToCapture = Board[end.x][end.y] != null;
            bool playingAnimation = false;
            bool animationFinished = false;

            Animator anim = piece.transform.GetChild(1).GetComponent<Animator>();

            float currentTime = 0f;
            float timeToMove = Data.PieceMoveTime * Vector3.Distance(GetWorldPosFromCoord(start), GetWorldPosFromCoord(end));

            piece.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;

            // Audio
            AudioController audioMgr = piece.gameObject.GetComponent<AudioController>();
            if (audioMgr == null)
                Debug.LogError("There was no AudioController component attached to the gameobject!");

            if (isGoingToCapture)
                audioMgr.PieceMoveAudio(timeToMove / 2, 0.3f);
            else
                audioMgr.PieceMoveAudio(timeToMove, 0.5f);


            while (currentTime <= timeToMove - 0.001f)
            {
                if (!playingAnimation)
                {
                    currentTime += Time.deltaTime;
                    piece.transform.position = Vector3.Lerp(initPos, newPos, Mathf.SmoothStep(0.0f, 1.0f, currentTime / timeToMove));

                    if (isGoingToCapture && !animationFinished)
                    {
                        if (currentTime / timeToMove >= 0.5f)
                        {
                            anim.SetTrigger("capture");
                            playingAnimation = true;
                            audioMgr.PieceDestroyAudio();
                        }
                    }

                    yield return null;
                }
                else if (!die)
                {
                    yield return null;
                }
                else if (!dead)
                {
                    if (Board[end.x][end.y] != null)
                    {
                        Board[end.x][end.y].GetComponent<ChessPiece>().Die();
                    }
                    dead = true;
                }
                else
                {
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName("end"))
                    {
                        anim.SetTrigger("reset");
                        animationFinished = true;
                        playingAnimation = false;
                        die = false;
                        dead = false;

                        // Audio
                        audioMgr.PieceMoveAudio(timeToMove / 2, 0.3f);
                    }
                    yield return null;
                }
            }

            piece.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            piece.transform.position = new Vector3(newPos.x, piece.transform.position.y, newPos.z);
            piece.Lock();

            UpdatePiecePosition(start, end);
        }
    }

    //IEnumerator AutoMove(Coord start, Coord end)
    //{
    //    ChessPiece piece = Board[start.x][start.y];
    //    if (piece == null)
    //        Debug.LogError("Ah shit here we go again");
    //    else
    //    {
    //        piece.Unlock();

    //        Vector3 newPos = GetWorldPosFromCoord(end);
    //        Vector3 initPos = piece.transform.position;

    //        bool isGoingToCapture = Board[end.x][end.y] != null;
    //        bool playingAnimation = false;
    //        bool animationFinished = false;

    //        Animator anim = piece.GetComponent<Animator>();

    //        float currentTime = 0f;
    //        float timeToMove = 0.7f * Vector3.Distance(GetWorldPosFromCoord(start), GetWorldPosFromCoord(end));

    //        piece.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;

    //        // Audio
    //        AudioController audioMgr = piece.gameObject.GetComponent<AudioController>();
    //        if (audioMgr == null)
    //            Debug.LogError("There was no AudioController component attached to the gameobject!");

    //        if(isGoingToCapture)
    //            audioMgr.PieceMoveAudio(timeToMove / 2, 0.3f);
    //        else
    //            audioMgr.PieceMoveAudio(timeToMove, 0.5f);


    //        while (currentTime <= timeToMove - 0.001f)
    //        {
    //            if (!playingAnimation)
    //            {
    //                currentTime += Time.deltaTime;
    //                piece.transform.position = Vector3.Lerp(initPos, newPos, Mathf.SmoothStep(0.0f, 1.0f, currentTime / timeToMove));

    //                if (isGoingToCapture && !animationFinished)
    //                {
    //                    if (currentTime / timeToMove >= 0.5f)
    //                    {
    //                        anim.SetTrigger("capture");
    //                        playingAnimation = true;
    //                        audioMgr.PieceDestroyAudio();
    //                    }
    //                }

    //                yield return null;
    //            }
    //            else if(!die)
    //            {
    //                yield return null;
    //            }
    //            else if(!dead)
    //            {
    //                if (Board[end.x][end.y] != null)
    //                {
    //                    Board[end.x][end.y].GetComponent<ChessPiece>().Die();
    //                }
    //                dead = true;
    //            }
    //            else
    //            {
    //                if (anim.GetCurrentAnimatorStateInfo(0).IsName("end"))
    //                {
    //                    anim.SetTrigger("reset");
    //                    animationFinished = true;
    //                    playingAnimation = false;
    //                    die = false;
    //                    dead = false;

    //                    // Audio
    //                    audioMgr.PieceMoveAudio(timeToMove / 2, 0.3f);
    //                }
    //                yield return null;
    //            }
    //        }

    //        piece.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    //        piece.transform.position = new Vector3(newPos.x, piece.transform.position.y, newPos.z);
    //        piece.Lock();

    //        UpdatePiecePosition(start, end);
    //    }
    //}

    bool die = false;
    bool dead = false;
    public void KillEnemy()
    {
        die = true;
    }

    //public string GetFenNotation()
    //{
    //    int empty = 0;
    //    string result = "";

    //    // Position of the pieces
    //    for(int y=7; y>=0; y--)
    //    {
    //        for(int x=0; x<Board[y].Length; x++)
    //        {
    //            ChessPiece piece = Board[x][y];

    //            if(piece != null)
    //            {
    //                if (empty > 0)
    //                {
    //                    result += empty.ToString();
    //                    empty = 0;
    //                }
    //                result += piece.Type;
    //            }
    //            else
    //            {
    //                empty++;
    //            }
    //        }

    //        if (empty > 0)
    //        {
    //            result += empty.ToString();
    //            empty = 0;
    //        }
    //        if (y > 0) 
    //        {
    //            result += "/";
    //        }
    //    }

    //    // Colors turn
    //    result += " " + ColorTurn;

    //    // Castling
    //    result += " ";
    //    result += Castle_K ? "K" : "";
    //    result += Castle_Q ? "Q" : "";
    //    result += Castle_k ? "k" : "";
    //    result += Castle_q ? "q" : "";
    //    result += (!Castle_K && !Castle_Q && !Castle_k && !Castle_q) ? "-" : "";

    //    // En passant
    //    result += " -";

    //    // Halfmove clock
    //    result += " 0";

    //    // Fullmove counter
    //    result += " 1";

    //    return result;
    //}

    public void VoiceMove(string type, Coord pos)
    {
        //ChessPiece firstPiece = null;
        //ChessPiece secondPiece = null;

        //for (int y = 0; y < Board.Length; y++)
        //{
        //    for (int x = 0; x < Board.Length; x++)
        //    {
        //        if (Board[x][y] != null)
        //            if (Board[x][y].Type == type)
        //            {
        //                if (!firstPiece)
        //                {
        //                    List<Coord> avalaibleCoords = MoveManager.GetMoveableSpots(Board[x][y].Coords, type[0], IsUserChecked);
        //                    foreach (Coord coord in avalaibleCoords)
        //                    {
        //                        if (coord.x == pos.x && coord.y == pos.y)
        //                        {
        //                            firstPiece = Board[x][y];
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    List<Coord> avalaibleCoords = MoveManager.GetMoveableSpots(Board[x][y].Coords, type[0], IsUserChecked);
        //                    foreach (Coord coord in avalaibleCoords)
        //                    {
        //                        if (coord.x == pos.x && coord.y == pos.y)
        //                        {
        //                            secondPiece = Board[x][y];
        //                        }
        //                    }
        //                    break;
        //                }
        //            }
        //    }
        //}

        //if (firstPiece)
        //{
        //    if (!secondPiece)
        //        StartCoroutine(AutoMove(firstPiece.Coords, pos));
        //    else
        //        Debug.LogError("Multiple possibilities!");
        //}
        //else
        //{
        //    // Invalid move
        //    AudioMgr.InvalidMove();
        //}
    }

    //public static class MoveManager
    //{
    //    public static List<Coord> GetMoveableSpots(Coord piecePos, char pieceType, bool isChecked)
    //    {
    //        List<Coord> result = new List<Coord>();

    //        bool white = false;
    //        if (char.IsUpper(pieceType))
    //            white = true;

    //        string type = pieceType.ToString().ToLower();

    //        result.Clear();

    //        switch (type)
    //        {
    //            case "k":
    //                result.AddRange(GetKingMoves(white, piecePos));
    //                break;
    //            case "q":
    //                result.AddRange(GetDiagonalMoves(white, piecePos));
    //                result.AddRange(GetStraightMoves(white, piecePos));
    //                break;
    //            case "n":
    //                result.AddRange(GetKnightMoves(white, piecePos));
    //                break;
    //            case "b":
    //                result.AddRange(GetDiagonalMoves(white, piecePos));
    //                break;
    //            case "r":
    //                result.AddRange(GetStraightMoves(white, piecePos));
    //                break;
    //            case "p":
    //                result.AddRange(GetPawnMoves(white, piecePos));
    //                break;
    //        }

    //        if (isChecked)
    //        {
    //            List<Coord> filteredResult = new List<Coord>();
    //            foreach (Coord coord in result)
    //            {
    //                ChessPiece start = Board[piecePos.x][piecePos.y];
    //                ChessPiece end = Board[coord.x][coord.y];

    //                Board[coord.x][coord.y] = start;
    //                Board[piecePos.x][piecePos.y] = null;
    //                if (!CheckPlayerChecked())
    //                {
    //                    filteredResult.Add(coord);
    //                }
    //                Board[piecePos.x][piecePos.y] = start;
    //                Board[coord.x][coord.y] = end;
    //            }

    //            return filteredResult;
    //        }

    //        return result;
    //    }

    //    static List<Coord> GetKnightMoves(bool white, Coord pos)
    //    {
    //        List<Coord> result = new List<Coord>();

    //        Coord[] coordsToCheck = new Coord[8];
    //        coordsToCheck[0] = new Coord(pos.x - 1, pos.y + 2);
    //        coordsToCheck[1] = new Coord(pos.x + 1, pos.y + 2);
    //        coordsToCheck[2] = new Coord(pos.x + 2, pos.y + 1);
    //        coordsToCheck[3] = new Coord(pos.x + 2, pos.y - 1);
    //        coordsToCheck[4] = new Coord(pos.x - 1, pos.y - 2);
    //        coordsToCheck[5] = new Coord(pos.x + 1, pos.y - 2);
    //        coordsToCheck[6] = new Coord(pos.x - 2, pos.y + 1);
    //        coordsToCheck[7] = new Coord(pos.x - 2, pos.y - 1);

    //        foreach (Coord coord in coordsToCheck)
    //        {
    //            int x = coord.x;
    //            int y = coord.y;

    //            if (0 <= x && x <= 7 && 0 <= y && y <= 7)
    //            {
    //                if (Board[x][y] == null || char.IsUpper(Board[x][y].Type[0]) == !white)
    //                {
    //                    result.Add(new Coord(x, y));
    //                }
    //            }
    //        }

    //        return result;
    //    }

    //    static List<Coord> GetKingMoves(bool white, Coord pos)
    //    {
    //        List<Coord> result = new List<Coord>();

    //        bool hl = pos.x - 1 >= 0;
    //        bool hr = pos.x + 1 <= 7;
    //        bool vd = pos.y - 1 >= 0;
    //        bool vu = pos.y + 1 <= 7;

    //        // Straight
    //        if (hl)
    //        {
    //            int x = pos.x - 1;
    //            int y = pos.y;
    //            if (Board[x][y] == null || char.IsUpper(Board[x][y].Type[0]) == !white)
    //                result.Add(new Coord(x, y));
    //        }
    //        if (hr)
    //        {
    //            int x = pos.x + 1;
    //            int y = pos.y;
    //            if (Board[x][y] == null || char.IsUpper(Board[x][y].Type[0]) == !white)
    //                result.Add(new Coord(x, y));
    //        }
    //        if (vd)
    //        {
    //            int x = pos.x;
    //            int y = pos.y - 1;
    //            if (Board[x][y] == null || char.IsUpper(Board[x][y].Type[0]) == !white)
    //                result.Add(new Coord(x, y));
    //        }
    //        if (vu)
    //        {
    //            int x = pos.x;
    //            int y = pos.y + 1;
    //            if (Board[x][y] == null || char.IsUpper(Board[x][y].Type[0]) == !white)
    //                result.Add(new Coord(x, y));
    //        }

    //        // Diagonal
    //        if (hl && vd)
    //        {
    //            int x = pos.x - 1;
    //            int y = pos.y - 1;
    //            if (Board[x][y] == null || char.IsUpper(Board[x][y].Type[0]) == !white)
    //                result.Add(new Coord(x, y));
    //        }
    //        if (hr && vd)
    //        {
    //            int x = pos.x + 1;
    //            int y = pos.y - 1;
    //            if (Board[x][y] == null || char.IsUpper(Board[x][y].Type[0]) == !white)
    //                result.Add(new Coord(x, y));
    //        }
    //        if (hl && vu)
    //        {
    //            int x = pos.x - 1;
    //            int y = pos.y + 1;
    //            if (Board[x][y] == null || char.IsUpper(Board[x][y].Type[0]) == !white)
    //                result.Add(new Coord(x, y));
    //        }
    //        if (hr && vu)
    //        {
    //            int x = pos.x + 1;
    //            int y = pos.y + 1;
    //            if (Board[x][y] == null || char.IsUpper(Board[x][y].Type[0]) == !white)
    //                result.Add(new Coord(x, y));
    //        }

    //        return result;
    //    }

    //    static List<Coord> GetStraightMoves(bool white, Coord pos)
    //    {
    //        List<Coord> result = new List<Coord>();

    //        // Horizontal moves left
    //        int maxHLmoves = pos.x;
    //        for (int i = 0; i < maxHLmoves; i++)
    //        {
    //            int x = pos.x - i - 1; // Zero based
    //            int y = pos.y;

    //            ChessPiece target = Board[x][y];
    //            if (target == null)
    //            {
    //                result.Add(new Coord(x, y));
    //            }
    //            else
    //            {
    //                if (char.IsUpper(target.Type[0]) == !white)
    //                    result.Add(target.Coords);
    //                break;
    //            }
    //        }

    //        // Horizontal moves right
    //        int maxHRmoves = 7 - pos.x;
    //        for (int i = 0; i < maxHRmoves; i++)
    //        {
    //            int x = pos.x + i + 1; // Zero based
    //            int y = pos.y;

    //            ChessPiece target = Board[x][y];
    //            if (target == null)
    //            {
    //                result.Add(new Coord(x, y));
    //            }
    //            else
    //            {
    //                if (char.IsUpper(target.Type[0]) == !white)
    //                    result.Add(target.Coords);
    //                break;
    //            }
    //        }

    //        // Vertical moves down
    //        int maxVDmoves = pos.y;
    //        for (int i = 0; i < maxVDmoves; i++)
    //        {
    //            int x = pos.x; // Zero based
    //            int y = pos.y - i - 1;

    //            ChessPiece target = Board[x][y];
    //            if (target == null)
    //            {
    //                result.Add(new Coord(x, y));
    //            }
    //            else
    //            {
    //                if (char.IsUpper(target.Type[0]) == !white)
    //                    result.Add(target.Coords);
    //                break;
    //            }
    //        }

    //        // Vertical moves up
    //        int maxVUmoves = 7 - pos.y;
    //        for (int i = 0; i < maxVUmoves; i++)
    //        {
    //            int x = pos.x; // Zero based
    //            int y = pos.y + i + 1;

    //            ChessPiece target = Board[x][y];
    //            if (target == null)
    //            {
    //                result.Add(new Coord(x, y));
    //            }
    //            else
    //            {
    //                if (char.IsUpper(target.Type[0]) == !white)
    //                    result.Add(target.Coords);
    //                break;
    //            }
    //        }

    //        return result;
    //    }

    //    static List<Coord> GetPawnMoves(bool white, Coord pos)
    //    {
    //        List<Coord> result = new List<Coord>();

    //        if (white)
    //        {
    //            // Double advance 2 forward
    //            if (pos.y == 1)
    //                if (Board[pos.x][pos.y + 2] == null)
    //                    result.Add(new Coord(pos.x, pos.y + 2));

    //            // Standard advance 1 forward (no y-check because a pawn can never exist at the last tile)
    //            if (Board[pos.x][pos.y + 1] == null)
    //                result.Add(new Coord(pos.x, pos.y + 1));

    //            // En passant left
    //            if (pos.x - 1 >= 0)
    //                if (Board[pos.x - 1][pos.y] != null)
    //                    if (Board[pos.x - 1][pos.y].Type == "p")
    //                        if (Board[pos.x - 1][pos.y].DoubleAdvanced)
    //                            result.Add(new Coord(pos.x - 1, pos.y + 1));

    //            // En passant right
    //            if (pos.x + 1 <= 7)
    //                if (Board[pos.x + 1][pos.y] != null)
    //                    if (Board[pos.x + 1][pos.y].Type == "p")
    //                        if (Board[pos.x + 1][pos.y].DoubleAdvanced)
    //                            result.Add(new Coord(pos.x + 1, pos.y + 1));

    //            // Standard take left
    //            if (pos.x - 1 >= 0)
    //                if (Board[pos.x - 1][pos.y + 1] != null)
    //                    if (char.IsLower(Board[pos.x - 1][pos.y + 1].Type[0]))
    //                        result.Add(new Coord(pos.x - 1, pos.y + 1));

    //            // Standard take right
    //            if (pos.x + 1 <= 7)
    //                if (Board[pos.x + 1][pos.y + 1] != null)
    //                    if (char.IsLower(Board[pos.x + 1][pos.y + 1].Type[0]))
    //                        result.Add(new Coord(pos.x + 1, pos.y + 1));
    //        }
    //        else if (!white)
    //        {
    //            // Double advance 2 forward
    //            if (pos.y == 6)
    //                if (Board[pos.x][pos.y - 2] == null)
    //                    result.Add(new Coord(pos.x, pos.y - 2));

    //            // Standard advance 1 forward (no y-check because a pawn can never exist at the last tile)
    //            if (Board[pos.x][pos.y - 1] == null)
    //                result.Add(new Coord(pos.x, pos.y - 1));

    //            // En passant left
    //            if (pos.x - 1 >= 0)
    //                if (Board[pos.x - 1][pos.y] != null)
    //                    if (Board[pos.x - 1][pos.y].Type == "P")
    //                        if (Board[pos.x - 1][pos.y].DoubleAdvanced)
    //                            result.Add(new Coord(pos.x - 1, pos.y - 1));

    //            // En passant right
    //            if (pos.x + 1 <= 7)
    //                if (Board[pos.x + 1][pos.y] != null)
    //                    if (Board[pos.x + 1][pos.y].Type == "P")
    //                        if (Board[pos.x + 1][pos.y].DoubleAdvanced)
    //                            result.Add(new Coord(pos.x + 1, pos.y - 1));

    //            // Standard take left
    //            if (pos.x - 1 >= 0)
    //                if (Board[pos.x - 1][pos.y - 1] != null)
    //                    if (char.IsUpper(Board[pos.x - 1][pos.y - 1].Type[0]))
    //                        result.Add(new Coord(pos.x - 1, pos.y - 1));

    //            // Standard take right
    //            if (pos.x + 1 <= 7)
    //                if (Board[pos.x + 1][pos.y - 1] != null)
    //                    if (char.IsUpper(Board[pos.x + 1][pos.y - 1].Type[0]))
    //                        result.Add(new Coord(pos.x + 1, pos.y - 1));
    //        }

    //        return result;
    //    }

    //    static List<Coord> GetDiagonalMoves(bool white, Coord pos)
    //    {
    //        List<Coord> result = new List<Coord>();

    //        // Upper right
    //        int maxURmoves = Mathf.Min(7 - pos.x, 7 - pos.y);
    //        for (int i = 0; i < maxURmoves; i++)
    //        {
    //            int x = pos.x + i + 1; // Zero based
    //            int y = pos.y + i + 1;

    //            ChessPiece target = Board[x][y];
    //            if (target == null)
    //            {
    //                result.Add(new Coord(x, y));
    //            }
    //            else
    //            {
    //                if (char.IsUpper(target.Type[0]) == !white)
    //                    result.Add(target.Coords);
    //                break;
    //            }
    //        }

    //        // Upper left
    //        int maxULmoves = Mathf.Min(pos.x, 7 - pos.y);
    //        for (int i = 0; i < maxULmoves; i++)
    //        {
    //            int x = pos.x - i - 1;
    //            int y = pos.y + i + 1;

    //            ChessPiece target = Board[x][y];
    //            if (target == null)
    //            {
    //                result.Add(new Coord(x, y));
    //            }
    //            else
    //            {
    //                if (char.IsUpper(target.Type[0]) == !white)
    //                    result.Add(target.Coords);
    //                break;
    //            }
    //        }

    //        // Lower right
    //        int maxLRmoves = Mathf.Min(7 - pos.x, pos.y);
    //        for (int i = 0; i < maxLRmoves; i++)
    //        {
    //            int x = pos.x + i + 1;
    //            int y = pos.y - i - 1;

    //            ChessPiece target = Board[x][y];
    //            if (target == null)
    //            {
    //                result.Add(new Coord(x, y));
    //            }
    //            else
    //            {
    //                if (char.IsUpper(target.Type[0]) == !white)
    //                    result.Add(target.Coords);
    //                break;
    //            }
    //        }

    //        // Lower left
    //        int maxLLmoves = Mathf.Min(pos.x, pos.y);
    //        for (int i = 0; i < maxLLmoves; i++)
    //        {
    //            int x = pos.x - i - 1;
    //            int y = pos.y - i - 1;

    //            ChessPiece target = Board[x][y];
    //            if (target == null)
    //            {
    //                result.Add(new Coord(x, y));
    //            }
    //            else
    //            {
    //                if (char.IsUpper(target.Type[0]) == !white)
    //                    result.Add(target.Coords);
    //                break;
    //            }
    //        }
    //        return result;
    //    }
    //}
}
