@echo off
rem 
rem Use gpg to generate different openpgp output
rem

set GPG=gpg --homedir=.. --batch --yes --passphrase test --no-version

for %%f in (*.bin) do (

rem Sign
%GPG% --output %%f.rsa.sha1.sign        -a --sign %%f
%GPG% --output %%f.rsa.sha1.clearsign   --clearsign %%f
%GPG% --output %%f.rsa.sha256.clearsign --digest-algo SHA256 --clearsign %%f

rem Encrypt
%GPG% --output %%f.rsa.enc -ae -r rsa@bob.com %%f

rem Encrypt & Sign
%GPG% --output %%f.rsa.enc.sign -aes -r rsa@bob.com %%f

)
