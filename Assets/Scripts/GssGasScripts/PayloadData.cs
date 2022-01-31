using System;
using UnityEngine;

namespace GssDbManageWrapper
{
    [Serializable]
    public class PayloadData
    {
        public string userName;
        public string data;

        public override string ToString()
        {
            return $"{nameof(this.userName)} : {this.userName}, {nameof(this.data)} : {this.data}";
        }

        public T ExtractData<T>()
        {
            return JsonUtility.FromJson<T>(this.data);
        }
    }

    [Serializable]
    public class SamplePayLoadDataStructure
    {
        public int areaId;
        public int vertexId;
        public Vector3 position;

        public SamplePayLoadDataStructure(bool isClosed, int areaId, int vertexId, Vector3 position)
        {
            this.areaId = areaId;
            this.vertexId = vertexId;
            this.position = position;
        }

        public override string ToString()
        {
            return $"{nameof(this.areaId)}={this.areaId}," +
                $" {nameof(this.vertexId)}={this.vertexId}," +
                $" {nameof(this.position)}={this.position}";
        }
    }
}