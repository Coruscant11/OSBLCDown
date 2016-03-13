using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace OSBLCDown
{
    class Program
    {
        static void Main(string[] args)
        {
            int time = int.Parse(System.IO.File.ReadAllText("config.txt"));

            Console.WriteLine("Lancement du programme avec pour paramètre " + time.ToString() + " secondes.\nAppuyez sur entrée pour valider...\n");
            Console.ReadLine();

            do
            {
                try
                {
                    MinimalisticTelnet.TelnetConnection connection = new MinimalisticTelnet.TelnetConnection("server.osblc.fr", 10011);
                    if (connection.Read().Length > 2)
                    {
                        Console.WriteLine("Le serveur est en ligne.");
                    }
                    else
                    {
                        Console.WriteLine("########################");
                        Console.WriteLine("Reponse invalide.\nEnvois d'un rapport par e-mail.");
                        Console.WriteLine("########################");
                        SendMail("mateox06@hotmail.fr", "La réponse du serveur est invalide", DateTime.Now.ToString());
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("########################");
                    Console.WriteLine("Le serveur est hors-ligne.\nEnvois d'un rapport par e-mail.");
                    Console.WriteLine("########################");
                    SendMail("mateox06@hotmail.fr", e.Message, DateTime.Now.ToString());
                }
                finally
                {
                    System.Threading.Thread.Sleep(time * 1000);
                }
                Console.WriteLine("\n");
            } while (true);
        }

        static void SendMail(string destination, string error, string date)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("teamspeakstats@gmail.com", "mateolepddu06"),
                EnableSsl = true
            };
            try
            {
                client.Send("teamspeakstats@gmail.com", destination, "Notification d'erreur du BOT", "Le serveur TeamSpeak semble être down d'après le BOT OSBLCDown, le " + date + ".\n" + "Mais que fait l'administration ?\n\nVoici l'erreur : \n" + error);
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur lors de l'envoi de l'e-mail\n" + e.Message);
            }
        }
    }
}
