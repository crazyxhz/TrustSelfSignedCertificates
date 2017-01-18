using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Windows;

namespace Trust
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            hostname.Text = GetFQDN();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var list = ports.Text.Split(',');
                var certs = new X509Certificate2Collection();
                foreach (var item in list)
                {
                    certs.Add(getCert(hostname.Text, int.Parse(item)));
                }
                WriteCerts(certs);
                MessageBox.Show("端口:"+ ports.Text+"信任成功");
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }

        }
        private X509Certificate2 getCert(string url, int port)
        {
            X509Certificate2 cert = null;
            using (TcpClient client = new TcpClient())
            {
                client.Connect(url, port);

                SslStream ssl = new SslStream(client.GetStream(), false,
                    new RemoteCertificateValidationCallback(
                            (sender, certificate, chain, sslPolicyErrors) =>
                            {
                                return true;
                            }
                        ), null);
                try
                {
                    ssl.AuthenticateAsClient(url);
                }
                catch (AuthenticationException e)
                {
                    ssl.Close();
                    client.Close();
                }
                catch (Exception e)
                {
                    ssl.Close();
                    client.Close();
                }
                cert = new X509Certificate2(ssl.RemoteCertificate);
                ssl.Close();
                client.Close();
                return cert;



            }

        }
        private void WriteCerts(X509Certificate2Collection certs)
        {
            X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            store.AddRange(certs);
            store.Close();
        }


        private string GetFQDN()
        {
            string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string hostName = Dns.GetHostName();

            domainName = "." + domainName;
            if (!hostName.EndsWith(domainName))  // if hostname does not already include domain name
            {
                hostName += domainName;   // add the domain name part
            }

            return hostName;                    // return the fully qualified name
        }
    }
}
