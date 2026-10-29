# Initial certification: Firebird 2.5.9 → 5.0.4

**Result:** passed on disposable live Linux servers in [GitHub Actions run 40884148600](https://github.com/williamwelff/firebird-migration-studio/actions/runs/40884148600).

The test boots Firebird 2.5.9 and 5.0.4, creates fictional CLIENTES and PEDIDOS tables with primary and foreign keys, inserts sample rows, performs a source-version `gbak` backup, restores it with Firebird 5 `gbak`, and fails if the restored row counts, selected values, sum, or foreign-key metadata are wrong.

**Scope:** a narrow synthetic end-to-end migration case only. This does **not** validate customer databases, stored procedures, triggers, views, larger BLOBs, varied charsets, production safety, or graphical orchestration. Commands run only in CI, not in the application UI. The legacy 2.5 image must remain isolated.

Workflow: [firebird-25-to-50-migration.yml](../.github/workflows/firebird-25-to-50-migration.yml).
