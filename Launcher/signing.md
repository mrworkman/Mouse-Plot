### Generate a new keypair
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