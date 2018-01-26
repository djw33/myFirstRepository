using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Bingo
{
    class Program
    {
        private static RelationshipGraph rg;

        // Read RelationshipGraph whose filename is passed in as a parameter.
        // Build a RelationshipGraph in RelationshipGraph rg
        private static void ReadRelationshipGraph(string filename)
        {
            rg = new RelationshipGraph();                           // create a new RelationshipGraph object

            string name = "";                                       // name of person currently being read
            int numPeople = 0;
            string[] values;
            Console.Write("Reading file " + filename + "\n");
            try
            {
                string input = System.IO.File.ReadAllText(filename);// read file
                input = input.Replace("\r", ";");                   // get rid of nasty carriage returns 
                input = input.Replace("\n", ";");                   // get rid of nasty new lines
                string[] inputItems = Regex.Split(input, @";\s*");  // parse out the relationships (separated by ;)
                foreach (string item in inputItems)
                {
                    if (item.Length > 2)                            // don't bother with empty relationships
                    {
                        values = Regex.Split(item, @"\s*:\s*");     // parse out relationship:name
                        if (values[0] == "name")                    // name:[personname] indicates start of new person
                        {
                            name = values[1];                       // remember name for future relationships
                            rg.AddNode(name);                       // create the node
                            numPeople++;
                        }
                        else
                        {
                            rg.AddEdge(name, values[1], values[0]); // add relationship (name1, name2, relationship)

                            // handle symmetric relationships -- add the other way
                            if (values[0] == "hasSpouse" || values[0] == "hasFriend")
                                rg.AddEdge(values[1], name, values[0]);

                            // for parent relationships add child as well
                            else if (values[0] == "hasParent")
                                rg.AddEdge(values[1], name, "hasChild");
                            else if (values[0] == "hasChild")
                                rg.AddEdge(values[1], name, "hasParent");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write("Unable to read file {0}: {1}\n", filename, e.ToString());
            }
            Console.WriteLine(numPeople + " people read");
        }

        // Show the relationships a person is involved in
        private static void ShowPerson(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
                Console.Write(n.ToString());
            else
                Console.WriteLine("{0} not found", name);
        }

        // Show a person's friends
        private static void ShowFriends(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
            {
                Console.Write("{0}'s friends: ", name);
                List<GraphEdge> friendEdges = n.GetEdges("hasFriend");
                foreach (GraphEdge e in friendEdges)
                {
                    Console.Write("{0} ", e.To());
                }
                Console.WriteLine();
            }
            else
                Console.WriteLine("{0} not found", name);
        }

        // Show a person's Descendants
        private static void ShowDescendents(string name)
        {
            int generation = 0; //Keep track of generations to know what realtion it is
            List<string> descendents = descend(name, 0);
            while (descendents.Count != 0)
            {
                List<string> forloopstring = new List<string>();
                generation++;
                string numberofgreats = "";
                if (generation >= 1)
                {
                    for (int i = 0; i < generation - 1; i++)
                    {
                        numberofgreats = numberofgreats + "great ";//use the int generation to add as
                    }                                                 //many greats are necesary. 
                    Console.Write("{0}'s {1}grandchildren: ", name, numberofgreats);//not it will print 1 to many times just asume if its blank there are not decendents that far down.
                    Console.WriteLine();
                }
                foreach (string s in descendents)
                {
                    forloopstring.AddRange(descend(s, generation));
                }
                descendents = forloopstring;
            }

        }

        //Finds the children of 1 node and prints them all out. This function is used with
        //ShowDescendents to print off all of descendents. 
        private static List<string> descend(string name, int generation)
        {
            List<string> children = new List<string>();
            GraphNode n = rg.GetNode(name);
            if (n != null)
            {
                List<GraphEdge> childrenEdges = n.GetEdges("hasChild");
                if (childrenEdges.Count != 0)
                {
                    if (generation == 0) //if its the first generation print Childen, otherwise the outide function will take care of it.
                    {
                        Console.Write("{0}'s children: ", name);
                        Console.WriteLine();
                    }
                    foreach (GraphEdge e in childrenEdges)
                    {
                        children.Add(e.To());
                        Console.Write("{0} ", e.To());
                        Console.WriteLine();
                    }
                }
            }
            else
                Console.WriteLine("{0} not found", name);
            return children;
        }
        

        // Show all orphans
        private static void ShowOrphans()
        {
            Console.Write("People with no Parents: ");
            Console.WriteLine();
            foreach (GraphNode n in rg.nodes)
            {
                List<GraphEdge> parentEdges = n.GetEdges("hasParent");
                if (parentEdges.Count == 0)
                {
                    Console.Write("{0} ", n.Name);
                    Console.WriteLine();
                }
                
            }
        }

        // Show a person's Cousins
        private static void ShowCousins(string name, string n, string k)
        {
            Console.Write("{0}'s Cousin {1} {2}: ", name, n, k);
            Console.WriteLine();
            int up = Convert.ToInt32(n) + 1;
            int down = Convert.ToInt32(n) + Convert.ToInt32(k) + 1;
            if (up == down)//if up and down are the same there are only 1 set of cousins
            {
                if (!cousin(name, down, up))
                    Console.Write("None Found");
            }
            else//if up and down are different there are 2 sets of cousins.
            {
                if (!cousin(name, up, down) && !cousin(name, down, up))
                    Console.Write("None Found");
            }
        }

        // Prints out the result from traversing the tree up Up times and down Down times. Used for Cousin finding cousins.
        //returns false if no cousins were found
        private static bool cousin(string name, int Up, int Down)
        {
            GraphNode node = rg.GetNode(name);
            if (node != null)
            {
                List<string> parents = new List<string>();
                parents.Add(name);
                List<string> lastparents = new List<string>(); //This is used to make sure you dont accidentally go down the path you took up.
                for (int i = 0; i < Up; i++)
                {
                    List<string> tempstring = new List<string>();
                    foreach (string p in parents)//Breadth First search
                    {
                        node = rg.GetNode(p);
                        List<GraphEdge> parentEdges = node.GetEdges("hasParent");
                        foreach (GraphEdge e in parentEdges)
                        {
                            tempstring.Add(e.To());//add the new parents to the temp string
                        }
                    }
                    lastparents = parents;//last parents are kept track of so that when we start traversing down the tree we dont accidentally backtrack.
                    parents = tempstring;//new parents are updated after going up a generation
                    if (tempstring.Count() == 0)//if a parent cant be found then no cousins for this set will exist
                    {
                        return false;
                    }
                }
                List<string> children = parents; //now we are going to go down Down times starting at parents. 
                for (int i = 0; i < Down; i++)
                {
                    List<string> tempstring = new List<string>();//this will be used to re update the children after going down a generation
                    foreach (string c in children)//breadth first search technique
                    {
                        node = rg.GetNode(c);
                        List<GraphEdge> childEdges = node.GetEdges("hasChild");
                        foreach (GraphEdge e in childEdges)
                        {
                            if (!lastparents.Contains(e.To()))//this is to make sure you dont go down where you came up from 
                                tempstring.Add(e.To());
                        }
                    }
                    children = tempstring;//update the new generation after going down once.
                    if (tempstring.Count() == 0)//if a child cant be found then no cousins for this set will exist
                    {
                        return false;
                    }
                }

                foreach (string c in children)//print off all of the cousins
                {
                    Console.Write("{0}", c);
                    Console.WriteLine();
                }
            }
            else
                Console.WriteLine("{0} not found", name);
            return true; //some cousins were found so return true
        }

        // Show the shortes chain of relationships between two people
        private static void Bingo(string person1, string person2)
        {
            Stack<string> shortestpath = new Stack<string>();
            List<List<string>> explored = new List<List<string>>();
            GraphNode node1 = rg.GetNode(person1);
            GraphNode node2 = rg.GetNode(person2);
            List<string> temp = new List<string>();
            temp.Add(person1);
            temp.Add(null);
            temp.Add(null);
            explored.Add(temp); //Add something to explored so the count is greater then 0. 

            if ((node1 != null) && (node2 != null))
            {
                for(int i=0; i < explored.Count(); i++)
                {
                    node1 = rg.GetNode(explored[i][0]);
                    if (node1 != null)
                    {
                        List<GraphEdge> edges = node1.GetEdges();
                        foreach (GraphEdge e in edges)
                        {
                            List<string> temp1 = new List<string>();
                            bool contains = false;
                            temp1.Add(e.To());          //next node
                            temp1.Add(node1.Name);      //current node
                            temp1.Add(e.Label);         //current node is a e.Label to next node
                            temp1.Add(i.ToString());    //the list location from last. This will be used to retrace steps.
                            for(int j= 0; j < explored.Count; j++)
                            {
                                //Check to see if the node has already been explored 
                                if (((explored[j][0] == node1.Name)&& (explored[j][1] == e.To()))|| ((explored[j][1] == node1.Name) && (explored[j][0] == e.To())))
                                    contains = true;
                            }
                            if (!contains) //Continue if node is unexplored
                            {
                                explored.Add(temp1); //Add to list of explored nodes
                                if (e.To() == person2)
                                {
                                    int j = explored.Count-1;
                                    while (j != 0)
                                    {
                                        shortestpath.Push(explored[j][1] + " " + explored[j][2] + " " + explored[j][0]);
                                        j = Convert.ToInt32(explored[j][3]); //Use "breadcrumbs" to trace your way back.
                                    }
                                    while (shortestpath.Count() != 0)
                                    {
                                        Console.Write(shortestpath.Pop());
                                        Console.WriteLine();
                                    }
                                    return;//break out of everything cause you're done!
                                }
                            }
                        }
                    }
                }
                Console.Write("No Connection"); //no connection has been made
            }
            else
                Console.WriteLine("{0} or {1} not found", person1, person2);//one of the names is not present in graph. 

        }

        // accept, parse, and execute user commands
        private static void CommandLoop()
        {
            string command = "";
            string[] commandWords;
            Console.Write("Welcome to Harry's Dutch Bingo Parlor!\n");

            while (command != "exit")
            {
                Console.Write("\nEnter a command: ");
                command = Console.ReadLine();
                commandWords = Regex.Split(command, @"\s+");        // split input into array of words
                command = commandWords[0];

                if (command == "exit");                                               // do nothing

                // read a relationship graph from a file
                else if (command == "read" && commandWords.Length > 1)
                    ReadRelationshipGraph(commandWords[1]);

                // show information for one person
                else if (command == "show" && commandWords.Length > 1)
                    ShowPerson(commandWords[1]);

                //Show friends of one person
                else if (command == "friends" && commandWords.Length > 1)
                    ShowFriends(commandWords[1]);

                //Show Descendants of one person
                else if (command == "descendents" && commandWords.Length > 1)
                    ShowDescendents(commandWords[1]);

                //Show all orphans
                else if (command == "orphans")
                    ShowOrphans();

                //Shows all the Cousins n k
                else if (command == "cousins" && commandWords.Length > 3)
                    ShowCousins(commandWords[1], commandWords[2], commandWords[3]);

                //Shows the shortes chain of relathionships
                else if (command == "bingo" && commandWords.Length > 2)
                    Bingo(commandWords[1], commandWords[2]);

                // dump command prints out the graph
                else if (command == "dump")
                    rg.Dump();

                // illegal command
                else
                    Console.Write("\nLegal commands: read [filename], dump, show [personname],\nfriends [personname], descendents [personname],\norphans, cousins [n] [removed], bingo [person] [person], exit\n");
            }
        }

        static void Main(string[] args)
        {
            CommandLoop();
        }
    }
}
