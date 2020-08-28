using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class StockFish : MonoBehaviour {

    public static string StockfishPath;

    private void Start()
    {
        StockfishPath = Application.streamingAssetsPath + "/stockfish_10_x64";
    }

    public static async void GetBestMove(string forsythEdwardsNotationString)
    {
        System.Diagnostics.Process p = new System.Diagnostics.Process();
        p.StartInfo.FileName = StockfishPath;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.Start();
        string setupString = "position fen " + forsythEdwardsNotationString;
        p.StandardInput.WriteLine(setupString);

        string processString = "go ";
        // Process for 5 seconds
        //processString += "movetime 5000";

        // Process only in depth
        processString += " depth " + Util.Util.StockFishDepth.ToString();

        p.StandardInput.WriteLine(processString);

        string _newMove = "";
        bool running = true;
        while(running)
        {
            string line = await p.StandardOutput.ReadLineAsync();
            if (line.StartsWith("bestmove"))
            {
                running = false;

                string[] _stripped = line.Split(' ');
                _newMove = _stripped[1];
            }
        }
        Debug.Log("Best move is: " + _newMove);
        ChessManager.AIOutput = _newMove;

        p.Close();
    }
}
