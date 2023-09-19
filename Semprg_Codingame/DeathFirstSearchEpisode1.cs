using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Player
{
    static void Main(string[] args)
    {
        var inputs = Console.ReadLine().Split(' ');
        var totalNodes = int.Parse(inputs[0]); // the total number of nodes in the level, including the gateways
        var totalLinks = int.Parse(inputs[1]); // the number of links
        var totalExits = int.Parse(inputs[2]); // the number of exit gateways
        
        //var links = new List<Link>(totalLinks);

        //Key: node, value: nodes that are connected to the key node
        var links = new Dictionary<int, List<int>>();
        var exits = new int[totalExits];

        for (int i = 0; i < totalLinks; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            var n1 = int.Parse(inputs[0]); // N1 and N2 defines a link between these nodes
            var n2 = int.Parse(inputs[1]);
            //links[i] = new Link(n1, n2);

            if (!links.ContainsKey(n1))
            {
                links[n1] = new List<int>();
            }
            links[n1].Add(n2);
        }
        for (int i = 0; i < totalExits; i++)
        {
            var exitNode = int.Parse(Console.ReadLine()); // the index of a gateway node
            exits[i] = exitNode;
        }

        while (true)
        {
            var virusNode = int.Parse(Console.ReadLine()); // The index of the node on which the Bobnet agent is positioned this turn

            //Console.Error.WriteLine($"links:");
            //foreach (var link in links)
            //{
            //    Console.Error.WriteLine($"{link.Key}: {string.Join(", ", link.Value)}");
            //}

            //break;
            //Find the nearest link that goes from the virus to the nearest exit
            
            var bfsNodes = new Queue<int>(links.Count);
            var visitedNodes = new List<int>(bfsNodes.Count);

            //Add all nodes going from the virus
            foreach (var connectedNode in links[virusNode])
            {
                bfsNodes.Enqueue(connectedNode);
            }

            //Search until we reach an exit from the virus, 
            while (visitedNodes.Count != bfsNodes.Count)
            {
                var searchNode = bfsNodes.Dequeue();
                visitedNodes.Add(searchNode);

                foreach (var connectedNode in links[searchNode])
                {
                    bfsNodes.Enqueue(connectedNode);
                }

                //If we've reached an exit
                if (exits.Contains(searchNode))
                {
                    
                }
            }

            // Example: 0 1 are the indices of the nodes you wish to sever the link between
            Console.WriteLine("0 1");
        }
    }

    public readonly struct Link
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
