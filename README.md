# asymmetriccrypto
A demo of asymmetric cryptography in C# Core. 3.1

This is a unit-test driven project.

## How to use

The `git bash` command `MakeKeys.sh` generates two matched files:

* `rsa4096.private` - Which we actually use
* `rsa4096.public` - Which matches the private key and is for completeness

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