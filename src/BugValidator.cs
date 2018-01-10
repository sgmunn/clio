﻿using System;
using System.Collections.Generic;
using System.Linq;
using clio.Model;

namespace clio
{
	public static class BugValidator
	{
		public static void Validate (BugCollection bugs, SearchOptions options)
		{
			bool explainStatus = Explain.Enabled;
			Explain.Enabled = true;
			
			Explain.Print ("Validating Bug Status:");
			if (options.Bugzilla != BugzillaLevel.Private)
				Explain.Print ("This will only cover public bugs as private as --bugzilla:private is not set.");

			Explain.Indent ();

			ProcessBugStatus (bugs);
			Explain.Print ("");
			ProcessTargetMilestones (bugs, options);

			Explain.Deindent ();
			Explain.Enabled = explainStatus;
		}

		static void ProcessBugStatus (BugCollection bugs)
		{
            // TODO: this is bugzilla specific
			foreach (var bug in bugs.Bugs)
			{
				switch (bug.IssueInfo.Status)
				{
					case "CLOSED":
					case "VERIFIED":
					case "RESOLVED":
						break;
					default:
					Explain.Print ($"{bug.Id} status may not be set correctly: {bug.IssueInfo.Status}.");
						break;
				}
			}
		}

		static void ProcessTargetMilestones (BugCollection bugs, SearchOptions options)
		{
			string targetMilestone = options.ExpectedTargetMilestone ?? GuessTargetMilestone (bugs);

			var unmatchingBugs = bugs.Bugs.Where (x => x.IssueInfo.TargetMilestone != targetMilestone);
			if (unmatchingBugs.Any ())
			{
				Explain.Print ($"The following bugs do not match the expected {targetMilestone}:");
				Explain.Indent ();

				foreach (var bug in unmatchingBugs)
					Explain.Print ($"{bug.Id} - {bug.IssueInfo.TargetMilestone}");
			}
			Explain.Deindent ();
		}

		static string GuessTargetMilestone (BugCollection bugs)
		{
			Explain.Print ("--expected-target-milestone was not set, so finding the most common Target Milestone.");
			var targetMilestoneCount = new Dictionary<string, int> ();
			foreach (var bug in bugs.Bugs)
			{
				if (targetMilestoneCount.ContainsKey (bug.IssueInfo.TargetMilestone))
					targetMilestoneCount[bug.IssueInfo.TargetMilestone] += 1;
				else
					targetMilestoneCount[bug.IssueInfo.TargetMilestone] = 1;
			}
			var targetMilestones = targetMilestoneCount.Keys.OrderByDescending (x => targetMilestoneCount[x]).ToList ();

			string guess = targetMilestones.FirstOrDefault ();
			Explain.Print ($"{guess} is the most common Target Milestone.");
			return guess;
		}
	}
}
