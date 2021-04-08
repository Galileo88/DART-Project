using System.IO;
using System;
using UnityEngine;
using System.Linq;
using System.Security.Principal;
using System.Collections.Generic;
using Contracts.Predicates;
using Smooth.Collections;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.SceneManagement;
using PQSModExpansion;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine.Rendering;
using System.Reflection;
using System.Threading;

[assembly: KSPAssembly("ParallaxDART", 1, 0)]
[assembly: KSPAssemblyDependency("ParallaxSubdivisionModDART", 1, 0)]
namespace ParallaxShader
{
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class ParallaxLoader : MonoBehaviour
    {
        public static Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();
        public void Awake()
        {
            string filePath = Path.Combine(KSPUtil.ApplicationRootPath + "GameData/" + "DART/Shaders/ParallaxDART");
            if (Application.platform == RuntimePlatform.LinuxPlayer || (Application.platform == RuntimePlatform.WindowsPlayer && SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL")))
            {
                filePath = (filePath + "-linux.unity3d");
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                filePath = (filePath + "-windows.unity3d");
            }
            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                filePath = (filePath + "-macosx.unity3d");
            }
            var assetBundle = AssetBundle.LoadFromFile(filePath);
            Debug.Log("Loaded bundle");
            if (assetBundle == null)
            {
                Debug.Log("Failed to load bundle at");
                Debug.Log("Path: " + filePath);
            }
            else
            {
                Shader[] theseShaders = assetBundle.LoadAllAssets<Shader>();
                Debug.Log("Loaded all shaders");
                foreach (Shader thisShader in theseShaders)
                {
                    shaders.Add(thisShader.name, thisShader);
                    Debug.Log("Loaded shader: " + thisShader.name);
                }



            }
            filePath = Path.Combine(KSPUtil.ApplicationRootPath + "GameData/" + "DART/Shaders/Wireframe");
            
            if (Application.platform == RuntimePlatform.WindowsPlayer && SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL"))
            {
                filePath = (filePath + "-linux.unity3d");
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                filePath = (filePath + "-windows.unity3d");
            }
            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                filePath = (filePath + "-macosx.unity3d");
            }
            var assetBundle3 = AssetBundle.LoadFromFile(filePath);
            Debug.Log("Loaded bundle");
            if (assetBundle3 == null)
            {
                Debug.Log("Failed to load bundle");
                Debug.Log(filePath);
            }
            else
            {
                Shader[] theseShaders = assetBundle3.LoadAllAssets<Shader>();
                Debug.Log("Loaded all shaders");
                foreach (Shader thisShader in theseShaders)
                {
                    shaders.Add(thisShader.name, thisShader);
                    Debug.Log("Loaded shader: " + thisShader.name);
                }
            
            
            
            }
            filePath = Path.Combine(KSPUtil.ApplicationRootPath + "GameData/" + "DART/Shaders/ParallaxDARTScaled");

            if (Application.platform == RuntimePlatform.WindowsPlayer && SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL"))
            {
                filePath = (filePath + "-linux.unity3d");
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                filePath = (filePath + "-windows.unity3d");
            }
            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                filePath = (filePath + "-macosx.unity3d");
            }
            var assetBundle4 = AssetBundle.LoadFromFile(filePath);
            Debug.Log("Loaded bundle");
            if (assetBundle4 == null)
            {
                Debug.Log("Failed to load bundle");
                Debug.Log(filePath);
            }
            else
            {
                Shader[] theseShaders = assetBundle4.LoadAllAssets<Shader>();
                Debug.Log("Loaded all shaders");
                foreach (Shader thisShader in theseShaders)
                {
                    shaders.Add(thisShader.name, thisShader);
                    Debug.Log("Loaded shader: " + thisShader.name);
                }



            }
            filePath = Path.Combine(KSPUtil.ApplicationRootPath + "GameData/" + "DART/Shaders/ParallaxDARTAsteroid");

            if (Application.platform == RuntimePlatform.WindowsPlayer && SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL"))
            {
                filePath = (filePath + "-linux.unity3d");
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                filePath = (filePath + "-windows.unity3d");
            }
            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                filePath = (filePath + "-macosx.unity3d");
            }
            var assetBundle5 = AssetBundle.LoadFromFile(filePath);
            Debug.Log("Loaded bundle");
            if (assetBundle5 == null)
            {
                Debug.Log("Failed to load bundle");
                Debug.Log(filePath);
            }
            else
            {
                Shader[] theseShaders = assetBundle5.LoadAllAssets<Shader>();
                Debug.Log("Loaded all shaders");
                foreach (Shader thisShader in theseShaders)
                {
                    shaders.Add(thisShader.name, thisShader);
                    Debug.Log("Loaded shader: " + thisShader.name);
                }



            }
        }
        public static Shader GetShader  (string name)
        {
            Debug.Log("Returning shader: " + shaders[name] + " // " + name + " // "+ shaders[name].name);
            return shaders[name];
        }
    }

    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class ParallaxSettings : MonoBehaviour   //Stuff here is used after the parallax bodies have been assigned - these are global settings
    {
        //Internal / constant global values
        public static float tessellationEdgeLength = 25;
        public static float tessellationRange = 99;
        public static float tessellationMax = 64;
        public static int refreshRate = 30;
        public static ReflectionProbeRefreshMode refreshMode = ReflectionProbeRefreshMode.ViaScripting;
        public static ReflectionProbeTimeSlicingMode timeMode = ReflectionProbeTimeSlicingMode.IndividualFaces;
        public static float reflectionResolution = 512;
        public static bool useReflections = true;
        public static bool tessellate = true;
        public static bool tessellateLighting = true;
        public static bool tessellateShadows = true;
        public static string trueLighting = "false";
        public static float tessMult = 1;
        public static bool collide = true;
        public static bool flatMinmus = false;
        public void Start()
        {
            AssignVariables();
            LogVariables();
        }
        public void AssignVariables()
        {
            UrlDir.UrlConfig[] nodeArray = GameDatabase.Instance.GetConfigs("ParallaxDARTGlobalConfig");
            if (nodeArray == null)
            {
                Debug.Log("[ParallaxDARTGlobalConfig] Exception: No global config detected! Using default values");
                return;
            }
            if (nodeArray.Length > 1)
            {
                Debug.Log("[ParallaxDARTGlobalConfig] Exception: Multiple global configs detected!");
            }
            UrlDir.UrlConfig config = nodeArray[0];
            ConfigNode tessellationSettings = config.config.nodes.GetNode("TessellationSettings");
            ConfigNode reflectionSettings = config.config.nodes.GetNode("ReflectionSettings");
            ConfigNode lightingSettings = config.config.nodes.GetNode("LightingSettings");
            ConfigNode collisionSettings = config.config.nodes.GetNode("CollisionSettings");
            tessellationEdgeLength = float.Parse(tessellationSettings.GetValue("edgeLength"));
            tessellationRange = float.Parse(tessellationSettings.GetValue("range"));
            tessellationMax = float.Parse(tessellationSettings.GetValue("maxTessellation"));

            string refreshModeString = reflectionSettings.GetValue("refreshRate").ToLower();
            if (refreshModeString == "instantly")
            {
                refreshMode = ReflectionProbeRefreshMode.EveryFrame;
            }
            else if (int.Parse(refreshModeString) < Screen.currentResolution.refreshRate)    //Don't let it be updated any faster than screen refresh rate
            {
                refreshMode = ReflectionProbeRefreshMode.ViaScripting;
                refreshRate = int.Parse(refreshModeString);
            }
            string timeModeString = reflectionSettings.GetValue("timeSlicing").ToLower();
            if (timeModeString == "instantly")
            {
                timeMode = ReflectionProbeTimeSlicingMode.NoTimeSlicing;
                refreshRate = Screen.currentResolution.refreshRate;
            }
            else if (timeModeString == "allfacesatonce")
            {
                timeMode = ReflectionProbeTimeSlicingMode.AllFacesAtOnce;
            }
            else if (timeModeString == "individualfaces")
            {
                timeMode = ReflectionProbeTimeSlicingMode.IndividualFaces;
            }
            else
            {
                Debug.Log("[ParallaxGlobalConfig] Exception: Unable to detect refresh setting: timeSlicing");
            }
            reflectionResolution = int.Parse(reflectionSettings.GetValue("resolution"));

            string reflectionsString = reflectionSettings.GetValue("reflections").ToLower();
            if (reflectionsString == "true")
            {
                useReflections = true;
            }
            else
            {
                useReflections = false;
            }

            string tessellateLightingString = lightingSettings.GetValue("tessellateLighting").ToLower();
            string tessellateShadowsString = lightingSettings.GetValue("tessellateShadows").ToLower();
            string tessellateString = lightingSettings.GetValue("tessellate").ToLower();

            if (tessellateLightingString == "true")
            {
                tessellateLighting = true;
            }
            else
            {
                tessellateShadows = false;
            }
            if (tessellateShadowsString == "true")
            {
                tessellateShadows = true;
            }
            else
            {
                tessellateLighting = false;
            }
            if (tessellateString == "true")
            {
                tessellate = true;
            }
            else
            {
                tessellate = false;
            }
            string trueLightingString = lightingSettings.GetValue("trueLighting").ToLower();
            if (trueLightingString == "true")
            {
                trueLighting = "true";
            }
            else if (trueLightingString == "false")
            {
                trueLighting = "false";
            }
            else //Use adaptive true lighting
            {
                trueLighting = "adaptive";
            }
            string tessQualityString = tessellationSettings.GetValue("tessellationQuality").ToLower();
            if (tessQualityString == "low")
            {
                tessMult = 0.75f;
            }
            if (tessQualityString == "normal")
            {
                tessMult = 1f;
            }
            if (tessQualityString == "high")
            {
                tessMult = 1.25f;
            }
            if (tessQualityString == "higher")
            {
                tessMult = 1.5f;
            }
            string collisionString = collisionSettings.GetValue("collide").ToLower();
            string flatMinmusString = collisionSettings.GetValue("flatMinmus").ToLower();
            if (collisionString == "false")
            {
                collide = false;
            }
            else
            {
                collide = true;
            }
            if (flatMinmusString == "false")
            {
                flatMinmus = false;
            }
            else
            {
                flatMinmus = true;
            }
        }
        public void LogVariables()
        {
            Log("Tessellation Edge Length = " + tessellationEdgeLength);
            Log("Tessellation Range = " + tessellationRange);
            Log("Maximum Tessellation = " + tessellationMax);
            Log("Reflection Resolution = " + reflectionResolution);
            Log("Reflection Refresh Rate = " + refreshRate);
            Log("Reflection Mode = " + refreshMode);
            Log("Reflection Time Slicing = " + timeMode);
            Log("Reflections = " + useReflections);
            Log("TessLighting = " + tessellateLighting);
            Log("TessShadows = " + tessellateShadows);
            Log("Tess = " + tessellate);
            Log("TessMult = " + tessMult);
            Log("True Lighting = " + trueLighting);
            Log("Collide = " + collide);
            Log("Flat Minmus = " + flatMinmus);
        }
        public void Log(string message)
        {
            Debug.Log("[ParallaxGlobalConfig] " + message);
        }
    }


    //public class CameraRotationDetection : MonoBehaviour      //Currently not implemented
    //{
    //    bool isRotating = false;
    //    bool isMouseDown = false;
    //    ReflectionProbe refProbe;
    //    Vector3 oldPosition;
    //    public void Update()
    //    {
    //
    //        //Vector3 newPosition = Camera.main.transform.position;
    //        //float distance = Vector3.Distance(oldPosition, newPosition);
    //        //
    //        //if (ParallaxReflectionProbes.probe != null && ParallaxReflectionProbes.probeActive == true)
    //        //{
    //        //    refProbe = ParallaxReflectionProbes.probe.GetComponent<ReflectionProbe>();
    //        //}
    //        //if (distance > 200000 && ParallaxReflectionProbes.probe != null && ParallaxReflectionProbes.probeActive == true)
    //        //{
    //        //    refProbe.resolution = 256;
    //        //    refProbe.timeSlicingMode = UnityEngine.Rendering.ReflectionProbeTimeSlicingMode.NoTimeSlicing;
    //        //    refProbe.RenderProbe();
    //        //    var tex = ParallaxReflectionProbes.probe.GetComponent<ReflectionProbe>().texture;
    //        //    FlightGlobals.currentMainBody.pqsController.surfaceMaterial.SetTexture("_ReflectionMap", tex);
    //        //    foreach (KeyValuePair<string, GameObject> quad in QuadMeshDictionary.subdividedQuadList)
    //        //    {
    //        //        quad.Value.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_ReflectionMap", tex);
    //        //    }
    //        //    oldPosition = newPosition;
    //        //}
    //        //else if (distance <= 200000 && ParallaxReflectionProbes.probe != null)  //Always this one, for now
    //        //{
    //        //    //Cam not moving
    //        //    refProbe.resolution = 256;
    //        //    refProbe.timeSlicingMode = UnityEngine.Rendering.ReflectionProbeTimeSlicingMode.NoTimeSlicing;
    //        //    refProbe.RenderProbe();
    //        //
    //        //    var tex = ParallaxReflectionProbes.probe.GetComponent<ReflectionProbe>().texture;
    //        //        FlightGlobals.currentMainBody.pqsController.surfaceMaterial.SetTexture("_ReflectionMap", tex);
    //        //        foreach (KeyValuePair<string, GameObject> quad in QuadMeshDictionary.subdividedQuadList)
    //        //        {
    //        //            quad.Value.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_ReflectionMap", tex);
    //        //        }
    //        //}
    //    }
    //}
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class GetCameraAltitude : MonoBehaviour
    {
        CelestialBody body;
        Material pqsMaterial;
        public void Start()
        {
            body = FlightGlobals.currentMainBody;
        }
        public void Update()
        {
            if (ParallaxDARTShaderLoader.parallaxBodies.ContainsKey(FlightGlobals.currentMainBody.name) && HighLogic.LoadedScene == GameScenes.FLIGHT && Camera.main != null)
            {
                //Do camera height here
                GetHeightFromTerrain(Camera.main.transform);
            }
            
            PlanetariumCamera.Camera.nearClipPlane = 0.001f;
        }
        public float GetHeightFromTerrain(Transform pos)    //Use fancy raycasting to achieve fancy things
        {
            float heightFromTerrain = 0;
            Vector3 vector = FlightGlobals.getUpAxis(FlightGlobals.currentMainBody, pos.position);
            float num = FlightGlobals.getAltitudeAtPos(pos.position, FlightGlobals.currentMainBody);
            if (num < 0f)
            {
                //Camera is underwater 
            }
            num += 600f;
            RaycastHit heightFromTerrainHit;
            if (Physics.Raycast(pos.position, -vector, out heightFromTerrainHit, num, 32768, QueryTriggerInteraction.Ignore))
            {
                heightFromTerrain = heightFromTerrainHit.distance;
                //this.objectUnderVessel = heightFromTerrainHit.collider.gameObject;
            }
            CameraRaycast.cameraAltitude = heightFromTerrain;
            return heightFromTerrain;
        }


    }
    public class CameraRaycast
    {
        public static float cameraAltitude = 0;
    }
    [KSPAddon(KSPAddon.Startup.FlightAndKSC, false)]
    public class ParallaxDARTPosition : MonoBehaviour
    {
        public CelestialBody lastBody;
        public PQSMod_CelestialBodyTransform fader;
        public static Vector3 floatUV = Vector3.zero;
        public void Start()
        {
            QualitySettings.shadowDistance = 10000;
            QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
            QualitySettings.shadowProjection = ShadowProjection.StableFit;
            QualitySettings.shadows = ShadowQuality.All;
            QualitySettings.shadowCascade4Split = new Vector3(0.003f, 0.034f, 0.101f);
            Camera.main.nearClipPlane = 0.1f;
            Camera.current.nearClipPlane = 0.1f;
        }
        public void Update()
        {

            CelestialBody body = FlightGlobals.currentMainBody;

            if (ParallaxDARTShaderLoader.parallaxBodies.ContainsKey(body.name))
            {
                body.pqsController.surfaceMaterial.SetVector("_PlanetOrigin", (Vector3)body.transform.position);
                body.pqsController.lowQualitySurfaceMaterial.SetVector("_PlanetOrigin", (Vector3)body.transform.position);
                body.pqsController.mediumQualitySurfaceMaterial.SetVector("_PlanetOrigin", (Vector3)body.transform.position);
                body.pqsController.highQualitySurfaceMaterial.SetVector("_PlanetOrigin", (Vector3)body.transform.position);
                body.pqsController.ultraQualitySurfaceMaterial.SetVector("_PlanetOrigin", (Vector3)body.transform.position);

                //body.pqsController.lowQualitySurfaceMaterial.SetVector("_LightPos", (Vector3)(FlightGlobals.Bodies[0].transform.position));
                //body.pqsController.mediumQualitySurfaceMaterial.SetVector("_LightPos", (Vector3)(FlightGlobals.Bodies[0].transform.position));
                //body.pqsController.surfaceMaterial.SetVector("_LightPos", (Vector3)(FlightGlobals.Bodies[0].transform.position));
                //body.pqsController.highQualitySurfaceMaterial.SetVector("_LightPos", (Vector3)(FlightGlobals.Bodies[0].transform.position));
                //body.pqsController.ultraQualitySurfaceMaterial.SetVector("_LightPos", (Vector3)(FlightGlobals.Bodies[0].transform.position));

                ParallaxDARTShaderLoader.parallaxBodies[FlightGlobals.currentMainBody.name].ParallaxBodyMaterial.ParallaxMaterial.SetVector("_PlanetOrigin", (Vector3)body.transform.position);
                ParallaxDARTShaderLoader.parallaxBodies[FlightGlobals.currentMainBody.name].ParallaxBodyMaterial.ParallaxMaterial.SetVector("_LightPos", (Vector3)body.transform.position);

                ParallaxDARTShaderLoader.parallaxBodies[FlightGlobals.currentMainBody.name].ParallaxBodyMaterial.ParallaxMaterialSINGLESTEEPLOW.SetVector("_PlanetOrigin", (Vector3)body.transform.position);
                ParallaxDARTShaderLoader.parallaxBodies[FlightGlobals.currentMainBody.name].ParallaxBodyMaterial.ParallaxMaterialSINGLESTEEPLOW.SetVector("_LightPos", (Vector3)(body.transform.position));



            }


            if (FlightGlobals.ActiveVessel != null && HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                if (ParallaxDARTShaderLoader.parallaxBodies.ContainsKey(body.name))
                {

                    Vector3d accuratePlanetPosition = FlightGlobals.currentMainBody.position;   //Double precision planet origin
                    double surfaceTexture_ST = ParallaxDARTShaderLoader.parallaxBodies[FlightGlobals.currentMainBody.name].ParallaxBodyMaterial.SurfaceTextureScale;    //Scale of surface texture
                    Vector3d UV = accuratePlanetPosition * surfaceTexture_ST;
                    UV = new Vector3d(Clamp(UV.x), Clamp(UV.y), Clamp(UV.z));
                    floatUV = new Vector3((float)UV.x, (float)UV.y, (float)UV.z);
                    FlightGlobals.currentMainBody.pqsController.surfaceMaterial.SetVector("_SurfaceTextureUVs", floatUV);
                    FlightGlobals.currentMainBody.pqsController.highQualitySurfaceMaterial.SetVector("_SurfaceTextureUVs", floatUV);
                    FlightGlobals.currentMainBody.pqsController.mediumQualitySurfaceMaterial.SetVector("_SurfaceTextureUVs", floatUV);
                    FlightGlobals.currentMainBody.pqsController.lowQualitySurfaceMaterial.SetVector("_SurfaceTextureUVs", floatUV);
                    FlightGlobals.currentMainBody.pqsController.ultraQualitySurfaceMaterial.SetVector("_SurfaceTextureUVs", floatUV);
                    if (floatUV != null)
                    {
                        ParallaxDARTShaderLoader.parallaxBodies[FlightGlobals.currentMainBody.name].ParallaxBodyMaterial.ParallaxMaterial.SetVector("_SurfaceTextureUVs", floatUV);
                        ParallaxDARTShaderLoader.parallaxBodies[FlightGlobals.currentMainBody.name].ParallaxBodyMaterial.ParallaxMaterialSINGLESTEEPLOW.SetVector("_SurfaceTextureUVs", floatUV);
 
                    }



                }



            }

            if (ParallaxDARTShaderLoader.parallaxBodies.ContainsKey(body.name))
            {
                float _PlanetOpacity = FlightGlobals.currentMainBody.pqsController.surfaceMaterial.GetFloat("_PlanetOpacity");
                ParallaxDARTShaderLoader.parallaxBodies[FlightGlobals.currentMainBody.name].ParallaxBodyMaterial.ParallaxMaterial.SetFloat("_PlanetOpacity", _PlanetOpacity);
                ParallaxDARTShaderLoader.parallaxBodies[FlightGlobals.currentMainBody.name].ParallaxBodyMaterial.ParallaxMaterialSINGLESTEEPLOW.SetFloat("_PlanetOpacity", _PlanetOpacity);
            }


            lastBody = body;
            //FlightGlobals.GetHomeBody().pqsController.ultraQualitySurfaceMaterial.SetVector("_PlanetOrigin", (Vector3)FlightGlobals.GetHomeBody().transform.position);
        }
        public double Clamp(double input)
        {
            if (CameraRaycast.cameraAltitude < 250 && CameraRaycast.cameraAltitude != 0)
            {
                return input % 32;  //When close to the ground, 
            }
            if (CameraRaycast.cameraAltitude == 0)  //Outside ray dir
            {
                return input % 1024.0;
            }
            return input % 1024.0;
        }
    }

    [KSPAddon(KSPAddon.Startup.PSystemSpawn, false)]
    public class ParallaxDARTShaderLoader : MonoBehaviour
    {
        public static Dictionary<string, ParallaxBody> parallaxBodies = new Dictionary<string, ParallaxBody>();
        public static float timeElapsed = 0;
        public void Start()
        {
            Log("Starting...");
            timeElapsed = Time.realtimeSinceStartup;
            GetConfigNodes();
            ActivateConfigNodes();
            timeElapsed = Time.realtimeSinceStartup - timeElapsed;
            Log("Parallax took " + timeElapsed + " milliseconds [" + timeElapsed / 1000 + " seconds] to load from start to finish.");



        }

        public void GetConfigNodes()
        {
            UrlDir.UrlConfig[] nodeArray = GameDatabase.Instance.GetConfigs("ParallaxDART");
            Log("Retrieving config nodes...");
            for (int i = 0; i < nodeArray.Length; i++)
            {
                Debug.Log("This parallax node has: " + nodeArray[i].config.nodes.Count + " nodes");
                for (int b = 0; b < nodeArray[i].config.nodes.Count; b++)
                {
                    ConfigNode parallax = nodeArray[i].config;
                    string bodyName = parallax.nodes[b].GetValue("name");
                    Log("////////////////////////////////////////////////");
                    Log("////////////////" + bodyName + "////////////////");
                    Log("////////////////////////////////////////////////\n");
                    ConfigNode parallaxBody = parallax.nodes[b].GetNode("Textures");
                    if (parallaxBody == null)
                    {
                        Log(" - !!!Parallax Body is null! Cancelling load!!!"); //Essentially, you ****** up
                        return;
                    }
                    Log(" - Retrieved body node");
                    ParallaxBody thisBody = new ParallaxBody();
                    if (bodyName == "Dimorphos")
                    {
                        thisBody.isPart = true;
                        Debug.Log("Dimorphos is a part");
                    }
                    thisBody.Body = bodyName;
                    thisBody.ParallaxBodyMaterial = CreateParallaxBodyMaterial(parallaxBody, bodyName);

                    Log(" - Assigned parallax body material");

                    try
                    {
                        parallaxBodies.Add(bodyName, thisBody); //Add to the list of parallax bodies
                        Log(" - Added " + bodyName + "'s parallax config successfully");
                        Log(parallaxBodies[bodyName].Body);
                    }
                    catch (Exception e)
                    {
                        Log(" - Duplicate body detected!\n" + " - " + e.ToString());
                        parallaxBodies[bodyName] = thisBody;
                        Log(" - Overwriting current body");
                    }
                    //DebugAllProperties(parallaxBody, thisBody);
                    Log("////////////////////////////////////////////////\n");
                }

            }

            Log("Activating config nodes...");
        }
        public void ActivateConfigNodes()
        {

            foreach (KeyValuePair<string, ParallaxBody> body in parallaxBodies)
            {
                Log("////////////////////////////////////////////////");
                Log("////////////////" + body.Key + "////////////////");
                Log("////////////////////////////////////////////////\n");
                body.Value.CreateMaterial();

                Log(" - Created material successfully");
                body.Value.Apply();
                Debug.Log(" - Applied config nodes");

            }
        }
        public static ParallaxBodyMaterial CreateParallaxBodyMaterial(ConfigNode parallaxBody, string bodyName)
        {
            ParallaxBodyMaterial material = new ParallaxBodyMaterial();
            material.PlanetName = bodyName;
            material.SurfaceTexture = ParseString(parallaxBody, "surfaceTexture");
            material.SurfaceTextureParallaxMap = ParseString(parallaxBody, "surfaceTessellationMap");
            material.SurfaceTextureBumpMap = ParseString(parallaxBody, "surfaceTextureBumpMap");
            material.SteepTexture = ParseString(parallaxBody, "steepTexture");
            material.SurfaceTextureScale = ParseFloat(parallaxBody, "surfaceTextureScale");
            material.SurfaceParallaxHeight = ParseFloat(parallaxBody, "tessellationHeight");
            material.SteepTextureScale = ParseFloat(parallaxBody, "steepTextureScale");
            material.SteepPower = ParseFloat(parallaxBody, "steepPower");
            material.Smoothness = ParseFloat(parallaxBody, "smoothness");
            material.Hapke = ParseFloat(parallaxBody, "hapke");
            material.Zoom1 = ParseFloat(parallaxBody, "zoom1");
            material.Zoom2 = ParseFloat(parallaxBody, "zoom2");
            material.Zoom3 = ParseFloat(parallaxBody, "zoom3");
            material.Zoom4 = ParseFloat(parallaxBody, "zoom4");
            material.Zoom5 = ParseFloat(parallaxBody, "zoom5");
            material.Zoom6 = ParseFloat(parallaxBody, "zoom6");
            material.Zoom7 = ParseFloat(parallaxBody, "zoom7");



            material.BumpMapSteep = ParseString(parallaxBody, "surfaceBumpMapSteep");

            material.PlanetName = bodyName;
            material.InfluenceMap = ParseString(parallaxBody, "influenceMap");


            material.DisplacementOffset = ParseFloat(parallaxBody, "displacementOffset");
            material.NormalSpecularInfluence = ParseFloat(parallaxBody, "normalSpecularInfluence");
            material.HasEmission = ParseBoolNumber(parallaxBody, "hasEmission");

            material.ReversedNormal = ParseBoolNumber(parallaxBody, "useReversedNormals");
            string color = ParseString(parallaxBody, "tintColor"); //it pains me to write colour this way as a brit
            material.TintColor = new Color(float.Parse(color.Split(',')[0]), float.Parse(color.Split(',')[1]), float.Parse(color.Split(',')[2]));

            color = ParseString(parallaxBody, "emissionColor");
            if (color != null)
            {
                material.EmissionColor = new Color(float.Parse(color.Split(',')[0]), float.Parse(color.Split(',')[1]), float.Parse(color.Split(',')[2]));
            }
            return material;

        }
        public static string ParseString(ConfigNode parallaxBody, string value)
        {
            string output = "";
            try
            {
                output = parallaxBody.GetValue(value);
                Log(" - " + value + " = " + output);
                if (output == null)
                {
                    Log("Notice: " + value + " was not assigned - Returning 0,0,0");
                    return "0,0,0";
                }
            }
            catch
            {
                Debug.Log(parallaxBody.name + " / " + value + " was not assigned");
                output = "Unused_Texture";
            }
            return output;
        }
        public static float ParseFloat(ConfigNode parallaxBody, string value)
        {
            string output = "";
            float realOutput = 0;
            try
            {
                
                output = parallaxBody.GetValue(value);
                Log(" - " + value + " = " + output);
                if (output == null)
                {
                    Log("Notice: " + value + " was not assigned - Returning 0");
                    return 0;
                }
            }
            catch
            {
                Debug.Log(value + " was not assigned");
                output = "Unused_Texture";
                return realOutput;
            }
            try
            {

                realOutput = float.Parse(output);
            }
            catch
            {
                Debug.Log("Critical error: Input was a string, but it should have been a float: " + parallaxBody.name + " / " + value);
            }
            return realOutput;
        }
        public static bool ParseBool(ConfigNode parallaxBody, string value)
        {
            string output = "";
            bool realOutput = false;
            try
            {
                
                output = parallaxBody.GetValue(value);
                Log(" - " + value + " = " + output);
                if (output == null)
                {
                    Log("Notice: " + value + " was not assigned - Returning false");
                    return false;
                }
            }
            catch
            {
                Debug.Log(value + " was not assigned");
                output = "Unused_Texture";
            }
            if (output.ToString().ToLower() == "true")
            {
                realOutput = true;
            }
            else
            {
                realOutput = false;
            }
            return realOutput;
        }
        public static int ParseBoolNumber(ConfigNode parallaxBody, string value)
        {
            string output = "";
            int realOutput = 0;
            try
            {
                
                output = parallaxBody.GetValue(value);
                Log(" - " + value + " = " + output);
                if (output == null)
                {
                    Log("Notice: " + value + " was not assigned - Returning 0 (false)");
                    return 0;
                }
            }
            catch
            {
                Debug.Log(value + " was not assigned");
                output = "Unused_Texture";
            }
            if (output.ToString().ToLower() == "true")
            {
                realOutput = 1;
            }
            else
            {
                realOutput = 0;
            }
            return realOutput;
        }
        public static Vector4 ParseVector(ConfigNode parallaxBody, string value)
        {
            string output = "";
            Vector4 realOutput = new Vector4(0, 0, 0, 0);
            try
            {
                
                output = parallaxBody.GetValue(value);
                Log(" - " + value + " = " + output);
                if (output == null)
                {
                    Log("Notice: " + value + " was not assigned - Returning Vector3(0, 0, 0)");
                    return Vector3.zero;
                }
                else
                {
                    float[] values = new float[4];
                    string[] data = output.ToLower().Replace(" ", string.Empty).Split(',');
                    for (int i = 0; i < 4; i++)
                    {
                        values[i] = float.Parse(data[i]);
                    }
                    realOutput = new Vector4(values[0], values[1], values[2], values[3]);
                    return realOutput;
                }
            }
            catch
            {
                Debug.Log("Error parsing vector4: " + value);
                return realOutput;
            }
        }
        public static void Log(string message)
        {
            Debug.Log("[Parallax]" + message);
        }

    }
    public class PhysicsTexHolder
    {
        public static Texture2D physicsTexLow;
        public static Texture2D physicsTexMid;
        public static Texture2D physicsTexHigh;
        public static Texture2D physicsTexSteep;
        public static Texture2D displacementTex;
    }
    public class ParallaxBody
    {
        private string bodyName;
        public bool isPart = false;
        private ParallaxBodyMaterial parallaxBodyMaterial;
        public Texture2DArray detailTexturesLower;   //Texture2D array passed to the shader as a sampler2darray declared by Unity - Bypass the sampler limit
        public Texture2DArray detailTexturesUpper;
        private Material parallaxMaterial;
        private Material parallaxMaterialSINGLELOW;
        private Material parallaxMaterialSINGLEMID;
        private Material parallaxMaterialSINGLEHIGH;

        private Material parallaxMaterialSINGLESTEEPLOW;
        private Material parallaxMaterialSINGLESTEEPMID;
        private Material parallaxMaterialSINGLESTEEPHIGH;

        private Material parallaxMaterialDOUBLELOW;
        private Material parallaxMaterialDOUBLEHIGH;
        public string Body
        {
            get { return bodyName; }
            set { bodyName = value; }
        }
        public ParallaxBodyMaterial ParallaxBodyMaterial
        {
            get { return parallaxBodyMaterial; }
            set { parallaxBodyMaterial = value; }
        }

        public void CreateMaterial()
        {
            Material[] materials = parallaxBodyMaterial.CreateMaterial();
            parallaxMaterial = materials[0];
            parallaxMaterialSINGLESTEEPLOW = materials[1];
        }
        public void Apply()
        {
            if (!isPart)
            {
                CelestialBody body = FlightGlobals.GetBodyByName(bodyName);
                if (body == null)
                {
                    Debug.Log("Unable to get body by name: " + bodyName);
                }
                ParallaxDARTShaderLoader.parallaxBodies[bodyName].parallaxBodyMaterial.ParallaxMaterialSINGLESTEEPLOW = parallaxMaterialSINGLESTEEPLOW;
            }
            else
            {
                ParallaxDARTShaderLoader.parallaxBodies[bodyName].parallaxBodyMaterial.ParallaxMaterialSINGLESTEEPLOW = parallaxMaterialSINGLESTEEPLOW;
                Debug.Log("Assigned material to part: " + bodyName);
            }
        }
    }
    public class ParallaxBodyMaterial : MonoBehaviour
    {
        private Material parallaxMaterial;
        private Material parallaxMaterialSINGLESTEEPLOW;

        private string planetName;
        private string surfaceTexture;
        private string surfaceTextureParallaxMap;
        private string surfaceTextureBumpMap;
        private string steepTexture;

        private float steepPower;
        private float surfaceTextureScale;
        private float surfaceParallaxHeight;
        private float displacementOffset;
        private float steepTextureScale;
        private float smoothness;
        private Color tintColor;
        private float normalSpecularInfluence;

        private string bumpMapSteep;

        private float planetRadius;
        private string influenceMap;

        private int reversedNormal;
        private float hapke;
        private bool useReflections = false;
        private float zoom1;
        private float zoom2;
        private float zoom3;
        private float zoom4;
        private float zoom5;
        private float zoom6;
        private float zoom7;

        private Vector4 reflectionMask = new Vector4(0, 0, 0, 0);

        private int hasEmission = 0;
        private Color emissionColor = new Color(0, 0, 0);

        #region getsets
       
        public int ReversedNormal
        {
            get { return reversedNormal; }
            set { reversedNormal = value; }
        }
       
        public float Hapke
        {
            get { return hapke; }
            set { hapke = value; }
        }
        public float Zoom1
        {
            get { return zoom1; }
            set { zoom1 = value; }
        }
        public float Zoom2
        {
            get { return zoom2; }
            set { zoom2 = value; }
        }
        public float Zoom3
        {
            get { return zoom3; }
            set { zoom3 = value; }
        }
        public float Zoom4
        {
            get { return zoom4; }
            set { zoom4 = value; }
        }
        public float Zoom5
        {
            get { return zoom5; }
            set { zoom5 = value; }
        }
        public float Zoom6
        {
            get { return zoom6; }
            set { zoom6 = value; }
        }
        public float Zoom7
        {
            get { return zoom7; }
            set { zoom7 = value; }
        }
        public Color EmissionColor
        {
            get { return emissionColor; }
            set { emissionColor = value; }
        }
        public int HasEmission
        {
            get { return hasEmission; }
            set { hasEmission = value; }
        }
        public float NormalSpecularInfluence
        {
            get { return normalSpecularInfluence; }
            set { normalSpecularInfluence = value; }
        }
        public float DisplacementOffset
        {
            get { return displacementOffset; }
            set { displacementOffset = value; }
        }
        public Vector4 ReflectionMask
        {
            get { return reflectionMask; }
            set { reflectionMask = value; }
        }
        public bool UseReflections
        {
            get { return useReflections; }
            set { useReflections = value; }
        }
        public string InfluenceMap
        {
            get { return influenceMap; }
            set { influenceMap = value; }
        }
        public Color TintColor
        {
            get { return tintColor; }
            set { tintColor = value; }
        }
        public string PlanetName
        {
            get { return planetName; }
            set { planetName = value; }
        }

        public float PlanetRadius
        {
            get { return planetRadius; }
            set { planetRadius = value; }
        }
        public string BumpMapSteep
        {
            get { return bumpMapSteep; }
            set { bumpMapSteep = value; }
        }
        public string SurfaceTexture
        {
            get { return surfaceTexture; }
            set { surfaceTexture = value; }
        }
        public string SurfaceTextureParallaxMap
        {
            get { return surfaceTextureParallaxMap; }
            set { surfaceTextureParallaxMap = value; }
        }
        public string SurfaceTextureBumpMap
        {
            get { return surfaceTextureBumpMap; }
            set { surfaceTextureBumpMap = value; }
        }
        public string SteepTexture
        {
            get { return steepTexture; }
            set { steepTexture = value; }
        }
        public float SteepPower
        {
            get { return steepPower; }
            set { steepPower = value; }
        }
        public float SurfaceTextureScale
        {
            get { return surfaceTextureScale; }
            set { surfaceTextureScale = value; }
        }

        public float SurfaceParallaxHeight
        {
            get { return surfaceParallaxHeight; }
            set { surfaceParallaxHeight = value; }
        }
        public float SteepTextureScale
        {
            get { return steepTextureScale; }
            set { steepTextureScale = value; }
        }
        public float Smoothness
        {
            get { return smoothness; }
            set { smoothness = value; }
        }
        public Material ParallaxMaterial
        {
            get { return parallaxMaterial; }
            set { parallaxMaterial = value; }
        }
        public Material ParallaxMaterialSINGLESTEEPLOW
        {
            get { return parallaxMaterialSINGLESTEEPLOW; }
            set { parallaxMaterialSINGLESTEEPLOW = value; }
        }
        #endregion
        public void EnableKeywordFromInteger(Material material, int value, string keywordON, string keywordOFF)
        {
            if (value == 1)
            {
                material.EnableKeyword(keywordON);
                material.DisableKeyword(keywordOFF);
            }
            else
            {
                material.EnableKeyword(keywordOFF);
                material.DisableKeyword(keywordON);
            }
        }
        public Material[] CreateMaterial()    //Does the ingame and stuffs
        {
            Log("Beginning material creation");
            Material[] output = new Material[2];
            ParallaxMaterial = new Material(ParallaxLoader.GetShader("Custom/ParallaxDART"));
            ParallaxMaterialSINGLESTEEPLOW = new Material(ParallaxLoader.GetShader("Custom/ParallaxDART"));
            if (ParallaxDARTShaderLoader.parallaxBodies[planetName].isPart == true)
            {
                ParallaxMaterialSINGLESTEEPLOW = new Material(ParallaxLoader.GetShader("Custom/ParallaxDARTAsteroid"));
                ParallaxMaterial = new Material(ParallaxLoader.GetShader("Custom/ParallaxDARTAsteroid"));
            }
            ////if (HighLogic.LoadedScene == GameScenes.FLIGHT)
            ////{
            ////    parallaxMaterial = FlightGlobals.currentMainBody.pqsController.surfaceMaterial;
            ////    parallaxMaterial.shader = ParallaxLoader.GetShader("Custom/ParallaxOcclusion");
            ////} //Not needed any more
            //
            //
            ////influenceMap = "BeyondHome/Terrain/DDS/BlankAlpha";
            //
            ////parallaxMaterial.SetTexture("_SurfaceTexture", LoadTexture(surfaceTexture));
            ////parallaxMaterial.SetTexture("_DispTex", LoadTexture(surfaceTextureParallaxMap));
            ////parallaxMaterial.SetTexture("_BumpMap", LoadTexture(surfaceTextureBumpMap));
            ////parallaxMaterial.SetTexture("_SteepTex", LoadTexture(steepTexture));
            //parallaxMaterial.SetFloat("_SteepPower", steepPower);
            //parallaxMaterial.SetTextureScale("_SurfaceTexture", CreateVector(surfaceTextureScale));
            //parallaxMaterial.SetFloat("_displacement_scale", surfaceParallaxHeight);
            //parallaxMaterial.SetTextureScale("_SteepTex", CreateVector(steepTextureScale));
            //parallaxMaterial.SetFloat("_Metallic", smoothness);
            //parallaxMaterial.SetFloat("_Hapke", hapke);
            ////parallaxMaterial.SetTexture("_SurfaceTextureMid", LoadTexture(surfaceTextureMid));
            ////parallaxMaterial.SetTexture("_SurfaceTextureHigh", LoadTexture(surfaceTextureHigh));
            ////parallaxMaterial.SetTexture("_BumpMapMid", LoadTexture(bumpMapMid));
            ////parallaxMaterial.SetTexture("_BumpMapHigh", LoadTexture(bumpMapHigh));
            ////parallaxMaterial.SetTexture("_BumpMapSteep", LoadTexture(bumpMapSteep));
            ////parallaxMaterial.SetFloat("_PlanetRadius", (float)FlightGlobals.GetBodyByName(planetName).Radius);
            //parallaxMaterial.SetColor("_MetallicTint", tintColor);
            ////parallaxMaterial.SetTexture("_InfluenceMap", LoadTexture(influenceMap));
            //parallaxMaterial.SetFloat("_TessellationRange", 99);
            ////parallaxMaterial.SetTexture("_FogTexture", LoadTexture(fogTexture));
            //parallaxMaterial.SetFloat("_displacement_offset", displacementOffset);
            //parallaxMaterial.SetFloat("_NormalSpecularInfluence", normalSpecularInfluence);
            //parallaxMaterial.SetFloat("_HasEmission", hasEmission);
            //parallaxMaterial.SetColor("_EmissionColor", emissionColor);
            //
            //parallaxMaterial.SetVector("_ReflectionMask", reflectionMask);
            //parallaxMaterial.SetFloat("_TessellationEdgeLength", ParallaxSettings.tessellationEdgeLength);
            //parallaxMaterial.SetFloat("_TessellationRange", ParallaxSettings.tessellationRange);
            //parallaxMaterial.SetFloat("_TessellationMax", ParallaxSettings.tessellationMax);
            //
            //parallaxMaterial.SetFloat("zoom1", zoom1);
            //parallaxMaterial.SetFloat("zoom2", zoom2);
            //parallaxMaterial.SetFloat("zoom3", zoom3);
            //parallaxMaterial.SetFloat("zoom4", zoom4);
            //parallaxMaterial.SetFloat("zoom5", zoom5);
            //parallaxMaterial.SetFloat("zoom6", zoom6);
            //parallaxMaterial.SetFloat("zoom7", zoom7);

            parallaxMaterialSINGLESTEEPLOW.SetFloat("_SteepPower", steepPower);
            parallaxMaterialSINGLESTEEPLOW.SetTextureScale("_SurfaceTexture", CreateVector(surfaceTextureScale));
            parallaxMaterialSINGLESTEEPLOW.SetFloat("_displacement_scale", surfaceParallaxHeight);
            parallaxMaterialSINGLESTEEPLOW.SetTextureScale("_SteepTex", CreateVector(steepTextureScale));
            parallaxMaterialSINGLESTEEPLOW.SetFloat("_Metallic", smoothness);
            //parallaxMaterialSINGLESTEEPLOW.SetTexture("_BumpMapSteep", LoadTexture(bumpMapSteep));
            //parallaxMaterialSINGLESTEEPLOW.SetFloat("_PlanetRadius", (float)FlightGlobals.GetBodyByName(planetName).Radius);
            parallaxMaterialSINGLESTEEPLOW.SetColor("_MetallicTint", tintColor);
            //parallaxMaterialSINGLESTEEPLOW.SetTexture("_InfluenceMap", LoadTexture(influenceMap));
            parallaxMaterialSINGLESTEEPLOW.SetFloat("_TessellationRange", 99);
            //parallaxMaterialSINGLESTEEPLOW.SetTexture("_FogTexture", LoadTexture(fogTexture));
            parallaxMaterialSINGLESTEEPLOW.SetFloat("_displacement_offset", displacementOffset);
            ParallaxMaterialSINGLESTEEPLOW.SetFloat("_NormalSpecularInfluence", normalSpecularInfluence);
            parallaxMaterialSINGLESTEEPLOW.SetFloat("_HasEmission", hasEmission);
            parallaxMaterialSINGLESTEEPLOW.SetColor("_EmissionColor", emissionColor);
            parallaxMaterialSINGLESTEEPLOW.SetVector("_ReflectionMask", reflectionMask);
            parallaxMaterialSINGLESTEEPLOW.SetFloat("_TessellationEdgeLength", ParallaxSettings.tessellationEdgeLength);
            parallaxMaterialSINGLESTEEPLOW.SetFloat("_TessellationRange", ParallaxSettings.tessellationRange);
            parallaxMaterialSINGLESTEEPLOW.SetFloat("_TessellationMax", ParallaxSettings.tessellationMax);
            parallaxMaterialSINGLESTEEPLOW.SetFloat("_Hapke", hapke);

            parallaxMaterialSINGLESTEEPLOW.SetFloat("zoom1", zoom1);
            parallaxMaterialSINGLESTEEPLOW.SetFloat("zoom2", zoom2);
            parallaxMaterialSINGLESTEEPLOW.SetFloat("zoom3", zoom3);
            parallaxMaterialSINGLESTEEPLOW.SetFloat("zoom4", zoom4);
            parallaxMaterialSINGLESTEEPLOW.SetFloat("zoom5", zoom5);
            parallaxMaterialSINGLESTEEPLOW.SetFloat("zoom6", zoom6);
            parallaxMaterialSINGLESTEEPLOW.SetFloat("zoom7", zoom7);

            if (ParallaxDARTShaderLoader.parallaxBodies[planetName].isPart == false)
            {
                if (ParallaxSettings.trueLighting == "adaptive" && FlightGlobals.GetBodyByName(planetName).atmosphere == true)
                {
                    Log("ATTENUATION_OVERRIDE", "true (adaptive)");
                }
                if (ParallaxSettings.trueLighting == "adaptive" && FlightGlobals.GetBodyByName(planetName).atmosphere == false)
                {
                    Log("ATTENUATION_OVERRIDE", "false (adaptive)");
                }
                if (ParallaxSettings.trueLighting == "true")
                {
                    Log("ATTENUATION_OVERRIDE", "true");
                }
                if (ParallaxSettings.trueLighting == "false")
                {
                    Log("ATTENUATION_OVERRIDE", "false");
                }
            }
            
            ParseKeywords();    //Quality settings
            output[0] = parallaxMaterial;
            output[1] = parallaxMaterialSINGLESTEEPLOW;
            return output;
        }
        public void ParseKeywords()
        {
            if (ParallaxSettings.tessellate == true)
            {
                parallaxMaterial.EnableKeyword("TESS_ON");
                parallaxMaterial.DisableKeyword("TESS_OFF");

                parallaxMaterialSINGLESTEEPLOW.EnableKeyword("TESS_ON");
                parallaxMaterialSINGLESTEEPLOW.DisableKeyword("TESS_OFF");

            }
            else
            {
                parallaxMaterial.EnableKeyword("TESS_OFF");
                parallaxMaterial.DisableKeyword("TESS_ON");

                parallaxMaterialSINGLESTEEPLOW.EnableKeyword("TESS_OFF");
                parallaxMaterialSINGLESTEEPLOW.DisableKeyword("TESS_ON");
                
            }

            if (ParallaxSettings.tessellateLighting == true)
            {
                parallaxMaterial.EnableKeyword("HQ_LIGHTS_ON");
                parallaxMaterial.DisableKeyword("HQ_LIGHTS_OFF");

                parallaxMaterialSINGLESTEEPLOW.EnableKeyword("HQ_LIGHTS_ON");
                parallaxMaterialSINGLESTEEPLOW.DisableKeyword("HQ_LIGHTS_OFF");
                
            }
            else
            {
                parallaxMaterial.EnableKeyword("HQ_LIGHTS_OFF");
                parallaxMaterial.DisableKeyword("HQ_LIGHTS_ON");

                parallaxMaterialSINGLESTEEPLOW.EnableKeyword("HQ_LIGHTS_OFF");
                parallaxMaterialSINGLESTEEPLOW.DisableKeyword("HQ_LIGHTS_ON");
                
            }

            if (ParallaxSettings.tessellateShadows == true)
            {
                parallaxMaterial.EnableKeyword("HQ_SHADOWS_ON");
                parallaxMaterial.DisableKeyword("HQ_SHADOWS_OFF");

                parallaxMaterialSINGLESTEEPLOW.EnableKeyword("HQ_SHADOWS_ON");
                parallaxMaterialSINGLESTEEPLOW.DisableKeyword("HQ_SHADOWS_OFF");
                
            }
            else
            {
                parallaxMaterial.EnableKeyword("HQ_SHADOWS_OFF");
                parallaxMaterial.DisableKeyword("HQ_SHADOWS_ON");

                parallaxMaterialSINGLESTEEPLOW.EnableKeyword("HQ_SHADOWS_OFF");
                parallaxMaterialSINGLESTEEPLOW.DisableKeyword("HQ_SHADOWS_ON");
                
            }

        }

        private void Log(string name, string value)
        {
            Debug.Log("\t - " + name + " is " + value);
        }
        private void Log(string name, float value)
        {
            Debug.Log("\t - " + name + " is " + value);
        }
        private void Log(string message)
        {
            Debug.Log("\t" + message);
        }
        public Texture LoadTexture(string name)
        {
            try
            {
                return Resources.FindObjectsOfTypeAll<Texture>().FirstOrDefault(t => t.name == name);
            }
            catch
            {
                Debug.Log("The texture, '" + name + "', could not be found");
                return Resources.FindObjectsOfTypeAll<Texture>().FirstOrDefault(t => t.name == "TessellationBlank");
            }
        }
        public static Vector2 CreateVector(float size)
        {
            return new Vector2(size, size);
        }
    }
    [KSPAddon(KSPAddon.Startup.FlightAndKSC, false)]
    public class ParallaxOnDemandLoader : MonoBehaviour
    {
        bool thisBodyIsLoaded = false;
        CelestialBody lastKnownBody;
        CelestialBody currentBody;
        public static Dictionary<string, Texture2D> activeTextures = new Dictionary<string, Texture2D>();
        public static Dictionary<string, string> activeTexturePaths = new Dictionary<string, string>();
        public static bool finishedMainLoad = false;
        float timeElapsed = 0;
        public void Start()
        {
            Log("Starting Parallax On-Demand loader");
            Sun.Instance.sunLight.shadowStrength = 1;
        }
        public void LateUpdate()
        {
            if (ParallaxDARTShaderLoader.parallaxBodies.ContainsKey(FlightGlobals.currentMainBody.name))
            {
                Sun.Instance.sunLight.shadowStrength = 1;
            }

        }
        public void Update()
        {
            //foreach (Light l in GameObject.FindObjectsOfType<Light>())
            //{
            //    Debug.Log(l.name + ": " + l.shadowStrength);
            //}

            timeElapsed = Time.realtimeSinceStartup;
            currentBody = FlightGlobals.currentMainBody;
            bool key = Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha2);
            if (key)
            {
                Debug.Log("Reactivating config nodes");
                ActivateNewConfigNodes();
            }
            if (currentBody != lastKnownBody)           //Vessel is around a new planet, so its textures must be loaded
            {
                finishedMainLoad = false;
                bool bodyExists = ParallaxDARTShaderLoader.parallaxBodies.ContainsKey(currentBody.name);
                if (bodyExists)
                {

                    Log("SOI changed, beginning transfer of surface textures for " + currentBody.name);
                    Log("Unloading " + activeTextures.Count + " textures");
                    UnloadTextures();
                    LoadTextures();
                }
                timeElapsed = Time.realtimeSinceStartup - timeElapsed;
                Debug.Log("Loading " + activeTextures.Count + " textures from disk took " + timeElapsed + "s");
                lastKnownBody = currentBody;
            }


        }
        public void GetConfigNodes()
        {
            
            UrlDir.UrlConfig[] nodeArray = GameDatabase.Instance.GetConfigs("ParallaxDART");
            Log("Retrieving config nodes...");
            //ParallaxDARTShaderLoader.parallaxBodies.Clear();
            for (int i = 0; i < nodeArray.Length; i++)
            {
                
                Debug.Log("This parallax node has: " + nodeArray[i].config.nodes.Count + " nodes");
                for (int b = 0; b < nodeArray[i].config.nodes.Count; b++)
                {
                    ConfigNode parallax = nodeArray[i].config;
                    string bodyName = parallax.nodes[b].GetValue("name");
                    Log("////////////////////////////////////////////////");
                    Log("////////////////" + bodyName + "////////////////");
                    Log("////////////////////////////////////////////////\n");
                    ConfigNode parallaxBody = parallax.nodes[b].GetNode("Textures");
                    if (parallaxBody == null)
                    {
                        Log(" - !!!Parallax Body is null! Cancelling load!!!"); //Essentially, you ****** up
                        return;
                    }
                    Log(" - Retrieved body node");
                    ParallaxBody thisBody = ParallaxDARTShaderLoader.parallaxBodies[bodyName];
                    thisBody.Body = bodyName;
                    thisBody.ParallaxBodyMaterial = ParallaxDARTShaderLoader.CreateParallaxBodyMaterial(parallaxBody, bodyName);

                    Log(" - Assigned parallax body material");

                    //try
                    //{
                    //    ParallaxDARTShaderLoader.parallaxBodies.Add(bodyName, thisBody); //Add to the list of parallax bodies
                    //    Log(" - Added " + bodyName + "'s parallax config successfully");
                    //    Log(ParallaxDARTShaderLoader.parallaxBodies[bodyName].Body);
                    //}
                    //catch (Exception e)
                    //{
                    //    Log(" - Duplicate body detected!\n" + " - " + e.ToString());
                    //    ParallaxDARTShaderLoader.parallaxBodies[bodyName] = thisBody;
                    //    Log(" - Overwriting current body");
                    //}
                    //DebugAllProperties(parallaxBody, thisBody);
                    Log("////////////////////////////////////////////////\n");
                }

            }

            Log("Activating config nodes...");
        }
        public void ActivateNewConfigNodes()
        {
            //foreach (KeyValuePair<string, ParallaxBody> body in ParallaxDARTShaderLoader.parallaxBodies)
            //{
            //    body.Value.ParallaxBodyMaterial.CreateMaterial();
            //}
            string bodName = "Kerbin";
            foreach (string line in File.ReadAllLines(Path.Combine(KSPUtil.ApplicationRootPath + "GameData/" + "Update.txt")))
            {
                Debug.Log("\t\t\t\t\t\t\t\t\t\tParsing LINE: " + line);
                if (line[0] == '#')
                {
                    break;
                }
                if (line[0] == '-')
                {
                    //This is the body title
                    bodName = line.Remove(0, 1).Trim();
                    Debug.Log("Parsing bodyName: " + bodName);
                }
                if (line[0] != '}' && line[0] != '{' && line[0] != '-')
                {
                    string parameter = line.Split('=')[0].Trim();
                    string value = line.Split('=')[1].Trim();
                    Debug.Log("Parsing " + parameter);
                    if (parameter == "_SurfaceTextureScale")
                    {
                        ParallaxDARTShaderLoader.parallaxBodies[bodName].ParallaxBodyMaterial.ParallaxMaterialSINGLESTEEPLOW.SetTextureScale("_SurfaceTexture", new Vector2(float.Parse(value), float.Parse(value)));
                    }
                    if (parameter == "_SteepTextureScale")
                    {
                        ParallaxDARTShaderLoader.parallaxBodies[bodName].ParallaxBodyMaterial.ParallaxMaterialSINGLESTEEPLOW.SetTextureScale("_SteepTex", new Vector2(float.Parse(value), float.Parse(value)));
                    }
                    if (parameter != "_SurfaceTextureScale" && parameter != "_SteepTextureScale")
                    {
                        try
                        {
                            var actualValue = float.Parse(value);   //float
                            ParallaxDARTShaderLoader.parallaxBodies[bodName].ParallaxBodyMaterial.ParallaxMaterialSINGLESTEEPLOW.SetFloat(parameter, actualValue);
                        }
                        catch
                        {
                            Debug.Log("No dynamic loading of strings");
                        }
                    }
                    
                       
                }
            }
        }
       
        private void Log(string message)
        {
            Debug.Log("[Parallax] " + message);
        }
        public void LoadTextures()
        {
            ParallaxBody thisBody = ParallaxDARTShaderLoader.parallaxBodies[currentBody.name];
            ValidateEverything(thisBody);
            AttemptTextureLoad(currentBody.name);
            finishedMainLoad = true;
        }
        public void UnloadTextures()
        {
            foreach (KeyValuePair<string, Texture2D> texture in activeTextures)
            {
                Destroy(texture.Value); //Destroy the texture before clearing from the dictionary
            }
            activeTextures.Clear();
            activeTexturePaths.Clear();

        }
        public void ValidateEverything(ParallaxBody body)
        {
            ValidatePath(body.ParallaxBodyMaterial.SurfaceTexture, "_SurfaceTexture");

            ValidatePath(body.ParallaxBodyMaterial.SteepTexture, "_SteepTex");

            ValidatePath(body.ParallaxBodyMaterial.SurfaceTextureBumpMap, "_BumpMap");

            ValidatePath(body.ParallaxBodyMaterial.BumpMapSteep, "_BumpMapSteep");

            ValidatePath(body.ParallaxBodyMaterial.SurfaceTextureParallaxMap, "_DispTex");
            ValidatePath(body.ParallaxBodyMaterial.InfluenceMap, "_InfluenceMap");

        }
        public void ValidatePath(string path, string name)
        {
            string actualPath = "";

            actualPath = Path.Combine(KSPUtil.ApplicationRootPath + "GameData/" + path);
            //string actualPath = Path.Combine(KSPUtil.ApplicationRootPath + "GameData/" + "Parallax/Shaders/Parallax");
            Log("Validating " + actualPath);
            bool fileExists = File.Exists(actualPath);
            if (fileExists)
            {
                Log(" - Texture exists");
                activeTexturePaths.Add(name, actualPath);
            }
            else
            {
                Log(" - Texture doesn't exist, skipping: " + name + " with filepath: " + path);
            }
            //if (File.Exists(Application.dataPath))
        }
        public void AttemptTextureLoad(string planetName)
        {
            ParallaxBody thisBody = ParallaxDARTShaderLoader.parallaxBodies[planetName]; ;
            if (thisBody == null)
            {
                Debug.Log("Body is null");
                return;
            }
            //ParallaxDARTShaderLoader.parallaxBodies[planetName].detailTexturesLower = new Texture2DArray(thisBody.ParallaxBodyMaterial.DetailResolution, thisBody.ParallaxBodyMaterial.DetailResolution, 8, TextureFormat.RGBA32, true);
            //ParallaxDARTShaderLoader.parallaxBodies[planetName].detailTexturesUpper = new Texture2DArray(thisBody.ParallaxBodyMaterial.DetailResolution, thisBody.ParallaxBodyMaterial.DetailResolution, 8, TextureFormat.RGBA32, true);
            //Holds 8 detail textures

            foreach (KeyValuePair<string, string> path in activeTexturePaths)
            {
                activeTextures.Add(path.Key, Texture2D.blackTexture);
                Texture2D textureRef = activeTextures[path.Key];
                byte[] bytes = System.IO.File.ReadAllBytes(path.Value);
                if (path.Key == "_PhysicsTexDisplacement")
                {
                    if (path.Value.EndsWith(".png"))
                    {
                        textureRef = LoadPNG(path.Value);
                    }
                    else
                    {
                        textureRef = LoadDDSTexture(bytes, path.Key + " | " + path.Value);
                    }

                    PhysicsTexHolder.displacementTex = textureRef;
                    activeTextures[path.Key] = textureRef;
                    Debug.Log("Loaded all physics textures");
                }
                else
                {
                    if (path.Value.EndsWith(".png"))
                    {
                        textureRef = LoadPNG(path.Value);
                    }
                    else
                    {
                        textureRef = LoadDDSTexture(bytes, path.Key + " | " + path.Value);
                    }
                    Debug.Log("Path.key = " + path.Key);
                    activeTextures[path.Key] = textureRef;
                    //FlightGlobals.currentMainBody.pqsController.surfaceMaterial.SetTexture(path.Key, textureRef);
                    //thisBody.ParallaxBodyMaterial.ParallaxMaterial.SetTexture(path.Key, textureRef);
                    if (thisBody.ParallaxBodyMaterial.ParallaxMaterialSINGLESTEEPLOW == null)
                    {
                        Debug.Log("Null");
                    }
                    thisBody.ParallaxBodyMaterial.ParallaxMaterialSINGLESTEEPLOW.SetTexture(path.Key, textureRef);
                    if (path.Key == "_SurfaceTexture")
                    {
                        thisBody.ParallaxBodyMaterial.ParallaxMaterialSINGLESTEEPLOW.SetTexture("_SurfaceTexture", textureRef);
                        Debug.Log("Surface texture set");
                    }
                    
                    if (path.Key == "_BumpMap")
                    {
                        thisBody.ParallaxBodyMaterial.ParallaxMaterialSINGLESTEEPLOW.SetTexture("_BumpMap", textureRef);
                    }
                    


                    //if (path.Key == "_DetailTexLow")
                    //{
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialSINGLESTEEPLOW.SetTexture("_DetailTex", textureRef);
                    //
                    //}
                  
                    //if (path.Key == "_DetailNormalLow")
                    //{
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialSINGLELOW.SetTexture("_DetailNormal", textureRef);
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialSINGLESTEEPLOW.SetTexture("_DetailNormal", textureRef);
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialDOUBLELOW.SetTexture("_DetailTexLowerNormal", textureRef);
                    //    //AttemptDetailSetLower(thisBody, textureRef, 1);
                    //}
                    //if (path.Key == "_DetailNormalMid")
                    //{
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialSINGLEMID.SetTexture("_DetailNormal", textureRef);
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialSINGLESTEEPMID.SetTexture("_DetailNormal", textureRef);
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialDOUBLEHIGH.SetTexture("_DetailTexLowerNormal", textureRef);
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialDOUBLELOW.SetTexture("_DetailTexHigherNormal", textureRef);
                    //    //AttemptDetailSetLower(thisBody, textureRef, 3);
                    //    //AttemptDetailSetUpper(thisBody, textureRef, 1);
                    //}
                    //if (path.Key == "_DetailNormalHigh")
                    //{
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialSINGLEHIGH.SetTexture("_DetailNormal", textureRef);
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialSINGLESTEEPHIGH.SetTexture("_DetailNormal", textureRef);
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialDOUBLEHIGH.SetTexture("_DetailTexHigherNormal", textureRef);
                    //    //AttemptDetailSetUpper(thisBody, textureRef, 3);
                    //}
                    //if (path.Key == "_DetailSteepLow")
                    //{
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialSINGLESTEEPLOW.SetTexture("_DetailSteep", textureRef);
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialSINGLELOW.SetTexture("_DetailSteep", textureRef);
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialDOUBLELOW.SetTexture("_DetailSteepLower", textureRef);
                    //    //AttemptDetailSetLower(thisBody, textureRef, 4);
                    //}
                    //if (path.Key == "_DetailSteepNormalLow")
                    //{
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialSINGLESTEEPLOW.SetTexture("_DetailSteepNormal", textureRef);
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialSINGLELOW.SetTexture("_DetailSteepNormal", textureRef);
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialDOUBLELOW.SetTexture("_DetailSteepLowerNormal", textureRef);
                    //}
                    //if (path.Key == "_DetailSteepMid")
                    //{
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialSINGLESTEEPMID.SetTexture("_DetailSteep", textureRef);
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialSINGLEMID.SetTexture("_DetailSteep", textureRef);
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialDOUBLELOW.SetTexture("_DetailSteepHigher", textureRef);
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialDOUBLEHIGH.SetTexture("_DetailSteepLower", textureRef);
                    //}
                    //if (path.Key == "_DetailSteepNormalMid")
                    //{
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialSINGLESTEEPMID.SetTexture("_DetailSteepNormal", textureRef);
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialSINGLEMID.SetTexture("_DetailSteepNormal", textureRef);
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialDOUBLELOW.SetTexture("_DetailSteepHigherNormal", textureRef);
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialDOUBLEHIGH.SetTexture("_DetailSteepLowerNormal", textureRef);
                    //}
                    //if (path.Key == "_DetailSteepHigh")
                    //{
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialSINGLESTEEPHIGH.SetTexture("_DetailSteep", textureRef);
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialSINGLEHIGH.SetTexture("_DetailSteep", textureRef);
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialDOUBLEHIGH.SetTexture("_DetailSteepHigher", textureRef);
                    //}
                    //if (path.Key == "_DetailSteepNormalHigh")
                    //{
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialSINGLESTEEPHIGH.SetTexture("_DetailSteepNormal", textureRef);
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialSINGLEHIGH.SetTexture("_DetailSteepNormal", textureRef);
                    //    thisBody.ParallaxBodyMaterial.ParallaxMaterialDOUBLEHIGH.SetTexture("_DetailSteepHigherNormal", textureRef);
                    //}
                    Debug.Log("Set texture: " + path.Key + " // " + textureRef.name);
                    //.pqsController.surfaceMaterial.SetTexture(path.Key, textureRef);
                }
            }

            //ParallaxDARTShaderLoader.parallaxBodies[planetName].ParallaxBodyMaterial.SetTexturesOnDemand();
            //
            //FlightGlobals.currentMainBody.pqsController.surfaceMaterial = ParallaxDARTShaderLoader.parallaxBodies[planetName].ParallaxBodyMaterial.ParallaxMaterial;
            //FlightGlobals.currentMainBody.pqsController.highQualitySurfaceMaterial = ParallaxDARTShaderLoader.parallaxBodies[planetName].ParallaxBodyMaterial.ParallaxMaterial;
            //FlightGlobals.currentMainBody.pqsController.mediumQualitySurfaceMaterial = ParallaxDARTShaderLoader.parallaxBodies[planetName].ParallaxBodyMaterial.ParallaxMaterial;
            //FlightGlobals.currentMainBody.pqsController.lowQualitySurfaceMaterial = ParallaxDARTShaderLoader.parallaxBodies[planetName].ParallaxBodyMaterial.ParallaxMaterial;
            Debug.Log("Completed load on demand");
        }
        public void AttemptDetailSetLower(ParallaxBody body, Texture2D map, int level)
        {
            body.detailTexturesLower.SetPixels32(map.GetPixels32(), level);
        }
        public void AttemptDetailSetUpper(ParallaxBody body, Texture2D map, int level)
        {
            body.detailTexturesUpper.SetPixels32(map.GetPixels32(), level);
        }
        public static Texture2D LoadPNG(string filePath)
        {
            Debug.Log("[ParallaxLoader] Warning: Attempting to load a PNG texture from disk.");
            Texture2D tex = null;

            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                ImageConversion.LoadImage(tex, fileData); //..this will auto-resize the texture dimensions.
                return tex;

            }
            return tex;
        }
        public Texture2D LoadDDSTexture(byte[] data, string name)
        {
            Debug.Log("Loading: " + name);
            byte ddsSizeCheck = data[4];
            if (ddsSizeCheck != 124)
            {
                Log("This DDS texture is invalid - Unable to read the size check value from the header.");
            }


            int height = data[13] * 256 + data[12];
            int width = data[17] * 256 + data[16];
            Log("Texture width = " + width);
            Log("Texture height = " + height);


            int DDS_HEADER_SIZE = 128;
            byte[] dxtBytes = new byte[data.Length - DDS_HEADER_SIZE];
            Buffer.BlockCopy(data, DDS_HEADER_SIZE, dxtBytes, 0, data.Length - DDS_HEADER_SIZE);
            int mipMapCount = (data[28]) | (data[29] << 8) | (data[30] << 16) | (data[31] << 24);
            Debug.Log("Mipmap count = " + mipMapCount);

            TextureFormat format = TextureFormat.DXT1;
            if (data[84] == 'D')
            {

                if (data[87] == 49) //Also char '1'
                {
                    format = TextureFormat.DXT1;
                }
                else if (data[87] == 53)    //Also char '5'
                {
                    format = TextureFormat.DXT5;
                }
                else
                {
                    Debug.Log("Texture is not a DXT 1 or DXT5");
                }
            }
            Texture2D texture;
            if (mipMapCount == 1)
            {
                texture = new Texture2D(width, height, format, false);
            }
            else
            {
                texture = new Texture2D(width, height, format, true);
                Debug.Log("Empty texture created");
            }
            try
            {
                Debug.Log("Loading raw bytes...");
                texture.LoadRawTextureData(dxtBytes);
                Debug.Log("Loading raw bytes completed");
            }
            catch
            {
                Log("CRITICAL ERROR: Parallax has halted the OnDemand loading process because texture.LoadRawTextureData(dxtBytes) would have resulted in overread");
                Log("Please check the format for this texture and refer to the wiki if you're unsure:");
                Log("Exception: " + name);
            }
            Debug.Log("Applying texture...");
            texture.Apply();

            return (texture);
        }
        public Texture2D LoadPNGTexture(string url)
        {
            Texture2D tex;
            tex = new Texture2D(2, 2);
            tex.LoadRawTextureData(File.ReadAllBytes(url));
            tex.Apply();

            Debug.Log("Loaded physics image: " + tex.width + " x " + tex.height);
            return tex;
        }
    }
    public class TextureLoader
    {
        public static Texture2D LoadPNGTexture(string url)
        {
            Texture2D tex;
            tex = new Texture2D(2, 2);
            tex.LoadRawTextureData(File.ReadAllBytes(url));
            tex.Apply();

            Debug.Log("Loaded physics image: " + tex.width + " x " + tex.height);
            return tex;
        }
        public static Texture2D LoadDDSTexture(byte[] data, string name)
        {

            byte ddsSizeCheck = data[4];
            if (ddsSizeCheck != 124)
            {
                Debug.Log("This DDS texture is invalid - Unable to read the size check value from the header.");
            }


            int height = data[13] * 256 + data[12];
            int width = data[17] * 256 + data[16];
            Debug.Log("Texture width = " + width);
            Debug.Log("Texture height = " + height);


            int DDS_HEADER_SIZE = 128;
            byte[] dxtBytes = new byte[data.Length - DDS_HEADER_SIZE];
            Buffer.BlockCopy(data, DDS_HEADER_SIZE, dxtBytes, 0, data.Length - DDS_HEADER_SIZE);
            int mipMapCount = (data[28]) | (data[29] << 8) | (data[30] << 16) | (data[31] << 24);
            Debug.Log("Mipmap count = " + mipMapCount);

            TextureFormat format = TextureFormat.DXT1;
            if (data[84] == 'D')
            {

                if (data[87] == 49) //Also char '1'
                {
                    format = TextureFormat.DXT1;
                }
                else if (data[87] == 53)    //Also char '5'
                {
                    format = TextureFormat.DXT5;
                }
                else
                {
                    Debug.Log("Texture is not a DXT 1 or DXT5");
                }
            }
            Texture2D texture;
            if (mipMapCount == 1)
            {
                texture = new Texture2D(width, height, format, false);
            }
            else
            {
                texture = new Texture2D(width, height, format, true);
            }
            try
            {
                texture.LoadRawTextureData(dxtBytes);
            }
            catch
            {
                Debug.Log("CRITICAL ERROR: Parallax has halted the OnDemand loading process because texture.LoadRawTextureData(dxtBytes) would have resulted in overread");
                Debug.Log("Please check the format for this texture and refer to the wiki if you're unsure:");
                Debug.Log("Exception: " + name);
            }
            texture.Apply();

            return (texture);
        }
    }
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class FastLoad : MonoBehaviour
    {
        public void Start()
        {
            QualitySettings.vSyncCount = 0; //For some reason VSYNC is forced on during loading. We don't like that. That makes things load slowly.

        }
    }

    [KSPAddon(KSPAddon.Startup.PSystemSpawn, false)]
    public class ScaledParallaxDARTLoader : MonoBehaviour
    {
        private GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        public void Start()
        {
            ScaledParallaxBodies.scaledParallaxBodies = new Dictionary<string, ScaledParallaxBody>();
            GetConfigNodes();
            ActivateConfigNodes();
            Debug.Log("Finished scaled parallax");
            
            
            //var planetMeshFilter = fakePlanet.GetComponent<MeshFilter>();//FlightGlobals.GetBodyByName("Gilly").scaledBody.GetComponent<MeshFilter>();
            //var planetMeshRenderer = fakePlanet.GetComponent<MeshRenderer>();//FlightGlobals.GetBodyByName("Gilly").scaledBody.GetComponent<MeshRenderer>();
            //Destroy(fakePlanet.GetComponent<MeshCollider>());
            //Material a = new Material(ParallaxLoader.GetShader("Custom/ParallaxScaled"));
            //a.SetTexture("_ColorMap", Resources.FindObjectsOfTypeAll<Texture>().FirstOrDefault(t => t.name == "Parallax_StockTextures/_Scaled/Duna_Color"));
            //a.SetTexture("_NormalMap", Resources.FindObjectsOfTypeAll<Texture>().FirstOrDefault(t => t.name == "Parallax_StockTextures/_Scaled/Duna_Normal"));
            //a.SetTexture("_HeightMap", Resources.FindObjectsOfTypeAll<Texture>().FirstOrDefault(t => t.name == "Parallax_StockTextures/_Scaled/Duna_Height"));
            //a.SetTextureScale("_ColorMap", new Vector2(1, 1));
            //a.SetFloat("_PlanetRadius", 1);
            //a.SetFloat("_displacement_scale", 0.07f);
            //a.SetFloat("_TessellationRange", 10000f);
            //a.SetFloat("_TessellationMax", 64);
            //a.SetFloat("_SteepPower", 0.001f);
            //a.SetFloat("_Metallic", 0.01f);
            //a.SetFloat("_TessellationEdgeLength", 8f);
            //a.SetFloat("_PlanetOpacity", 1);
            //a.SetFloat("_FresnelExponent", 23.6f);
            //a.SetFloat("_TransitionWidth", 0.5f);
            //a.SetTexture("_FogTexture", Resources.FindObjectsOfTypeAll<Texture>().FirstOrDefault(t => t.name == "Parallax_StockTextures/_Scaled/Duna_Fog"));
            //fakePlanet.SetActive(true);
            //planetMeshRenderer.enabled = true;
            //planetMeshRenderer.material = a;
            //planetMeshRenderer.sharedMaterial = a;
            //fakePlanet.layer = 10;
            //fakePlanet.transform.parent = FlightGlobals.GetHomeBody().scaledBody.transform;
            //fakePlanet.transform.localPosition = new Vector3(0, 0, 0);
            //
            //fakePlanet.transform.localScale = new Vector3(3000, 3000, 3000);
            //Debug.Log("Material set");
        }
        public void GetConfigNodes()
        {
            UrlDir.UrlConfig[] nodeArray = GameDatabase.Instance.GetConfigs("ScaledParallax");
            for (int i = 0; i < nodeArray.Length; i++)
            {
                for (int b = 0; b < nodeArray[i].config.nodes.Count; b++)
                {
                    ConfigNode parallax = nodeArray[i].config;
                    string bodyName = parallax.nodes[b].GetValue("name");
                    Log("////////////////////////////////////////////////");
                    Log("////////////////" + bodyName + "////////////////");
                    Log("////////////////////////////////////////////////\n");
                    GameObject fakePlanet = GameDatabase.Instance.GetModel("DART/Models/KSPPlanet");
                    GameObject fakeLocalPlanet = GameDatabase.Instance.GetModel("DART/Models/KSPPlanet");
                    fakePlanet.SetActive(true);
                    fakeLocalPlanet.SetActive(true);
                    if (fakePlanet == null)
                    {
                        Debug.Log("It's null, what are you doing?");
                    }
                    Destroy(fakePlanet.GetComponent<MeshCollider>());
                    Destroy(fakeLocalPlanet.GetComponent<MeshCollider>());
                    //fakePlanet.GetComponent<MeshFilter>().sharedMesh = GameDatabase.Instance.GetModel("Parallax_StockTextures/_Scaled/KSPPlanet").GetComponent<MeshFilter>().mesh;
                    ConfigNode parallaxBody = parallax.nodes[b].GetNode("Textures");
                    if (parallaxBody == null)
                    {
                        Log(" - !!!Parallax Body is null! Cancelling load!!!"); //Essentially, you ****** up
                        return;
                    }
                    Log(" - Retrieved body node");
                    ScaledParallaxBody thisBody = new ScaledParallaxBody();
                    fakePlanet.transform.parent = FlightGlobals.GetBodyByName(bodyName).scaledBody.transform;
                    fakePlanet.transform.localPosition = new Vector3(0, 0, 0);
                    fakeLocalPlanet.transform.parent = FlightGlobals.GetBodyByName(bodyName).transform;
                    fakePlanet.transform.localPosition = new Vector3(0, 0, 0);
                    //fakePlanet.transform.Rotate(0, 90, 0);
                    float scaledSpaceFactor = (float)FlightGlobals.GetBodyByName(bodyName).Radius / 1000;
                    fakePlanet.transform.localScale = new Vector3((float)(FlightGlobals.GetBodyByName(bodyName).Radius / scaledSpaceFactor), (float)(FlightGlobals.GetBodyByName(bodyName).Radius / scaledSpaceFactor), (float)(FlightGlobals.GetBodyByName(bodyName).Radius / scaledSpaceFactor)) ;
                    fakeLocalPlanet.transform.localScale = new Vector3((float)FlightGlobals.GetBodyByName(bodyName).Radius, (float)FlightGlobals.GetBodyByName(bodyName).Radius, (float)FlightGlobals.GetBodyByName(bodyName).Radius);
                    fakePlanet.layer = 10;  //So it's visible in scaled space, the jammy bastard
                    fakeLocalPlanet.layer = 15;
                    fakeLocalPlanet.transform.Rotate(0, 90, 0);
                    Debug.Log(fakeLocalPlanet.transform.rotation + " is the transform rotation of the planet");
                    thisBody.scaledBody = fakePlanet;
                    thisBody.localBody = fakeLocalPlanet;
                    thisBody.bodyName = bodyName;
                    thisBody.scaledMaterial = CreateScaledBodyMaterial(parallaxBody, bodyName);
                    FlightGlobals.GetBodyByName(bodyName).scaledBody.GetComponent<MeshFilter>().sharedMesh = Instantiate(primitive.GetComponent<MeshFilter>().mesh);
    
                    Log(" - Assigned parallax body material");
    
                    try
                    {
                        ScaledParallaxBodies.scaledParallaxBodies.Add(bodyName, thisBody); //Add to the list of parallax bodies
                        Log(" - Added " + bodyName + "'s parallax config successfully");
                        Log(ScaledParallaxBodies.scaledParallaxBodies[bodyName].bodyName);
                    }
                    catch (Exception e)
                    {
                        Log(" - Duplicate body detected!\n" + " - " + e.ToString());
                        ScaledParallaxBodies.scaledParallaxBodies[bodyName] = thisBody;
                        Log(" - Overwriting current body");
                    }
                    Log("////////////////////////////////////////////////\n");
                }
    
            }
        }
        public ScaledParallaxBodyMaterial CreateScaledBodyMaterial(ConfigNode scaledBody, string name)
        {
            ScaledParallaxBodyMaterial material = new ScaledParallaxBodyMaterial();
    
            material.bodyName = name;
            material.colorMap = ParseString(scaledBody, "colorMap");
            material.normalMap = ParseString(scaledBody, "normalMap");
            material.heightMap = ParseString(scaledBody, "heightMap");
    
            material.tessellationEdgeLength = 5; //Constants for now
            material.tessellationRange = ParseFloat(scaledBody, "tessellationRange"); //Hella risky, yo
            material.tessellationMax = ParseFloat(scaledBody, "tessellationMax");
    
            material.displacementOffset = ParseFloat(scaledBody, "displacementOffset");
            material.displacementScale = ParseFloat(scaledBody, "displacementScale");
            material.metallic = ParseFloat(scaledBody, "metallic");
            material.normalSpecularInfluence = ParseFloat(scaledBody, "normalSpecularInfluence");
            material.hapke = ParseFloat(scaledBody, "hapke");

            material.surfaceTexture = ParseString(scaledBody, "surfaceTexture");
            material.bumpMap = ParseString(scaledBody, "surfaceBumpMap");
            material.surfaceTextureScale = ParseFloat(scaledBody, "surfaceTextureScale");
            material.influenceMap = ParseString(scaledBody, "influenceMap");
            material.surfaceNormalPower = ParseFloat(scaledBody, "surfaceNormalPower");
            material.fadeStart = ParseFloat(scaledBody, "fadeStart");
            material.fadeEnd = ParseFloat(scaledBody, "fadeEnd");
            material.trueFadeStart = ParseFloat(scaledBody, "trueScaledFadeStart");
            material.trueFadeEnd = ParseFloat(scaledBody, "trueScaledFadeEnd");

            string color = ParseString(scaledBody, "metallicTint"); //it pains me to write colour this way as a brit
            material.metallicTint = new Color(float.Parse(color.Split(',')[0]), float.Parse(color.Split(',')[1]), float.Parse(color.Split(',')[2]));
            //Might as well reuse the same variable
            return material;
        }
        
        public void ActivateConfigNodes()
        {
    
            foreach (KeyValuePair<string, ScaledParallaxBody> body in ScaledParallaxBodies.scaledParallaxBodies)
            {
                Log("////////////////////////////////////////////////");
                Log("////////////////" + body.Key + "////////////////");
                Log("////////////////////////////////////////////////\n");
                body.Value.scaledMaterial.GenerateMaterial(body.Value.scaledMaterial);
                Log(" - Created material successfully");
    
    
            }
        }
        public string ParseString(ConfigNode node, string input)
        {
            string output = node.GetValue(input);
            Log(" - " + input + " = " + output);
            if (output == null)
            {
                Log("NullReferenceException: " + input + " was not defined in the config and has been automatically set to NULL");
                return "NULL";
            }
            else { return output; }
        }
        public float ParseFloat(ConfigNode node, string input)
        {
            string output = node.GetValue(input);
            Log(" - " + input + " = " + output);
            float realOutput = 0;
            if (output == null)
            {
                Log("NullReferenceException: " + input + " was not defined in the config and has been automatically set to 0");
                return 0;
            }
            else
            {
                try
                {
                    realOutput = float.Parse(output);
                }
                catch
                {
                    Log("InvalidTypeException: " + input + " was defined, could not be converted to a float, and has been automatically set to 0");
                    return 0;
                }
            }
            return realOutput;
        }
        public bool ParseBool(ConfigNode node, string input)
        {
            string output = node.GetValue(input).ToLower();
            Log(" - " + input + " = " + output);
            bool realOutput = false;
            if (output == null)
            {
                Log("NullReferenceException: " + input + " was not defined in the config and has been automatically set to FALSE");
                return false;
            }
            else
            {
                try
                {
                    realOutput = bool.Parse(output);
                }
                catch
                {
                    Log("InvalidTypeException: " + input + " was defined, could not be converted to a float, and has been automatically set to FALSE");
                    return false;
                }
            }
            return realOutput;
        }
        public void Log(string message)
        {
            Debug.Log("[ParallaxScaled] " + message);
        }
        
    }
    public class ScaledParallaxBody
    {
        public string bodyName;
        public ScaledParallaxBodyMaterial scaledMaterial;
        public GameObject scaledBody;
        public GameObject localBody;
    }
    public class ScaledParallaxBodyMaterial
    {
        public Material material;
        public string bodyName;
    
        public string colorMap; //Textures
        public string normalMap;
        public string heightMap;
        public string surfaceTexture;
        public string bumpMap;
        public string influenceMap;
        public float surfaceTextureScale;
        
        public float tessellationMax;   //Quality settings
        public float tessellationEdgeLength;
        public float tessellationRange;
        public float displacementScale;
        public float displacementOffset = -0.5f;

        public float metallic;  //PBR settings
        public Color metallicTint;
        public float normalSpecularInfluence;
        public float hapke;
        public float surfaceNormalPower;

        public float fadeStart;
        public float fadeEnd;
        public float trueFadeStart;
        public float trueFadeEnd;
        public ScaledParallaxBody GetScaledParallaxBody(string bodyName)
        {
            return ScaledParallaxBodies.scaledParallaxBodies[bodyName];
        }
        public void GenerateMaterial(ScaledParallaxBodyMaterial scaledBody)
        {
            float time = Time.realtimeSinceStartup;
            Material scaledMaterial = new Material(ParallaxLoader.GetShader("Custom/ParallaxDARTScaled"));
            scaledMaterial.SetTexture("_ColorMap", LoadTexture(scaledBody.colorMap));
            scaledMaterial.SetTexture("_NormalMap", LoadTexture(scaledBody.normalMap));
            scaledMaterial.SetTexture("_HeightMap", LoadTexture(scaledBody.heightMap));

            scaledMaterial.SetTexture("_SurfaceTexture", LoadTexture(scaledBody.surfaceTexture));
            scaledMaterial.SetTexture("_BumpMap", LoadTexture(scaledBody.bumpMap));
            scaledMaterial.SetTexture("_InfluenceMap", LoadTexture(scaledBody.influenceMap));
            Debug.Log("Texture loading took " + (time - Time.realtimeSinceStartup) + " seconds");
            scaledMaterial.SetTextureScale("_SurfaceTexture", new Vector2(surfaceTextureScale * 2, surfaceTextureScale));
            
    
            scaledMaterial.SetFloat("_TessellationEdgeLength", scaledBody.tessellationEdgeLength);
            scaledMaterial.SetFloat("_TessellationMax", scaledBody.tessellationMax);
            scaledMaterial.SetFloat("_TessellationRange", scaledBody.tessellationRange);
            scaledMaterial.SetFloat("_displacement_offset", scaledBody.displacementOffset);
            scaledMaterial.SetFloat("_displacement_scale", scaledBody.displacementScale);
            scaledMaterial.SetFloat("_Metallic", scaledBody.metallic);
            scaledMaterial.SetColor("_MetallicTint", scaledBody.metallicTint);
            scaledMaterial.SetFloat("_TessellationEdgeLength", scaledBody.tessellationEdgeLength);
            scaledMaterial.SetFloat("_PlanetRadius", 1);
            scaledMaterial.SetFloat("_SteepPower", 0.0001f);
            scaledMaterial.SetFloat("_SurfaceNormalPower", scaledBody.surfaceNormalPower);
            

            scaledMaterial.SetFloat("_Hapke", hapke);
            scaledBody.material = scaledMaterial;
            scaledBody.GetScaledParallaxBody(scaledBody.bodyName).scaledBody.GetComponent<MeshRenderer>().material = scaledBody.material;
            scaledBody.GetScaledParallaxBody(scaledBody.bodyName).scaledBody.GetComponent<MeshRenderer>().sharedMaterial = scaledBody.material;
            scaledBody.GetScaledParallaxBody(scaledBody.bodyName).scaledBody.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
            scaledBody.GetScaledParallaxBody(scaledBody.bodyName).scaledBody.GetComponent<MeshRenderer>().receiveShadows = false;

            scaledBody.GetScaledParallaxBody(scaledBody.bodyName).localBody.GetComponent<MeshRenderer>().material = scaledBody.material;    //SHARED MATERIAL IN THE FUTURE!!
            scaledBody.GetScaledParallaxBody(scaledBody.bodyName).localBody.GetComponent<MeshRenderer>().material = scaledBody.material;
            scaledBody.GetScaledParallaxBody(scaledBody.bodyName).localBody.GetComponent<MeshRenderer>().material.SetFloat("_displacement_scale", 1.3f);
            scaledBody.GetScaledParallaxBody(scaledBody.bodyName).localBody.GetComponent<MeshRenderer>().material.SetFloat("_displacement_offset", 0.01f);
            scaledBody.GetScaledParallaxBody(scaledBody.bodyName).localBody.GetComponent<MeshRenderer>().material.SetFloat("_TessellationRange", 40000f);
            scaledBody.GetScaledParallaxBody(scaledBody.bodyName).localBody.GetComponent<MeshRenderer>().material.SetFloat("_TessellationMax", 1f);
            //Debug.Log("Set the displacement scale let's have a gander shall we lads");
        }
        public static Texture2D LoadTexture(string name)
        {
            if (name.EndsWith(".png"))
            {
                return LoadPNG(name);
            }
            else if (name.EndsWith(".dds"))
            {
                name = KSPUtil.ApplicationRootPath + "GameData/" + name;
                float a = Time.realtimeSinceStartup;
                byte[] fileData = File.ReadAllBytes(name);
                Texture2D tex = TextureLoader.LoadDDSTexture(fileData, name);
                return tex;
            }
            else
            {
                return new Texture2D(2, 2);
            }
            
        }
        public static Texture2D LoadPNG(string filePath)
        {
            Debug.Log("[ParallaxLoader] Warning: Attempting to load a PNG texture from disk.");
            Texture2D tex = null;
    
            byte[] fileData;
            filePath = Path.Combine(KSPUtil.ApplicationRootPath + "GameData/" + filePath);
            //filePath = KSPUtil.ApplicationRootPath + "GameData/" + filePath;
            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                ImageConversion.LoadImage(tex, fileData); //..this will auto-resize the texture dimensions.
                return tex;
    
            }
            return tex;
        }
        
    }
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class ScaledParallaxFader : MonoBehaviour
    {
        public static bool InMap = false;
        public static bool InTracking = false;
        public void Update()
        {
            
            string body = FlightGlobals.currentMainBody.name;
            if (ScaledParallaxBodies.scaledParallaxBodies.ContainsKey(body) && MapView.MapIsEnabled == false) //
            {
				//nearCamera = Camera.allCameras.FirstOrDefault (_cam => _cam.name == "Camera 00");

                float planetOpacity = FlightGlobals.currentMainBody.pqsController.surfaceMaterial.GetFloat("_PlanetOpacity");
                float dist = Vector3.Distance(Camera.allCameras.FirstOrDefault(_cam => _cam.name == "Camera 00").transform.position, FlightGlobals.currentMainBody.transform.position) + (float)FlightGlobals.currentMainBody.Radius;
                //Debug.Log("Distance from planet is " + dist);
                ScaledParallaxBody thisBody = ScaledParallaxBodies.scaledParallaxBodies[body];
                float localPlanetOpacity = Mathf.Clamp((dist - thisBody.scaledMaterial.fadeStart) / (thisBody.scaledMaterial.fadeEnd - thisBody.scaledMaterial.fadeStart), 0, 1);
                float scaledPlanetOpacity = Mathf.Clamp((dist - thisBody.scaledMaterial.trueFadeStart) / (thisBody.scaledMaterial.trueFadeEnd - thisBody.scaledMaterial.trueFadeStart), 0, 1);
                //Debug.Log("FSTART: " + thisBody.scaledMaterial.trueFadeStart + " // FEND: " + thisBody.scaledMaterial.trueFadeEnd);
                //Debug.Log("Local planet opacity: " + localPlanetOpacity);
                //Debug.Log("True scaled planet opacity: " + scaledPlanetOpacity);
                ScaledParallaxBodies.scaledParallaxBodies[body].localBody.GetComponent<MeshRenderer>().material.SetFloat("_PlanetOpacity", localPlanetOpacity);
                if (scaledPlanetOpacity == 1)
                {
                    ScaledParallaxBodies.scaledParallaxBodies[body].localBody.GetComponent<MeshRenderer>().material.SetFloat("_PlanetOpacity", 0);
                    ScaledParallaxBodies.scaledParallaxBodies[body].localBody.GetComponent<MeshRenderer>().material.SetFloat("_TessellationMax", 0);
                }
                else
                {
                    ScaledParallaxBodies.scaledParallaxBodies[body].localBody.GetComponent<MeshRenderer>().material.SetFloat("_TessellationMax", 4);
                }
                ScaledParallaxBodies.scaledParallaxBodies[body].scaledBody.GetComponent<MeshRenderer>().material.SetFloat("_PlanetOpacity", scaledPlanetOpacity);
                //ScaledParallaxBodies.scaledParallaxBodies[body].scaledMaterial.material.SetFloat("_PlanetOpacity", planetOpacity);
                if (planetOpacity == 0)
                {
                    ScaledParallaxBodies.scaledParallaxBodies[body].scaledMaterial.material.SetFloat("_TessellationMax", 0);
                    
                }
                else
                {
                    ScaledParallaxBodies.scaledParallaxBodies[body].scaledMaterial.material.SetFloat("_TessellationMax", ScaledParallaxBodies.scaledParallaxBodies[body].scaledMaterial.tessellationMax);
                }
                if (localPlanetOpacity == 0)
                {
                    ScaledParallaxBodies.scaledParallaxBodies[body].localBody.GetComponent<MeshRenderer>().material.SetFloat("_PlanetOpacity", 0);
                    ScaledParallaxBodies.scaledParallaxBodies[body].localBody.GetComponent<MeshRenderer>().material.SetFloat("_TessellationMax", 0);
                }
                else
                {
                    ScaledParallaxBodies.scaledParallaxBodies[body].localBody.GetComponent<MeshRenderer>().material.SetFloat("_TessellationMax", 4);
                }
            }
            if (ScaledParallaxBodies.scaledParallaxBodies.ContainsKey(body) && MapView.MapIsEnabled == true)
            {
                ScaledParallaxBodies.scaledParallaxBodies[body].scaledBody.GetComponent<MeshRenderer>().material.SetFloat("_PlanetOpacity", 1);
                ScaledParallaxBodies.scaledParallaxBodies[body].localBody.GetComponent<MeshRenderer>().material.SetFloat("_PlanetOpacity", 1);
            }

            bool flag = Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha2);
            if (flag)
            {
                Debug.Log("Keypress - Loading new values");
                ActivateNewConfigNodes();
            }
        }
        
        public void ActivateNewConfigNodes()
        {
            //foreach (KeyValuePair<string, ParallaxBody> body in ParallaxDARTShaderLoader.parallaxBodies)
            //{
            //    body.Value.ParallaxBodyMaterial.CreateMaterial();
            //}
            string bodName = "Kerbin";
            foreach (string line in File.ReadAllLines(Path.Combine(KSPUtil.ApplicationRootPath + "GameData/" + "UpdateScaled.txt")))
            {
                Debug.Log("\t\t\t\t\t\t\t\t\t\tParsing LINE: " + line);
                if (line[0] == '#')
                {
                    break;
                }
                if (line[0] == '-')
                {
                    //This is the body title
                    bodName = line.Remove(0, 1).Trim();
                    Debug.Log("Parsing bodyName: " + bodName);
                }
                if (line[0] != '}' && line[0] != '{' && line[0] != '-')
                {
                    string parameter = line.Split('=')[0].Trim();
                    string value = line.Split('=')[1].Trim();
                    Debug.Log("Parsing " + parameter);
                    if (parameter == "_SurfaceTextureScale")
                    {
                        ScaledParallaxBodies.scaledParallaxBodies[bodName].scaledMaterial.material.SetTextureScale("_SurfaceTexture", new Vector2(float.Parse(value), float.Parse(value) / 2));
                    }
                    if (parameter != "_SurfaceTextureScale")
                    {
                        try
                        {
                            var actualValue = float.Parse(value);   //float
                            ScaledParallaxBodies.scaledParallaxBodies[bodName].scaledMaterial.material.SetFloat(parameter, actualValue);
                            ScaledParallaxBodies.scaledParallaxBodies[bodName].localBody.GetComponent<MeshRenderer>().material.SetFloat(parameter, actualValue);
                        }
                        catch
                        {
                            Debug.Log("No string support sos boss");
                        }
                    }


                }
            }
        }
    }
   // public class ScaledBodyComponent : MonoBehaviour
   // {
   //     public Transform bodyTransform;
   //     public MeshRenderer planet;
   //     public string bodyName = "NoPlanetSpecified";
   //     bool inRange = false;
   //     public void Start()
   //     {
   //         Debug.Log(bodyName + " was visible from the start");
   //         if (planet.isVisible)
   //         {
   //             StartCoroutine("WaitForInvisible");
   //         }
   //         else
   //         {
   //             StartCoroutine("WaitForVisible");
   //         }
   //     }
   //     IEnumerator WaitForVisible()
   //     {
   //         yield return new WaitUntil(() => (inRange == true) && bodyName != "NoPlanetSpecified");
   //         ScaledParallaxBodies.scaledParallaxBodies[bodyName].scaledMaterial.GenerateMaterial(ScaledParallaxBodies.scaledParallaxBodies[bodyName].scaledMaterial);
   //         StartCoroutine("WaitForInvisible");
   //     }
   //     IEnumerator WaitForInvisible()
   //     {
   //         yield return new WaitUntil(() => (inRange == false) && bodyName != "NoPlanetSpecified");
   //         Destroy(ScaledParallaxBodies.scaledParallaxBodies[bodyName].scaledMaterial.material.GetTexture("_ColorMap"));
   //         Destroy(ScaledParallaxBodies.scaledParallaxBodies[bodyName].scaledMaterial.material.GetTexture("_NormalMap"));
   //         Destroy(ScaledParallaxBodies.scaledParallaxBodies[bodyName].scaledMaterial.material.GetTexture("_HeightMap"));
   //         Destroy(ScaledParallaxBodies.scaledParallaxBodies[bodyName].scaledMaterial.material.GetTexture("_FogTexture"));
   //         StartCoroutine("WaitForVisible");
   //     }
   //     public void Update()
   //     {
   //         if (transform == null)
   //         {
   //             return;
   //         }
   //         if (Camera.current == null)
   //         {
   //             inRange = true;
   //             return;
   //         }
   //         if (Vector3.Distance(bodyTransform.position, Camera.current.transform.position) > 70000)
   //         {
   //             inRange = false;
   //         }
   //         else
   //         {
   //             inRange = true;
   //         }
   //     }
   // }
    public class ScaledParallaxBodies
    {
        public static Dictionary<string, ScaledParallaxBody> scaledParallaxBodies;
    }
    [KSPAddon(KSPAddon.Startup.TrackingStation, false)]
    public class ParallaxScaledOnDemand : MonoBehaviour
    {
        public void Start()
        {
            foreach (KeyValuePair<string, ScaledParallaxBody> body in ScaledParallaxBodies.scaledParallaxBodies)
            {
                if (body.Value.scaledBody.gameObject != null)
                {
                    body.Value.scaledBody.gameObject.GetComponent<MeshRenderer>().material.SetFloat("_PlanetOpacity", 1);
                    Debug.Log("Set opacity of " + body.Key + " to 1");
                }
            }
        }
    }
    public class PartOnDemandLoader
    {
        public static Dictionary<string, Texture2D> activeTextures = new Dictionary<string, Texture2D>();
        public static void ValidateEverything(ParallaxBody body)
        {
            activeTextures.Clear();
            ValidatePath(body.ParallaxBodyMaterial.SurfaceTexture, "_SurfaceTexture");

            ValidatePath(body.ParallaxBodyMaterial.SteepTexture, "_SteepTex");

            ValidatePath(body.ParallaxBodyMaterial.SurfaceTextureBumpMap, "_BumpMap");

            ValidatePath(body.ParallaxBodyMaterial.BumpMapSteep, "_BumpMapSteep");

            ValidatePath(body.ParallaxBodyMaterial.SurfaceTextureParallaxMap, "_DispTex");
            ValidatePath(body.ParallaxBodyMaterial.InfluenceMap, "_InfluenceMap");

        }
        public static void ValidatePath(string path, string name)
        {
            string actualPath = "";

            actualPath = Path.Combine(KSPUtil.ApplicationRootPath + "GameData/" + path);
            //string actualPath = Path.Combine(KSPUtil.ApplicationRootPath + "GameData/" + "Parallax/Shaders/Parallax");
            bool fileExists = File.Exists(actualPath);
            if (fileExists)
            {
                Log(" - Texture exists: " + name);
                if (path.EndsWith(".png"))
                {
                    Texture2D tex = TextureLoader.LoadPNGTexture(actualPath);
                    Debug.Log(" - Successfully loaded PNG texture");
                    activeTextures.Add(name, tex);
                }
                else
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(actualPath);
                    Texture2D tex = TextureLoader.LoadDDSTexture(bytes, actualPath);
                    Debug.Log(" - Successfully loaded DDS texture");
                    activeTextures.Add(name, tex);
                }
            }
            else
            {
                Log(" - Texture doesn't exist, skipping: " + name + " with filepath: " + actualPath);
            }
            //if (File.Exists(Application.dataPath))
        }
        public static void Log(string msg)
        {
            Debug.Log("[Parallax] " + msg);
        }
    }

    public class ModuleParallax : PartModule
    {
        public MeshRenderer partMeshRenderer;
        public override void OnAwake()
        {
            if (HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                partMeshRenderer = part.gameObject.GetComponentsInChildren<MeshRenderer>()[0];
                partMeshRenderer.material = ParallaxDARTShaderLoader.parallaxBodies["Dimorphos"].ParallaxBodyMaterial.ParallaxMaterialSINGLESTEEPLOW;
                partMeshRenderer.sharedMaterial = ParallaxDARTShaderLoader.parallaxBodies["Dimorphos"].ParallaxBodyMaterial.ParallaxMaterialSINGLESTEEPLOW;
                partMeshRenderer.material = Instantiate(ParallaxDARTShaderLoader.parallaxBodies["Dimorphos"].ParallaxBodyMaterial.ParallaxMaterialSINGLESTEEPLOW);
                PartOnDemandLoader.ValidateEverything(ParallaxDARTShaderLoader.parallaxBodies["Dimorphos"]);
                foreach (KeyValuePair<string, Texture2D> texture in PartOnDemandLoader.activeTextures)
                {
                    partMeshRenderer.material.SetTexture(texture.Key, texture.Value);
                    Debug.Log("[Parallax] Set part texture: " + texture.Key);
                }
                partMeshRenderer.material.SetFloat("_PlanetOpacity", 0f);
                partMeshRenderer.material.SetFloat("_TessellationRange", 20f);
                partMeshRenderer.material.SetFloat("_TessellationMax", 64f);
                partMeshRenderer.material.SetFloat("_TessellationEdgeLength", 16f);
                //The values are set but since the part is not a body, its textures cannot be loaded.
                //We need to load its textures ourselves via the part module
            }
        }
        public void LoadPartTextures()
        {
            
        }
        public override void OnUpdate()
        {
            partMeshRenderer = part.gameObject.GetComponentsInChildren<MeshRenderer>()[0];
            partMeshRenderer.material.SetVector("_SurfaceTextureUVs", gameObject.transform.position);
            bool key = Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha2);
            if (key)
            {
                
                //foreach (KeyValuePair<string, ParallaxBody> body in ParallaxDARTShaderLoader.parallaxBodies)
                //{
                //    body.Value.ParallaxBodyMaterial.CreateMaterial();
                //}
                string bodName = "Kerbin";
                foreach (string line in File.ReadAllLines(Path.Combine(KSPUtil.ApplicationRootPath + "GameData/" + "Update.txt")))
                {
                    Debug.Log("\t\t\t\t\t\t\t\t\t\tParsing LINE: " + line);
                    if (line[0] == '#')
                    {
                        break;
                    }
                    if (line[0] == '-')
                    {
                        //This is the body title
                        bodName = line.Remove(0, 1).Trim();
                        Debug.Log("Parsing bodyName: " + bodName);
                    }
                    if (line[0] != '}' && line[0] != '{' && line[0] != '-')
                    {
                        string parameter = line.Split('=')[0].Trim();
                        string value = line.Split('=')[1].Trim();
                        Debug.Log("Parsing " + parameter);
                        if (parameter == "_SurfaceTextureScale")
                        {
                            partMeshRenderer.material.SetTextureScale("_SurfaceTexture", new Vector2(float.Parse(value), float.Parse(value)));
                        }
                        if (parameter == "_SteepTextureScale")
                        {
                            partMeshRenderer.material.SetTextureScale("_SteepTex", new Vector2(float.Parse(value), float.Parse(value)));
                        }
                        if (parameter != "_SurfaceTextureScale" && parameter != "_SteepTextureScale")
                        {
                            try
                            {
                                var actualValue = float.Parse(value);   //float
                                partMeshRenderer.material.SetFloat(parameter, actualValue);
                            }
                            catch
                            {
                                Debug.Log("No string support for dynamic update");
                            }
                        }


                    }
                }
            }
        }
        public void OnDestroy()
        {   
            //Debug.Log("[Parallax] Part unloaded, destroying textures");
            //foreach (KeyValuePair<string, Texture2D> texture in PartOnDemandLoader.activeTextures)
            //{
            //    Destroy(texture.Value);
            //}
            //PartOnDemandLoader.activeTextures.Clear();
        }
    }
}
