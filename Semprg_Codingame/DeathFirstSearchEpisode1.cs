/// <summary>
/// BFS algorithm
/// </summary>

class Player
{
    static void Main(string[] args)
    {
        var inputs = Console.ReadLine().Split(' ');
        var totalNodes = int.Parse(inputs[0]); // the total number of nodes in the level, including the gateways
        var totalLinks = int.Parse(inputs[1]); // the number of links
        var totalExits = int.Parse(inputs[2]); // the number of exit gateways

        var links = new List<Link>(totalLinks);

        var exits = new int[totalExits];

        for (int i = 0; i < totalLinks; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            var n1 = int.Parse(inputs[0]); // N1 and N2 defines a link between these nodes
            var n2 = int.Parse(inputs[1]);
            links.Add(new Link(n1, n2));
        }
        for (int i = 0; i < totalExits; i++)
        {
            var exitNode = int.Parse(Console.ReadLine()); // the index of a gateway node
            exits[i] = exitNode;
        }

        while (true)
        {
            var virusNode = int.Parse(Console.ReadLine()); // The index of the node on which the Bobnet agent is positioned this turn

            var bfsNodes = new Queue<int>(links.Count);
            var visitedNodes = new List<int>(bfsNodes.Count);

            //Start with virus node and go until we find an exit
            bfsNodes.Enqueue(virusNode);

            while (visitedNodes.Count < bfsNodes.Count)
            {
                //While there are nodes to visit

                var searchNode = bfsNodes.Dequeue();
                Console.Error.WriteLine($"Searching {searchNode}");

                visitedNodes.Add(searchNode);

                var connectedNodes = GetNodesConnectedTo(searchNode, links);
                foreach (var connectedNode in connectedNodes)
                {
                    //If node is exit, we're done
                    if (exits.Contains(connectedNode))
                    { 
                        Console.WriteLine($"{searchNode} {connectedNode}");
                        goto nextIteration;
                    }

                    //Otherwise, add to bfs for further searching
                    if (!visitedNodes.Contains(connectedNode))
                    {
                        bfsNodes.Enqueue(connectedNode);
                    }
                }
            }

            Console.Error.WriteLine("No exit found");

            nextIteration: ;
        }
    }

    private static int[] GetNodesConnectedTo(int node, IEnumerable<Link> links)
    {
        return links
            .Where(x => x.Node1 == node || x.Node2 == node)
            .Select(x => x.Node1 == node ? x.Node2 : x.Node1).ToArray();
    }

    private readonly struct Link
    {
        public readonly int Node1;
        public readonly int Node2;

        public Link(int node1, int node2)
        {
            Node1 = node1;
            Node2 = node2;
        }
    }
}
