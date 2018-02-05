using System;
using clio.Model;

namespace clio.Providers.Issues
{
	public sealed class VstsIssue : IIssue
	{
		public IssueSource IssueSource => IssueSource.Vsts;

		public int Id { get; }
		public string Title { get; }
		public string MoreInfo { get; }
		public string TargetMilestone { get; }
		public string Status { get; }
		public string Importance { get; }
		public bool IsEnhancement { get; }
		public string IssueUrl { get; }
		public bool IsClosed { get; }

		public VstsIssue (int issueId, VisualStudioBug bug)
		{
			Id = issueId;
			Title = GetValue (bug, "System.Title");
			MoreInfo = $"{GetValue (bug, "System.AreaPath")} - {GetValue (bug, "Microsoft.DevDiv.Milestone")} {GetValue (bug, "System.State")}";
			TargetMilestone = GetValue (bug, "Microsoft.DevDiv.Milestone");
			Status = GetValue (bug, "System.State");
			Importance = GetValue (bug, "Microsoft.VSTS.Common.Priority");

			// TODO: is UserStory the correct or only workitem type we want to call an enhancement?
			IsEnhancement = GetValue (bug, "System.WorkItemType") == "UserStory";
			IssueUrl = $"https://devdiv.visualstudio.com/DevDiv/_workitems/edit/{this.Id}";

			// TODO: is "closed" the only status to define a bug / work item as closed?
			IsClosed = this.Status == "Closed";
		}

		static string GetValue(VisualStudioBug bug, string name) 
		{
			if (bug.Fields.ContainsKey (name)) {
				return bug.Fields[name];
			}

			Explain.Print ($"{name} was not a field in VSTS bug {bug.Id}");
			return string.Empty;
		}
	}
}
