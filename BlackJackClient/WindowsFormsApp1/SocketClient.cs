using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class SocketClient
    {
        //creates client that connects to server
        //synchronus socket means client app is suspended until server responds
        //app sends a string and then displays string returned from server
        public void StartClient(TextBox SendBox, TextBox ReceiverBox, TextBox ErrorBox, int deposit)
        {
            //data buffer for incoming data
            byte[] bytes = new byte[1024];

            //connect to remote device
            try
            {
                //establish the remote endpoint for the socket
                //grabs Host name from local computer (GetHostName), converts it to an IPhost instance (ipHostInfo)
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName()); //IPHostEntry: containment class for internet host address. Dns: Provides simple domain name resolution

                IPAddress ipAddress = ipHostInfo.AddressList[0]; //IPAddress: provides IP address. AddressList[0]: gets or sets all IP adresses associated with host
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000); //IPEndPoint represents network endpoint as an ip address and port number

                //create a TCP/IP socket
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp); //Socket: Berkley sockets interface. Stream: supports reliable 2 way connection based byte streams without preservation of boundaries
                //socketType: specifies type of socket this instance (sender) represents. ProtocolType: specifies protocols supported by this socket

                try
                {
                    sender.Connect(remoteEP); //connect: establishes connections to remote host

                    ErrorBox.Text = "Socket connected to " + sender.RemoteEndPoint.ToString(); //displays server
                    
                    string sentFile = Convert.ToString(deposit) + "<EOF>"; //user winnings are sent to server

                    SendBox.Text = sentFile;

                    //encode the data through the socket
                    byte[] msg = Encoding.ASCII.GetBytes(sentFile); //translates string to ASCII

                    //send data through socket
                    int bytesSent = sender.Send(msg); //send: sends data to a connected device

                    //recieve the response from remote device
                    int bytesRec = sender.Receive(bytes); //recieves data from a bound socket into a receiver buffer
                    ReceiverBox.Text = "From server: " + Encoding.ASCII.GetString(bytes, 0, bytesRec); //translates ASCII into string (decodes server message)

                    Release(sender);
                }
                catch (ArgumentNullException ane) //occurs when null 
                {
                    ErrorBox.Text = "ArgumentNullException " + ane.ToString();
                }
                catch (SocketException se) //occurs when socket has trouble accessing another socket
                {
                    ErrorBox.Text = "SocketException " + se.ToString();
                }
                catch (Exception e) //occurs during app execution
                {
                    ErrorBox.Text = "Unexpected exception " + e.ToString();
                }
            }
            catch (Exception e) //occurs during app execution
            {
                ErrorBox.Text = e.ToString();
            }

            void Release(Socket sender)
            {
                //release socket
                sender.Shutdown(SocketShutdown.Both); //Shutdown: disables 'send' and 'receive'  on a socket
                sender.Close(); //Close: closes socket connection
            }
        }

        internal void StartClient()
        {
            throw new NotImplementedException();
        }
    }
}