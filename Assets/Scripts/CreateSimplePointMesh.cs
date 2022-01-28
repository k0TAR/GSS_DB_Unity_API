using UnityEngine;

public class CreateSimplePointMesh : MonoBehaviour
{
    void Start()
    {
        int numPoints = 60000; // 点の個数
        float r = 1.0f; // 半径

        Mesh meshSurface = CreateSimpleSurfacePointMesh(numPoints, r);
        GetComponent<MeshFilter>().mesh = meshSurface;
    }

    /// <summary>
    /// 球の表面にランダムに点を生成
    /// </summary>
    /// <param name="numPoints">点の数</param>
    /// <param name="radius">球の半径</param>
    /// <returns></returns>
    Mesh CreateSimpleSurfacePointMesh(int numPoints, float radius)
    {
        Vector3[] points = new Vector3[numPoints];
        int[] indecies = new int[numPoints];

        for(int i = 0; i < numPoints; i++)
        {
            float z = Random.Range(-1.0f, 1.0f);
            float th = Mathf.Deg2Rad * Random.Range(0.0f, 360.0f);
            float x = Mathf.Sqrt(1.0f - z * z) * Mathf.Cos(th);
            float y = Mathf.Sqrt(1.0f - z * z) * Mathf.Sin(th);

            points[i] = new Vector3(x, y, z) * radius; // 頂点座標
            indecies[i] = i; // 配列番号をそのままインデックス番号に流用
        }

        Mesh mesh = new Mesh
        {
            vertices = points
        };
        mesh.SetIndices(indecies, MeshTopology.Points, 0); // 1頂点が1インデックスの関係

        return mesh;
    }
}