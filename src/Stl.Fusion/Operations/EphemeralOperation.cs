using System;
using Stl.Collections;

namespace Stl.Fusion.Operations
{
    public class EphemeralOperation : IOperation
    {
        public string Id { get; set; } = "";
        public string AgentId { get; set; } = "";
        public DateTime StartTime { get; set; }
        public DateTime CommitTime { get; set; }
        public object? Command { get; set; }
        public ImmutableOptionSet Items { get; set; } = ImmutableOptionSet.Empty;
    }
}
