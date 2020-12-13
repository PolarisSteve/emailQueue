using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace emailQueue
{
    public static class EmailRoutine
    {

        //Borrowed structure from https://docs.microsoft.com/en-us/dotnet/api/system.net.mail.smtpclient?view=netframework-4.8

        //Per Microsoft. -> SmtpClient and its network of types are poorly designed, we strongly recommend you use https://github.com/jstedfast/MailKit and https://github.com/jstedfast/MimeKit instead"
        //This sample uses MimeKit to serialize to a database and MailKit to deserialize and send via SMTP

        //To test this, I used Papercut -> https://github.com/ChangemakerStudios/Papercut


        public static void SendEmail(string To, string From)
        {

    //  Lets not use the current client, lets use our new MimeRoutine instead.
    //        using (SmtpClient client = new SmtpClient("localhost"))
    //        {

                // Specify the email sender.
                // Create a mailing address that includes a UTF8 character
                // in the display name.
                MailAddress from = new MailAddress(From, From, System.Text.Encoding.UTF8);
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

                    //Add an inline image
                    string attachmentPath = Environment.CurrentDirectory + @"\testimage.png";
                    Attachment inline = new Attachment(attachmentPath);
                    inline.ContentDisposition.Inline = true;
                    inline.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                    inline.ContentType.MediaType = "image/png";
                    inline.ContentType.Name = Path.GetFileName(attachmentPath);

                    message.Attachments.Add(inline);


                    //client.Send(message); Lets not use local SMTP
                    MimeRoutine.SendMimeMessage(message);


                    Console.WriteLine("Message Sent. Press any  key to exit.");

                    string answer = Console.ReadLine();
                }
    //        }
            Console.WriteLine("Goodbye.");

        }


    }
}
