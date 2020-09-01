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


		#region RayPointerSettings
		public static class RayPointerSettings
		{
			// How for should be first point be from the hand in comparison to the end point
			public static float CLOSEST_FURTHEST_RATIO { get; } = 0.4f;		// 2/5

			// By what factor the thickness increases for increases ray length
			public static float DISTANCE_THICKNESS_MULTIPLIER { get; } = 0.002f;

			// Starting width, is not influenced by the factor above
			public static float START_WIDTH { get; } = 0.005f;
		}

        #endregion /RayPointerSettings
    }
}
