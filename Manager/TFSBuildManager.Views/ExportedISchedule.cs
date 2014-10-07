//-----------------------------------------------------------------------
// <copyright file="ExportedISchedule.cs">(c) https://github.com/tfsbuildextensions/BuildManager. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace TfsBuildManager.Views
{
    using System;
    using Microsoft.TeamFoundation.Build.Client;

    public class ExportedISchedule : ISchedule
    {
        public IBuildDefinition BuildDefinition { get; private set; }

        public ScheduleType Type { get; private set; }

        public int StartTime { get; set; }

        public ScheduleDays DaysToBuild { get; set; }

        public TimeZoneInfo TimeZone { get; set; }
    }
}
