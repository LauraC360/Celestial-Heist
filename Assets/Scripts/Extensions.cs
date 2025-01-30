using UnityEngine;

public static class Extensions
{
    /// <summary>
    /// Converts a vector from the local basis of one transform to another transform's local basis.
    /// </summary>
    /// <param name="vector">The vector to convert, in the local space of sourceTransform.</param>
    /// <param name="sourceTransform">The transform whose local basis the vector is currently in.</param>
    /// <param name="targetTransform">The transform to whose local basis the vector will be converted.</param>
    /// <returns>The vector in the local basis of targetTransform.</returns>
    public static Vector3 ConvertVectorBetweenBases(this Vector3 vector, Transform sourceTransform, Transform targetTransform)
    {
        Vector3 worldSpaceVector = sourceTransform.TransformDirection(vector);

        Vector3 targetLocalVector = targetTransform.InverseTransformDirection(worldSpaceVector);

        return targetLocalVector;
    }
    
    public static Vector2 RotateVector(this Vector2 v, float angleDegrees)
    {
        // Convert angle from degrees to radians
        float angleRadians = angleDegrees * Mathf.Deg2Rad;

        // Calculate the rotated vector
        float x = v.x * Mathf.Cos(angleRadians) - v.y * Mathf.Sin(angleRadians);
        float y = v.x * Mathf.Sin(angleRadians) + v.y * Mathf.Cos(angleRadians);

        return new Vector2(x, y);
    }
    
    public static float Map(float value, float fromLow, float fromHigh, float toLow, float toHigh) 
    {
        return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
    }
}