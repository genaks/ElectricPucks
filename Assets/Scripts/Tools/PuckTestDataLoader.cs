using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using RedEngine;
using UnityEditor;
using UnityEngine;

namespace Tools
{
	public static class PuckTestDataLoader
	{
		private const string JsonPathFormat = "Assets/Data/puck_data_{0}.json";

		[MenuItem("Red Engine/Log Puck Test/Data 1")]
		private static void LogPuckData()
		{
			LogPuckData(1);
		}
		
		[MenuItem("Red Engine/Log Puck Test/Data 2")]
		private static void LogPuckData2()
		{
			LogPuckData(2);
		}
		
		[MenuItem("Red Engine/Log Puck Test/Data 3")]
		private static void LogPuckData3()
		{
			LogPuckData(3);
		}

		public static IReadOnlyList<FrameData> LoadPuckData(int fileNumber)
		{
			var path = string.Format(JsonPathFormat, fileNumber);

			if (!File.Exists(path))
			{
				Debug.LogError($"File not found at {path}");
				return Array.Empty<FrameData>();
			}

			string json = File.ReadAllText(path);
			var frames = JsonConvert.DeserializeObject<List<FrameData>>(json);

			return frames == null || frames.Count == 0 ? Array.Empty<FrameData>() : frames;
		}

		private static void LogPuckData(int fileNumber)
		{
			var data = LoadPuckData(fileNumber);

			foreach (var frame in data)
			{
				Debug.Log($"Frame Timestamp: {frame.Timestamp}, Puck Count: {frame.Pucks.Count}");

				foreach (var puck in frame.Pucks)
				{
					Debug.Log(
						$"  Puck #{puck.PuckNumber} | Colour: {puck.PuckColour.ToString()} | Score: {puck.Score} | Pos: ({puck.X:F2}, {puck.Y:F2}) | Speed: ({puck.SpeedX:F2}, {puck.SpeedY:F2}) | Foul: {puck.IsFoulPuck} | Stationary: {puck.Stationary}");
				}
			}
		}
	}
}