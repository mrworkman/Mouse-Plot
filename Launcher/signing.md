# Generate a new keypair

## Creating a certificate in PowerShell 

```ps
# Create a new code-signing certificate (self-signed).
$thumbprint = ( `
   New-SelfSignedCertificate `
      -Type CodeSigningCert `
      -KeyUsage DigitalSignature `
      -Subject mrworkman `
      -CertStoreLocation Cert:\CurrentUser\My `
      -NotAfter (Get-Date 2030-01-01)
).Thumbprint

# Move the certificate to the Root Certification Authorities store.
Move-Item `
   -Path Cert:\CurrentUser\My\$thumbprint `
   -Destination Cert:\CurrentUser\Root
```

### Signing an application's .exe

```ps
Set-AuthenticodeSignature `
   -Certificate ( `
      Get-ChildItem `
         -Path Cert:\CurrentUser\Root | `
            Where-Object { `
               $_.Thumbprint -like $thumbprint `
            })[0] `
   -TimestampServer http://timestamp.comodoca.com/authenticode `
   '.\Mouse Plot.exe'
```

#### Get an existing certificate's thumbprint

```ps
$thumbprint = ( `
   Get-ChildItem -Path Cert:\CurrentUser\My | `
   Where-Object { $_.Subject -like "CN=mrworkman" } `
).Thumbprint
```

## Creating a certificate using makecert (deprecated)

```
makecert -sv mykey.pvk -n "CN=MyCompany" -len 2048 mycert.cer -r
pvk2pfx -pvk mykey.pvk -spc mycert.cer -pfx mycert.pfx -po mypassword
```

### Import the pfx file
Import the certificate to `Trusted Root Certification Authorities/Certificates`.

### Sign application's .exe
```
signtool sign /f mycert.pfx /p <password> /t http://timestamp.comodoca.com/authenticode /v Renfrew.exe
```

### Install .exe
Copy the .exe and dependencies under `c:\Program Files\<something>`

### Reference:
https://www.curlybrace.com/words/2015/01/08/minimal-steps-to-fake-authenticode-signature-self-signing/
