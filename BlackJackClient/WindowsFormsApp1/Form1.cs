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
    public partial class MainWindow : Form
    {
        SocketClient client = new SocketClient(); //client socket
        private Hand player = new Hand(); //player
        //bots
        private Bot BOT1 = new Bot();
        private Bot BOT2 = new Bot();
        //game functions
        private Deck deck = new Deck(); //the deck. holds all cards at start of round
        private Dealer deal = new Dealer(); //dealer.handles majority of money transactions
        int rounds = 0;
        int winnings = 0;
        bool hit = false;
        bool match = false; //determines if player has bet the same amount

        /// <summary>
        /// start of new round
        /// </summary>
        void StartRound()
        {
            rounds++; //counts how many rounds have been played
            player.Reset();
            BOT1.Reset();
            BOT2.Reset();
            deal.Init(deck); 
            deck.Shuffle();
            HitMeButton.Visible = true;
            RaiseButton1.Visible = true;

            MessageBox.Show("Round " + Convert.ToString(rounds));
            //pick up 2 to start
            HitMe(player, HitMeButton);
            HitMe(BOT1, BOT1HitKey);
            HitMe(BOT2, BOT2HitKey);
            HitMe(player, HitMeButton);
            HitMe(BOT1, BOT1HitKey);
            HitMe(BOT2, BOT2HitKey);
            DisplayStat();
        }

        /// <summary>
        /// Actions taken at the end of the round 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="BOT1"></param>
        /// <param name="BOT2"></param>
        private void EndCycle(Hand player, Bot BOT1, Bot BOT2)
        {
            BOT1.hidden = false;
            BOT2.hidden = false;
            DisplayStat();
            bool endCycle = false;
            while (endCycle == false)
            {
                //check if all players have bet the same
                if ((player.match == deal.match || player.fold == true) && (BOT1.match == deal.match || BOT1.fold == true) && (BOT2.match == deal.match || BOT2.fold == true))
                {
                    MessageBox.Show("Winner to be decided");
                    //winner decided
                    WinDecider(player, BOT1, BOT2);
                    //pot and match ammount reset
                    deal.match = 0;
                    //player matches reset}
                    endCycle = true;
                    BOT1.hidden = true;
                    BOT2.hidden = true;
                }
                else //searches for player who haven't matched
                {
                    BOTDecision(BOT1, BOT1HitKey, BOT1MatchBox);
                    BOTDecision(BOT2, BOT2HitKey, BOT2MatchBox);
                }
            }
        }


        /// <summary>
        /// Decides winner between 2 parties
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="hand"></param>
        Hand WinDecider(Hand hand1, Hand hand2)
        {
            //conditions to win
            //only decides on who wins, doesn't transfer the money
            if (hand2.fold == false && hand1.fold == false) //if both hands haven't folded
            {
                if (hand2.Score() > hand1.Score()) //hand 2 wins
                {
                    hand1.winner = false;
                    hand2.winner = true;
                    return hand2;
                }
                else if (hand1.Score() > hand2.Score()) //hand 1 wins
                //(if both hands haven't folded) Hand1 must be higher than hand2, not have folded or be higher than 21 || hand2 has folded but hand1 hasn't
                {
                    hand1.winner = true;
                    hand2.winner = false;
                    return hand1;
                }
                else //if theres a tie and both players haven't folded
                {
                    Random rnd = new Random(); //new randomizer
                    int coin = rnd.Next(0, 2); 
                    if (coin == 1) //flip of a coin decides winner (Heads)
                    {
                        hand1.winner = true;
                        hand2.winner = false;
                        return hand1; //hand1 wins
                    }
                    else //Tails
                    {
                        hand1.winner = false;
                        hand2.winner = true;
                        return hand2; //hand2 wins
                    }
                }
            }
            else if (hand2.fold == false || hand1.fold == true) //hand 2 fold, hand 1 hasn't
            {
                hand1.winner = false;
                hand2.winner = true;
                return hand2;
            }
            else if (hand1.fold == false || hand2.fold == true) //hand 2 folded, but hand 1 hasn't
            {

                hand1.winner = true;
                hand2.winner = false;
                return hand1;
            }
            else //if no player is qualified to win
            {
                MessageBox.Show("both failed");
                return hand1; //either hand can be played. both have lost
            }
        }

        /// <summary>
        /// decides winner between 3 players
        /// </summary>
        /// <param name="hand1"></param>
        /// <param name="hand2"></param>
        /// <param name="hand3"></param>
        void WinDecider(Hand hand1, Hand hand2, Hand hand3)
        {
            MessageBox.Show(hand1.name + "vs" + hand2.name);
            Hand tempWinner = WinDecider(hand1, hand2); //first 2 hands compared. best contender chosen
            MessageBox.Show(tempWinner.name + " " + tempWinner.Score());

            MessageBox.Show(hand3.name + "vs" + tempWinner.name);
            Hand realWin = WinDecider(tempWinner, hand3); //final hand is checked to see if It has the best conditions to win
            MessageBox.Show(realWin.name + realWin.Score());

            SearchWinner(hand1, hand2, hand3); //searches for who the decided winner is
        }
        
        /// <summary>
        /// searches for winner and decides who wins the prize
        /// </summary>
        /// <param name="player"></param>
        /// <param name="BOT1"></param>
        /// <param name="BOT2"></param>
        void SearchWinner(Hand player, Hand BOT1, Hand BOT2)
        {
            //search is conducted through 'win' boolean.
            if (player.winner == true)
            {
                PrizeWin(player);
            }
            else if (BOT1.winner == true)
            {
                PrizeWin(BOT1);
            }
            else if (BOT2.winner == true)
            {
                PrizeWin(BOT2);
            }
        }

        /// <summary>
        /// presents winner with the pot
        /// </summary>
        /// <param name="winner"></param>
        void PrizeWin(Hand winner)
        {
            MessageBox.Show(winner.name + " Wins $" + Convert.ToString(deal.pot)); //displays who won and how much money received
            winner.wallet += deal.pot; //money from pot to winner wallet
            deal.match = 0; //match ammount reset
            deal.pot = 0; //pot reet
        }

        /// <summary>
        /// Selected player raises this amount of money given to pot
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="match"></param>
        void RaiseFunc(Hand hand, int match) //raise function
        {            
            hand.wallet -= match;
            hand.match += match;
            deal.pot += match;
            deal.match += match;
            PotBox.Text = Convert.ToString(deal.pot);
            DisplayStat();
        }

        /// <summary>
        /// Selected player mathches 
        /// </summary>
        /// <param name="hand"></param>
        void MatchFunc(Hand hand) //Match function
        {
            if (deal.match > hand.wallet) //if the ammount needed to bet is higher than the wallet
            {
                deal.pot += hand.wallet; //entire wallet is emptied into pot
                hand.match = deal.match; //displays player has kept up with bet
                DisplayStat(); //displays stats
            }
            else
            {
                hand.wallet -= deal.match - hand.match; //Match amount removed from player wallet (what player hasn't yet matched)
                hand.match = deal.match; //shows that player has the new matched amount
                deal.pot += deal.match; //Match added to pot
                DisplayStat(); //displays stats
            }
        }


        /// <summary>
        /// Selected player is given a new card from the deck
        /// </summary>     
        void HitMe(Hand hand, Button hitKey) //(player asking for card, button belonging to player
        {
            if (hand.handCount > 1 && hand.handCount <= 5) //if the card limit hasn't been reached
            {
                deal.PickUp(deck, hand); //card dealt from deck
                hand.handCount--; //player recieving the card has one less card they are able to pick up
            }
            hitKey.Text = "Hit me (" + Convert.ToInt32(hand.handCount) + ")";
            DisplayStat();
        }

        /// <summary>
        /// hitme function
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="hitKey"></param>
        void HitMe(Bot hand, Button hitKey) //(player asking for card, button belonging to player
        {
            if (hand.handCount > 1 && hand.handCount <= 5) //if the card limit hasn't been reached
            {
                deal.PickUp(deck, hand); //card dealt from deck
                hand.handCount--; //player recieving the card has one less card they are able to pick up
            }
            hitKey.Text = "Hit me (" + Convert.ToInt32(hand.handCount) + ")";
            DisplayStat();
        }

        /// <summary>
        /// Prints all stats
        /// </summary>
        void DisplayStat() //displays stats of game data and players
        {
            //player stats
            DisplayPlayerStats(player, PlayerCardBox, PlayerWalletBox); //player 1
            DisplayPlayerStats(BOT1, BOT1CardBox, BOT1WalletBox); //BOT 1
            DisplayPlayerStats(BOT2, BOT2CardBox, BOT2WalletBox); //BOT 2
            //game stats
            PotBox.Text = Convert.ToString("$" + deal.pot); //display winnable money
            DeckCountBox.Text = Convert.ToString(deck.count); //displays remaining cards
            RoundMatch.Text = Convert.ToString(deal.match); //displays amount of money that must be played to participate in winning the pot
        }


        /// <summary>
        /// displays the bot's stats, both hidden and unhidden
        /// </summary>
        /// <param name="BOT"></param>
        /// <param name="BOTCardBox"></param>
        /// <param name="BOTWalletBox"></param>
        void DisplayPlayerStats(Bot BOT, TextBox Hand, TextBox Wallet)
        {
            if (BOT.hidden == true)
            {
                string faceDown = "";
                for (int i = 0; i < BOT.count; i++)
                {
                    faceDown += "CARD, ";
                }
                Hand.Text = faceDown;
            }
            else if (BOT.hidden == false)
            {
                Hand.Text = BOT.Print(); //display player hand
            }
            Wallet.Text = "$" + Convert.ToString(BOT.wallet); //display player funds
        }

        /// <summary>
        /// displays selected player's stats
        /// </summary>
        /// <param name="player"></param>
        /// <param name="Hand"></param>
        /// <param name="Wallet"></param>
        void DisplayPlayerStats(Hand player, TextBox Hand, TextBox Wallet) //(player with stats being displayed, their cards, their wallet)
        {
            Hand.Text = player.Print(); //display player hand
            Wallet.Text = "$" + Convert.ToString(player.wallet); //display player funds
        }
        
        /// <summary>
        /// BOT decision tree
        /// </summary>
        private void BOTDecision(Bot BOT, Button hitKey, TextBox MatchBox) //Bot thought process
        {
            Random rnd = new Random(); //randomizer used as a 'dice roll' for decisions
            int decider = 0; //blank number. used with randomizer as a 'dice roll' to allow bot to choose
            //decides the probability the diceroll will have on:
            int foldPeg = 0; //folding
            int raisePeg = 0; //raising
            int hitmePeg = 0; //picking up
            int matchThis = 20; //amount the bot is willing to raise per turn
            bool settle = false;

            if (BOT.fold == false) //if BOT hasn't folded
            {
                BOTStrat(foldPeg, raisePeg, hitmePeg, BOT);
                //Match required?
                if (BOT.match != deal.match) //if BOT is asked to raise
                {
                    decider = rnd.Next(0, 6); //dice rolled
                    //decide to match
                    if (decider >= foldPeg || BOT.stand == true) //decided to match (either through choice or because they're standing)
                    {
                        MatchFunc(BOT); //Match
                        MatchBox.Text = "Match"; //matchbox shows win
                        MessageBox.Show(BOT.name + ": 'I choose to match'"); //decision is vocalised
                    }
                    else if (decider < foldPeg && BOT.stand == false)//decided to fold (Can't be standing)
                    {
                        // *ADD fold function
                        BOT.fold = true; //
                        MatchBox.Text = "fold";
                        MessageBox.Show(BOT.name + ": 'nah. fold.'");
                    }
                }
                //these following commands can happen regardless if match is required or not
                decider = rnd.Next(0, 7); //dice rolled
                //hit me?
                if (BOT.stand == false && BOT.fold == false) //bot hasn't decided to stand or fold
                {
                    if (decider <= hitmePeg && BOT.Score() < 21) //decision made is also determined on the current card score
                    {
                        HitMe(BOT, hitKey); //add card from deck
                        MessageBox.Show(BOT.name + ": 'Hit Me'");
                        settle = true;
                    }
                    decider = rnd.Next(0, 7); //dice rolled
                    //Raise?
                    if (decider <= raisePeg && BOT.wallet > matchThis)
                    {
                        RaiseFunc(BOT, matchThis); //raise money
                        MessageBox.Show(BOT.name + ": 'How about, Raise 20'");
                        settle = true;
                    }

                    decider = rnd.Next(0, 4);
                    if (settle == false) //on the flip of a coin, if no action in this category is taken, bot decides to stand 
                    {
                        BOT.stand = true;
                        MessageBox.Show(BOT.name + ": 'Stand'");
                    }
                }

                if (BOT.Score() > 21)
                {
                    BOT.fold = true;
                    MatchBox.Text = "fold";
                    MessageBox.Show(BOT.name + BOT.lossMessage);
                }
                BOT.inRound++;
            }
            DisplayStat();
        }

        /// <summary>
        /// determines likelyhood of bot making a decision. (Chooses a strategy)
        /// </summary>
        /// <param name="fChance"></param>
        /// <param name="rChance"></param>
        /// <param name="hmChance"></param>
        private void BOTStrat(int fChance, int rChance, int hmChance, Hand BOT)
        {
            //on a scale of 1 - 6, how likely?
            if (BOT.Score() < 6 && BOT.Score() >= 0) //if i have x amount of cards, my likely hood of y would be:
            {
                rChance = 1; 
                hmChance = 6;
                fChance = 3;
            }
            else if (BOT.Score() < 11 && BOT.Score() >= 6) //very
            {
                rChance = 2;
                hmChance = 5;
                fChance = 2;
            }
            else if (BOT.Score() < 16 && BOT.Score() >= 11) //not very
            {
                rChance = 3;
                hmChance = 2;
                fChance = 0; 
            }
            else if (BOT.Score() < 21 && BOT.Score() >= 16) //high unlikely
            {
                rChance = 5;
                hmChance = 1;
                fChance = 0;
            }

            if (BOT.match < 20) //if less than $20 has been made as a bet
            {
                rChance = 5;
            }
        }

        /// <summary>
        /// repaints buttons
        /// </summary>
        /// <param name="Painted"></param>
        void ColourButtons(Color Painted)
        {
            QuitMainPanel.BackColor = Painted;
            PlayGam.BackColor = Painted;
            DepositToServer.BackColor = Painted;
            QuitProgram.BackColor = Painted;
            Colorscheme.BackColor = Painted;
            CloseColors.BackColor = Painted;
            OrangePreset.BackColor = Painted;
            WhitePreset.BackColor = Painted;
            GreenPreset.BackColor = Painted;
            RedPreset.BackColor = Painted;
            FoldButton.BackColor = Painted;
            HitMeButton.BackColor = Painted;
            CancelRaise.BackColor = Painted;
            RaiseButton1.BackColor = Painted;
            RaiseButton2.BackColor = Painted;
            StandButton.BackColor = Painted;
            BOT1HitKey.BackColor = Painted;
            BOT2HitKey.BackColor = Painted;
            SendWins.BackColor = Painted;
            QuitServerPanel.BackColor = Painted;
        }

        /// <summary>
        /// Repaints textbox
        /// </summary>
        /// <param name="Painted"></param>
        void ColourTextBox(Color Painted)
        {
            DeckCountBox.BackColor = Painted;
            PotBox.BackColor = Painted;
            RoundMatch.BackColor = Painted;
            PlayerCardBox.BackColor = Painted;
            RoundMatch.BackColor = Painted;
            PlayerCardBox.BackColor = Painted;
            PlayerWalletBox.BackColor = Painted;
            RaiseInputText.BackColor = Painted;
            BOT1MatchBox.BackColor = Painted;
            BOT1WalletBox.BackColor = Painted;
            BOT1CardBox.BackColor = Painted;
            BOT2MatchBox.BackColor = Painted;
            BOT2WalletBox.BackColor = Painted;
            BOT2CardBox.BackColor = Painted;
            Send.BackColor = Painted;
            Receive.BackColor = Painted;
            ErrorBox.BackColor = Painted;
        }

        /// <summary>
        /// recolours panels
        /// </summary>
        /// <param name="Painted"></param>
        void ColourPanel(Color Painted)
        {
            MainPanel.BackColor = Painted;
            GamePanel.BackColor = Painted;
            ServerPanel.BackColor = Painted;
        }

        /// <summary>
        /// Hides colour related textbox
        /// </summary>
        void HideColour()
        {
            RedPreset.Visible = false;
            GreenPreset.Visible = false;
            WhitePreset.Visible = false;
            OrangePreset.Visible = false;
            CloseColors.Visible = false;
        }


        public MainWindow()
        {
            InitializeComponent();
        }
          
        private void HitMe(object sender, EventArgs e) //Hit me
        {
            if (hit == false && player.Score() <= 21) //if the player hasn't folded or picked up this round
            {
                player.stand = false;
                StandButton.Text = "Next";
                HitMe(player, HitMeButton);
                hit = true;
                HitMeButton.Text = "Wait Next turn";
            }
            if (player.Score() > 21) //if the player breaks the card limit
            {
                player.fold = true;
                HitMeButton.Visible = false;
                RaiseButton1.Visible = false;
            }
        }      

        private void QuitGame(object sender, EventArgs e) //panel 1 'next'
        {
            winnings = 400 - player.wallet;
            rounds = 0;
            deal.pot = 0;
            deal.match = 0;
            hit = false;
            player.handCount = 5;
            player.Reset();
            BOT1.Reset();
            BOT2.Reset();
            MainPanel.Visible = true;
            GamePanel.Visible = false;
        }

        private void PlayGame(object sender, EventArgs e) //join game
        {
            //colour menu hidden
            HideColour();

            //give bots a name
            player.name = "Player";
            BOT1.name = "Michael";
            BOT2.name = "Jade";
            StartRound();
            GamePanel.Visible = true;
            MainPanel.Visible = false;
            DisplayStat();

            //match function. ensures all players have bet the same amount
            if (player.match != deal.match)
            {
                match = true;
                HitMeButton.Visible = false;
                RaiseButton1.Text = "Match?";
                StandButton.Visible = false;
            }
        }

        private void CancelRaise_Click(object sender, EventArgs e)
        {
            RaiseInputText.Text = "";
            RaiseButton2.Visible = false;
            CancelRaise.Visible = false;
            RaiseInputText.Visible = false;
            RaiseButton1.Visible = true;
            HitMeButton.Visible = true;
            FoldButton.Visible = true;
            StandButton.Visible = true;
        }

        /// <summary>
        /// Raise/Match Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Raise_Click(object sender, EventArgs e)
        {
            if (player.fold == false)
            {
                if (match == true) //raise button temporarily becomes a match button
                {
                    //Matching bets
                    MatchFunc(player);

                    //Match button reverts back to being a raise button
                    HitMeButton.Visible = true;
                    RaiseButton1.Text = "Raise";
                    StandButton.Visible = true;
                    match = false;
                }
                else //regular raise button function
                {
                    //hides other controls so that nothing can happen while money is being transfered
                    RaiseButton2.Visible = true;
                    CancelRaise.Visible = true;
                    RaiseInputText.Visible = true;
                    RaiseButton1.Visible = false;
                    HitMeButton.Visible = false;
                    FoldButton.Visible = false;
                    StandButton.Visible = false;
                }
            }
        }

        /// <summary>
        /// button used to verify and add user money
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RaiseIt_Click(object sender, EventArgs e)
        {
            try
            {
                int x = Convert.ToInt32(RaiseInputText.Text);
                if (player.wallet >= x)
                {
                    player.stand = false;
                    StandButton.Text = "Next";
                    RaiseFunc(player, x);

                    RaiseInputText.Text = "";
                    RaiseButton2.Visible = false;
                    CancelRaise.Visible = false;
                    RaiseInputText.Visible = false;
                    RaiseButton1.Visible = true;
                    HitMeButton.Visible = true;
                    StandButton.Visible = true;
                    FoldButton.Visible = true;
                }
                else
                {
                    //insufficient funds. player can't bet that much
                    MessageBox.Show("Insufficient funds");
                    RaiseInputText.Text = "";
                }

            }
            catch (FormatException)
            {
                if (RaiseInputText.Text == "")
                {
                    //No input to verify/add
                    MessageBox.Show("Input Number");
                    RaiseInputText.Text = "";
                }
                else
                {
                    //Input is not entered in digits
                    MessageBox.Show("In DIGITS");
                    RaiseInputText.Text = "";
                }
            }
        }

        /// <summary>
        /// Stand decision button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StandButton_Click(object sender, EventArgs e) //all actions being played out for the rest of the game are stored here
        {
            //butt
            StandButton.Text = "Stand";
            //BOTs take their turns
            BOTDecision(BOT1, BOT1HitKey, BOT1MatchBox); //BOT1's turn
            BOTDecision(BOT2, BOT2HitKey, BOT2MatchBox); //BOT2's turn
            //end of inner cycle
            //player allowed to ask for more cards
            hit = false;

            //The inner cycle is the continuation of the round until all players are finished.
            //Ensures all bets are either matched or folded

            //round conditions if player has folded
            if (player.fold == false)
            {
                HitMeButton.Visible = true;

                if (player.match < deal.match)
                {
                    match = true;
                    HitMeButton.Visible = false;
                    RaiseButton1.Text = "Match?";
                    StandButton.Visible = false;
                }
            }

            //end of round
            //occurs when all players have either folded or stand
            if ((player.stand == true || player.fold == true) && (BOT1.stand == true || BOT1.fold == true) && (BOT2.stand == true || BOT2.fold == true))
            {
                EndCycle(player, BOT1, BOT2); //end of round
                StartRound(); //start of new round
            }

            HitMeButton.Text = "Hit me (" + Convert.ToInt32(player.handCount) + ")";
            DisplayStat();
            player.stand = true;
        }

        private void FoldButton_Click(object sender, EventArgs e) //fold button
        {
            player.fold = true; //player is registered as folded
            StandButton.Visible = true;
            HitMeButton.Visible = false;
            RaiseButton1.Visible = false;
        }
        
        /// <summary>
        /// Sends player progress to server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendWins_Click(object sender, EventArgs e)
        {
            Send.Text = "";
            Receive.Text = "";
            ErrorBox.Text = "";
            client.StartClient(Send, Receive, ErrorBox, winnings);
        }

        /// <summary>
        /// sends user from 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowServer_Click(object sender, EventArgs e)
        {
            //hide colour menu
            HideColour();

            //Server panel visable
            ServerPanel.Visible = true;
            MainPanel.Visible = false;
        }

        /// <summary>
        /// sends user from server menu to main menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuitSend_Click(object sender, EventArgs e)
        {
            ServerPanel.Visible = false;
            MainPanel.Visible = true;
        }

        /// <summary>
        /// opens color scheme menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Colourscheme_Click(object sender, EventArgs e)
        {
            RedPreset.Visible = true;
            GreenPreset.Visible = true;
            WhitePreset.Visible = true;
            OrangePreset.Visible = true;
            CloseColors.Visible = true;
        }

        /// <summary>
        /// red theme button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RedPreset_Click(object sender, EventArgs e)
        {
            ColourButtons(Color.DarkRed);
            ColourTextBox(Color.Red);
            ColourPanel(Color.LightCoral);
        }

        /// <summary>
        /// white theme button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WhitePreset_Click(object sender, EventArgs e)
        {
            ColourButtons(Color.MistyRose);
            ColourTextBox(Color.White);
            ColourPanel(Color.Gainsboro);
        }

        /// <summary>
        /// green theme click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GreenPreset_Click(object sender, EventArgs e)
        {
            ColourButtons(Color.DarkOliveGreen);
            ColourTextBox(Color.Green);
            ColourPanel(Color.DarkSeaGreen);
        }

        /// <summary>
        /// orange them click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrangePreset_Click(object sender, EventArgs e)
        {
            ColourButtons(Color.SandyBrown);
            ColourTextBox(Color.Orange);
            ColourPanel(Color.PeachPuff);
        }

        /// <summary>
        /// close colour menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseColoerMenu_Click(object sender, EventArgs e)
        {
            HideColour();
        }

        /// <summary>
        /// exits application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuitProgram_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
