using System.IO;
using MailKit.Net.Smtp;
using MimeKit;

namespace emailQueue
{
    public static class MimeRoutine
    {

        #region "SMTP Client Replacement"
        // The following code will use MailKit and MimeKit together to transmit the MailMessage through a byte array.
        // While the message is in it's byte array form it an be transmitted from one system to another through various transports including as bytearray ( varbinary(max) ) in a database record
        // and be picked up through another process on another machine to be sent.
        // I Will show example of serializing, deserializing and sending.


        /// <summary>
        /// This takes the Microsoft Mail Message, and converts to a byte array
        /// which is stored in a local queue which is passed to the MimeMessageSender.
        /// </summary>
        /// <param name="message"></param>
        public static void SendMimeMessage(System.Net.Mail.MailMessage message) {

            // MimeMessageReciever is what creates the message
            byte[] QueueArray = MimeMessageReciever(message);

            /*
                QueueArray represents the data going across the wire in a serialized format.
                Here you would add to the database and a separate process would send
                using the same code as in MimeMessageSender
            */

            // In the real world the following code would be run in it's own process
            // and send the mail
            MimeMessageSender(QueueArray);


        }

        /// <summary>
        /// Converts the System.Net.Mail.Message to the byte array by
        /// coercing into a MimeMessage.
        /// </summary>
        /// <param name="message">System.Net.Mail.Message</param>
        /// <returns></returns>
        private static byte[] MimeMessageReciever(System.Net.Mail.MailMessage message)
        {
            // This line is really the most important.
            // It allows you to take a MailMessage type and coerce it into a MimeMesage which is serializable.
            // This exists in the DMZ server
            MimeMessage msg = (MimeMessage)message;
            using (MemoryStream stream = new MemoryStream())
            {
                msg.WriteTo(stream);
                var contents = stream.ToArray();
                return contents;
            }
        }


        /// <summary>
        /// This routine shows how to deserialize the object and send using MailKit.
        /// After pulling from queue, send using this routine.
        /// </summary>
        /// <param name="msg"></param>
        private static void MimeMessageSender(byte[] msg)
        {
            using (MemoryStream msgstream = new MemoryStream(msg))
            {
                // This would exist in it's own process using a queue / table to read and update.
                // on a machine inside your domain.

                MimeMessage msgMime = MimeMessage.Load(msgstream);
                using (var client = new SmtpClient())
                {
                    client.Connect("localhost");
                    client.Send(msgMime);
                }
            }
        }
        # endregion

    }
}
