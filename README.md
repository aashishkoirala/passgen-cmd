### Password Generator Command Line
##### By [Aashish Koirala](https://aashishkoirala.github.io)

[GitHub Repository](https://github.com/aashishkoirala/passgen-cmd) | [Download Binary](https://aashishkoirala.github.io/passgen-cmd/genpass.zip)

Command line application to generate secure passwords for use in different sites. Written in **F#**. Uses **PBKDF2** with 1000 iterations. Inspired by [this thing](https://github.com/dotcypress/password).

#### Installation
- Download and unzip the binary from the link above, OR:
- Clone the source code and run **build.cmd** (make sure **fsc.exe** is in the path - this is just a single F# source file that needs to be compiled using the F# compiler).

#### Usage
> **genpass** *master-password* *site-name* *user-name* [*password-length*]

Where:
- *master-password* is your one and only secret password - needs to be at least 8 characters, no other restrictions.
- *site-name* uniquely identifies the site for which your are generating the password. Use the same pattern across all sites for consistency (e.g. the TLD - google.com, linkedin.com, etc.).
- *user-name* is your user name for the site.
- *password-length* (Optional) is how long you want the password to be. 16 by default. Must be 12 or more.

**Examples:**

	genpass lovecraft innsmouth.com akoirala
	genpass lovecraft reallysecure.com akoirala123 30

The generated password will not be printed, instead it will be copied directly to the clipboard.

#### Algorithm
- The *site-name* and *user-name* are concatenated and SHA256-hashed to get the salt.
- A key is generated through PBKDF2 using *master-password* as the password, the salt from the previous step, 1000 iterations, and with the specified password length as the key size.
- Each byte of the key is modulo'ed against lookup tables made up of alphanumeric characters and symbols to generate the final password.
