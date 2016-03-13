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
            int time = int.Parse(System.IO.File.ReadAllText("config.txt")); // Récupère l'intervalle de vérification dans le fichier config.txt

            Console.WriteLine("Lancement du programme avec pour paramètre " + time.ToString() + " secondes.\nAppuyez sur entrée pour valider...\n");
            Console.ReadLine();

            do
            {
                Console.WriteLine("------------------------------------------------");

                try
                {
                    Process();
                } catch (Exception e) {

                    Console.WriteLine("Echec de la tentative. Erreur : \n" + e.Message + "\nRetentative dans 5 secondes...");
                    System.Threading.Thread.Sleep(5000); // Sleep le Thread de 5 secondes

                    try
                    {
                        Process();
                    }

                    catch (Exception e2)
                    {
                        Console.WriteLine("########################");
                        Console.WriteLine("Le serveur est bel et bien hors-ligne.\nEnvois d'un rapport par e-mail.");
                        SendMail("mateox06@hotmail.fr", e2.Message, DateTime.Now.ToString()); // Envois du rapport
                        Console.WriteLine("########################");
                    }
                }
                finally
                {
                    System.Threading.Thread.Sleep(time * 1000);
                }
                Console.WriteLine("------------------------------------------------\n");
            } while (true);
        }

        /// <summary>
        /// Processus de vérification du serveur
        /// </summary>
        static void Process()
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

        /// <summary>
        /// Envois un mail
        /// </summary>
        /// <param name="destination">Adresse du destinataire</param>
        /// <param name="error">Erreur a envoyer dans le mail</param>
        /// <param name="date">Date de l'erreur</param>
        static void SendMail(string destination, string error, string date)
        {
            var client = new SmtpClient("smtp.gmail.com", 587) // Connexion au serveur SMTP avec l'url et le port
            {
                Credentials = new NetworkCredential("teamspeakstats@gmail.com", "mateolepddu06"), // Entrée des données de connection
                EnableSsl = true // Nécessaire
            };
            try
            {
                /* Envois de l'email */
                client.Send("teamspeakstats@gmail.com", destination, "Notification d'erreur du BOT", "Le serveur TeamSpeak semble être down d'après le BOT OSBLCDown, le " + date + ".\n" + "Mais que fait l'administration ?\n\nVoici l'erreur : \n" + error);
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur lors de l'envoi de l'e-mail\n" + e.Message); // Erreur
            }
        }
    }
}
