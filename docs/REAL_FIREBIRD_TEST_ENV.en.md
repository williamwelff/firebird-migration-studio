# Automated real Firebird integration environment

The [integration workflow](../.github/workflows/firebird-integration.yml) runs disposable Docker services for official Firebird 3.0.14, 4.0.7 and 5.0.4 images on separate Linux GitHub runners, initializes a fake `integration.fdb` fixture, executes `FirebirdMigration.IntegrationTests`, and destroys the container afterward.

Coverage includes actual connection, simple/composite primary-key cursor pages, end-of-pagination, and rejection of unapproved sort keys.

The first **Firebird 5.0.4** integration run passed: [GitHub Actions evidence](https://github.com/williamwelff/firebird-migration-studio/actions/runs/39696196515). Other versions require their own passing matrix jobs before being considered verified.

Not covered: Windows/Fedora runtime behavior, Firebird 2.5 legacy authentication, large BLOBs, multi-page consistent snapshots, concurrent writes, restore/migration, or production suitability. The password is **only for a throwaway public CI fixture**, never for production use.

Run from GitHub > Actions > **Firebird real integration (disposable)** > Run workflow, or let changed core/test files trigger the workflow automatically.
