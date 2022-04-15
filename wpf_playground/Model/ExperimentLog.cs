using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_playground.Model
{
    public class ExperimentLog
    {
        public ExperimentLog(HistoryType historyType, int signalIndex, int buttonPositionIndex, double elapsedTime, double reactionTime, double distance, ClickState clickState, int delay, int pqPositionIndex = -1, double fDistance = -1, double fRms = -1)
        {

            HistoryType = historyType;
            SignalIndex = signalIndex;
            ButtonPositionIndex = buttonPositionIndex;
            ElapsedTime = elapsedTime;
            ReactionTime = reactionTime;
            Distance = distance;
            ClickState = clickState;
            Delay = delay;

            PQPositionIndex = pqPositionIndex;

            FDistance = fDistance;
            FRms = fRms;
        }

        /// <summary>
        /// Determine the type of this log PQ/SIGNAL/ CLICK
        /// </summary>
        public HistoryType HistoryType { get; private set; }

        /// <summary>
        /// Index of the signal
        /// 0 for top left
        /// 1 for top right
        /// 2 for bottom left
        /// 3 for bottom right
        /// </summary>
        public int SignalIndex { get; private set; }

        /// <summary>
        /// Index of the pressed button
        /// 0 for top left
        /// 1 for top right
        /// 2 for bottom left
        /// 3 for bottom right
        /// </summary>
        public int ButtonPositionIndex { get; set; }


        /// <summary>
        /// Index of displayed PQ
        /// 0 for left
        /// 1 for right
        /// </summary>
        public int PQPositionIndex { get; set; }

        /// <summary>
        /// Total Elapsed time of a single experiment
        /// </summary>
        public double ElapsedTime { get; private set; }

        /// <summary>
        /// Reaction time between the signal display and user click
        /// </summary>
        public double ReactionTime { get; private set; }

        /// <summary>
        /// The distance between the cursor and the bouncing ball
        /// </summary>
        public double Distance { get; private set; }

        /// <summary>
        /// Determine the click is hit/miss/wrong
        /// </summary>
        public ClickState ClickState { get; set; }

        /// <summary>
        /// Random 1-4s
        /// </summary>
        public int Delay { get; set; }

        /// <summary>
        /// (width1 - width2) ^2
        /// </summary>
        public double FDistance { get; set; }

        /// <summary>
        /// Average amount of deviation between the cursor and the following target
        /// </summary>
        public double FRms { get; set; }
    }
}
