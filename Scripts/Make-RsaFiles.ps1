<#
	Make a RSA Private/Public Key Pair
	See: https://docs.github.com/en/authentication/managing-commit-signature-verification/generating-a-new-gpg-key

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

[int]$klen = 1024;
[string]$name="test";
[string]$algo="ECDSA";
[string]$curv="NIST P-256";
[string]$cans="YES";
[string]$ktyp="MASTER";
[string]$emai="${name}@nomail.org";
[string]$uuid="name=${name},email=${emai}";
[string]$note="Test Comment";
[string]$pass="p@ss#w0rd-";

# Generate File
[string]$config=".\config.txt";
if (Test-Path $config -PathType Leaf) {
	Remove-Item $config
}
# Keyfile File
[string]$keyf=".\key.bin";
if (Test-Path $keyf -PathType Leaf) {
	Remove-Item $keyf
}

# Generate configuration for key
"%pubring ${keyf}" | Out-File -Encoding utf8 -Path $config -append
"Key-Type: ${algo}" | Out-File -Encoding utf8 -Path $config -append
"Key-Curve: ${curv}" | Out-File -Encoding utf8 -Path $config -append
"Name-Real: ${name}" | Out-File -Encoding utf8 -Path $config -append
"Name-Comment: ${note}" | Out-File -Encoding utf8 -Path $config -append
"Name-Email: ${emai}" | Out-File -Encoding utf8 -Path $config -append
"Expire-Date: 0" | Out-File -Encoding utf8 -Path $config -append
"Passphrase: ${pass}" | Out-File -Encoding utf8 -Path $config -append
"%commit" | Out-File -Encoding utf8 -Path $config -append

gpg --batch --generate-key $config

gpg --output public.pgp --armor --export $emai < $keyf
gpg --output private.pgp --armor --export-secret-key $emai < $keyf
