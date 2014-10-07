//-----------------------------------------------------------------------
// <copyright file="BuildRetentionPolicy.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Repository
{
    using Microsoft.TeamFoundation.Build.Client;

    public class BuildRetentionPolicy
    {
        public int StoppedKeep;

        public DeleteOptions StoppedDeleteOptions;

        public int FailedKeep;

        public DeleteOptions FailedDeleteOptions;

        public int PartiallySucceededKeep;

        public DeleteOptions PartiallySucceededDeleteOptions;

        public int SucceededKeep;

        public DeleteOptions SucceededDeleteOptions;
    }
}