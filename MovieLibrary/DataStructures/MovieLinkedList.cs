using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieLibrary.Models;

// MovieLinkedList.cs
// A Linked List stores movies as a chain of boxes called Nodes
// Each Node holds one Movie and knows where the NEXT Node is
// Week 2 lecture: Linked Lists

namespace MovieLibrary.DataStructures
{
    public class MovieLinkedList
    {
        // NODE CLASS
        // A Node is one box in the chain
        // It holds a movie AND points to the next box
        public class Node
        {
            public Movie Data;   // the movie stored in this box
            public Node Next;    // points to the next box (null if last)

            // Constructor - creates a new Node holding one movie
            public Node(Movie data)
            {
                Data = data;
                Next = null;
            }
        }

        // head = the FIRST node in the chain
        // If head is null, the list is empty
        private Node head;

        // Keeps count of how many movies are in the list
        public int Count { get; private set; }

        // Constructor - starts with an empty list
        public MovieLinkedList()
        {
            head = null;
            Count = 0;
        }

        // ADD 
        // Adds a new movie to the END of the chain
        public void Add(Movie movie)
        {
            // Create a new node to hold this movie
            Node newNode = new Node(movie);

            // If the list is empty, this new node is the first one
            if (head == null)
            {
                head = newNode;
            }
            else
            {
                // Walk along the chain until we reach the last node
                Node current = head;
                while (current.Next != null)
                {
                    current = current.Next;
                }
                // Attach the new node at the end
                current.Next = newNode;
            }

            Count++; // one more movie added
        }

        // REMOVE 
        // Removes a movie by its ID
        // Returns true if removed, false if not found
        public bool Remove(string movieID)
        {
            // Cannot remove from an empty list
            if (head == null)
            {
                return false;
            }

            // Check if the FIRST node is the one to remove
            if (head.Data.MovieID == movieID)
            {
                head = head.Next; // skip the first node
                Count--;
                return true;
            }

            // Walk the chain looking for the movie
            Node current = head;
            while (current.Next != null)
            {
                if (current.Next.Data.MovieID == movieID)
                {
                    // Skip over the node we want to remove
                    current.Next = current.Next.Next;
                    Count--;
                    return true;
                }
                current = current.Next;
            }

            return false; // not found
        }

        // TOLIST 
        // Converts the linked list into a normal List<Movie>
        // We use this for sorting and displaying in the UI
        public List<Movie> ToList()
        {
            List<Movie> result = new List<Movie>();
            Node current = head;

            while (current != null)
            {
                result.Add(current.Data);
                current = current.Next;
            }

            return result;
        }

        // REBUILDFROM 
        // Clears the list and rebuilds it from a plain List<Movie>
        // Used after sorting - we sort the plain list then rebuild
        public void RebuildFrom(List<Movie> movies)
        {
            head = null;
            Count = 0;

            foreach (Movie movie in movies)
            {
                Add(movie);
            }
        }
    }
}