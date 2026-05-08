using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// BorrowQueue.cs
// A Queue is a waiting list - First In First Out (FIFO)
// The FIRST person to join the queue is the FIRST to get the movie
// Just like a real queue at a shop - fair for everyone
// Week 3 lecture: Queues


namespace MovieLibrary.DataStructures
{
    public class BorrowQueue
    {
        // Each movie has its OWN waiting list
        // KEY   = MovieID  e.g. "M001"
        // VALUE = Queue of usernames waiting for that movie
        // e.g. "M001" -> ["Alice", "Bob", "Carol"]
        private Dictionary<string, Queue<string>> waitingLists
            = new Dictionary<string, Queue<string>>();

        // ENQUEUE
        // Adds a person to the BACK of the waiting list for a movie
        // This is like joining the back of a queue at a shop
        public void Enqueue(string movieID, string username)
        {
            // If no waiting list exists for this movie yet, create one
            if (!waitingLists.ContainsKey(movieID))
            {
                waitingLists[movieID] = new Queue<string>();
            }

            // Add the person to the back of the queue
            waitingLists[movieID].Enqueue(username);
        }

        // DEQUEUE 
        // Removes and returns the FIRST person waiting for a movie
        // This is like the front person leaving the queue when served
        // Returns null if nobody is waiting
        public string Dequeue(string movieID)
        {
            // Check if a waiting list exists for this movie
            if (!waitingLists.ContainsKey(movieID))
            {
                return null;
            }

            // Check if anyone is actually waiting
            if (waitingLists[movieID].Count == 0)
            {
                return null;
            }

            // Remove and return the person at the FRONT of the queue
            return waitingLists[movieID].Dequeue();
        }

        // GETQUEUECOUNT
        // Returns how many people are waiting for a specific movie
        public int GetQueueCount(string movieID)
        {
            // If no list exists for this movie, nobody is waiting
            if (!waitingLists.ContainsKey(movieID))
            {
                return 0;
            }

            return waitingLists[movieID].Count;
        }

        // GETWAITINGLIST
        // Returns the full waiting list for a movie as a simple List
        // Used to display the queue in the UI
        public List<string> GetWaitingList(string movieID)
        {
            // If no list exists for this movie, return an empty list
            if (!waitingLists.ContainsKey(movieID))
            {
                return new List<string>();
            }

            // Convert the Queue into a List so we can display it easily
            return new List<string>(waitingLists[movieID]);
        }

        // HASWAITING
        // Checks if ANYONE is waiting for a specific movie
        // Returns true or false
        public bool HasWaiting(string movieID)
        {
            if (!waitingLists.ContainsKey(movieID))
            {
                return false;
            }

            return waitingLists[movieID].Count > 0;
        }

        // CLEARQUEUE 
        // Removes the entire waiting list for a movie
        // Used when a movie is deleted from the system
        public void ClearQueue(string movieID)
        {
            if (waitingLists.ContainsKey(movieID))
            {
                waitingLists.Remove(movieID);
            }
        }
    }
}