using System.Collections.Generic;
using Data;

namespace RedEngine
{
	public class FrameData
	{
		public double Timestamp { get; }
		public IReadOnlyList<PuckData> Pucks { get; }

		public FrameData(double timestamp, IReadOnlyList<PuckData> pucks)
		{
			Timestamp = timestamp;
			Pucks = pucks;
		}
	}
}