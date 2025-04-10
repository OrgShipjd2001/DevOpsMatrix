﻿using DevOpsMatrix.Interface;
using Microsoft.TeamFoundation.Build.WebApi;

namespace TfsDevOpsMatrix.Tfs.ServerDevOpsServer
{
    public class TfsPipeline : IDevOpsPipeline
    {
        private BuildDefinition? m_buildDef = null;
        private IDevOpsBuildSystem m_buildSystem = null;

        public int Id
        {
            get
            {
                if (m_buildDef != null)
                    return m_buildDef.Id;
                return 0;
            }
        }

        public string Name 
        {
            get
            {
                if (m_buildDef != null) 
                    return m_buildDef.Name;
                return string.Empty;
            } 
        }

        public Dictionary<string,string> PipelineVariables
        {
            get
            {
                if (m_buildDef != null)
                {
                    Dictionary<string, string> retDict = new Dictionary<string, string>();
                    foreach (string varItem in m_buildDef.Variables.Keys)
                    {
                        if (m_buildDef.Variables[varItem].IsSecret)
                            retDict[varItem] = "********";
                        else
                            retDict[varItem] = m_buildDef.Variables[varItem].Value;
                    }
                    return retDict;
                }
                return new Dictionary<string, string>();
            }
        }

        public Dictionary<int, IDevOpsPipelineBuild> BuildList { get; } = new Dictionary<int, IDevOpsPipelineBuild>();

        protected TfsPipeline(IDevOpsBuildSystem buildsystem) 
        {
            m_buildSystem = buildsystem;
        }

        public TfsPipeline(BuildDefinition buildDef, IDevOpsBuildSystem buildsystem) : this(buildsystem)
        {
            m_buildDef = buildDef;
        }

        public IDevOpsPipelineBuild? GetLatestBuild()
        {
            if (m_buildDef != null)
            {
                if (m_buildDef.LatestBuild != null)
                {
                    if (BuildList.ContainsKey(m_buildDef.LatestBuild.Id))
                        return BuildList[m_buildDef.LatestBuild.Id];
                }
            }

            if (BuildList.Count > 0)
                return BuildList[BuildList.Keys.FirstOrDefault()];

            return null;
        }
    }
}
