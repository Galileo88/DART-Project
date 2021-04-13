using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using KSP.Localization;

// Created by Mike Billard (Angel-125) on 4/7/2021
namespace DART.SpaceObjects
{
    /// <summary>
    /// Describes a space object. Similar to asteroids, space anomalies are listed as unknown objects until tracked and visited. Each type of anomaly is defined by a SPACE_OBJECT config node.
    /// </summary>
    public class DARTSpaceObject
    {
        #region Constants
        /// <summary>
        /// Name of the space object's config node.
        /// </summary>
        public const string kNodeName = "SPACE_OBJECT";

        const string kObjectPrefix = "OBJ-";
        const string kName = "name";
        const string kPartName = "partName";
        const string kBodyOrbited = "bodyOrbited";
        const string kSMA = "orbitSMA";
        const string kEccentricity = "orbitEccentricity";
        const string kInclination = "orbitInclination";
        const string kMaxInstances = "maxInstances";
        const string kVesselId = "vesselId";
        const string kSizeClass = "sizeClass";
        const string kIsKnown = "isKnown";
        const string kVesselName = "vesselName";
        const string kVesselType = "vesselType";
        #endregion

        #region Fields
        /// <summary>
        /// Identifier for the space object.
        /// </summary>
        public string name = string.Empty;

        /// <summary>
        /// Name of the part to spawn
        /// </summary>
        public string partName = string.Empty;

        /// <summary>
        /// Space Objects are typically named "OBJ-" and a sequence of letters and numbers, but you can override the name of the vessel if desired.
        /// This field should be used with unique anomalies (maxInstances = 1).
        /// </summary>
        public string vesselName = string.Empty;

        /// <summary>
        /// Like asteroids, space objects have a size class that ranges from Size A (12 meters) to Size I (100+ meters).
        /// The default is A.
        /// </summary>
        public string sizeClass = "A";

        /// <summary>
        /// The celestial body to spawn around.
        /// </summary>
        public string bodyOrbited = string.Empty;

        /// <summary>
        /// The Semi-Major axis of the orbit.
        /// </summary>
        public double orbitSMA = 0;

        /// <summary>
        /// The eccentrcity of the orbit.
        /// </summary>
        public double orbitEccentricity = 0;

        /// <summary>
        /// Orbit inclination. If set to -1 then a random inclination will be used instead.
        /// </summary>
        public double orbitInclination = -1f;

        /// <summary>
        /// Maximum number of objects of this type that may exist at any given time. Default is 1.
        /// </summary>
        public int maxInstances = 1;

        /// <summary>
        /// ID of the vessel as found in the FlightGlobals.VesselsUnloaded.
        /// </summary>
        public string vesselId = string.Empty;

        /// <summary>
        /// Flag to indicate whether or not the space object is automatically tracked by the Tracking Station.
        /// </summary>
        public bool isKnown = false;

        /// <summary>
        /// Type of vessel to create.
        /// </summary>
        public VesselType vesselType = VesselType.SpaceObject;
        #endregion

        #region Housekeeping
        /// <summary>
        /// Last-used random seed value.
        /// </summary>
        protected int lastSeed = 0;
        #endregion

        #region Initializers
        /// <summary>
        /// Creates a space object from the supplied template node.
        /// </summary>
        /// <param name="templateNode">A ConfigNode representing the space object's template.</param>
        /// <returns>A DARTSpaceObject instance.</returns>
        public static DARTSpaceObject CreateFromNode(ConfigNode templateNode)
        {
            DARTSpaceObject spaceObject = new DARTSpaceObject();
            spaceObject.Load(spaceObject, templateNode);
            return spaceObject;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DARTSpaceObject()
        {

        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="copyFrom">A DARTSpaceObject to copy parameters from.</param>
        public DARTSpaceObject(DARTSpaceObject copyFrom)
        {
            CopyFrom(copyFrom);
        }

        /// <summary>
        /// Copies the fields from another space object.
        /// </summary>
        /// <param name="copyFrom">The DARTSpaceObject whose fields we're interested in.</param>
        public virtual void CopyFrom(DARTSpaceObject copyFrom)
        {
            name = copyFrom.name;
            partName = copyFrom.partName;
            vesselName = copyFrom.vesselName;
            sizeClass = copyFrom.sizeClass;
            bodyOrbited = copyFrom.bodyOrbited;
            orbitSMA = copyFrom.orbitSMA;
            orbitEccentricity = copyFrom.orbitEccentricity;
            orbitInclination = copyFrom.orbitInclination;
            maxInstances = copyFrom.maxInstances;
            vesselId = copyFrom.vesselId;
            isKnown = copyFrom.isKnown;
            vesselType = copyFrom.vesselType;

            lastSeed = UnityEngine.Random.Range(0, int.MaxValue);
            UnityEngine.Random.InitState(lastSeed);
        }

        /// <summary>
        /// Loads the ConfigNode data into the space object.
        /// </summary>
        /// <param name="spaceObject">A DARTSpaceObject to load the data into.</param>
        /// <param name="node">A ConfigNode containing serialized data.</param>
        public virtual void Load(DARTSpaceObject spaceObject, ConfigNode node)
        {
            if (node.HasValue(kName))
                spaceObject.name = node.GetValue(kName);

            if (node.HasValue(kPartName))
                spaceObject.partName = node.GetValue(kPartName);

            if (node.HasValue(kVesselName))
                spaceObject.vesselName = node.GetValue(kVesselName);

            if (node.HasValue(kSizeClass))
                spaceObject.sizeClass = node.GetValue(kSizeClass);

            if (node.HasValue(kBodyOrbited))
                spaceObject.bodyOrbited = node.GetValue(kBodyOrbited);

            if (node.HasValue(kSMA))
                double.TryParse(node.GetValue(kSMA), out spaceObject.orbitSMA);

            if (node.HasValue(kEccentricity))
                double.TryParse(node.GetValue(kEccentricity), out spaceObject.orbitEccentricity);

            if (node.HasValue(kInclination))
                double.TryParse(node.GetValue(kInclination), out spaceObject.orbitInclination);

            if (node.HasValue(kMaxInstances))
                int.TryParse(node.GetValue(kMaxInstances), out spaceObject.maxInstances);

            if (node.HasValue(kVesselId))
                spaceObject.vesselId = node.GetValue(kVesselId);

            if (node.HasValue(kIsKnown))
                bool.TryParse(node.GetValue(kIsKnown), out spaceObject.isKnown);

            if (node.HasValue(kVesselType))
                vesselType = (VesselType)Enum.Parse(typeof(VesselType), node.GetValue(kVesselType));
        }

        /// <summary>
        /// Serializes the space object to a ConfigNode.
        /// </summary>
        /// <param name="nodeName">A string containing the name of the node.</param>
        /// <returns>A ConfigNode with the serialized data.</returns>
        public virtual ConfigNode Save(string nodeName = kNodeName)
        {
            ConfigNode node = new ConfigNode(nodeName);

            node.AddValue(kName, name);
            node.AddValue(kPartName, partName);
            node.AddValue(kVesselName, vesselName);
            node.AddValue(kSizeClass, sizeClass);
            node.AddValue(kBodyOrbited, bodyOrbited);
            node.AddValue(kSMA, orbitSMA.ToString());
            node.AddValue(kEccentricity, orbitEccentricity.ToString());
            node.AddValue(kInclination, orbitInclination.ToString());
            node.AddValue(kMaxInstances, maxInstances.ToString());
            node.AddValue(kVesselId, vesselId);
            node.AddValue(kVesselType, vesselType.ToString());

            return node;
        }
        #endregion

        #region API
        /// <summary>
        /// Checks to see if we should create a new instance.
        /// </summary>
        public virtual void CreateNewInstancesIfNeeded(List<DARTSpaceObject> spaceObjects)
        {
            // Make sure that we can create at least one new instance.
            int currentInstanceCount = 0;
            if (!CanCreateNewInstance(spaceObjects, out currentInstanceCount))
                return;

            // Create the needed instances
            int instancesToCreate = maxInstances - currentInstanceCount;
            for (int index = 0; index < instancesToCreate; index++)
            {
                DARTSpaceObject spaceObject = new DARTSpaceObject(this);
                ConfigNode vesselNode = CreateVesselNode(spaceObject);
                spaceObjects.Add(spaceObject);
                DARTSpaceObjectScenario.onSpaceObjectCreated.Fire(spaceObject);
            }
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Creates the vessel node for the requested space object, and adds it to the current game.
        /// </summary>
        /// <param name="spaceObject">A DARTSpaceObject to base the new vessel on.</param>
        /// <returns>A ConfigNode containing the new vessel data.</returns>
        public virtual ConfigNode CreateVesselNode(DARTSpaceObject spaceObject)
        {
            // Generate vessel name
            if (string.IsNullOrEmpty(vesselName))
            {
                vesselName = DiscoverableObjectsUtil.GenerateAsteroidName();
                string prefix = Localizer.Format("#autoLOC_6001923");
                prefix = prefix.Replace(" <<1>>", "");
                vesselName = vesselName.Replace(prefix, kObjectPrefix);
                vesselName = vesselName.Replace("- ", "-");
            }

            // Generate orbit
            Orbit orbit = GenerateOrbit(spaceObject);

            // Create part node
            ConfigNode partNode = ProtoVessel.CreatePartNode(spaceObject.partName, 0);

            // Determine lifetime
            double minLifetime = double.MaxValue;
            double maxLifetime = double.MaxValue;

            // Setup object class
            UntrackedObjectClass objectClass = UntrackedObjectClass.A;
            Enum.TryParse(spaceObject.sizeClass, out objectClass);

            // Create discovery and additional nodes.
            DiscoveryLevels discoveryLevels = !isKnown ? DiscoveryLevels.Presence : DiscoveryLevels.StateVectors;
            ConfigNode discoveryNode = ProtoVessel.CreateDiscoveryNode(discoveryLevels, objectClass, minLifetime, maxLifetime);
            ConfigNode[] additionalNodes = new ConfigNode[] { new ConfigNode("ACTIONGROUPS"), discoveryNode };

            // Create vessel node
            ConfigNode vesselNode = ProtoVessel.CreateVesselNode(vesselName, vesselType, orbit, 0, new ConfigNode[] { partNode }, additionalNodes);

            // Add vessel node to the game.
            ProtoVessel protoVessel = HighLogic.CurrentGame.AddVessel(vesselNode);

            // Get vessel ID
            if (vesselNode.HasValue("pid"))
                spaceObject.vesselId = vesselNode.GetValue("pid");

            Debug.Log("[DARTSpaceObject] - Created new vessel named " + spaceObject.vesselName);
            Debug.Log("[DARTSpaceObject] - vesselNode: " + vesselNode.ToString());
            return vesselNode;
        }

        /// <summary>
        /// Generates a new orbit for the supplied space object.
        /// </summary>
        /// <param name="spaceObject">A DARTSpaceObject with orbit parameters</param>
        /// <returns>An Orbit if one can be generated for the space object's orbited body, or null if not.</returns>
        public virtual Orbit GenerateOrbit(DARTSpaceObject spaceObject)
        {
            Orbit orbit = null;
            CelestialBody body = null;

            body = FlightGlobals.GetBodyByName(spaceObject.bodyOrbited);
            if (body != null)
            {
                double inclination = spaceObject.orbitInclination;
                if (inclination < 0)
                    inclination = UnityEngine.Random.Range(0, 90);
                return new Orbit(inclination, spaceObject.orbitEccentricity, spaceObject.orbitSMA, 0, 0, 0, Planetarium.GetUniversalTime(), body);
            }

            return orbit;
        }

        /// <summary>
        /// Determines whether or not we can create new instances.
        /// </summary>
        /// <param name="existingSpaceObjects">A List of DARTSpaceObject containg the current space object instances.</param>
        /// <param name="currentInstanceCount">A count of the current number of instances of the space object.</param>
        /// <returns>True if we can create a new instance, false if not.</returns>
        public virtual bool CanCreateNewInstance(List<DARTSpaceObject> existingSpaceObjects, out int currentInstanceCount)
        {
            currentInstanceCount = 0;

            // Count the number of existing instances.
            int count = existingSpaceObjects.Count;
            for (int index = 0; index < count; index++)
            {
                if (existingSpaceObjects[index].partName == partName)
                    currentInstanceCount += 1;
            }

            return currentInstanceCount < maxInstances;
        }
        #endregion
    }
}