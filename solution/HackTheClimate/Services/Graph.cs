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
        public Node(string id, double confidenceScore, string title)
        {
            Id = id;
            ConfidenceScore = confidenceScore;
            Title = title;
        }

        public string Id { get; set; }
        public double ConfidenceScore { get; set; }
        public string Title { get; set; }
    }

    public class Link
    {
        public Link(string source, string target, double similarityScore)
        {
            Source = source;
            Target = target;
            SimilarityScore = similarityScore;
        }

        public string Source { get; set; }
        public string Target { get; set; }
        public double SimilarityScore { get; set; }
    }
}