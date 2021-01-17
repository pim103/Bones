using System.Collections;
using System.Collections.Generic;
using Model;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Utils;

[ExecuteInEditMode]
public class Controller : MonoBehaviour
{
    [SerializeField] private GameObject humanGo;
    [SerializeField] private bool displayBarycenter;
    [SerializeField] private bool displayExtrem;

    [SerializeField] private GameObject someTest;
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
        goInScene = new List<GameObject>();

        instance = this;
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
    
    public static GameObject DrawOneEdge(Vector3 pos1, Vector3 pos2)
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

        return seg;
    }

    public static void DeleteEdge(GameObject go)
    {
        if (goInScene.Contains(go))
        {
            goInScene.Remove(go);
        }

        go.SetActive(false);
    }

    public void GenerateBones()
    {
        RiggedCharacter riggedCharacter = AutoRigging.CreateRigging(humanGo, displayBarycenter, displayExtrem);
    }
 
    public void ClearScene()
    {
        while (goInScene.Count > 0)
        {
            goInScene[0].SetActive(false);
            goInScene.RemoveAt(0);
        }
        
        goInScene.Clear();
    }

    public void SomeTest()
    {
        Debug.Log("Nothing to test sry :/");
    }
}