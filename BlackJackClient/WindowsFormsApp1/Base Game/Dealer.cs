using System;

namespace WindowsFormsApp1
{
    public class Dealer
    {
        public int pot = 0;
        public int match = 0;
        private int maxScore = 21;

        public int Max()
        {
            return maxScore;
        }

        //switch cards
        //giver, rciever
        public void PickUp(List giver, List reciever)
        {
            //get values (card face, card score)
            reciever.AddStart(giver.GetFace(), giver.GetValue());
            //remove card from first list
            giver.RemoveX(1);
        }

        //init deck
        public void Init(Deck deck)
        {
            deck.Reset();
            string face; //the card's face (user use)
            int num = 0; //cards value (determine score)
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    num = j + 1;
                    if (j >= 10)
                    {
                        num = 10;
                    }

                    if (j == 0)
                    {
                        face = "Ace";
                    }
                    else if (j == 10)
                    {
                        face = "Jack";
                    }
                    else if (j == 11)
                    {
                        face = "Queen";
                    }
                    else if (j == 12)
                    {
                        face = "King";
                    }
                    else
                    {
                        face = Convert.ToString(j + 1);
                        num = j + 1;
                    }

                    if (i == 0)
                    {
                        face += "of Spades";
                    }
                    else if (i == 1)
                    {
                        face += "of Clubs";
                    }
                    else if (i == 2)
                    {
                        face += "of Hearts";
                    }
                    else if (i == 3)
                    {
                        face += "of Diamonds";
                    }

                    deck.AddStart(face, num);
                }
            }
        }
    }
}