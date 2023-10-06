<#
	Make a RSA Private/Public Key Pair
	

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

# In a real examplee these should be your actual values
[string]$name="test";
[string]$emai="${name}@nomail.org";
[string]$note="Test Comment";

# Choose the algo and curve that suits your purposes (encrypt,sign,auth)
[string]$allo="encrypt,sign,auth";
[string]$algo="RSA";
[string]$curv="ELG-E";
[int]$bits = 1024;

# This is purely for demo purposes, don't do this
[string]$pass="p@ss#w0rd-";

# Create a Ephemeral-home-directory (so keys do not go into your key store)
$env:GNUPGHOME=$null;
[string]$tempFolder = "c:\temp\gpgtemp";
Remove-IfExists -name $tempFolder -isFolder $true;
$null = New-Item -ItemType Directory -Force -Path $tempFolder;
$env:GNUPGHOME=$tempFolder;

# Put the password into a file for script use
[string]$passFile= "${tempFolder}\pass.txt";
Remove-IfExists -name $passFile
$pass | Out-File -FilePath $passFile -Encoding utf8 -NoNewline

# Make sure script to feed to GPG is removed
[string]$config  = "${tempFolder}\config.txt";
Remove-IfExists -name $config

# Where to put output
[string]$parent = (get-item $PSScriptRoot).parent.FullName
[string]$pubFile = "${parent}\public.pgp.txt";
Remove-IfExists -name $pubFile
[string]$priFile = "${parent}\private.pgp.txt";
Remove-IfExists -name $priFile

# Make a script GPG will use to genenrate secret
"Key-Type: ${algo}" | Out-File -Encoding utf8 -Path $config -append
"Key-Length: ${bits}"  | Out-File -Encoding utf8 -Path $config -append
#"Key-Curve: ${curv}" | Out-File -Encoding utf8 -Path $config -append
#"Key-Usage: ${allo}" | Out-File -Encoding utf8 -Path $config -append
#"Subkey-Length: ${bits}"  | Out-File -Encoding utf8 -Path $config -append
"Name-Real: ${name}" | Out-File -Encoding utf8 -Path $config -append
"Name-Comment: ${note}" | Out-File -Encoding utf8 -Path $config -append
"Name-Email: ${emai}" | Out-File -Encoding utf8 -Path $config -append
"Expire-Date: 0" | Out-File -Encoding utf8 -Path $config -append
"Passphrase: ${pass}" | Out-File -Encoding utf8 -Path $config -append
#"%no-protection"  | Out-File -Encoding utf8 -Path $config -append
"%commit" | Out-File -Encoding utf8 -Path $config -append

# Use the script and make the secret
gpg `
	--batch `
	--generate-key $config

# Export the secret's public key for distribution to encypher files
# You will have to tell them the algo + curve you used
gpg `
	--output $pubFile `
	--armor `
	--export $emai

# Export the private key for just your use, this should be kept secure!
gpg `
	--pinentry-mode loopback `
	--passphrase-file $passFile `
	--batch `
	--yes `
	--armor `
	--output $priFile `
	--export-secret-key $emai

# Get rid of Ephemeral-home-directory
Remove-IfExists -name $tempFolder -isFolder $true
$env:GNUPGHOME=$null;

# Exit Ok
return;