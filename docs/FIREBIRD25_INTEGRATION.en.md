# Initial real Firebird 2.5.9 integration

The [legacy integration workflow](../.github/workflows/firebird-legacy-integration.yml) passed against an isolated disposable Firebird 2.5.9 server: https://github.com/williamwelff/firebird-migration-studio/actions/runs/40877281511.

It uses the community-maintained historical image `jacobalberty/firebird:v2.5.9-ss`, whose source repository was archived in 2025. Use it **only for isolated CI tests**, never for production or customer data. The test creates synthetic fixtures and verifies real connection, simple and composite primary keys, pagination, data reads and rejection of invalid keys.

**Not verified:** actual 2.5→5 migration, backups/restores, large data, concurrent transactions, desktop behavior on Windows/Fedora, large BLOBs or production security.
