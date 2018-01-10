﻿using clio.Model;
using CodeRinseRepeat.Bugzilla;

namespace clio.Providers
{
    public sealed class BugzillaIssue : IIssue
    {
        public BugzillaIssue(Bug bug)
        {
            this.Id = (int)bug.Id;
            this.Title = bug.Summary;
            this.MoreInfo = $"({bug.Product}) - {bug.Milestone} {bug.Status}";
            this.TargetMilestone = bug.Milestone;
            this.Status = bug.Status;
            this.Importance = bug.Severity;
        }

		public IssueSource IssueSource => IssueSource.Bugzilla;

        public int Id { get; }

        public string Title { get; }

        public string MoreInfo { get; }

        public string TargetMilestone { get; }

        public string Status { get; }

        public string Importance { get; }
    }
}
