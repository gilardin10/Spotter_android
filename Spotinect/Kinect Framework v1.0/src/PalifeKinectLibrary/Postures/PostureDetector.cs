using System;
using Microsoft.Kinect;
using System.Collections.Generic;
using Paelife.KinectFramework.Gestures;

namespace Paelife.KinectFramework.Postures
{
    public abstract class PostureDetector
    {
        //! @cond
        readonly List<Entry> entries = new List<Entry>();

        readonly int windowSize;

        protected List<Entry> Entries
        {
            get { return entries; }
        }
        //! @endcond

        /// <summary>
        /// The joint that is tracked in the current gesture detector.
        /// </summary>
        public JointType TrackedJoint { get; set; }

        public event Action<string> PostureDetected;

        readonly int accumulatorTarget;
        string previousPosture = "";
        int accumulator;
        string accumulatedPosture = "";

        public string CurrentPosture
        {
            get { return previousPosture; }
            protected set { previousPosture = value; }
        }

        protected PostureDetector(int accumulators)
        {
            accumulatorTarget = accumulators;
            MinimalPeriodBetweenPostures = 5000;
        }

        /// <summary>
        /// Gets the number of recorded positions that are considered when detecting gestures.
        /// </summary>
        public int WindowSize
        {
            get { return windowSize; }
        }

        public double MinimalPeriodBetweenPostures { get; private set; }

        public abstract void TrackPostures(Skeleton skeleton);

        public virtual void Add(Skeleton skeleton, KinectSensor sensor)
        {

            // Look for gestures
            TrackPostures(skeleton);
        }

        protected void RaisePostureDetected(string posture)
        {
            // Too close?
            if (DateTime.Now.Subtract(GeneralConfig.lastDetection).TotalMilliseconds > MinimalPeriodBetweenPostures)
            {
                if (PostureDetected != null)
                    PostureDetected(posture);

                GeneralConfig.lastDetection = DateTime.Now;
            }

            Entries.Clear();
        }

        protected void Reset()
        {
            previousPosture = "";

        }
    }
}
