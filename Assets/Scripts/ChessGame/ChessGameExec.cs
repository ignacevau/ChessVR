using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class ChessGameExec : MonoBehaviour {

    public struct ChessGameExecOutput
    {
        public string FEN;
        public string[] PossibleMoves;
    }

    public static string ChessGamePath;

    private void Awake()
    {
        ChessGamePath = Application.streamingAssetsPath + "/ChessGame";
    }

    //public static async void ExecuteCommand(string FEN, string nextMove)
    //{
        //Debug.LogWarning("Fen input is: " + FEN);
        //Debug.LogWarning("next move input is: " + nextMove);
        //System.Diagnostics.Process p = new System.Diagnostics.Process();
        //p.StartInfo.FileName = ChessGamePath;
        //p.StartInfo.UseShellExecute = false;
        //p.StartInfo.CreateNoWindow = true;
        //p.StartInfo.RedirectStandardError = true;
        //p.StartInfo.RedirectStandardInput = true;
        //p.StartInfo.RedirectStandardOutput = true;
        //p.Start();

        //p.StandardInput.WriteLine(FEN);
        //p.StandardInput.WriteLine(nextMove);


        ////First output line contains all possible moves
        //string moves = await p.StandardOutput.ReadLineAsync();
        ////output.PossibleMoves = moves.Split(',');

        ////Second output line contains the new FEN notation
        //string fen = await p.StandardOutput.ReadLineAsync();
        ////output.FEN = fen;


        //Debug.LogWarning("Output exec generated!");

        //p.Close();
    //}
}
