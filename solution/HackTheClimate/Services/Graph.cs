using System.Collections.Generic;

namespace HackTheClimate.Services
{
    public class Graph
    {
        public Graph()
        {
            Links = new List<Link>();
            Nodes = new List<Node>();
        }

        public IList<Link> Links { get; }
        public IList<Node> Nodes { get; }
    }

    public class Node
    {
        public Node(string id, double confidenceScore)
        {
            Id = id;
            ConfidenceScore = confidenceScore;
        }

        public string Id { get; set; }
        public double ConfidenceScore { get; set; }
    }

    public class Link
    {
        public Link(string source, string target, double value)
        {
            Source = source;
            Target = target;
            Value = value;
        }

        public string Source { get; set; }
        public string Target { get; set; }
        public double Value { get; set; }
    }
}