using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace NEA_Project
{

    public class QuadTree
    {
        // Maximum number of objects a node can hold before it splits into subnodes
        // Helps to limit the number of objects in a single node, improving search efficiency
        private const int MAX_OBJECTS = 50;
        // Maximum levels of the quadtree to prevent infinite splitting
        // Ensures the tree does not grow too deep, which could degrade performance
        private const int MAX_LEVELS = 10;

        // Current level of this node in the quadtree
        // Helps to track the depth of the node, which is used to control splitting
        private int level;
        // List of objects contained in this node
        // Stores the objects that are within the bounds of this node
        private List<Sprite> objects;
        // The bounding rectangle of this node
        // Defines the area covered by this node
        private Rectangle bounds;
        // Array of child nodes (subnodes)
        // Stores references to the four subnodes created when this node splits
        private QuadTree[] nodes;

        // Constructor to initialize the quadtree node with a level and bounds
        public QuadTree(int level, Rectangle bounds)
        {
            this.level = level;
            this.objects = new List<Sprite>();
            this.bounds = bounds;
            this.nodes = new QuadTree[4]; // Initialize the array for 4 subnodes
        }

        // Clears the quadtree by removing all objects and clearing subnodes
        // Helps to reset the quadtree, freeing up memory and preparing it for reuse
        public void Clear()
        {
            objects.Clear(); // Clear the list of objects
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] != null)
                {
                    nodes[i].Clear(); // Recursively clear subnodes
                    nodes[i] = null; // Set subnode to null
                }
            }
        }

        // Splits the node into 4 subnodes
        // Helps to distribute objects into smaller regions, reducing the number of objects per node
        private void Split()
        {
            int subWidth = bounds.Width / 2; // Calculate width of subnodes
            int subHeight = bounds.Height / 2; // Calculate height of subnodes
            int x = bounds.X; // X coordinate of the current node
            int y = bounds.Y; // Y coordinate of the current node

            // Create 4 subnodes with calculated bounds
            nodes[0] = new QuadTree(level + 1, new Rectangle(x + subWidth, y, subWidth, subHeight));
            nodes[1] = new QuadTree(level + 1, new Rectangle(x, y, subWidth, subHeight));
            nodes[2] = new QuadTree(level + 1, new Rectangle(x, y + subHeight, subWidth, subHeight));
            nodes[3] = new QuadTree(level + 1, new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight));
        }

        // Determines which subnode the object belongs to
        // Helps to find the appropriate subnode for an object, ensuring efficient insertion and retrieval
        private int GetIndex(Sprite sprite)
        {
            int index = -1; // Default index is -1 (object does not fit in any subnode)
            double verticalMidpoint = bounds.X + (bounds.Width / 2); // Vertical midpoint of the current node
            double horizontalMidpoint = bounds.Y + (bounds.Height / 2); // Horizontal midpoint of the current node

            // Check if the object can completely fit within the top quadrants
            bool topQuadrant = (sprite.Position.Y < horizontalMidpoint && sprite.Position.Y + sprite.Texture.Height < horizontalMidpoint);
            // Check if the object can completely fit within the bottom quadrants
            bool bottomQuadrant = (sprite.Position.Y > horizontalMidpoint);

            // Check if the object can completely fit within the left quadrants
            if (sprite.Position.X < verticalMidpoint && sprite.Position.X + sprite.Texture.Width < verticalMidpoint)
            {
                if (topQuadrant)
                {
                    index = 1; // Top-left quadrant
                }
                else if (bottomQuadrant)
                {
                    index = 2; // Bottom-left quadrant
                }
            }
            // Check if the object can completely fit within the right quadrants
            else if (sprite.Position.X > verticalMidpoint)
            {
                if (topQuadrant)
                {
                    index = 0; // Top-right quadrant
                }
                else if (bottomQuadrant)
                {
                    index = 3; // Bottom-right quadrant
                }
            }

            return index; // Return the index of the subnode
        }

        // Insert the object into the quadtree
        // Helps to organize objects spatially, reducing the number of comparisons needed for collision detection
        public void Insert(Sprite sprite)
        {
            // If there are subnodes, find the appropriate subnode to insert the object
            if (nodes[0] != null)
            {
                int index = GetIndex(sprite);

                if (index != -1)
                {
                    nodes[index].Insert(sprite); // Insert the object into the appropriate subnode
                    return;
                }
            }

            // If no subnodes or object cannot fit in a subnode, add it to this node
            objects.Add(sprite);

            // If the node exceeds the capacity and the level is less than the maximum level, split and redistribute objects
            if (objects.Count > MAX_OBJECTS && level < MAX_LEVELS)
            {
                if (nodes[0] == null)
                {
                    Split(); // Split the node into subnodes
                }

                int i = 0;
                while (i < objects.Count)
                {
                    int index = GetIndex(objects[i]);
                    if (index != -1)
                    {
                        nodes[index].Insert(objects[i]); // Insert the object into the appropriate subnode
                        objects.RemoveAt(i); // Remove the object from the current node
                    }
                    else
                    {
                        i++; // Move to the next object
                    }
                }
            }
        }

        // Retrieve all objects that could collide with the given object
        // Helps to quickly find potential collisions by narrowing down the search area
        public List<Sprite> Retrieve(List<Sprite> returnObjects, Sprite sprite)
        {
            int index = GetIndex(sprite); // Determine which subnode the object belongs to
            if (index != -1 && nodes[0] != null)
            {
                nodes[index].Retrieve(returnObjects, sprite); // Recursively retrieve objects from the appropriate subnode
            }

            returnObjects.AddRange(objects); // Add objects from the current node to the return list

            return returnObjects; // Return the list of potential colliding objects
        }
    }

}
