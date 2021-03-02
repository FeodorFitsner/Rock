﻿// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//

using System.ComponentModel;
using Rock.Attribute;
using Rock.Model;
using Rock.Obsidian.Blocks;

namespace com_rocksolidchurchdemo.PageDebug.Blocks
{
    /// <summary>
    /// Shows the page's context entities.
    /// </summary>
    /// <seealso cref="Rock.Blocks.ObsidianBlockType" />

    [DisplayName( "Context Entities" )]
    [Category( "Rock Solid Church Demo > Page Debug" )]
    [Description( "Shows the page's context entities" )]
    [IconCssClass( "fa fa-pizza-slice" )]

    public class ContextEntities : PluginObsidianBlockType
    {
        /// <summary>
        /// Gets a value indicating whether the block file is written in TypeScript.
        /// If false, then JavaScript.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is type script; otherwise, <c>false</c>.
        /// </value>
        public override bool IsTypeScript => false;

        /// <summary>
        /// Gets the block markup file identifier.
        /// </summary>
        /// <value>
        /// The block markup file identifier.
        /// </value>
        public override string BlockFileUrl => $"/ObsidianJs/PluginsJs/com_rocksolidchurchdemo/PageDebug/ContextEntities";
    }
}
