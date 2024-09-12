@echo off
setlocal
set TARGET=%~1
echo Target is %TARGET%.

IF "%SIGN_EXE%"=="" (
powershell -Command "Set-AuthenticodeSignature -Certificate ( Get-ChildItem -Path Cert:\CurrentUser\Root | Where-Object { $_.Thumbprint -like '8941c8a43f988fb0cf2abd2a11c926e956078028'})[0] -TimestampServer http://timestamp.comodoca.com/authenticode '%TARGET%'"
)

endlocal
