/**
 * Kopernicus ConfigNode Parser
 * -------------------------------------------------------------
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston,
 * MA 02110-1301  USA
 *
 * This library is intended to be used as a plugin for Kerbal Space Program
 * which is copyright of TakeTwo Interactive. Your usage of Kerbal Space Program
 * itself is governed by the terms of its EULA, not the license above.
 *
 * https://kerbalspaceprogram.com
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Kopernicus.ConfigParser.Attributes;
using Kopernicus.ConfigParser.Enumerations;
using Kopernicus.ConfigParser.Exceptions;
using Kopernicus.ConfigParser.Interfaces;
using Object = System.Object;

namespace Kopernicus.ConfigParser
{
    /// <summary>
    /// Class which manages loading from config nodes via reflection and attribution
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// A list of states that can be shared through the parser
        /// </summary>
        private static readonly Dictionary<String, Func<Object>> States = new Dictionary<String, Func<Object>>();

        /// <summary>
        /// Create an object form a configuration node (Generic)
        /// </summary>
        /// <typeparam name="T">The resulting class</typeparam>
        /// <param name="node">The node with the data that should be loaded</param>
        /// <param name="configName">The name of the mod that corresponds to the entry in ParserOptions</param>
        /// <param name="getChildren">Whether getters on the object should get called</param>
        /// <returns></returns>
        public static T CreateObjectFromConfigNode<T>(ConfigNode node, String configName = "Default",
            Boolean getChildren = true) where T : class, new()
        {
            T o = new T();
            LoadObjectFromConfigurationNode(o, node, configName, getChildren);
            return o;
        }

        /// <summary>
        /// Create an object form a configuration node (Runtime type identification)
        /// </summary>
        /// <param name="type">The resulting class</param>
        /// <param name="node">The node with the data that should be loaded</param>
        /// <param name="configName">The name of the mod that corresponds to the entry in ParserOptions</param>
        /// <param name="getChildren">Whether getters on the object should get called</param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public static Object CreateObjectFromConfigNode(Type type, ConfigNode node, String configName = "Default",
            Boolean getChildren = true)
        {
            Object o = Activator.CreateInstance(type);
            LoadObjectFromConfigurationNode(o, node, configName, getChildren);
            return o;
        }

        /// <summary>
        /// Create an object form a configuration node (Runtime type identification) with constructor parameters
        /// </summary>
        /// <param name="type">The resulting class</param>
        /// <param name="node">The node with the data that should be loaded</param>
        /// <param name="arguments">Arguments that should be passed to the constructor</param>
        /// <param name="configName">The name of the mod that corresponds to the entry in ParserOptions</param>
        /// <param name="getChildren">Whether getters on the object should get called</param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public static Object CreateObjectFromConfigNode(Type type, ConfigNode node, Object[] arguments,
            String configName = "Default", Boolean getChildren = true)
        {
            Object o = Activator.CreateInstance(type, arguments);
            LoadObjectFromConfigurationNode(o, node, configName, getChildren);
            return o;
        }

        /// <summary>
        /// Load data for an object's ParserTarget fields and properties from a configuration node
        /// </summary>
        /// <param name="o">Object for which to load data.  Needs to be instantiated object</param>
        /// <param name="node">Configuration node from which to load data</param>
        /// <param name="configName">The name of the mod that corresponds to the entry in ParserOptions</param>
        /// <param name="getChildren">Whether getters on the object should get called</param>
        public static void LoadObjectFromConfigurationNode(Object o, ConfigNode node, String configName = "Default",
            Boolean getChildren = true)
        {
            // Get the object as a parser event subscriber (will be null if 'o' does not conform)
            IParserEventSubscriber subscriber = o as IParserEventSubscriber;
            IParserApplyEventSubscriber applySubscriber = o as IParserApplyEventSubscriber;
            IParserPostApplyEventSubscriber postApplySubscriber = o as IParserPostApplyEventSubscriber;

            // Generate two lists -> those tagged preApply and those not
            List<KeyValuePair<Boolean, MemberInfo>> preApplyMembers = new List<KeyValuePair<Boolean, MemberInfo>>();
            List<KeyValuePair<Boolean, MemberInfo>> postApplyMembers = new List<KeyValuePair<Boolean, MemberInfo>>();

            // Discover members tagged with parser attributes
            foreach (MemberInfo member in o.GetType()
                .GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                // Is this member a parser target?
                ParserTarget[] attributes = (ParserTarget[]) member.GetCustomAttributes(typeof(ParserTarget), true);

                if (attributes.Length <= 0)
                {
                    continue;
                }

                // If this member is a collection
                Boolean isCollection = attributes[0].GetType() == typeof(ParserTargetCollection);

                // If this member has the preApply attribute, we need to process it
                if (member.GetCustomAttributes(typeof(PreApply), true).Length > 0)
                {
                    preApplyMembers.Add(new KeyValuePair<Boolean, MemberInfo>(isCollection, member));
                }
                else
                {
                    postApplyMembers.Add(new KeyValuePair<Boolean, MemberInfo>(isCollection, member));
                }
            }

            // Process the preApply members
            foreach (KeyValuePair<Boolean, MemberInfo> member in preApplyMembers)
            {
                if (member.Key)
                {
                    LoadCollectionMemberFromConfigurationNode(member.Value, o, node, configName, getChildren);
                }
                else
                {
                    LoadObjectMemberFromConfigurationNode(member.Value, o, node, configName, getChildren);
                }
            }

            // Call Apply
            applySubscriber?.Apply(node);
            subscriber?.Apply(node);

            // Process the postApply members
            foreach (KeyValuePair<Boolean, MemberInfo> member in postApplyMembers)
            {
                if (member.Key)
                {
                    LoadCollectionMemberFromConfigurationNode(member.Value, o, node, configName, getChildren);
                }
                else
                {
                    LoadObjectMemberFromConfigurationNode(member.Value, o, node, configName, getChildren);
                }
            }

            // Call PostApply
            postApplySubscriber?.PostApply(node);
            subscriber?.PostApply(node);
        }

        /// <summary>
        /// Load collection for ParserTargetCollection
        /// </summary>
        /// <param name="member">Member to load data for</param>
        /// <param name="o">Instance of the object which owns member</param>
        /// <param name="node">Configuration node from which to load data</param>
        /// <param name="configName">The name of the mod that corresponds to the entry in ParserOptions</param>
        /// <param name="getChildren">Whether getters on the object should get called</param>
        private static void LoadCollectionMemberFromConfigurationNode(MemberInfo member, Object o, ConfigNode node,
            String configName = "Default", Boolean getChildren = true)
        {
            // Get the target attributes
            ParserTargetCollection[] targets =
                (ParserTargetCollection[]) member.GetCustomAttributes(typeof(ParserTargetCollection), true);

            // Process the target attributes
            foreach (ParserTargetCollection target in targets)
            {
                // Figure out if this field exists and if we care
                Boolean isNode = node.HasNode(target.FieldName) || target.FieldName == "self";
                Boolean isValue = node.HasValue(target.FieldName);

                // Obtain the type the member is (can only be field or property)
                Type targetType;
                Object targetValue = null;
                if (member.MemberType == MemberTypes.Field)
                {
                    targetType = ((FieldInfo) member).FieldType;
                    targetValue = getChildren ? ((FieldInfo) member).GetValue(o) : null;
                }
                else
                {
                    targetType = ((PropertyInfo) member).PropertyType;
                    try
                    {
                        if (((PropertyInfo) member).CanRead && getChildren)
                        {
                            targetValue = ((PropertyInfo) member).GetValue(o, null);
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                // Get settings data
                ParserOptions.Data data = ParserOptions.Options[configName];

                // Log
                data.LogCallback("Parsing Target " + target.FieldName + " in (" + o.GetType() + ") as (" + targetType +
                                 ")");

                // If there was no data found for this node
                if (!isNode && !isValue)
                {
                    if (!target.Optional && !(target.AllowMerge && targetValue != null))
                    {
                        // Error - non optional field is missing
                        throw new ParserTargetMissingException(
                            "Missing non-optional field: " + o.GetType() + "." + target.FieldName);
                    }

                    // Nothing to do, so return
                    continue;
                }

                // If we are dealing with a generic collection
                if (targetType.IsGenericType)
                {
                    // If the target is a generic dictionary
                    if (typeof(IDictionary).IsAssignableFrom(targetType))
                    {
                        // We need a node for this decoding
                        if (!isNode)
                        {
                            throw new Exception("Loading a generic dictionary requires sources to be nodes");
                        }

                        // Get the target value as a dictionary
                        IDictionary collection = targetValue as IDictionary;

                        // Get the internal type of this collection
                        Type genericTypeA = targetType.GetGenericArguments()[0];
                        Type genericTypeB = targetType.GetGenericArguments()[1];

                        // Create a new collection if merge is disallowed or if the collection is null
                        if (collection == null || !target.AllowMerge)
                        {
                            collection = Activator.CreateInstance(targetType) as IDictionary;
                            targetValue = collection;
                        }

                        // Process the node
                        ConfigNode targetNode = target.FieldName == "self" ? node : node.GetNode(target.FieldName);

                        // Check the config type
                        RequireConfigType[] attributes =
                            (RequireConfigType[]) genericTypeA.GetCustomAttributes(typeof(RequireConfigType), true);
                        if (attributes.Length > 0)
                        {
                            if (attributes[0].Type == ConfigType.Node)
                            {
                                throw new ParserTargetTypeMismatchException(
                                    "The key value of a generic dictionary must be a Value");
                            }
                        }

                        attributes =
                            (RequireConfigType[]) genericTypeB.GetCustomAttributes(typeof(RequireConfigType), true);
                        if (attributes.Length > 0 || genericTypeB == typeof(String))
                        {
                            ConfigType type = genericTypeB == typeof(String) ? ConfigType.Value : attributes[0].Type;
                            if (type == ConfigType.Node)
                            {
                                // Iterate over all of the nodes in this node
                                foreach (ConfigNode subnode in targetNode.nodes)
                                {
                                    // Check for the name significance
                                    switch (target.NameSignificance)
                                    {
                                        case NameSignificance.None:
                                            // Just processes the contents of the node
                                            collection.Add(ProcessValue(genericTypeA, subnode.name),
                                                CreateObjectFromConfigNode(genericTypeB, subnode, configName,
                                                    target.GetChild));
                                            break;
                                        case NameSignificance.Type:
                                            throw new Exception(
                                                "NameSignificance.Type isn't supported on generic dictionaries.");
                                        case NameSignificance.Key:
                                            throw new Exception(
                                                "NameSignificance.Key isn't supported on generic dictionaries");
                                        default:
                                            throw new ArgumentOutOfRangeException();
                                    }
                                }
                            }
                            else
                            {
                                // Iterate over all of the values in this node
                                foreach (ConfigNode.Value value in targetNode.values)
                                {
                                    // Check for the name significance
                                    switch (target.NameSignificance)
                                    {
                                        case NameSignificance.None:
                                            collection.Add(ProcessValue(genericTypeA, value.name),
                                                ProcessValue(genericTypeB, value.value));
                                            break;
                                        case NameSignificance.Type:
                                            throw new Exception(
                                                "NameSignificance.Type isn't supported on generic dictionaries.");
                                        case NameSignificance.Key:
                                            throw new Exception(
                                                "NameSignificance.Key isn't supported on generic dictionaries");
                                        default:
                                            throw new ArgumentOutOfRangeException();
                                    }
                                }
                            }
                        }
                    }

                    // If the target is a generic collection
                    else if (typeof(IList).IsAssignableFrom(targetType))
                    {
                        // We need a node for this decoding
                        if (!isNode)
                        {
                            throw new Exception("Loading a generic list requires sources to be nodes");
                        }

                        // Get the target value as a collection
                        IList collection = targetValue as IList;

                        // Get the internal type of this collection
                        Type genericType = targetType.GetGenericArguments()[0];

                        // Create a new collection if merge is disallowed or if the collection is null
                        if (collection == null || !target.AllowMerge)
                        {
                            collection = Activator.CreateInstance(targetType) as IList;
                            targetValue = collection;
                        }

                        // Store the objects that were already patched
                        List<Object> patched = new List<Object>();

                        // Process the node
                        ConfigNode targetNode = target.FieldName == "self" ? node : node.GetNode(target.FieldName);

                        // Check the config type
                        RequireConfigType[] attributes =
                            (RequireConfigType[]) genericType.GetCustomAttributes(typeof(RequireConfigType), true);
                        if (attributes.Length > 0 || genericType == typeof(String))
                        {
                            ConfigType type = genericType == typeof(String) ? ConfigType.Value : attributes[0].Type;
                            if (type == ConfigType.Node)
                            {
                                // Iterate over all of the nodes in this node
                                foreach (ConfigNode subnode in targetNode.nodes)
                                {
                                    // Check for the name significance
                                    switch (target.NameSignificance)
                                    {
                                        case NameSignificance.None:
                                        case NameSignificance.Key when subnode.name == target.Key:
                                            
                                            // Check if the type represents patchable data
                                            Object current = null;
                                            if (typeof(IPatchable).IsAssignableFrom(genericType) &&
                                                collection.Count > 0)
                                            {
                                                for (Int32 i = 0; i < collection.Count; i++)
                                                {
                                                    if (collection[i].GetType() != genericType)
                                                    {
                                                        continue;
                                                    }

                                                    if (patched.Contains(collection[i]))
                                                    {
                                                        continue;
                                                    }

                                                    IPatchable patchable = (IPatchable) collection[i];
                                                    PatchData patchData =
                                                        CreateObjectFromConfigNode<PatchData>(subnode, "Internal");
                                                    if (patchData.name == patchable.name)
                                                    {
                                                        // Name matches, check for an index
                                                        if (patchData.index == collection.IndexOf(collection[i]))
                                                        {
                                                            // Both values match
                                                            current = collection[i];
                                                            break;
                                                        }

                                                        if (patchData.index > -1)
                                                        {
                                                            // Index doesn't match, continue
                                                            continue;
                                                        }

                                                        // Name matches, and no index exists
                                                        current = collection[i];
                                                        break;

                                                    }

                                                    if (patchData.name != null)
                                                    {
                                                        // The name doesn't match, continue the search
                                                        continue;
                                                    }

                                                    // We found the first object that wasn't patched yet
                                                    current = collection[i];
                                                    break;
                                                }
                                            }

                                            // If no object was found, check if the type implements custom constructors
                                            if (current == null)
                                            {
                                                current = Activator.CreateInstance(genericType);
                                                collection?.Add(current);
                                            }

                                            // Parse the config node into the object
                                            LoadObjectFromConfigurationNode(current, subnode, configName,
                                                target.GetChild);
                                            patched.Add(current);
                                            if (collection != null)
                                            {
                                                collection[collection.IndexOf(current)] = current;
                                            }

                                            break;

                                        case NameSignificance.Type:

                                            // Generate the type from the name
                                            Type elementType = ModTypes.FirstOrDefault(t =>
                                                t.Name == subnode.name &&
                                                !Equals(t.Assembly, typeof(HighLogic).Assembly) &&
                                                genericType.IsAssignableFrom(t));

                                            // Check if the type represents patchable data
                                            current = null;
                                            if (typeof(IPatchable).IsAssignableFrom(elementType) &&
                                                collection.Count > 0)
                                            {
                                                for (Int32 i = 0; i < collection.Count; i++)
                                                {
                                                    if (collection[i].GetType() != elementType)
                                                    {
                                                        continue;
                                                    }

                                                    if (patched.Contains(collection[i]))
                                                    {
                                                        continue;
                                                    }

                                                    IPatchable patchable = (IPatchable) collection[i];
                                                    PatchData patchData =
                                                        CreateObjectFromConfigNode<PatchData>(subnode, "Internal");
                                                    if (patchData.name == patchable.name)
                                                    {
                                                        // Name matches, check for an index
                                                        if (patchData.index == i)
                                                        {
                                                            // Both values match
                                                            current = collection[i];
                                                            break;
                                                        }

                                                        if (patchData.index > -1)
                                                        {
                                                            // Index doesn't match, continue
                                                            continue;
                                                        }

                                                        // Name matches, and no index exists
                                                        current = collection[i];
                                                        break;

                                                    }

                                                    if (patchData.name != null)
                                                    {
                                                        // The name doesn't match, continue the search
                                                        continue;
                                                    }

                                                    // We found the first object that wasn't patched yet
                                                    current = collection[i];
                                                    break;
                                                }
                                            }

                                            // If no object was found, check if the type implements custom constructors
                                            if (current == null)
                                            {
                                                current = Activator.CreateInstance(elementType);
                                                collection?.Add(current);
                                                if (typeof(ICreatable).IsAssignableFrom(elementType))
                                                {
                                                    ICreatable creatable = (ICreatable) current;
                                                    creatable.Create();
                                                }
                                            }

                                            // Parse the config node into the object
                                            LoadObjectFromConfigurationNode(current, subnode, configName,
                                                target.GetChild);
                                            patched.Add(current);
                                            if (collection != null)
                                            {
                                                collection[collection.IndexOf(current)] = current;
                                            }

                                            break;

                                        default:
                                            continue;
                                    }
                                }
                            }
                            else
                            {
                                // Iterate over all of the nodes in this node
                                foreach (ConfigNode.Value value in targetNode.values)
                                {
                                    // Check for the name significance
                                    switch (target.NameSignificance)
                                    {
                                        case NameSignificance.None:

                                            // Just processes the contents of the node
                                            collection?.Add(ProcessValue(genericType, value.value));
                                            break;

                                        case NameSignificance.Type:

                                            // Generate the type from the name
                                            Type elementType = ModTypes.FirstOrDefault(t =>
                                                t.Name == value.name &&
                                                !Equals(t.Assembly, typeof(HighLogic).Assembly) &&
                                                genericType.IsAssignableFrom(t));

                                            // Add the object to the collection
                                            collection?.Add(ProcessValue(elementType, value.value));
                                            break;

                                        case NameSignificance.Key when value.name == target.Key:

                                            // Just processes the contents of the node
                                            collection?.Add(ProcessValue(genericType, value.value));
                                            break;

                                        default:
                                            continue;
                                    }
                                }
                            }
                        }
                    }
                }

                // If we are dealing with a non generic collection
                else
                {
                    // Check for invalid scenarios
                    if (target.NameSignificance == NameSignificance.None)
                    {
                        throw new Exception(
                            "Can not infer type from non generic target; can not infer type from zero name significance");
                    }
                }

                // If the member type is a field, set the value
                if (member.MemberType == MemberTypes.Field)
                {
                    ((FieldInfo) member).SetValue(o, targetValue);
                }

                // If the member wasn't a field, it must be a property.  If the property is writable, set it.
                else if (((PropertyInfo) member).CanWrite)
                {
                    ((PropertyInfo) member).SetValue(o, targetValue, null);
                }
            }
        }

        /// <summary>
        /// Load data for ParserTarget field or property from a configuration node
        /// </summary>
        /// <param name="member">Member to load data for</param>
        /// <param name="o">Instance of the object which owns member</param>
        /// <param name="node">Configuration node from which to load data</param>
        /// <param name="configName">The name of the mod that corresponds to the entry in ParserOptions</param>
        /// <param name="getChildren">Whether getters on the object should get called</param>
        private static void LoadObjectMemberFromConfigurationNode(MemberInfo member, Object o, ConfigNode node,
            String configName = "Default", Boolean getChildren = true)
        {
            // Get the parser targets
            ParserTarget[] targets = (ParserTarget[]) member.GetCustomAttributes(typeof(ParserTarget), true);

            // Process the targets
            foreach (ParserTarget target in targets)
            {
                // Figure out if this field exists and if we care
                Boolean isNode = node.GetNodes()
                    .Any(n => n.name.StartsWith(target.FieldName + ":") || n.name == target.FieldName);
                Boolean isValue = node.HasValue(target.FieldName);

                // Obtain the type the member is (can only be field or property)
                Type targetType;
                Object targetValue = null;
                if (member.MemberType == MemberTypes.Field)
                {
                    targetType = ((FieldInfo) member).FieldType;
                    targetValue = getChildren ? ((FieldInfo) member).GetValue(o) : null;
                }
                else
                {
                    targetType = ((PropertyInfo) member).PropertyType;
                    try
                    {
                        if (((PropertyInfo) member).CanRead && getChildren)
                        {
                            targetValue = ((PropertyInfo) member).GetValue(o, null);
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                // Get settings data
                ParserOptions.Data data = ParserOptions.Options[configName];

                // Log
                data.LogCallback("Parsing Target " + target.FieldName + " in (" + o.GetType() + ") as (" + targetType +
                                 ")");

                // If there was no data found for this node
                if (!isNode && !isValue)
                {
                    if (!target.Optional && !(target.AllowMerge && targetValue != null))
                    {
                        // Error - non optional field is missing
                        throw new ParserTargetMissingException(
                            "Missing non-optional field: " + o.GetType() + "." + target.FieldName);
                    }

                    // Nothing to do, so DONT return!
                    continue;
                }

                // Does this node have a required config source type (and if so, check if valid)
                RequireConfigType[] attributes =
                    (RequireConfigType[]) targetType.GetCustomAttributes(typeof(RequireConfigType), true);
                if (attributes.Length > 0)
                {
                    if (attributes[0].Type == ConfigType.Node && !isNode ||
                        attributes[0].Type == ConfigType.Value && !isValue)
                    {
                        throw new ParserTargetTypeMismatchException(
                            target.FieldName + " requires config value of " + attributes[0].Type);
                    }
                }

                // If this object is a value (attempt no merge here)
                if (isValue)
                {
                    // Process the value
                    Object val = ProcessValue(targetType, node.GetValue(target.FieldName));

                    // Throw exception or print error
                    if (val == null)
                    {
                        data.LogCallback("[Kopernicus]: Configuration.Parser: ParserTarget \"" + target.FieldName +
                                         "\" is a non parsable type: " + targetType);
                        continue;
                    }

                    targetValue = val;
                }

                // If this object is a node (potentially merge)
                else
                {
                    // If the target type is a ConfigNode, this works natively
                    if (targetType == typeof(ConfigNode))
                    {
                        targetValue = node.GetNode(target.FieldName);
                    }

                    // We need to get an instance of the object we are trying to populate
                    // If we are not allowed to merge, or the object does not exist, make a new instance
                    // Otherwise we can merge this value
                    else if (targetValue == null || !target.AllowMerge)
                    {
                        if (!targetType.IsAbstract)
                        {
                            targetValue = Activator.CreateInstance(targetType);
                            
                        }
                    }

                    // Check for the name significance
                    switch (target.NameSignificance)
                    {
                        case NameSignificance.None:

                            // Just processes the contents of the node
                            LoadObjectFromConfigurationNode(targetValue, node.GetNode(target.FieldName), configName,
                                target.GetChild);
                            break;

                        case NameSignificance.Type:

                            // Generate the type from the name
                            ConfigNode subnode = node.GetNodes().First(n => n.name.StartsWith(target.FieldName + ":"));
                            String[] split = subnode.name.Split(':');
                            Type elementType = ModTypes.FirstOrDefault(t =>
                                t.Name == split[1] && !Equals(t.Assembly, typeof(HighLogic).Assembly) &&
                                targetType.IsAssignableFrom(t));

                            // If no object was found, check if the type implements custom constructors
                            targetValue = Activator.CreateInstance(elementType);
                            if (typeof(ICreatable).IsAssignableFrom(elementType))
                            {
                                ICreatable creatable = (ICreatable) targetValue;
                                creatable.Create();
                            }

                            // Parse the config node into the object
                            LoadObjectFromConfigurationNode(targetValue, subnode, configName, target.GetChild);
                            break;

                        case NameSignificance.Key:
                            throw new Exception("NameSignificance.Key is not supported on ParserTargets");
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                // If the member type is a field, set the value
                if (member.MemberType == MemberTypes.Field)
                {
                    ((FieldInfo) member).SetValue(o, targetValue);
                }

                // If the member wasn't a field, it must be a property.  If the property is writable, set it.
                else if (((PropertyInfo) member).CanWrite)
                {
                    ((PropertyInfo) member).SetValue(o, targetValue, null);
                }
            }
        }

        /// <summary>
        /// Processes a value from a config
        /// </summary>
        /// <returns></returns>
        private static Object ProcessValue(Type targetType, String nodeValue)
        {
            // If the target is a String, it works natively
            if (targetType == typeof(String))
            {
                return nodeValue;
            }

            // Figure out if this object is a parsable type
            if (!typeof(IParsable).IsAssignableFrom(targetType))
            {
                return null;
            }

            // Create a new object
            IParsable targetParsable = (IParsable) Activator.CreateInstance(targetType);
            targetParsable.SetFromString(nodeValue);
            return targetParsable;
        }

        /// <summary>
        /// Loads ParserTargets from other assemblies in GameData/
        /// </summary>
        public static void LoadParserTargetsExternal(ConfigNode node, String modName, String configName = "Default",
            Boolean getChildren = true)
        {
            LoadExternalParserTargets(node, modName, configName, getChildren);
            foreach (ConfigNode childNode in node.GetNodes())
            {
                LoadParserTargetsExternal(childNode, configName, modName, getChildren);
            }
        }

        /// <summary>
        /// Loads ParserTargets from other assemblies in GameData/
        /// </summary>
        private static void LoadExternalParserTargets(ConfigNode node, String calling, String configName = "Default",
            Boolean getChildren = true)
        {
            // Look for types in other assemblies with the ExternalParserTarget attribute and the parentNodeName equal to this node's name
            try
            {
                foreach (Type type in ModTypes)
                {
                    try
                    {
                        ParserTargetExternal[] attributes =
                            (ParserTargetExternal[]) type.GetCustomAttributes(typeof(ParserTargetExternal), false);
                        if (attributes.Length == 0)
                        {
                            continue;
                        }

                        ParserTargetExternal external = attributes[0];
                        if (node.name != external.ParentNodeName)
                        {
                            continue;
                        }

                        if (calling != external.ModName)
                        {
                            continue;
                        }

                        String nodeName = external.ConfigNodeName ?? type.Name;

                        // Get settings data
                        ParserOptions.Data data = ParserOptions.Options[configName];

                        if (!node.HasNode(nodeName))
                        {
                            continue;
                        }

                        try
                        {
                            data.LogCallback("Parsing ParserTarget " + nodeName + " in node " +
                                             external.ParentNodeName + " from Assembly " + type.Assembly.FullName);
                            ConfigNode nodeToLoad = node.GetNode(nodeName);
                            CreateObjectFromConfigNode(type, nodeToLoad, configName, getChildren);
                        }
                        catch (MissingMethodException missingMethod)
                        {
                            data.LogCallback("Failed to load ParserTargetExternal " + nodeName +
                                             " because it does not have a parameter-less constructor");
                            data.ErrorCallback(missingMethod);
                        }
                        catch (Exception exception)
                        {
                            data.LogCallback("Failed to load ParserTargetExternal " + nodeName + " from node " +
                                             external.ParentNodeName);
                            data.ErrorCallback(exception);
                        }
                    }
                    catch (TypeLoadException)
                    {
                        // ignored
                    }
                }
            }
            catch (ReflectionTypeLoadException)
            {
            }
        }

        /// <summary>
        /// Adds a new state accessor into the parser
        /// </summary>
        public static void SetState<T>(String name, Func<T> accessor)
        {
            if (States.ContainsKey(name))
            {
                throw new InvalidOperationException();
            }

            States.Add(name, () => accessor());
        }

        /// <summary>
        /// Removes a state accessor from the parser
        /// </summary>
        public static void ClearState(String name)
        {
            if (!States.ContainsKey(name))
            {
                throw new IndexOutOfRangeException();
            }

            States.Remove(name);
        }

        /// <summary>
        /// Accesses the shared state
        /// </summary>
        public static T GetState<T>(String name)
        {
            if (!States.ContainsKey(name))
            {
                throw new IndexOutOfRangeException();
            }

            return (T) States[name]();
        }

        // Custom Assembly query since AppDomain and Assembly loader are not quite what we want in 1.1
        private static List<Type> _modTypes;

        public static List<Type> ModTypes
        {
            get
            {
                if (_modTypes == null)
                {
                    GetModTypes();
                }

                return _modTypes;
            }
        }

        private static void GetModTypes()
        {
            _modTypes = new List<Type>();
            List<Assembly> assemblies = new List<Assembly>();
            assemblies.AddRange(AssemblyLoader.loadedAssemblies.Select(la => la.assembly));
            assemblies.AddUnique(typeof(PQSMod_VertexSimplexHeightAbsolute).Assembly);
            assemblies.AddUnique(typeof(PQSLandControl).Assembly);
            foreach (Type t in assemblies.SelectMany(a => a.GetTypes()))
            {
                _modTypes.Add(t);
            }
        }
    }
}