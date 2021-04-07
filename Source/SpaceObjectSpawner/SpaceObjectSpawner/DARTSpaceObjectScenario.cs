using System;
using System.Collections.Generic;
using UnityEngine;

// Created by Mike Billard (Angel-125) on 4/7/2021
namespace DART.SpaceObjects
{
    /// <summary>
    ///  This class helps keep track of custom space objects.
    /// </summary>
    [KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.SPACECENTER)]
    public class DARTSpaceObjectScenario: ScenarioModule
    {
        #region Constants
        const string kSpaceObjectSettingsNode = "DART_SPACEOBJECT_SETTINGS";
        const string kRecreateObjectsOnStart = "recreateObjectsOnStart";
        #endregion

        #region Housekeeping
        /// <summary>
        /// Shared instance of the helper.
        /// </summary>
        public static DARTSpaceObjectScenario shared;

        /// <summary>
        /// List of space object instances
        /// </summary>
        public List<DARTSpaceObject> spaceObjects;

        /// <summary>
        /// List of templates used to create space objects.
        /// </summary>
        public List<DARTSpaceObject> spaceObjectTemplates;

        /// <summary>
        /// Flag to indicate whether we need to recreate the space object upon game start.
        /// </summary>
        bool recreateObjectsOnStart = false;
        #endregion

        #region Overrides
        /// <summary>
        /// Handles the start of the scenario by creating new space object instances as needed.
        /// </summary>
        public void Start()
        {
            if (spaceObjects == null || spaceObjectTemplates == null)
                return;

            // Delete old instances if needed
            if (recreateObjectsOnStart)
                deleteCurrentInstances();

            // Create new instances if needed.
            int count = spaceObjectTemplates.Count;
            DARTSpaceObject spaceObjectTemplate;
            for (int index = 0; index < count; index++)
            {
                spaceObjectTemplate = spaceObjectTemplates[index];
                spaceObjectTemplate.CreateNewInstancesIfNeeded(spaceObjects);
            }
        }

        /// <summary>
        /// Handles awake tasks.
        /// </summary>
        public override void OnAwake()
        {
            base.OnAwake();
            shared = this;

            spaceObjects = new List<DARTSpaceObject>();
            spaceObjectTemplates = new List<DARTSpaceObject>();

            loadSettings();
        }

        /// <summary>
        /// Loads the templates and space object instances
        /// </summary>
        /// <param name="node">A ConfigNode containing serialized object data.</param>
        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            // Load templates
            ConfigNode[] templateNodes = GameDatabase.Instance.GetConfigNodes(DARTSpaceObject.kNodeName);
            DARTSpaceObject spaceObject;
            if (templateNodes != null)
            {
                for (int index = 0; index < templateNodes.Length; index++)
                {
                    spaceObject = DARTSpaceObject.CreateFromNode(templateNodes[index]);
                    spaceObjectTemplates.Add(spaceObject);
                }
            }

            // Load space object instances list.
            if (node.HasNode(DARTSpaceObject.kNodeName))
            {
                ConfigNode[] nodes = node.GetNodes(DARTSpaceObject.kNodeName);
                for (int index = 0; index < nodes.Length; index++)
                    spaceObjects.Add(DARTSpaceObject.CreateFromNode(nodes[index]));
            }
        }

        /// <summary>
        /// Saves object data to the supplied config node.
        /// </summary>
        /// <param name="node">A ConfigNode to add object data to.</param>
        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);

            // Space objects
            int count = spaceObjects.Count;
            for (int index = 0; index < count; index++)
                node.AddNode(spaceObjects[index].Save());
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Attempts to locate the destination vessel based on the ID supplied.
        /// </summary>
        /// <param name="vesselID">A string containing the vessel ID</param>
        /// <returns>A Vessel if one can be found, null if not.</returns>
        public Vessel GetVessel(string vesselID)
        {
            // Check unloaded vessels first.
            int count = FlightGlobals.VesselsUnloaded.Count;
            string pid;
            string vesselPID = vesselID.Replace("-", "");
            Vessel vessel;

            for (int index = 0; index < count; index++)
            {
                vessel = FlightGlobals.VesselsUnloaded[index];
                pid = vessel.id.ToString().Replace("-", "");
                if (pid == vesselPID)
                    return vessel;
            }

            // Check loaded vessels.
            count = FlightGlobals.VesselsLoaded.Count;
            for (int index = 0; index < count; index++)
            {
                vessel = FlightGlobals.VesselsLoaded[index];
                pid = vessel.id.ToString().Replace("-", "");
                if (pid == vesselPID)
                    return vessel;
            }

            return null;
        }
        #endregion

        #region Helpers
        private void loadSettings()
        {
            // Load the settings for the scenario.
            ConfigNode[] nodes = GameDatabase.Instance.GetConfigNodes(kSpaceObjectSettingsNode);
            if (nodes != null && nodes.Length > 0)
            {
                ConfigNode nodeSettings = nodes[0];

                if (nodeSettings.HasValue(kRecreateObjectsOnStart))
                    bool.TryParse(nodeSettings.GetValue(kRecreateObjectsOnStart), out recreateObjectsOnStart);
            }
        }

        private void deleteCurrentInstances()
        {
            int count = spaceObjects.Count;
            Vessel doomed;
            for (int index = 0; index < count; index++)
            {
                doomed = GetVessel(spaceObjects[index].vesselId);
                if (doomed != null)
                    FlightGlobals.RemoveVessel(doomed);
            }

            spaceObjects.Clear();
        }
        #endregion
    }
}
