using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalData
{
	public static class Data
	{
		public static float PieceMoveTime { get; set; } = 5f;
		public static int TableHeight { get; set; } = 2;
		public static int maxTableHeight { get; } = 5;
		public static int minTableHeight { get; } = 0;
		public static float MoveTableFactor { get; } = 0.1f;
		public static GameObject grabbedObject { get; set; }

	}
}
