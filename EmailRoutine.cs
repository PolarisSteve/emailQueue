using MimeKit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace emailQueue
{
    public static class EmailRoutine
    {

        //Borrowed structure from https://docs.microsoft.com/en-us/dotnet/api/system.net.mail.smtpclient?view=netframework-4.8


        #region "SMTP Client Replacement"
        // The following code will use MailKit and MimeKit together to transmit the mailmessage through a byte array.
        // While the message is in it's byte array form it an be transmitted from one system to another through various transports including as bytearray ( varbinary(max) ) in a database record
        // and be picked up through another process on another machine to be sent.
        // I Will show example of serializing, deserializing and sending.

        public static byte[] AddMessage(MailMessage message)
        {

            MimeMessage msg = (MimeMessage)message;
            
            using (MemoryStream stream = new MemoryStream())
            {
                msg.WriteTo(stream);
                var contents = stream.ToArray();

                //send message through queue
                return contents;
            }
            
        }


        public static void SendMimeMessage(byte[] msg)
        {

            using (MemoryStream msgstream = new MemoryStream(msg))
            {
                MimeMessage msgMime = MimeMessage.Load(msgstream);

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("localhost");
                    client.Send(msgMime);

                }

            }
        }



        #endregion



        public static void SendEmail(string To, string From)
        {

    //        using (SmtpClient client = new SmtpClient("localhost"))
    //        {
                // Specify the email sender.
                // Create a mailing address that includes a UTF8 character
                // in the display name.
                MailAddress from = new MailAddress(From,
                   From,
                System.Text.Encoding.UTF8);
                // Set destinations for the email message.
                MailAddress to = new MailAddress(To);
                // Specify the message content.

                using (MailMessage message = new MailMessage(from, to))
                {
                    message.Body = "This is a test email message sent by an application. ";
                    // Include some non-ASCII characters in body and subject.
                    string someArrows = new string(new char[] { '\u2190', '\u2191', '\u2192', '\u2193' });
                    message.Body += Environment.NewLine + someArrows;
                    message.BodyEncoding = System.Text.Encoding.UTF8;
                    message.Subject = "test message 1" + someArrows;
                    message.SubjectEncoding = System.Text.Encoding.UTF8;



                //client.Send(message); Lets not use local SMTP
                // inTransitMessage represents our ability to queue up and send over wire.
                    var inTransitMessage = AddMessage(message);
                    SendMimeMessage(inTransitMessage);
                    
                    
                    Console.WriteLine("Messgae Sent. Press any  key to exit.");

                    string answer = Console.ReadLine();
                }
    //        }
            Console.WriteLine("Goodbye.");

        }
    
    
    }
}
