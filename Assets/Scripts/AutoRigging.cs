using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;
using Utils;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

public class AutoRigging
{
    private static List<Vector3> GetPointListForPart(Transform meshPart)
    {
        List<Vector3> pointsInMesh = new List<Vector3>();

        SkinnedMeshRenderer skinnedMeshRenderer;
        MeshFilter meshFilter;
        
        if ((skinnedMeshRenderer = meshPart.GetComponent<SkinnedMeshRenderer>()) != null)
        {
            pointsInMesh = skinnedMeshRenderer.sharedMesh.vertices.ToList();
        }
        else if ((meshFilter = meshPart.GetComponent<MeshFilter>()) != null)
        {
            pointsInMesh = meshFilter.sharedMesh.vertices.ToList();
        }

        return pointsInMesh;
    }

    private static void CenterPoints(List<Vector3> points, Vector3 barycenter)
    {
        for (int i = 0; i < points.Count; ++i)
        {
            points[i] = points[i] - barycenter;
        }
    }

    private static Matrix4x4 CalculCovarianceMatrix(List<Vector3> points)
    {
        List<float> allX = new List<float>();
        List<float> allY = new List<float>();
        List<float> allZ = new List<float>();

        foreach (Vector3 point in points)
        {
            allX.Add(point.x);
            allY.Add(point.y);
            allZ.Add(point.z);
        }

        Matrix4x4 matrix = Matrix4x4.zero;
        matrix.SetRow(0, new Vector4(MathRigging.Variance(allX), MathRigging.Covariance(allX, allY), MathRigging.Covariance(allX, allZ), 0));
        matrix.SetRow(1, new Vector4(MathRigging.Covariance(allY, allX), MathRigging.Variance(allY), MathRigging.Covariance(allY, allZ), 0));
        matrix.SetRow(2, new Vector4(MathRigging.Covariance(allZ, allX), MathRigging.Covariance(allZ, allY), MathRigging.Variance(allZ), 0));
        matrix.SetRow(3, new Vector4(0, 0, 0, 1));

        return matrix;
    }

    private static float GetMaxValue(Vector4 v)
    {
        float[] vectorToList = { Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z) };

        return Mathf.Max(vectorToList);
    }

    private static Vector3 GetVectorPropre(Matrix4x4 matrix)
    {
        Vector4 initialVector = new Vector4(0, 0, 1, 1);
        
        for (int i = 0; i < 100; ++i)
        {
            initialVector = matrix * initialVector;
            float lambda = GetMaxValue(initialVector);

            initialVector /= lambda;
        }

        Vector3 vectorPropre = new Vector3(initialVector.x, initialVector.y, initialVector.z);
        vectorPropre.Normalize();
        
        return vectorPropre;
    }

    private static List<Vector3> CalculProjectedPoints(List<Vector3> points, Vector3 vectorPropre)
    {
        List<Vector3> projectedPoints = new List<Vector3>();

        foreach (Vector3 point in points)
        {
            projectedPoints.Add(Vector3.Dot(point, vectorPropre) * vectorPropre);
        }

        return projectedPoints;
    }

    private static List<Vector3> CalculExtremePoints(List<Vector3> projectedPoints, Vector3 vectorPropre)
    {
        List<Vector3> extremPoints = new List<Vector3>();
        Vector3 minPoint = projectedPoints[0];
        Vector3 maxPoint = projectedPoints[0];

        foreach (Vector3 projectedPoint in projectedPoints)
        {
            float scalarProduct = Vector3.Dot(projectedPoint, vectorPropre);

            if (scalarProduct < 0 && minPoint.magnitude < projectedPoint.magnitude)
            {
                minPoint = projectedPoint;
            } else if (scalarProduct > 0 && maxPoint.magnitude < projectedPoint.magnitude)
            {
                maxPoint = projectedPoint;
            }
        }

        extremPoints.Add(minPoint);
        extremPoints.Add(maxPoint);

        return extremPoints;
    }

    private static void ReplacePoints(List<Vector3> points, Vector3 barycenter, Vector3 offset)
    {
        for (int i = 0; i < points.Count; ++i)
        {
            points[i] = points[i] + barycenter + offset;
        }
    }
    
    public static Bone CreateBoneForPart(Transform childPart, bool displayBarycenter, bool displayExtrem)
    {
        Vector3 offset = childPart.position;
        
        // Get point from mesh
        List<Vector3> pointsInMesh = GetPointListForPart(childPart);
        
        // Calcul Barycenter and display
        Vector3 barycenter = MathRigging.CalculBarycenter(pointsInMesh);

        if (displayBarycenter)
        {
            Controller.AddPoint(barycenter, Vector3.one * 10, "Barycenter");
        }

        // Center point
        CenterPoints(pointsInMesh, barycenter);

        // Matrice de covariance
        Matrix4x4 matrix = CalculCovarianceMatrix(pointsInMesh);

        // Valeur propre et vecteur associé
        Vector3 vectorPropre = GetVectorPropre(matrix);

        // Projected Points
        List<Vector3> projectedPoints = CalculProjectedPoints(pointsInMesh, vectorPropre);

        // Extreme Points
        List<Vector3> extremPoints = CalculExtremePoints(projectedPoints, vectorPropre);
        ReplacePoints(extremPoints, barycenter, offset);

        if (displayExtrem)
        {
            Controller.DisplayPointsList(extremPoints, "extremPoint-" + childPart.name);
        }

        Point p1 = new Point
        {
            position = extremPoints[0]
        };

        Point p2 = new Point
        {
            position = extremPoints[1]
        };

        return new Bone(p1, p2, childPart);
    }

    public static RiggedCharacter CreateRigging(GameObject humanGo, bool displayBarycenter, bool displayExtrem)
    {
        RiggedCharacter riggedCharacter = new RiggedCharacter();
        
        for (int i = 0; i < humanGo.transform.childCount; ++i)
        {
            Transform childPart = humanGo.transform.GetChild(i);

            if (childPart.name.ToLower().Contains("ignore"))
            {
                continue;
            }

            riggedCharacter.AddBone(CreateBoneForPart(childPart, displayBarycenter, displayExtrem));
        }

        riggedCharacter.JoinsRelatedBones();
        
        return riggedCharacter;
    }
}