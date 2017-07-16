using System;
using Microsoft.Kinect;
using SpotifyAPI.Local;
using System.Threading;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;

namespace Paelife.KinectFramework.Gestures
{
    /// <summary>
    /// An implementation of a <see cref="GestureDetector"/>
    /// that detects swipes motions.
    /// </summary>
    public class SwipeGestureDetector : GestureDetector
    {
        /// <summary>
        /// 
        String id = "";
        private SpotifyWebAPI _spotifyWeb;
        private readonly SpotifyLocalAPI _spotify;
        /// The minimal lenght the swipe gesture should have.
        /// </summary>
        public float SwipeMinimalLength {get;set;}

        /// <summary>
        /// The maximal lenght the swipe gesture should have.
        /// </summary>
        public float SwipeMaximalHeight {get;set;}

        /// <summary>
        /// The minimal duration the swipe gesture should have.
        /// </summary>
        public int SwipeMininalDuration {get;set;}

        /// <summary>
        /// The maximal duration the swipe gesture should have.
        /// </summary>
        public int SwipeMaximalDuration {get;set;}


        /// <summary>
        /// Default constructor that receives the joint that should be tracked and the
        /// number of recorded positions that are considered when detecting gestures.
        /// </summary>
        public SwipeGestureDetector(JointType trackedJoint, int windowSize = 20)
            : base(trackedJoint, windowSize)
        {
            _spotify = new SpotifyLocalAPI();
            SwipeMinimalLength = 0.5f;
            SwipeMaximalHeight = 0.3f;
            SwipeMininalDuration = 400;
            SwipeMaximalDuration = 1500;
        }

        //! @cond
        protected bool ScanPositions(Func<Vector3, Vector3, bool> heightFunction, Func<Vector3, Vector3, bool> directionFunction, 
            Func<Vector3, Vector3, bool> lengthFunction, int minTime, int maxTime)
        {
            int start = 0;

            for (int index = 1; index < Entries.Count - 1; index++)
            {
                if (!heightFunction(Entries[0].Position, Entries[index].Position) || !directionFunction(Entries[index].Position, Entries[index + 1].Position))
                {
                    start = index;
                }

                if (lengthFunction(Entries[index].Position, Entries[start].Position))
                {
                    double totalMilliseconds = (Entries[index].Time - Entries[start].Time).TotalMilliseconds;
                    if (totalMilliseconds >= minTime && totalMilliseconds <= maxTime)
                    {
                        return true;

                    }
                }
            }

            return false;
        }
        //! @endcond

        /// <summary>
        /// The implementation of the method that will actualy look for swipes.
        /// </summary>
        protected override void LookForGesture()
        {

            if (SpotifyLocalAPI.IsSpotifyRunning())
            {

                // Swipe to right
                if (ScanPositions((p1, p2) => Math.Abs(p2.Y - p1.Y) < SwipeMaximalHeight, // Height
                    (p1, p2) => p2.X - p1.X > -0.01f, // Progression to right
                    (p1, p2) => Math.Abs(p2.X - p1.X) > SwipeMinimalLength, // Length
                    SwipeMininalDuration, SwipeMaximalDuration)) // Duration
                {
                    _spotify.Skip();
                    RaiseGestureDetected("Música seguinte");
                    return;
                }

                // Swipe to left
                if (ScanPositions((p1, p2) => Math.Abs(p2.Y - p1.Y) < SwipeMaximalHeight,  // Height
                    (p1, p2) => p2.X - p1.X < 0.01f, // Progression to right
                    (p1, p2) => Math.Abs(p2.X - p1.X) > SwipeMinimalLength, // Length
                    SwipeMininalDuration, SwipeMaximalDuration))// Duration
                {
                    _spotify.Previous();
                    RaiseGestureDetected("Música anterior");
                    return;
                }
            }
 
        }
    }
}