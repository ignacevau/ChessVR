using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Util
{
    public interface ICustomStart
    {
        void CustomStart();
    }

    public class UnityEvent_Collider : UnityEvent<Collider> { };

    public class Util
    {
        //public static string FEN_StartNotation = ;
        public static string[] AllowedStartMoves = new string[] { "a2a3", "a2a4", "b2b3", "b2b4", "c2c3", "c2c4", "d2d3", "d2d4", "e2e3", "e2e4", "f2f3", "f2f4", "g2g3", "g2g4", "h2h3", "h2h4", "b1c3", "b1a3", "g1h3", "g1f3" };
        public static int StockFishDepth = 10;

        private static Dictionary<char, int> CharCoordDict = new Dictionary<char, int>
        {
            { 'a', 0 },
            { 'b', 1 },
            { 'c', 2 },
            { 'd', 3 },
            { 'e', 4 },
            { 'f', 5 },
            { 'g', 6 },
            { 'h', 7 },
        };

        private static Dictionary<int, char> CoordCharDict = new Dictionary<int, char>
        {
            { 0, 'a' },
            { 1, 'b' },
            { 2, 'c' },
            { 3, 'd' },
            { 4, 'e' },
            { 5, 'f' },
            { 6, 'g' },
            { 7, 'h' },
        };

        public static string GetTurnColorFromFEN(string FEN)
        {
            string[] split = FEN.Split(' ');
            return split[1];
        }

        //public static bool IsOpponentPiece(ChessPiece piece)
        //{
        //    return char.IsLower(piece.Type[0]);
        //}

        public class Coord
        {
            public int x;
            public int y;

            public Coord(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public static bool operator ==(Coord lhs, Coord rhs)
            {
                bool status = false;
                if (lhs.x == rhs.x && lhs.y == rhs.y)
                    status = true;
                return status;
            }

            public override bool Equals(object obj)
            {
                if (obj.GetType() == this.GetType())
                {
                    return this == (Coord)obj;
                }
                return false;
            }

            public static bool operator !=(Coord lhs, Coord rhs)
            {
                bool status = true;
                if (lhs.x == rhs.x || lhs.y == rhs.y)
                    status = false;
                return status;
            }

            public override int GetHashCode()
            {
                return 0;
            }

            public Coord Clone()
            {
                return new Coord(x, y);
            }
        }

        public static Coord AlgToCoord(string str)
        {
            if (str.Length != 2)
            {
                Debug.LogError("Algebraic input was not 2 chars!");
            }

            int coord1 = CharCoordDict[str[0]];
            int.TryParse(str.Substring(1, 1), out int coord2);

            return new Coord(coord1, coord2 - 1);
        }

        public static string CoordToAlg(Coord coord)
        {
            if (coord.x < 0 || coord.y < 0 || coord.x >= 8 || coord.y >= 8)
            {
                Debug.LogError("Invalid Coord, x or y property was invalid");
            }

            string alg = "";
            alg += CoordCharDict[coord.x];
            alg += (coord.y + 1).ToString();

            return alg;
        }

        public static string getCodedChessGameExecData(string FEN, string nextMove)
        {
            return FEN + ":" + nextMove;
        }
    }
}