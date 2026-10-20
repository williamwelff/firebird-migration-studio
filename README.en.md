# Firebird Migration Studio

[Português (Brasil)](README.md) | **English**

Open-source cross-platform desktop application for **migration, comparison and controlled synchronization** of Firebird databases.

> **Status: early development preview.** It is not ready for real production migrations or synchronization.

## Goals

- Target Windows 10/11, Ubuntu and Fedora with C#, .NET 10 and Avalonia UI.
- Plan migrations from Firebird **2.5, 3.0 and 4.0 to Firebird 5.0** using version-appropriate official tools; 3.5 and 4.5 are not official Firebird release series.
- Compare master (reference) and target database schemas and records.
- Preview differences and a correction plan; only modify the target after explicit approval.
- Provide a graphical user interface for technicians without manual terminal commands.

## Planned modules

| Module | Responsibilities |
| --- | --- |
| Desktop UI | Connection profiles, guided workflows, progress and reports |
| Migration | Source backup, target restore, version detection and verification |
| Schema comparison | Tables, columns, indexes, constraints, views, procedures, triggers and generators |
| Data comparison | Stable-key row matching; inserts, updates, optionally deletes |
| Synchronization | Preview, risk assessment, approval, audit trail and verification |
| Reports | Differences, results, logs and unresolved issues |

## Safety

- Never modify the master during synchronization.
- Read-only comparison by default; target writes require explicit authorization.
- Deletions are disabled by default.
- Require backups, verified recovery paths and appropriate transaction boundaries.
- Tables lacking a stable primary/unique key require explicit mapping.
- Account for dependencies, triggers, character sets, BLOBs, concurrency and large data volumes.
- No production use until integration testing on disposable databases.

## Supported platforms (planned)

Windows 10/11 x64, Ubuntu LTS x64, Fedora x64. Installers and packages are not available yet.

## Stack

C# / .NET 10 / Avalonia UI, Firebird .NET provider (compatibility pending), official `gbak` and `gstat` utilities.

## Documentation

- [Product requirements (English)](docs/PRODUCT_REQUIREMENTS.en.md)
- [Product requirements (Portuguese)](docs/PRODUCT_REQUIREMENTS.md)
- [Run the preview (Portuguese)](docs/RUNNING_PREVIEW.md)

## User interface languages

Português (Brasil) and English. Portuguese is the default. Localization is being introduced as part of the early preview.

## Design process

Visual changes require approval before implementation.

## License

MIT; bundled third-party tools retain their respective licenses.
