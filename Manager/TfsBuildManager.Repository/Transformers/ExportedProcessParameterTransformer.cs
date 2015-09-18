using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Workflow.Activities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TfsBuildManager.Repository.Transformers
{
    public static class ExportedProcessParameterTransformer
    {
        public static TestSpecList ToSpecList(this List<ExportedAgileTestPlatformSpec> agileTestPlatformSpecs)
        {
            TestSpecList tsl = new TestSpecList();
            tsl.AddRange(agileTestPlatformSpecs.Select(aitem => (TestSpec) aitem));
            return tsl;
        }

        public static object ProcessParameterDeserializer(string[] parameterSpec)
        {
            string paramKey = parameterSpec[0], paramValue = parameterSpec[1];
            switch (paramKey)
            {
                case "BuildSettings":
                    var buildSettings = JsonConvert.DeserializeObject<dynamic>(paramValue);
                    return new BuildSettings
                    {
                        ProjectsToBuild = ((JArray)buildSettings.ProjectsToBuild).ToObject<StringList>(), 
                        PlatformConfigurations = ((JArray)buildSettings.ConfigurationsToBuild).ToObject<PlatformConfigurationList>()
                    };

                case "AgentSettings":
                    var agentSettings = JsonConvert.DeserializeObject<dynamic>(paramValue);
                    if (agentSettings.TfvcAgentSettings != null)
                    {
                        AgentSettingsBuildParameter tfvcAgentSettings = ((JObject)agentSettings.TfvcAgentSettings).ToObject<AgentSettingsBuildParameter>();
                        return (AgentSettings)tfvcAgentSettings;
                    }
                    if (agentSettings.GitAgentSettings != null)
                    {
                        return ((JObject)agentSettings.GitAgentSettings).ToObject<BuildParameter>();
                    }
                    throw new InvalidOperationException(paramKey + " isn't supported for the specified value. Try { \"TfvcAgentSettings\": {your settings} } or { \"GitAgentSettings\": {your settings}");

                case "TestSpecs":
                    return JsonConvert.DeserializeObject<List<ExportedAgileTestPlatformSpec>>(paramValue).ToSpecList();

                default:
                    return paramValue;
            }
        }
    }
}
