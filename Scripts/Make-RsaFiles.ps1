<#
	Make a RSA Private/Public Key Pair
	
	See: 
	* https://docs.github.com/en/authentication/managing-commit-signature-verification/generating-a-new-gpg-key
    * https://www.gnupg.org/documentation/manuals/gnupg/Unattended-GPG-key-generation.html

	Supported algorithms:
		Pubkey: 
			RSA, ELG, DSA, ECDH, ECDSA, EDDSA
		Cipher: 
			IDEA, 3DES, CAST5, BLOWFISH, AES, AES192, AES256, TWOFISH,
			CAMELLIA128, CAMELLIA192, CAMELLIA256
		Hash: 
			SHA1, RIPEMD160, SHA256, SHA384, SHA512, SHA224
		Compression: 
			Uncompressed, ZIP, ZLIB, BZIP2
#>

<#
	Functions
#>

function Remove-IfExists {
	Param (
		[string]$name,
		[bool]$isFolder = $false
	)

	if($isFolder) {
		if(Test-Path -Path $name -PathType Container) {
			Remove-Item -Path $name -Force -Recurse
		}
	} else {
		if(Test-Path -Path $name -PathType Leaf) {
			Remove-Item -Path $name -Force
		}
	}
}

<#
	Main
#>

[string]$name="test";
[string]$emai="${name}@nomail.org";
[string]$note="Test Comment";

[string]$algo="ECDSA";
[string]$curv="NIST P-256";
[string]$pass="p@ss#w0rd-";

$env:GNUPGHOME=$null;
[string]$tempFolder = "c:\temp\gpgtemp";
Remove-IfExists -name $tempFolder -isFolder $true;
$null = New-Item -ItemType Directory -Force -Path $tempFolder;
$env:GNUPGHOME=$tempFolder;

[string]$passFile= "${tempFolder}\pass.txt";
Remove-IfExists -name $passFile
$pass | Out-File -FilePath $passFile -Encoding utf8 -NoNewline

[string]$config  = "${tempFolder}\config.txt";
Remove-IfExists -name $config

[string]$pubFile = "${PSScriptRoot}\public.pgp.txt";
Remove-IfExists -name $pubFile
[string]$priFile = "${PSScriptRoot}\private.pgp.txt";
Remove-IfExists -name $priFile

# Generate configuration for key
"Key-Type: ${algo}" | Out-File -Encoding utf8 -Path $config -append
"Key-Curve: ${curv}" | Out-File -Encoding utf8 -Path $config -append
"Name-Real: ${name}" | Out-File -Encoding utf8 -Path $config -append
"Name-Comment: ${note}" | Out-File -Encoding utf8 -Path $config -append
"Name-Email: ${emai}" | Out-File -Encoding utf8 -Path $config -append
"Expire-Date: 0" | Out-File -Encoding utf8 -Path $config -append
"Passphrase: ${pass}" | Out-File -Encoding utf8 -Path $config -append
"%commit" | Out-File -Encoding utf8 -Path $config -append

gpg `
	--batch `
	--generate-key $config

gpg `
	--output $pubFile `
	--armor `
	--export $emai

gpg `
	--pinentry-mode loopback `
	--passphrase-file $passFile `
	--batch `
	--yes `
	--armor `
	--output $priFile `
	--export-secret-key $emai

Remove-IfExists -name $tempFolder -isFolder $true
$env:GNUPGHOME=$null;
exit 0;