// Unity SDK for Qualisys Track Manager. Copyright 2015-2018 Qualisys AB
//
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;

namespace QualisysRealTime.Unity
{
    public class RTForces : MonoBehaviour
    {
        private List<ForcePlate> forcePlates;
        private RTClient rtClient;
        private bool streaming = false;

        public GameObject YAxis;
        private List<GameObject> YAxisList = new List<GameObject>();

        public float minDrawMagnitude = 0.2f;

        [Space]
        [Header("Transform and flip ")]
        public Vector3 EulerTransformPlate = new Vector3(0, -90, 0);
        public bool[] EulerInvertAxisPlate = new bool[3] { false, false, false };
        public Vector3 EulerTransformCOP = new Vector3(0, -90, 0);
        public bool[] EulerInvertAxisCOP = new bool[3] { false, false, false };
        public Vector3 EulerTransformForce = new Vector3(0, 0, 0);
        public bool[] EulerInvertAxisForce = new bool[3] { false, false, false };
        [Space]

        [Space]
        [Header("Draw Gizmo")]
        public bool drawForcePlateGizmo = false;
        public bool drawForceGizmo = false;
        //public bool drawMomentGizmo = false;
        public bool drawCOPGizmo = false;
        public Color plateColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        public float gizmoDelay = 5;

        // Use this for initialization
        void Start()
        {

            rtClient = RTClient.GetInstance();
            YAxisList.Add(YAxis);
            YAxisList[0].transform.localScale = new Vector3(0, 0, 0);
        }

        //Draw Force Plate Square
        //void OnSceneGUI(SceneView sv)
        Vector3[] plateGozmo = new Vector3[4];
        void OnDrawGizmos()
        {
            //Draw your handles here
            if (drawForcePlateGizmo && forcePlates != null)
            {
                for (int i = 0; i < forcePlates.Count; i++)
                {
                    Handles.color = plateColor;

                    for(int j =0; j < forcePlates[i].ForcePlateCorners.Length;j++)
                    {
                        Quaternion rotateMatrix = Quaternion.Euler(EulerTransformPlate);
                        Vector3 plate = forcePlates[i].ForcePlateCorners[j];
                        plate = rtClient.TransformQTMPointToUnityCoord(plate, rotateMatrix, EulerInvertAxisPlate[0], EulerInvertAxisPlate[1], EulerInvertAxisPlate[2]);
                        plate = transform.TransformPoint(plate);

                        plateGozmo[j] = transform.TransformPoint(plate);
                    }

                    Handles.DrawAAConvexPolygon(plateGozmo);
                    /*
                    Debug.DrawLine(plateGozmo[0], plateGozmo[1], Color.red);
                    Debug.DrawLine(plateGozmo[1], plateGozmo[2], Color.green);
                    Debug.DrawLine(plateGozmo[2], plateGozmo[3], Color.blue);
                    Debug.DrawLine(plateGozmo[3], plateGozmo[0], Color.cyan);
                    */
                }

            }
        }

        protected virtual void ShowForces()
        {

            try  //just to avoid error when forcePlates[i].ForceSamples[j] is null
            {
                int axisInstancesCount = 0;

                if (forcePlates != null)
                {
                    for (int i = 0; i < forcePlates.Count; i++)
                    {
                        for (int j = 0; j < forcePlates[i].ForceSamples.Length; j++)
                        {
                            if (forcePlates[i].ForceSamples[j] != null)
                            {
                                
                                Vector3 COP = forcePlates[i].ForceSamples[j].ApplicationPoint;
                                Vector3 force = forcePlates[i].ForceSamples[j].Force;
                                Vector3 plateCenter = forcePlates[i].GetPlateCenter();
                                //Vector3 moment = forcePlates[i].ForceSamples[j].Moment;

                                //Transform COP
                                Quaternion rotateMatrix = Quaternion.Euler(EulerTransformCOP);
                                COP = rtClient.TransformQTMPointToUnityCoord(COP, rotateMatrix, EulerInvertAxisCOP[0], EulerInvertAxisCOP[1], EulerInvertAxisCOP[2]);
                                COP = transform.TransformPoint(COP);

                                //Transform Force
                                rotateMatrix = Quaternion.Euler(EulerTransformForce);
                                force = rtClient.TransformQTMPointToUnityCoord(force, rotateMatrix, EulerInvertAxisForce[0], EulerInvertAxisForce[1], EulerInvertAxisForce[2]);
                                force = transform.TransformPoint(force);

                                //Transform Plates
                                rotateMatrix = Quaternion.Euler(EulerTransformPlate);
                                plateCenter = rtClient.TransformQTMPointToUnityCoord(plateCenter, rotateMatrix, EulerInvertAxisPlate[0], EulerInvertAxisPlate[1], EulerInvertAxisPlate[2]);
                                plateCenter = transform.TransformPoint(plateCenter);

                                //TODO 
                                //moment
                                if (force.magnitude > minDrawMagnitude)
                                {
                                    if (drawCOPGizmo)
                                        Debug.DrawRay(plateCenter, COP, Color.green, gizmoDelay);
                                    if (drawForceGizmo)
                                        Debug.DrawRay(plateCenter + COP, force, Color.red, gizmoDelay);
                                    //if (drawMomentGizmo)
                                    //Debug.DrawRay(root + COP, rotateMatrix * moment, Color.blue, gizmoDelay);
                                    axisInstancesCount++;
                                }

                                if ((i + j) >= YAxisList.Count)
                                {
                                    GameObject go = GameObject.Instantiate<GameObject>(YAxis);
                                    go.transform.parent = this.transform;
                                    YAxisList.Add(go);
                                }

                                if (YAxisList[i + j] && force.magnitude > minDrawMagnitude)
                                {
                                    YAxisList[i + j].transform.rotation = Quaternion.LookRotation(force) * Quaternion.Euler(90,0,0);
                                    YAxisList[i + j].transform.position = plateCenter + COP;
                                    YAxisList[i + j].transform.localScale = new Vector3(force.magnitude, force.magnitude, force.magnitude);
                                }
                                else
                                {
                                    YAxisList[i + j].transform.localScale = new Vector3(0, 0, 0);
                                }
                            }
                        }
                    }
                }
                
                //Debug.Log(axisInstancesCount +"<"+ YAxisList.Count);

                if (axisInstancesCount < YAxisList.Count)
                {
                    for (int yAxisCount = YAxisList.Count-1; yAxisCount > axisInstancesCount; yAxisCount--)
                    {
                        if (yAxisCount >1)
                        {
                            GameObject go = YAxisList[yAxisCount];
                            YAxisList.Remove(go);
                            Destroy(go);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message + " " + e.Source + " " + e.StackTrace);
            }
        }

            // Update is called once per frame
            void Update()
        {
            if (rtClient == null) rtClient = RTClient.GetInstance();

            if (rtClient.GetStreamingStatus() && !streaming)
            {
                streaming = true;
            }
            if (!rtClient.GetStreamingStatus() && streaming)
            {
                streaming = false;
            }
            
            forcePlates = rtClient.ForcePlates;

            if (forcePlates == null || forcePlates.Count == 0)
                return;

                ShowForces();
        }
    }
}
