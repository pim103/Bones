using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

public class AutoRigging
{
    private List<Vector3> GetPointListForPart(Transform meshPart)
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

    private void CenterPoints(List<Vector3> points, Vector3 barycenter)
    {
        for (int i = 0; i < points.Count; ++i)
        {
            points[i] = points[i] - barycenter;
        }
    }

    private Matrix4x4 CalculCovarianceMatrix(List<Vector3> points)
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

    private float GetMaxValue(Vector4 v)
    {
        float[] vectorToList = { Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z) };

        return Mathf.Max(vectorToList);
    }

    private Vector3 GetVectorPropre(Matrix4x4 matrix)
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

    private List<Vector3> CalculProjectedPoints(List<Vector3> points, Vector3 vectorPropre)
    {
        List<Vector3> projectedPoints = new List<Vector3>();

        foreach (Vector3 point in points)
        {
            projectedPoints.Add(Vector3.Dot(point, vectorPropre) * vectorPropre);
        }

        return projectedPoints;
    }

    private List<Vector3> CalculExtremePoints(List<Vector3> projectedPoints, Vector3 vectorPropre)
    {
        List<Vector3> extremPoints = new List<Vector3>();
        Vector3 minPoint = projectedPoints[0];
        Vector3 maxPoint = projectedPoints[0];

        foreach (Vector3 projectedPoint in projectedPoints)
        {
            float alpha = Vector3.Dot(projectedPoint, vectorPropre);

            if (alpha < 0 && minPoint.magnitude < projectedPoint.magnitude)
            {
                minPoint = projectedPoint;
            } else if (alpha > 0 && maxPoint.magnitude < projectedPoint.magnitude)
            {
                maxPoint = projectedPoint;
            }
        }

        extremPoints.Add(minPoint);
        extremPoints.Add(maxPoint);

        return extremPoints;
    }

    private void ReplacePoints(List<Vector3> points, Vector3 barycenter)
    {
        for (int i = 0; i < points.Count; ++i)
        {
            points[i] = points[i] + barycenter;
        }
    }
    
    public void ComputeAutorigging(Transform childPart, bool displayBarycenter, bool displayExtrem, bool displayProjectedPoints)
    {
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
        ReplacePoints(extremPoints, barycenter);

        if (displayExtrem)
        {
            Controller.DisplayPointsList(extremPoints, "extremPoint");
        }

        if (displayProjectedPoints)
        {
            ReplacePoints(projectedPoints, barycenter);
            Controller.DisplayPointsList(projectedPoints, "projectedPoint");
        }
        else
        {
            Controller.DrawOneEdge(extremPoints[0], extremPoints[1]);
        }
    }
}