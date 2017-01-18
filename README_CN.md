#一键信任自签名证书小程序

##概览
ArcGIS Enterprise产品体系中，在调试、测试环境下，大量使用了自签名证书，Server,Portal,DataStore,WebAdaptor每一个产品默认都使用了自签名证书。手动在浏览器中去下载，安装，信任这些自签名证书是单调且枯燥的工作（而且由于浏览器的缓存，信任了证书之后大部分情况下还需要重启浏览器）。该小工具就是为了把我们从以上重复劳动中解放出来而开发的。


 
##说明：

[直接下载地址](https://github.com/crazyxhz/TrustSelfSignedCertificates/raw/master/bin/Trust.exe)

[源码](https://github.com/crazyxhz/TrustSelfSignedCertificates/tree/master/src)

信任证书需要管理员权限，程序启动后会自动请求管理员权限。默认会读取本机的FQDN，如果需要信任远程机器的证书，请手动填写正确的远程机器的FQDN或者IP地址。默认端口是Server(6443),Portal(7443),DataStore(2443),IIS(443)端口，可以根据实际情况信任所需的SSL端口。

![](http://p1.bpimg.com/514597/1c7f3cee2e644b10.png)

##关键代码
```
            X509Certificate2 cert = null;
            using (TcpClient client = new TcpClient())
            {
                client.Connect(url, port);
                //建立SSL链接并且忽略证书错误
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
                //将远程证书保存
                cert = new X509Certificate2(ssl.RemoteCertificate);
                ssl.Close();
                client.Close();
                return cert;
            }
            
            //将保存下来的远程证书数组插入本地信任根证书（需要管理员权限）
            X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            store.AddRange(certs);
            store.Close();
            
```

##系统需求：
.Net Framework 4.0及以上








