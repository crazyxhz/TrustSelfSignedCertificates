#One Click trust Self-Signed Certificates

##概览
ArcGIS Enterprise's products: Server, Portal, DataStore, WebAdaptor, when installed in debug and testing environments all use Self-Signed certificates. Manually download these certificates from webpage and install it to local machine's trust root will be a tedious job. Here's this little tool come to the rescure. Save us from the repetitive job.


 
##Introduction:

[Binary download](https://github.com/crazyxhz/TrustSelfSignedCertificates/raw/master/bin/Trust.exe)

[Source code](https://github.com/crazyxhz/TrustSelfSignedCertificates/tree/master/src)

Install a certificate to local machine's cert store needs adminstrator previlege. So when this app starts it will prompt for admin elevation. After admin previlege is granted. It will read local machine's FQDN (fully qualified domain name). If you need to trust a remote machine's certificates. Please enter the remote machine's FQDN or Ip address into the first textbox. The default ports is Server(6443),Portal(7443),DataStore(2443),IIS(443) ports, you can change these ports when necessary (comma-separated)



![](http://p1.bpimg.com/514597/1c7f3cee2e644b10.png)

##Key codes
```
            X509Certificate2 cert = null;
            using (TcpClient client = new TcpClient())
            {
                client.Connect(url, port);
                //eastablish TLS connection and ingore certificate error
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
                //save the connection's certificate
                cert = new X509Certificate2(ssl.RemoteCertificate);
                ssl.Close();
                client.Close();
                return cert;
            }
            
            //save the certificates to the localmachine's trust root store which requires admin previlege
            X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            store.AddRange(certs);
            store.Close();
            
```

##System requirements：
.Net Framework 4.0 or above








