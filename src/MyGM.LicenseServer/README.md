# MyGM License Server

Set `MYGM_LICENSE_ADMIN_KEY` and `MYGM_LICENSE_PRIVATE_KEY_PEM`, run behind HTTPS, then set the matching public key and endpoint in `config/license.json` of the Companion build.

Admin calls require `X-Admin-Key`. A license binds to the first device that logs in. Passwords are stored only through ASP.NET Core `PasswordHasher`.
