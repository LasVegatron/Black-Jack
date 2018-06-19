using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    /// <summary>
    /// Holds cards. holds functions for adding and removing cards.
    /// </summary>
    public class List
    {
        public Card head; //head node. no value, but acts as entry to list
        private Card current; //latest/tail node
        public int count; //total nodes (could be used as a placement identifier)

        public string GetFace()
        {
            return head.next.face;
        }
        
        public int GetValue()
        {
            return head.next.value;
        }

        //constructor
        public List()
        {
            //initially head and current value will be the same and have empty code
            head = new Card(); //new node created
            current = head; //the section of the list being called
        }

        //adds a node to the end of the list
        public void AddLast(string data, int num /*data entering list */)
        {
            Card newNode = new Card(); //new node created
            newNode.value = num;
            newNode.face = data; //new data stored
            current.next = newNode; //node reference is given to the final pre-existing node
            current = newNode; //the amount of 'next's
            count++; //list count extended
        }

        //adds to the start
        public void AddStart(string data, int num /*data entering list */)
        {
            Card newNode = new Card(); //new node created
            newNode.value = num;
            newNode.face = data;
            newNode.next = head.next; //new node will have refence of head's next reference
            head.next = newNode; //head will refer to new node
            count++; //list count extended
        }

        //remove (x)
        public void RemoveX(int x /*selected number*/)
        {
            Card fake = head;
            if (count >= x) //if the selected number is equal or smaller than the max number of cards
            {
                for (int j = 0; j < x; j++) //search will only continue until the node is found
                {
                    if (j == x - 1 /* stops just before node*/) //if node found
                    {
                        fake.next = fake.next.next; //node references deleted
                        count--; //list count shortened
                    }
                    fake = fake.next; //moving on to next node
                }
            }
        }

        public void Reset()
        {
            while (count > 0)
            {
                RemoveX(1);
            }
        }

        public string Print()
        {
            string faces = ""; 
            Card fake = head; //fake becomes a 'fake' head and sorts through list without changing anything
            for (int i = 0; i < count; i++)
            {
                fake = fake.next;
                faces += fake.face;
                faces += ", ";
            }
            return faces;
        }
    }

    public class Hand : List //contains methods only the player would need
    {
        public string name;
        public int wallet = 400; //amount of money held by the player
        public int match = 0; //money player has bet in the current round
        public bool stand = false; //determines whether or not player is ready to end the round
        public bool fold = false; //whether player has folded this round
        public bool winner = false; //active if player is the winner of this round
        public bool nonStand = false; //records if player has made an action before ending their turn;
        public int handLimit = 5; //number of cards maximum per player (not editable)
        public int handCount = 5; //number of card spaces left

        public void Reset()
        {
            // resets
            match = 0; //money player has bet in the current round
            stand = false; //determines whether or not player is ready to end the round
            fold = false; //whether player has folded this round
            winner = false; //active if player is the winner of this round
            nonStand = false; //records if player has made an action before ending their turn;
            handCount = 5;

            while (count > 0)
            {
                RemoveX(1);
            }
        }

        public int Score()  //the value of all player cards combined
        {
            int counter = 0;
            int score = 0; //score reset for count
            Card fake = head; //substitute card
            while (count >= counter) //searches through player hand
            {
                score += fake.value; //card value added
                fake = fake.next; //next node selected
                counter++; //counts how many cards have been counted
            }
            return score;
        }
    }

    public class Bot : Hand
    {
        public int inRound; //determines how many times a turn has been had by a BOT per round
        public string winMessage = " 'ha suck suck m80'";
        public string lossMessage = " 'dang nabbot damnit dabbot davit'";

        public void Reset()
        {
            //resets
            match = 0; //money player has bet in the current round
            stand = false; //determines whether or not player is ready to end the round
            fold = false; //whether player has folded this round
            winner = false; //active if player is the winner of this round
            nonStand = false; //records if player has made an action before ending their turn;
            handCount = 5;
            inRound = 0;

            while (count > 0)
            {
                RemoveX(1);
            }
        }
    }

    public class Deck : List //contains methods only the deck would need to use
    {
        //switch values. x = 1st value y = second
        public void SwitchV(int x, int y)
        {
            Card a = head; //fake head 1
            Card b = head; //fake head 2
            string dum1; //dummy value to sort
            int num;
            bool aStop = false; //boolean gate 1
            bool bStop = false; //boolean gate 2

            for (int i = 0; i < count; i++)
            {
                //get both values
                if (x == i) //if first value is found
                {
                    aStop = true; //stop searching for value 1
                }
                if (y == i) //if second value is found
                {
                    bStop = true; //stop searching for value 2
                }
                if (aStop == false) //if not found 
                {
                    a = a.next; //countinue search
                }
                if (bStop == false) //if not found
                {
                    b = b.next; //continue search
                }
                if (aStop == bStop && aStop == true) //when both values are found 
                {
                    i = count; //linear search stops
                }
            }

            //value swap
            //face
            dum1 = a.face;
            a.face = b.face;
            b.face = dum1;
            //score
            num = a.value;
            a.value = b.value;
            b.value = num;
        }
        
        //shuffle
        public void Shuffle()
        {
            Random r = new Random();
            int dum;
            for (int i = 0; i <= count; i++) //deck is only walked through once
            {
                if (i > 0)
                {
                    dum = r.Next(1, count); //between the first and last node
                    SwitchV(i, dum); //switch node 'i' with a randomly selected node
                }
            }
        }
    }
}
