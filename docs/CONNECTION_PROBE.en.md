# Real connection probe — first implementation

## Status
The desktop app uses the official `FirebirdSql.Data.FirebirdClient` provider, version 10.3.4, to **attempt real connections** from the Connections screen for either master or target. No data-changing SQL is executed.

The result reports the server version offered by the provider or a generic failure. Required fields and invalid ports are checked first.

## Security
- Passwords are masked and not persisted.
- Connection strings and provider exception details are not logged or shown.
- Pooling is disabled and a short connection timeout is configured.
- A successful connection **does not establish** physical database identity or enforce server-side read-only permissions.
- **Migration, data comparison and synchronization are not implemented.**

## Pending verification
GitHub Actions compilation and unit tests **do not prove** real connectivity across Firebird versions. Disposable integration environments for Firebird **2.5, 3.0, 4.0 and 5.0**, on Windows, Ubuntu and Fedora, are still required, including wrong credentials, outages and legacy authentication cases.
