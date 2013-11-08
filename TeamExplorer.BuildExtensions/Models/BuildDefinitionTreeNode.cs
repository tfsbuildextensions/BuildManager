using System.Collections.Generic;
using Microsoft.TeamFoundation.Build.Client;

namespace BuildTree.Models
{
    public class BuildDefinitionTreeNode
    {
        public string Name { get; set; }
        public IBuildDefinition BuildDefinition { get; set; }
        public List<BuildDefinitionTreeNode> Children { get; set; }

        public BuildDefinitionTreeNode(string name)
        {
            Name = name;
            Children = new List<BuildDefinitionTreeNode>();
        }
    }
}
