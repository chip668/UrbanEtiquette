using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Security.Cryptography;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Management;



namespace Anzeige
{
    public class EmailSender
    {


        public string smtpServer;
        public int smtpPort;
        public string smtpUsername;
        public string smtpPassword;
        public static string GenerateKey()
        {
            // Maschineninformationen sammeln
            string machineInfo = GetMachineInformation();

            // Benutzernamen abrufen
            string username = Environment.UserName;

            // Kombination von Maschineninformationen und Benutzernamen für den Schlüssel
            string combinedInfo = machineInfo + username;

            // Schlüssel erstellen
            string key = ComputeHash(combinedInfo);

            return key;
        }

        private static string GetMachineInformation()
        {
            // Beispiel: Hier werden Informationen über den Prozessor und die Festplatte gesammelt
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                ManagementObjectCollection collection = searcher.Get();
                string processorInfo = "";

                foreach (ManagementObject obj in collection)
                {
                    processorInfo += obj["Name"].ToString();
                }

                searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
                collection = searcher.Get();
                string diskInfo = "";

                foreach (ManagementObject obj in collection)
                {
                    diskInfo += obj["Model"].ToString();
                }

                return processorInfo + diskInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler beim Abrufen der Maschineninformationen: " + ex.Message);
                return "";
            }
        }

        private static string ComputeHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }


        public EmailSender(string filePath)
        {
            LoadConfigurationFromFile(filePath);
        }
        public EmailSender(string server, int port, string username, string password)
        {
            UpdateConfiguration(server, port, username, password);
        }
        public void SendEmailWithAttachments(string to, string subject, string body, string attachmentPath)
        {
            List<string> attachmentPaths = new List<string>();
            attachmentPaths.Add(attachmentPath);
            SendEmailWithAttachments(to, subject, body, attachmentPaths);
        }



        public void SendEmailWithAttachments(string to, string subject, string body, List<string> attachmentPaths)
        {
            if (IsValidEmail(to))
            {
                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient smtpClient = new SmtpClient(smtpServer);

                    // Absender
                    mail.From = new MailAddress(smtpUsername);

                    // Empfänger
                    mail.To.Add(to);

                    // Betreff und Inhalt der E-Mail
                    mail.Subject = subject;
                    mail.Body = body;

                    // Anhänge hinzufügen
                    foreach (var attachmentPath in attachmentPaths)
                    {
                        Attachment attachment = new Attachment(attachmentPath);
                        mail.Attachments.Add(attachment);

                        // Anhang sofort freigeben
                        attachment.Dispose();
                    }

                    // Konfiguration des SMTP-Clients
                    smtpClient.Port = smtpPort;
                    smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    smtpClient.EnableSsl = true;

                    // E-Mail senden
                    smtpClient.Send(mail);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fehler beim Senden der E-Mail: " + ex.Message);
                }
            }
        }
        public void SaveConfigurationToFile(string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Verschlüsselte Konfigurationsdaten speichern
                    writer.WriteLine(EncryptString(smtpServer));
                    writer.WriteLine(EncryptString(smtpPort.ToString()));
                    writer.WriteLine(EncryptString(smtpUsername));
                    writer.WriteLine(EncryptString(smtpPassword));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler beim Speichern der Konfigurationsdaten: " + ex.Message);
            }
        }
        public void LoadConfigurationFromFile(string filePath)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    // Verschlüsselte Konfigurationsdaten laden und entschlüsseln
                    smtpServer = DecryptString(reader.ReadLine());
                    smtpPort = int.Parse(DecryptString(reader.ReadLine()));
                    smtpUsername = DecryptString(reader.ReadLine());
                    smtpPassword = DecryptString(reader.ReadLine());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler beim Laden der Konfigurationsdaten: " + ex.Message);
            }
        }
        private string EncryptString(string input)
        {
            using (Aes aesAlg = Aes.Create())
            {
                // Hier müsstest du einen sicheren Schlüssel und Initialisierungsvektor verwenden
                // Für dieses Beispiel sind sie einfach festgelegt, was in der Praxis unsicher wäre.
                aesAlg.Key = Encoding.UTF8.GetBytes(GenerateKey());
                aesAlg.IV = Encoding.UTF8.GetBytes(GenerateKey());

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(input);
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
        private string DecryptString(string cipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                // Hier müsstest du denselben Schlüssel und Initialisierungsvektor verwenden,
                // den du beim Verschlüsseln verwendet hast.
                aesAlg.Key = Encoding.UTF8.GetBytes(GenerateKey());
                aesAlg.IV = Encoding.UTF8.GetBytes(GenerateKey());

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
        public void UpdateConfiguration(string server, int port, string username, string password)
        {
            smtpServer = server;
            smtpPort = port;
            smtpUsername = username;
            smtpPassword = password;
        }
        public bool IsValidConfiguration()
        {
            // Hier können weitere Überprüfungen je nach Anforderungen durchgeführt werden
            // Zum Beispiel: Überprüfung auf leere Strings, gültige Portbereiche usw.

            return !string.IsNullOrEmpty(smtpServer) &&
                   smtpPort > 0 &&
                   !string.IsNullOrEmpty(smtpUsername) &&
                   !string.IsNullOrEmpty(smtpPassword) &&
                   IsValidSmtpServer(smtpServer, smtpPort)
                   ;
        }
        public static bool IsValidSmtpServer(string server, int port)
        {
            if (IsValidIPAddress(server))
                return IsValidPort(port);

            return IsValidDomainWithSmtpProtocol(server, port);
        }
        private static bool IsValidIPAddress(string ipAddress)
        {
            if (IPAddress.TryParse(ipAddress, out IPAddress address))
            {
                if (address.AddressFamily == AddressFamily.InterNetwork || address.AddressFamily == AddressFamily.InterNetworkV6)
                    return true;
            }

            return false;
        }
        private static bool IsValidDomainWithSmtpProtocol(string domain, int port)
        {
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(domain);

                foreach (IPAddress address in entry.AddressList)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        // Überprüfe den Port
                        if (IsValidPort(port))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (SocketException)
            {
                // Das bedeutet, dass die Domain nicht aufgelöst werden konnte
            }

            return false;
        }
        private static bool IsValidPort(int port)
        {
            // Überprüfe, ob der Port einem Standard-E-Mail-Port oder einem verschlüsselten E-Mail-Port entspricht
            return port == 25 || port == 587 || port == 465;
        }
        public static bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);
            if (regex.IsMatch(email))
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(email);

                    // Überprüfe die Gültigkeit der Domain durch einen Netzwerkzugriff
                    return addr.Address == email && IsDomainReachable(addr.Host);
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
        private static bool IsDomainReachable(string domain)
        {
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(domain);

                // Hier könntest du weitere Überprüfungen vornehmen
                // Zum Beispiel: Überprüfen, ob der DNS-Eintrag einen gültigen Mailserver hat

                return true;
            }
            catch (SocketException)
            {
                return false;
            }
        }

    }
}
