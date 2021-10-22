﻿/**
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Kopernicus.ConfigParser.Attributes;
using Kopernicus.ConfigParser.Enumerations;
using Kopernicus.ConfigParser.Interfaces;

namespace Kopernicus.ConfigParser.BuiltinTypeParsers
{
    /// <summary>
    /// Parser for enum
    /// </summary>
    [RequireConfigType(ConfigType.Value)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class EnumParser<T> : IParsable, ITypeParser<T> where T : struct, IConvertible
    {
        /// <summary>
        /// The value that is being parsed
        /// </summary>
        public T Value { get; set; }
        
        /// <summary>
        /// Parse the Value from a string
        /// </summary>
        public void SetFromString(String s)
        {
            Value = (T)(Object)ConfigNode.ParseEnum(typeof(T), s);
        }

        /// <summary>
        /// Convert the value to a parsable String
        /// </summary>
        public String ValueToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
        
        /// <summary>
        /// Create a new EnumParser
        /// </summary>
        public EnumParser()
        {

        }
        
        /// <summary>
        /// Create a new EnumParser from an already existing value
        /// </summary>
        public EnumParser(T i)
        {
            Value = i;
        }

        /// <summary>
        /// Convert Parser to Value
        /// </summary>
        public static implicit operator T(EnumParser<T> parser)
        {
            return parser.Value;
        }
        
        /// <summary>
        /// Convert Value to Parser
        /// </summary>
        public static implicit operator EnumParser<T>(T value)
        {
            return new EnumParser<T>(value);
        }
    }
}