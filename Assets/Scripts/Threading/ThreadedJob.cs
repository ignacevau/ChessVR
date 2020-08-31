using System.Collections;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class ThreadedJob : MonoBehaviour
{
    private static JobHandle jobHandle;

    public enum Jobs
    {
        GetMove
    }

    struct GetMove_Job : IJob
    {
        public NativeArray<byte> inStr;
        public NativeArray<byte> outStr;

        public void Execute()
        {
            string _data = System.Text.Encoding.ASCII.GetString(inStr.ToArray());
            StockFish.GetBestMove(_data);
        }
    }

    //struct ChessGameUpdater_Job : IJob
    //{
    //    public NativeArray<byte> inStr;
    //    public NativeArray<byte> outStr;

    //    public void Execute()
    //    {
    //        Debug.Log("Made it in execute");
    //        string _data = System.Text.Encoding.ASCII.GetString(inStr.ToArray());
    //        Debug.Log("full _data is: " + _data);
    //        string[] split = _data.Split(':');
    //        string _FEN = split[0];
    //        string _nextMove = split[1];
    //        ChessGameExec.ExecuteCommand(_FEN, _nextMove);
    //    }
    //}

    //public static void ChessGameUpdateData(string FEN)
    //{
    //    Debug.Log("Made it inside the job");
    //    ChessGameUpdater_Job chessGameJob = new ChessGameUpdater_Job();

    //    chessGameJob.inStr = new NativeArray<byte>(FEN.Length, Allocator.Persistent);
    //    chessGameJob.outStr = new NativeArray<byte>(7, Allocator.Persistent);

    //    chessGameJob.inStr.CopyFrom(System.Text.Encoding.ASCII.GetBytes(FEN));

    //    jobHandle = chessGameJob.Schedule();
    //    JobHandle.ScheduleBatchedJobs();
    //    jobHandle.Complete();

    //    if (jobHandle.IsCompleted)
    //    {
    //        string result = System.Text.Encoding.ASCII.GetString(chessGameJob.outStr.ToArray());
    //        chessGameJob.inStr.Dispose();
    //        chessGameJob.outStr.Dispose();
    //    }
    //}

    public static void GetMove(string gameString)
    {
        GetMove_Job getMoveJob = new GetMove_Job();

        getMoveJob.inStr = new NativeArray<byte>(gameString.Length, Allocator.Persistent);
        getMoveJob.outStr = new NativeArray<byte>(7, Allocator.Persistent);

        getMoveJob.inStr.CopyFrom(System.Text.Encoding.ASCII.GetBytes(gameString));

        jobHandle = getMoveJob.Schedule();
        JobHandle.ScheduleBatchedJobs();
        jobHandle.Complete();

        if(jobHandle.IsCompleted)
        {
            string result = System.Text.Encoding.ASCII.GetString(getMoveJob.outStr.ToArray());
            getMoveJob.inStr.Dispose();
            getMoveJob.outStr.Dispose();
        }
    }
}