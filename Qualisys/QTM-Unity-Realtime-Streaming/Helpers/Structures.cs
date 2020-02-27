// Unity SDK for Qualisys Track Manager. Copyright 2015-2018 Qualisys AB
//
using UnityEngine;
using System.Collections.Generic;

namespace QualisysRealTime.Unity
{
    // Class for 6DOF with unity data types
    public class SixDOFBody
    {
        public SixDOFBody() { }
        public string Name;
        public Vector3 Position;
        public Quaternion Rotation;
        public Color Color;
    }

    // Class for labeled markers with unity data types
    public class LabeledMarker
    {
        public LabeledMarker() { }
        public string Name;
        public Vector3 Position;
        public Color Color;
        public float Residual;
    }

    public class UnlabeledMarker
    {
        public uint Id;
        public Vector3 Position;
        public float Residual;
    }

    // Class for user bones
    public class Bone
    {
        public Bone() { }
        public string From;
        public LabeledMarker FromMarker;
        public string To;
        public LabeledMarker ToMarker;
        public Color Color = Color.yellow;
    }

    // Class for gaze vectors
    public class GazeVector
    {
        public GazeVector() { }
        public string Name;
        public Vector3 Position;
        public Vector3 Direction;
    }

    public class AnalogChannel
    {
        public string Name;
        public float[] Values;
    }
    public class Segment
    {
        public string Name;
        public uint Id;
        public uint ParentId;
        public Vector3 Position = Vector3.zero;
        public Quaternion Rotation = Quaternion.identity;
        public Vector3 TPosition = Vector3.zero;
        public Quaternion TRotation = Quaternion.identity;
    }
    public class Skeleton
    {
        public string Name;
        public Dictionary<uint, Segment> Segments = new Dictionary<uint, Segment>();
    }

    // Class for ForcePlate unity data types
    public class ForcePlate
    {
        public string Name;
        /// <summary>ID of plate</summary>
        public int PlateId;
        /// <summary>Number of forces in frame</summary>
        public int ForceCount;
        /// <summary>Force number, increased with the force frequency</summary>
        public int ForceNumber;
        /// <summary></summary>
        public Vector3 Origin = Vector3.zero;
        /// <summary> Force Plate 4 corners in Unity coordinate </summary>
        public Vector3[] ForcePlateCorners = new Vector3[4];
        /// <summary>Samples collected from plate</summary>
        public ForceSample[] ForceSamples = new ForceSample[1];
        /// <summary>Samples collected from plate</summary>
        //public Matrix4x4 calibrationMatrix;

        public Vector3 GetPlateCenter()
        {
            Vector3 pos = Vector3.zero;
            foreach(Vector3 p in ForcePlateCorners)
            {
                pos += p;
            }
            pos /= ForcePlateCorners.Length;
            return pos;
        }
    }

    // Class for Sample ForcePlate unity data types
    public class ForceSample
    {
        public ForceSample() { }
        /// <summary>Coordinate of the force </summary>
        private Vector3 force = Vector3.zero;
        public Vector3 Force {
            get {
                if (force == null)
                    return Vector3.zero;
                else
                    return force;
            }
            set {
                force = value;
            }
        }
        /// <summary>Coordinate of the moment </summary>
        private Vector3 moment = Vector3.zero;
        public Vector3 Moment {
            get {
                if (moment == null)
                    return Vector3.zero;
                else
                    return moment;
            }
            set {
                moment = value;
            }
        }
        /// <summary>Coordinate of the force application point </summary>
        private Vector3 applicationPoint = Vector3.zero;
        public Vector3 ApplicationPoint {
            get {
                if (applicationPoint == null)
                    return Vector3.zero;
                else
                    return applicationPoint;
            }
            set {
                applicationPoint = value;
            }
        }
    }
}
