using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paelife.KinectFramework.Postures
{
    public static class PostureVerify
    {
        public static bool posture = false;

        public static float lastVol = 0.0f;

        public static void setPosture(bool val)
        {
            posture = val;
        }

        public static bool getPosture()
        {
            return posture;
        }

        public static void setVol(float val)
        {
           lastVol = val;
        }

        public static float getVol()
        {
            return lastVol;
        }


    }
}
