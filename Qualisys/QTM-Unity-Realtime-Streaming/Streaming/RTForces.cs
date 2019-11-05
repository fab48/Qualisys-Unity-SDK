// Unity SDK for Qualisys Track Manager. Copyright 2015-2018 Qualisys AB
//
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

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

        void OnEnable()
        {
            SceneView.onSceneGUIDelegate += OnSceneGUI;
        }

        void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
        }

        //Draw Force Plate Square
        void OnSceneGUI(SceneView sv)
        {
            //Draw your handles here
            if (drawForcePlateGizmo && forcePlates != null)
            {
                for (int i = 0; i < forcePlates.Count; i++)
                {
                    Handles.color = plateColor;
                    Handles.DrawAAConvexPolygon(forcePlates[i].ForcePlateCorners);
                }
            }
        }

        protected virtual void ShowForces()
        {
            if (forcePlates != null)
            {
                for (int i = 0; i < forcePlates.Count; i++)
                {
                    if (forcePlates[i].ForceSamples != null)
                    {
                        for (int j = 0; j < forcePlates[i].ForceSamples.Length; j++)
                        {
                            //just to avoid error when ApplicationPoint is NaN
                            if (forcePlates[i].ForceSamples[j] != null)
                            {

                                //just to avoid error when ApplicationPoint is NaN                            
                                if (forcePlates!=null 
                                    && forcePlates[i]!=null 
                                    && forcePlates[i].ForceSamples[j]!=null 
                                    && !float.IsNaN(forcePlates[i].ForceSamples[j].Force.sqrMagnitude)
                                    && !float.IsNaN(forcePlates[i].ForceSamples[j].ApplicationPoint.sqrMagnitude)
                                    && !float.IsNaN(forcePlates[i].ForceSamples[j].Moment.sqrMagnitude))
                                {
                                    Vector3 root = forcePlates[i].GetPlateCenter();

                                    Vector3 force = forcePlates[i].ForceSamples[j].Force * forceScale;
                                    Vector3 moment = forcePlates[i].ForceSamples[j].Moment;
                                    Vector3 COP = forcePlates[i].ForceSamples[j].ApplicationPoint;

                                    //rotate for 
                                    Quaternion rotateMatrix = QuaternionHelper.RotationX(Mathf.Deg2Rad * -90f);

                                    if (drawForceGizmo)
                                        Debug.DrawRay(root + COP, rotateMatrix * force, Color.red, gizmoDelay);
                                    if(drawMomentGizmo)
                                        Debug.DrawRay(root + COP, rotateMatrix * moment, Color.blue, gizmoDelay);
                                    if(drawCOPGizmo)
                                        Debug.DrawRay(root, COP, Color.green, 10);

                                    if ((i+j) >= YAxisGOList.Count)
                                    {
                                        GameObject go = GameObject.Instantiate<GameObject>(YAxisGO);
                                        go.transform.parent = this.transform;
                                        YAxisGOList.Add(go);
                                    }

                                    if (YAxisGOList[i+j] && force.magnitude > 0.05f)
                                    {
                                        YAxisGOList[i+j].transform.rotation = Quaternion.LookRotation(force);
                                        YAxisGOList[i+j].transform.position = root + COP;
                                        YAxisGOList[i+j].transform.localScale = new Vector3(force.magnitude, force.magnitude, force.magnitude);
                                    }
                                    else
                                    {
                                        YAxisGOList[i+j].transform.localScale = new Vector3(0, 0, 0);
                                    }
                                }
                            }
                        }
                    }
                }
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
