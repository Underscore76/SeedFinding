using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewValley.StardewClasses
{
    public class OtherNamesAttribute : Attribute
    {
        /// <summary>The alternate names for the method.</summary>
        public string[] Aliases { get; }

        /// <summary>Construct an instance.</summary>
        /// <param name="aliases">The alternate names for the method.</param>
        public OtherNamesAttribute(params string[] aliases)
        {
            this.Aliases = aliases;
        }
    }
}