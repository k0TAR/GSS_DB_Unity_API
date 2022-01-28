using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCreateScript : MonoBehaviour
{

    Vector3[] vertices; //頂点
    int[] triangles;    //index
    int nPoly; //頂点数
    float max;

    // Start is called before the first frame update
    void Start()
    {
        max = 6;
        nPoly = 4;
        makeParams(new float[] { 5, 5, 5, 5 });
       setParams(GameObject.CreatePrimitive(PrimitiveType.Quad), new Color(0, 1, 0, 0.5f), 0);

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(verList[0]);
    }


    void makeParams(float[] values)
    {
        List<Vector3> vertList = new List<Vector3>();
        List<int> triList = new List<int>();
        vertList.Add(new Vector3(0, 0, 0));  //原点
        for (int n = 0; n < nPoly; n++)
        {
            float _x = values[n] / max * Mathf.Cos(n * 2 * Mathf.PI / nPoly);
            float _y = values[n] / max * Mathf.Sin(n * 2 * Mathf.PI / nPoly);
            vertList.Add(new Vector3(_x, _y));
            if (n != nPoly - 1)
            {
                triList.Add(0); triList.Add(n + 2); triList.Add(n + 1);
            }
            else
            {//last
                triList.Add(0); triList.Add(1); triList.Add(n + 1);
            }
        }
        vertices = vertList.ToArray();
        triangles = triList.ToArray();
        Debug.Log(vertList[0]);
        Debug.Log(vertList[1]);
        Debug.Log(vertList[2]);
        Debug.Log(vertList[3]);
        Debug.Log(vertList[4]);
 
    }

    //GameObjectにメッシュを適用
    void setParams(GameObject t, Color color, int order)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        t.GetComponent<MeshFilter>().sharedMesh = mesh;
        t.GetComponent<MeshCollider>().sharedMesh = mesh;
        t.GetComponent<MeshRenderer>().material.shader = Shader.Find("Sprites/Default");
        t.GetComponent<MeshRenderer>().material.color = color;
        t.transform.localScale = new Vector3(2.8f, 2.8f, 1);
        t.transform.position = new Vector3(0, 2f, order);
        t.transform.rotation = Quaternion.Euler(0, 0, 90);
    }
}
