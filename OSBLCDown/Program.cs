using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;

namespace OSBLCDown
{
    class Program
    {
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();
         
        public static int SW_HIDE = 0;
        public static int SW_SHOW = 5; 

        public static bool isShowed;

        static void Main(string[] args)
        {
            Console.Title = "OSBLCDown - by Coruscant11";
            isShowed = true;

            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(LoadTrayIcon));
            thread.Start();

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

                    int newtime = 10000;
                    bool boucle = true;
                    SendMail("mateox06@hotmail.fr", e.Message, DateTime.Now.ToString()); // Envois du rapport

                    while (boucle)
                    {
                        Console.WriteLine("Echec de la tentative. Erreur : \n" + e.Message + "\nRetentative dans 10 secondes...");
                        System.Threading.Thread.Sleep(newtime); // Sleep le Thread de 10 secondes

                        try
                        {
                            Process();
                            boucle = false;
                            Console.WriteLine("########################");
                            Console.WriteLine("Le serveur est de nouveau en ligne. Envois d'un e-mail.");
                            SendMailSuccess("mateox06@hotmail.fr", DateTime.Now.ToString()); // Envois du rapport
                            Console.WriteLine("########################");
                        }
                        catch
                        {
                            Console.WriteLine("########################");
                            Console.WriteLine("Le serveur est toujours hors-ligne.");
                            Console.WriteLine("########################");
                            boucle = true;
                        }
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
        /// Envois un mail d'erreur
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
                client.Send("teamspeakstats@gmail.com", destination, "ERREUR ! - OSBLCDown pas content", "Le serveur TeamSpeak semble être down d'après le BOT OSBLCDown, le " + date + ".\n" + "Mais que fait l'administration ?\n\nVoici l'erreur : \n" + error);
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur lors de l'envoi de l'e-mail\n" + e.Message); // Erreur
            }
        }

        /// <summary>
        /// Envois un mail de succès
        /// </summary>
        /// <param name="destination">Adresse du destinataire</param>
        /// <param name="date">Date de l'évènement</param>
        static void SendMailSuccess(string destination, string date)
        {
            var client = new SmtpClient("smtp.gmail.com", 587) // Connexion au serveur SMTP avec l'url et le port
            {
                Credentials = new NetworkCredential("teamspeakstats@gmail.com", "mateolepddu06"), // Entrée des données de connection
                EnableSsl = true // Nécessaire
            };
            try
            {
                /* Envois de l'email */
                client.Send("teamspeakstats@gmail.com", destination, "C'EST BON ! - OSBLCDown content", "Le serveur TeamSpeak semble être redevenue en ligne d'après le BOT OSBLCDown, le " + date + ".\n" + "GG l'administration ?");
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur lors de l'envoi de l'e-mail\n" + e.Message); // Erreur
            }
        }

        static void LoadTrayIcon()
        {
            Application.EnableVisualStyles();
            new TrayIcon();
            Application.Run();
        }
    }
}
