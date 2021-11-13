using System.Numerics;

public static class HelperMethods
{
        public static bool ObjIsOutOfScope(Vector3 controlVec, Vector3 vec, int threshold) => IsOverThreshold(controlVec - vec, threshold); 

        private static bool IsOverThreshold(Vector3 diff, int threshold) => 
            (diff.X > threshold || diff.X < -threshold || diff.Y > threshold || diff.X < -threshold); 
}
