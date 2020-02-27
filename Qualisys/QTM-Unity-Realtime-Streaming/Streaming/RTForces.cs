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

        public GameObject YAxisGO;
        private List<GameObject> YAxisGOList = new List<GameObject>();

        public bool drawForcePlateGizmo = false;
        public bool drawForceGizmo = false;
        public bool drawMomentGizmo = false;
        public bool drawCOPGizmo = false;
        public Color plateColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        public float gizmoDelay = 5;
        public float forceScale = 1.5f;//same Scale as QTM

        // Use this for initialization
        void Start()
        {
            rtClient = RTClient.GetInstance();

            YAxisGOList.Add(YAxisGO);
            YAxisGOList[0].transform.localScale = new Vector3(0, 0, 0);
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
                    //Vector3 pos = transform.position;
                    //Quaternion rot = transform.rotation;
                    //Handles.TransformHandle(ref pos ,ref rot);

                    for(int j =0; j < forcePlates[i].ForcePlateCorners.Length;j++)
                    {
                        plateGozmo[j] = transform.TransformPoint( forcePlates[i].ForcePlateCorners[j]);
                    }

                    Handles.DrawAAConvexPolygon(plateGozmo);
                }
            }
        }

        protected virtual void ShowForces()
        {
            try  //just to avoid error when forcePlates[i].ForceSamples[j] is null
            {
                if (forcePlates != null)
                {
                    for (int i = 0; i < forcePlates.Count; i++)
                    {
                        for (int j = 0; j < forcePlates[i].ForceSamples.Length; j++)
                        {
                            if (forcePlates[i].ForceSamples[j] != null)
                            {
                                Vector3 force = Vector3.zero;
                                force = (transform.TransformPoint(forcePlates[i].ForceSamples[j].Force) - transform.position) * forceScale;

                                Vector3 moment = Vector3.zero;
                                moment = transform.TransformPoint(forcePlates[i].ForceSamples[j].Moment);

                                Vector3 COP = Vector3.zero;
                                COP = transform.TransformPoint(forcePlates[i].ForceSamples[j].ApplicationPoint) - transform.position;

                                Vector3 root = transform.TransformPoint(forcePlates[i].GetPlateCenter());

                                //rotate for 
                                Quaternion rotateMatrix = QuaternionHelper.RotationX(Mathf.Deg2Rad * -90f);

                                if (drawForceGizmo)
                                    Debug.DrawRay(root + COP, rotateMatrix * force, Color.red, gizmoDelay);
                                if (drawMomentGizmo)
                                    Debug.DrawRay(root + COP, rotateMatrix * moment, Color.blue, gizmoDelay);
                                if (drawCOPGizmo)
                                    Debug.DrawRay(root, COP, Color.green, 10);

                                if ((i + j) >= YAxisGOList.Count)
                                {
                                    GameObject go = GameObject.Instantiate<GameObject>(YAxisGO);
                                    go.transform.parent = this.transform;
                                    YAxisGOList.Add(go);
                                }

                                if (YAxisGOList[i + j] && force.magnitude > 0.05f)
                                {
                                    YAxisGOList[i + j].transform.rotation = Quaternion.LookRotation(force);
                                    YAxisGOList[i + j].transform.position = root + COP;
                                    YAxisGOList[i + j].transform.localScale = new Vector3(force.magnitude, force.magnitude, force.magnitude);
                                }
                                else
                                {
                                    YAxisGOList[i + j].transform.localScale = new Vector3(0, 0, 0);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message+ " "+e.Source+" "+e.StackTrace);
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
