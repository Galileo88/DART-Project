/**
 * Kopernicus Planetary System Modifier
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
using System.Diagnostics.CodeAnalysis;
using Kopernicus.Components.MaterialWrapper;
using Kopernicus.ConfigParser.Attributes;
using Kopernicus.ConfigParser.BuiltinTypeParsers;
using Kopernicus.ConfigParser.Enumerations;
using Kopernicus.Configuration.Parsing;
using UnityEngine;

namespace Kopernicus.Configuration.MaterialLoader
{
    [RequireConfigType(ConfigType.Node)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class AerialTransCutoutLoader : AerialTransCutout
    {
        // Main Color, default = (1,1,1,1)
        [ParserTarget("color")]
        public ColorParser ColorSetter
        {
            get { return Color; }
            set { Color = value; }
        }

        // Base (RGB) Trans (A), default = "white" { }
        [ParserTarget("mainTex")]
        public Texture2DParser MainTexSetter
        {
            get { return MainTex; }
            set { MainTex = value; }
        }

        [ParserTarget("mainTexScale")]
        public Vector2Parser MainTexScaleSetter
        {
            get { return MainTexScale; }
            set { MainTexScale = value; }
        }

        [ParserTarget("mainTexOffset")]
        public Vector2Parser MainTexOffsetSetter
        {
            get { return MainTexOffset; }
            set { MainTexOffset = value; }
        }

        // Alpha cutoff, default = 0.5
        [ParserTarget("texCutoff")]
        public NumericParser<Single> TexCutoffSetter
        {
            get { return TexCutoff; }
            set { TexCutoff = value; }
        }

        // AP Fog Color, default = (0,0,1,1)
        [ParserTarget("fogColor")]
        public ColorParser FogColorSetter
        {
            get { return FogColor; }
            set { FogColor = value; }
        }

        // AP Height Fall Off, default = 1
        [ParserTarget("heightFallOff")]
        public NumericParser<Single> HeightFallOffSetter
        {
            get { return HeightFallOff; }
            set { HeightFallOff = value; }
        }

        // AP Global Density, default = 1
        [ParserTarget("globalDensity")]
        public NumericParser<Single> GlobalDensitySetter
        {
            get { return GlobalDensity; }
            set { GlobalDensity = value; }
        }

        // AP Atmosphere Depth, default = 1
        [ParserTarget("atmosphereDepth")]
        public NumericParser<Single> AtmosphereDepthSetter
        {
            get { return AtmosphereDepth; }
            set { AtmosphereDepth = value; }
        }

        // Constructors
        public AerialTransCutoutLoader()
        {
        }

        [Obsolete("Creating materials from shader source String is no longer supported. Use Shader assets instead.")]
        public AerialTransCutoutLoader(String contents) : base(contents)
        {
        }

        public AerialTransCutoutLoader(Material material) : base(material)
        {
        }
    }
}