using System.Collections;
using System.Collections.Generic;
using Model;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Utils;

[ExecuteInEditMode]
public class Controller : MonoBehaviour
{
    [SerializeField] private int nbPoints = 100;

    [SerializeField] private GameObject humanGo;
    [SerializeField] private bool displayBarycenter;
    [SerializeField] private bool displayExtrem;
    [SerializeField] private bool displayProjectedPoints;

    private AutoRigging autoRigging;
    private static List<Point> currentPointsInScene;
    private static List<GameObject> goInScene;

    private static Controller instance;

    private void Update()
    {
        if (instance == null)
        {
            Init();
        }
    }
    
    void Init()
    {
        currentPointsInScene = new List<Point>();
        goInScene = new List<GameObject>();

        instance = this;
        autoRigging = new AutoRigging();
    }

    public void GeneratePointCloud()
    {
        Debug.Log(nbPoints);
        for (int i = 0; i < nbPoints; ++i)
        {
            GameObject ob = ObjectPooler.SharedInstance.GetPooledObject(0);

            Vector3 pos = new Vector3();
            pos.x = Random.Range(-50, 50);
            pos.y = Random.Range(-50, 50);
            pos.z = Random.Range(-50, 50);

            if (currentPointsInScene.Exists(p => p.position == pos))
            {
                --i;
                continue;
            }

            currentPointsInScene.Add(new Point{ position = pos });

            ob.transform.position = pos;
            ob.SetActive(true);
            goInScene.Add(ob);
        }
    }

    public static void DisplayPointsList(List<Vector3> points, string nameOfObject = "Point")
    {
        foreach (Vector3 point in points)
        {
            AddPoint(point, Vector3.one * 5, nameOfObject);
        }
    }

    public static void AddPoint(Vector3 pos, Vector3 scale, string nameOfObject = "Point")
    {
        GameObject ob = ObjectPooler.SharedInstance.GetPooledObject(0);
        ob.transform.position = pos;
        ob.transform.localScale = scale;

        ob.name = nameOfObject;
        
        ob.SetActive(true);
        goInScene.Add(ob);
    }
    
    public static void DrawOneEdge(Vector3 pos1, Vector3 pos2)
    {
        GameObject seg = ObjectPooler.SharedInstance.GetPooledObject(1);

        Vector3 pos = (pos1 + pos2)/2;

        if (pos1 - pos2 != Vector3.zero)
        {
            seg.transform.rotation = Quaternion.LookRotation(pos1 - pos2, Vector3.up);
            seg.transform.Rotate(Vector3.right * 90);
        }

        Vector3 localScale = Vector3.one;
        localScale.y = Vector3.Distance(pos1, pos2) / 2;
        seg.transform.localScale = localScale;

        seg.transform.position = pos;

        seg.SetActive(true);
        goInScene.Add(seg);
    }

    public void GenerateBones()
    {
        for (int i = 0; i < humanGo.transform.childCount; ++i)
        {
            autoRigging.ComputeAutorigging(humanGo.transform.GetChild(i), displayBarycenter, displayExtrem, displayProjectedPoints);
        }
    }
 
    public void ClearScene()
    {
        while (goInScene.Count > 0)
        {
            goInScene[0].SetActive(false);
            goInScene.RemoveAt(0);
        }
        
        currentPointsInScene.Clear();
        goInScene.Clear();
    }

    public void SomeTest()
    {
        List<float> data1 = new List<float>{ 3, 7, 28, 14, 35};
        List<float> data2 = new List<float>{ -1, 3, 15, 7, 77};
        
        Debug.Log(MathRigging.Covariance(data1, data2));
    }
}