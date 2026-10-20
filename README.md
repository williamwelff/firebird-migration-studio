# Firebird Migration Studio

Open-source, cross-platform desktop application for **migration, comparison and controlled synchronization** of Firebird databases.

> **Status: planning / early development.** No production-ready migration or synchronization functionality has been released.

## Goals

- Support Windows, Ubuntu and Fedora with one desktop codebase (C# / .NET 10 / Avalonia UI).
- Migrate databases from Firebird 2.5 to Firebird 5.0 using version-appropriate official Firebird backup/restore tools.
- Compare a **master/source** database with a **target** database, including schema and records.
- Generate a human-reviewable synchronization plan; apply approved changes **only to the target**.
- Let technicians perform operations from a graphical interface, without typing commands.

## Planned modules

| Module | Responsibilities |
| --- | --- |
| Desktop UI | Connection profiles, step-by-step wizards, progress, preview, approval and reports |
| Migration | Prerequisite checks; source backup; destination restore; log capture; post-restore validation |
| Schema compare | Compare tables, columns, types, constraints, indexes, views, triggers, procedures, generators/sequences |
| Data compare | Row matching by primary keys or explicitly configured stable unique keys; inserts, updates, optional deletes |
| Sync engine | Dependency ordering, dry-run, backup preconditions, conflict handling, bounded transactions, post-run verification |
| Reports | Differences, selected actions, execution audit and remaining issues |

## Safety principles

1. **Never modify the master.** It is the reference database; perform comparison using read-only access when feasible.
2. Never silently delete target rows. Deletions require explicit opt-in and an additional confirmation.
3. Default to **compare-only** until a synchronization plan has been reviewed.
4. Do not claim synchronization is safe without a tested backup and rollback/recovery plan.
5. Use parameterized queries for values; validate and quote Firebird identifiers correctly.
6. Match rows only with a primary key or explicitly confirmed stable unique identifier; otherwise report the table as requiring manual mapping.
7. Take into account foreign keys, triggers, generated values, transactions, BLOBs, charsets/collations, concurrency and schema/version differences.
8. Handle large tables in batches and show resumable/auditable progress; do not load entire databases into memory.
9. Use separate version-appropriate Firebird utilities for migrations; do not redistribute binaries without checking their licenses.
10. Test all behavior on disposable databases before any production use.

## Platform targets

- Windows 10/11 x64
- Ubuntu LTS x64
- Fedora x64

Planned distribution: Windows installer, Ubuntu `.deb`, Fedora `.rpm`. Packaging and platform compatibility remain to be implemented and validated.

## Proposed technology

- .NET 10, C#
- Avalonia UI, MVVM
- Firebird .NET data provider (exact compatible versions to be validated)
- Official `gbak` and `gstat` utilities for migration and verification
- Automated unit and integration tests with disposable Firebird instances

Read the [product requirements](docs/PRODUCT_REQUIREMENTS.md) for the initial roadmap and acceptance criteria.

## License

MIT (subject to addition of the license file). Third-party utilities retain their own licenses.
