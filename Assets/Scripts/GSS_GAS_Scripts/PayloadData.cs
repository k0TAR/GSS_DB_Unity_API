using System;
using UnityEngine;

namespace GssDbManageWrapper
{
    [Serializable]
    public class PayloadData
    {
        public string userName;
        public string message;

        public override string ToString()
        {
            return $"userName : {this.userName}, message : {this.message}";
        }

        public MessageJson ExtractMessageJson()
        {
            return JsonUtility.FromJson<MessageJson>(this.message);
        }
    }

    [Serializable]
    public class MessageJson
    {
        public bool isClosed;
        public int areaId;
        public int vertexId;
        public Vector3 position;

        public MessageJson(bool isClosed, int areaId, int vertexId, Vector3 position)
        {
            this.isClosed = isClosed;
            this.areaId = areaId;
            this.vertexId = vertexId;
            this.position = position;
        }

        public override string ToString()
        {
            return $"isClosed={isClosed}, areaId={this.areaId}, vertexId={this.vertexId}, position={this.position}";
        }
    }
}