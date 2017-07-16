using System;
using System.Collections.Generic;
using Microsoft.Kinect;
using Paelife.KinectFramework.Gestures;
using SpotifyAPI.Local;
using System.Threading;

namespace Paelife.KinectFramework.Postures
{
    public class AlgorithmicPostureDetector : PostureDetector
    {
        /// <summary>
        /// 
        public int MinimalPeriodBetweenGestures { get; set; }

        private readonly SpotifyLocalAPI _spotify;

        public float Epsilon {get;set;}
        public float MaxRange { get; set; }

        public AlgorithmicPostureDetector() : base(10)
        {
            Epsilon = 0.1f;
            MaxRange = 0.1f;
            _spotify = new SpotifyLocalAPI();
        }

        public override async void TrackPostures(Skeleton skeleton)
        {
            
            if (skeleton.TrackingState != SkeletonTrackingState.Tracked)
                return;

            Vector3? headPosition = null;
            Vector3? leftHandPosition = null;
            Vector3? rightHandPosition = null;
            Vector3? leftElbowPosition = null;
            Vector3? rightElbowPosition = null;
            Vector3? leftHipPosition = null;
            Vector3? rightHipPosition = null;

            foreach (Joint joint in skeleton.Joints)
            {
                if (joint.TrackingState != JointTrackingState.Tracked)
                    continue;
                
                switch (joint.JointType)
                {
                    case JointType.Head:
                        headPosition = new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
                        break;
                    case JointType.HandLeft:
                        leftHandPosition = new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
                        break;
                    case JointType.HandRight:
                        rightHandPosition = new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
                        break;
                    case JointType.ElbowLeft:
                        leftElbowPosition = new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
                        break;
                    case JointType.ElbowRight:
                        rightElbowPosition = new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
                        break;
                    case JointType.HipLeft:
                        leftHipPosition = new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
                        break;
                    case JointType.HipRight:
                        rightHipPosition = new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
                        break;

                }
            }


            if (SpotifyLocalAPI.IsSpotifyRunning())
            {
                // Posture T
                if (CheckPostureT(leftHandPosition, leftElbowPosition, rightHandPosition, rightElbowPosition))
                {
                    await _spotify.Play();
                    RaisePostureDetected("Em reprodução");
                    return;

                }

                // Posture U
                if (CheckPostureU(leftHandPosition, leftElbowPosition, rightHandPosition, rightElbowPosition, headPosition))
                {
                    await _spotify.Pause();
                    RaisePostureDetected("Música em pausa");
                    return;
                }

                // Posture Volume
                if(CheckPostureV(leftHandPosition, leftHipPosition, rightHandPosition, headPosition))
                {
                    PostureVerify.setPosture(true);
                    PostureVerify.setVol(_spotify.GetSpotifyVolume());
                    RaisePostureDetected("Alterar volume");
                    return;
                }

            }
            Reset();
        }

        bool CheckPostureT(Vector3? leftHand, Vector3? leftElbow, Vector3? rightHand, Vector3? rightElbow)
        {
            
            if (!leftHand.HasValue || !rightHand.HasValue || !leftElbow.HasValue || !rightElbow.HasValue)
                return false;

            if (Math.Abs(leftHand.Value.Y - leftElbow.Value.Y) < MaxRange)
                if (Math.Abs(rightHand.Value.Y - rightElbow.Value.Y) < MaxRange)
                    return true;

            return false;
        }

        bool CheckPostureU(Vector3? leftHand, Vector3? leftElbow, Vector3? rightHand, Vector3? rightElbow, Vector3? head)
        {
            if (!leftHand.HasValue || !rightHand.HasValue || !leftElbow.HasValue || !rightElbow.HasValue || !head.HasValue)
                return false;

            if (Math.Abs(leftHand.Value.X - leftElbow.Value.X) < MaxRange)
                if (leftHand.Value.Y > leftElbow.Value.Y)
                    if (Math.Abs(rightHand.Value.X - rightElbow.Value.X) < MaxRange)
                        if (rightHand.Value.Y > rightElbow.Value.Y)
                            if (Math.Abs(rightElbow.Value.Z - head.Value.Z) < MaxRange)
                                if (Math.Abs(leftElbow.Value.Z - head.Value.Z) < MaxRange)
                                    return true;

            return false;
        }

        bool CheckPostureV(Vector3? leftHand, Vector3? leftHip, Vector3? rightHand, Vector3? head)
        {
            if (!leftHand.HasValue || !rightHand.HasValue || !leftHip.HasValue || !head.HasValue)
                return false;

            if (leftHand.Value.Y < leftHip.Value.Y)
                if (Math.Abs(head.Value.Y - rightHand.Value.Y) < 0.3f)
                        return true;

            return false;
        }

    }
}
