using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineAreaCalculator : MonoBehaviour
{
    [SerializeField]
    public List<Vector2> vertices;
    [SerializeField]
    public List<uint>    indices;
    // Start is called before the first frame update
    public float CalulateArea(){
        if (indices == null){
            return 0.0f;
        }
        if (indices.Count< 3){
            return 0.0f;
        }
        var area = 0.0f;
        for (var i = 0;i<indices.Count-1;++i)
        {
            var triAreaWithSign = Vector3.Cross(new Vector3(vertices[(int)indices[i]][0],vertices[(int)indices[i]][1],0.0f),new Vector3(vertices[(int)indices[i+1]][0],vertices[(int)indices[i+1]][1],0.0f))[2];
            area+=triAreaWithSign;
        }
        return Mathf.Abs(area)/2.0f;
    } 
    void Start()
    {}

    // Update is called once per frame
    void Update()
    {
        
    }
}
