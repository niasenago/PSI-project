##When running your ASP.NET Core MVC application, you encounter an error related to SSL certificate validation?
*LINUX*
The error might include messages such as "The remote certificate is invalid because of errors in the certificate chain" or "UntrustedRoot."
-  Obtain the SSL Certificate from the API

```bs 
openssl s_client -connect your-api-host:your-api-port -showcerts
```

e.g. ` openssl s_client -connect localhost:7109 -showcerts`

Copy the entire certificate block starting from -----BEGIN CERTIFICATE----- to -----END CERTIFICATE-----

- Save Certificate:
Save certificate in a separate text file with the `.pem `extension.
- Copy Certificate to the `/usr/local/share/ca-certificates/` :
```bash
sudo cp localhost.pem /usr/local/share/ca-certificates/
```
- Update Certificate Store:
Run the following command to update the trusted certificate store:
```bash  
sudo update-ca-certificates
```