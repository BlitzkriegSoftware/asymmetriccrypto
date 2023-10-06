# asymmetriccrypto
A demo of asymmetric cryptography in C# 9.0, NET 6.0

This is a unit-test driven project.

## References

* https://docs.github.com/en/authentication/managing-commit-signature-verification/generating-a-new-gpg-key

* https://www.gnupg.org/documentation/manuals/gnupg/Unattended-GPG-key-generation.html

* https://www.gnupg.org/documentation/manuals/gnupg/Ephemeral-home-directories.html

* https://github.com/mattosaurus/PgpCore

## How to use

The `PowerShell` command `Make-RsaFiles.ps1` generates two matched files:

* `private.pgp.txt` - Which we actually use
* `public.pgp.txt` - Which matches the private key and could be given to anyone to help encypher files

## The unit tests

The unit test start up loads the private key for use in tests.

> Notice that this format only allows input text strings of length of `AsymmetricCrypto.MaxCharsSupported` (250)

The tests exercise this class by encrypting and decrypting strings.

## AsymmetricCrypto.cs

1. There is a bit of indirection to this. 1st the `openssl` library makes a private key but with `---` header and footers, split into lines w. `CR/LF` so we have to clean that up using `KeyConverter()` to turn this into one long base64 string without line splits or headers and footers.

2. For compatibility we use `UnicodeEncoding` to turn the strings into byte arrays.

3. Then we call `rsa.Encrypt()` on the byte array, and turn the resulting byte array into base64 string.

4. The reverse is done on decryption

> The class is IDisposable to get rid of the `rsa` object which is large and holds resources.
