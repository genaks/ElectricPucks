using Newtonsoft.Json;
using RedEngine;

namespace Data
{
    public class PuckData
    {
        public PuckColour PuckColour { get; }
		
        /// <summary>
        /// The unique ID of the puck.
        /// Note: When pucks are moving fast this may not be reliable due to tracking misidentification.
        /// </summary>
        public uint PuckNumber { get; }
		
        /// <summary>
        /// The scoring region the puck is in.
        /// </summary>
        public int Score { get; }
		
        /// <summary>
        /// The distance in mm from the end of the shuffle table.
        /// </summary>
        public float X { get; }
		
        /// <summary>
        /// The distance in mm from the right edge of the shuffle table (from the perspective of a player standing at the throwing end of the table).
        /// </summary>
        public float Y { get; }
		
        public double SpeedX { get; }
        public double SpeedY { get; }
		
        public double LeadingEdge { get; }
        public double TrailingEdge { get; }
		
        /// <summary>
        /// Has the puck crossed the foul line? If not it should be discounted.
        /// </summary>
        public bool IsFoulPuck { get; }
		
        public bool Stationary { get; }

        [JsonConstructor]
        public PuckData(PuckColour puckColour,
            uint puckNumber,
            int score,
            bool isFoulPuck,
            double x,
            double y,
            double speedX,
            double speedY,
            double leadingEdge,
            double trailingEdge,
            bool stationary)
        {
            PuckColour = puckColour;
            PuckNumber = puckNumber;
            Score = score;
            IsFoulPuck = isFoulPuck;
            X = (float)x / 1000;
            Y = (float)y / 1000;
            SpeedX = speedX;
            SpeedY = speedY;
            LeadingEdge = leadingEdge;
            TrailingEdge = trailingEdge;
            Stationary = stationary;
        }
    }
}