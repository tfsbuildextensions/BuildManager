//-----------------------------------------------------------------------
// <copyright file="DGMLGenerator.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Repository
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.TeamFoundation.Build.Client;

    public class DgmlGenerator
    {
        private const string ProjectCollectionCategory = "ProjectCollection";

        private const string ProjectCollectionCategoryLabel = "Project Collection";

        private const string BuildControllerCategory = "BuildController";

        private const string BuildControllerCategoryLabel = "Build Controller";

        private const string DisabledBuildControllerCategory = "BuildController (Disabled)";

        private const string DisabledBuildControllerCategoryLabel = "Build Controller (Disabled)";

        private const string ServiceHostCategory = "ServiceHost";

        private const string ServiceHostCategoryLabel = "Service Host";

        private const string BuildAgentCategory = "BuildAgent";

        private const string DisabledBuildAgentCategory = "BuildAgent (Disabled)";

        private const string BuildAgentCategoryLabel = "Build Agent";

        private const string DisabledBuildAgentCategoryLabel = "Build Agent (Disabled)";

        private const string BuildAgentTagCategory = "Build Agent Tag";

        private const string BuildAgentTagCategoryLabel = "Build Agent Tag";

        private readonly IBuildServer buildServer;

        public DgmlGenerator(IBuildServer buildServer)
        {
            this.buildServer = buildServer;
        }

        public DirectedGraph GenerateGraph()
        {
            DirectedGraph dg = new DirectedGraph { Nodes = new List<DirectedGraphNode>() };
            var root = new DirectedGraphNode { Id = this.buildServer.TeamProjectCollection.Name, Label = this.buildServer.TeamProjectCollection.Name, Category1 = ProjectCollectionCategory };
            dg.Nodes.Add(root);
            dg.Links = new List<DirectedGraphLink>();
            var hosts = new List<DirectedGraphNode>();

            dg.Styles.Add(CreateStyle(BuildControllerCategory, BuildControllerCategoryLabel, "LightGreen"));
            dg.Styles.Add(CreateStyle(DisabledBuildControllerCategory, DisabledBuildControllerCategoryLabel, "Purple"));
            dg.Styles.Add(CreateStyle(ServiceHostCategory, ServiceHostCategoryLabel, "Yellow"));
            dg.Styles.Add(CreateStyle(BuildAgentCategory, BuildAgentCategoryLabel, "Green"));
            dg.Styles.Add(CreateStyle(DisabledBuildAgentCategory, DisabledBuildAgentCategoryLabel, "Red"));
            dg.Styles.Add(CreateStyle(ProjectCollectionCategory, ProjectCollectionCategoryLabel, "Orange"));
            dg.Styles.Add(CreateStyle(BuildAgentTagCategory, BuildAgentTagCategoryLabel, "LightBlue"));

            foreach (var controller in this.buildServer.QueryBuildControllers(true))
            {
                DirectedGraphNode hostNode;
                var host = controller.ServiceHost.Name;
                if (hosts.Any(h => h.Label == host))
                {
                    hostNode = hosts.First(h => h.Label == host);
                }
                else
                {
                    hostNode = CreateServiceHostNode(host, host);
                    hosts.Add(hostNode);
                    dg.Nodes.Add(hostNode);
                    var l = new DirectedGraphLink { Source = root.Label, Target = hostNode.Label };
                    dg.Links.Add(l);
                }

                var controllerNode = CreateControllerNode(controller);
                dg.Nodes.Add(controllerNode);

                var link = new DirectedGraphLink { Source = hostNode.Id, Target = controllerNode.Id, Category1 = "Contains" };
                dg.Links.Add(link);

                foreach (var a in controller.Agents)
                {
                    var agentNode = CreateBuildAgentNode(GetBuildAgentID(a), a.Name, a.Enabled);
                    host = a.ServiceHost.Name;

                    if (hosts.Any(h => h.Label == host))
                    {
                        hostNode = hosts.First(h => h.Label == host);
                        dg.Nodes.Add(agentNode);
                        link = new DirectedGraphLink { Source = controllerNode.Id, Target = agentNode.Id };
                        dg.Links.Add(link);
                    }
                    else
                    {
                        hostNode = CreateServiceHostNode(host, host);
                        dg.Nodes.Add(hostNode);
                        hosts.Add(hostNode);
                        dg.Nodes.Add(agentNode);
                        link = new DirectedGraphLink { Source = controllerNode.Id, Target = hostNode.Id };
                        dg.Links.Add(link);
                    }

                    link = new DirectedGraphLink { Source = hostNode.Id, Target = agentNode.Id, Category1 = "Contains" };
                    dg.Links.Add(link);

                    foreach (var tag in a.Tags)
                    {
                        var tagNode = CreateTagNode(tag);
                        dg.Nodes.Add(tagNode);
                        link = new DirectedGraphLink { Source = agentNode.Id, Target = tagNode.Id, Category1 = "Contains" };
                        dg.Links.Add(link);
                    }
                }
            }

            return dg;
        }

        private static DirectedGraphNode CreateControllerNode(IBuildController controller)
        {
            return new DirectedGraphNode { Id = controller.Name, Label = controller.Name, Category1 = controller.Enabled ? BuildControllerCategory : DisabledBuildControllerCategory };
        }

        private static DirectedGraphNode CreateServiceHostNode(string id, string label)
        {
            return new DirectedGraphNode
                {
                    Id = id,
                    Label = label,
                    Group = GroupEnum.Expanded,
                    GroupSpecified = true,
                    Category1 = ServiceHostCategory
                };
        }

        private static DirectedGraphNode CreateBuildAgentNode(string id, string label, bool enabled)
        {
            return new DirectedGraphNode
                {
                    Id = id,
                    Label = label,
                    Category1 = enabled ? BuildAgentCategory : DisabledBuildAgentCategory
                };
        }

        private static DirectedGraphNode CreateTagNode(string tag)
        {
            return new DirectedGraphNode
                {
                    Id = tag,
                    Label = tag,
                    Category1 = BuildAgentTagCategory
                };
        }

        private static DirectedGraphStyle CreateStyle(string category, string label, string backgroundColor)
        {
            return new DirectedGraphStyle
                {
                    TargetType = TargetTypeEnum.Node,
                    GroupLabel = label,
                    ValueLabel = "True",

                    Condition = new DirectedGraphStyleCondition { Expression = string.Format("HasCategory('{0}')", category) },
                    Setter = new List<DirectedGraphStyleSetter>
                        {
                            new DirectedGraphStyleSetter { Property = PropertyType.Background, Value = backgroundColor }
                        }
                };
        }

        private static string GetBuildAgentID(IBuildAgent agent)
        {
            return string.Format("{0}-{1}", agent.Controller.Uri.AbsolutePath, agent.Uri.AbsolutePath);
        }
    }
}