using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChessDotNet;
using System.Collections.ObjectModel;
using static Util.Util;
using System.Security.Cryptography;
using static ChessGameExec;
using System;
using GlobalData;

public class ChessDotNetManager : MonoBehaviour
{
    static ChessGame game;

    private void Start()
    {
        StartNewGame();
    }

    public static void StartNewGame()
    {
        game = new ChessGame(Data.StartFenNotation);
    }

    public static List<string> GetMoves()
    {
        Player player = game.WhoseTurn;
        var _moves = game.GetValidMoves(player);
        List<string> moves = new List<string>();

        foreach(Move move in _moves)
        {
            moves.Add(move.ToString());
        }

        return moves;
    }

    public static void MakeMove(string start, string end)
    {
        Player player = game.WhoseTurn;
        Position p1 = new Position(start);
        Position p2 = new Position(end);

        Move move = new Move(p1, p2, player);
        game.MakeMove(move, true);


        // Update the game info
        ChessGameExecOutput output = new ChessGameExecOutput();

        output.PossibleMoves = GetMoves().ToArray();
        output.FEN = GetFEN();

        ChessManager.UpdatedChessGameOutput = output;
    }

    public static string GetFEN()
    {
        return game.GetFen();
    }
}
