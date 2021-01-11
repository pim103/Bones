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

    private List<Point> currentPointsInScene;
    private List<GameObject> goInScene;

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
}