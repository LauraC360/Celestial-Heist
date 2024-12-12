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
}