using System.Collections.Generic;

namespace Data
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