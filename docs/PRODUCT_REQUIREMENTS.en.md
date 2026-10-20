# Product requirements — Firebird Migration Studio

## Purpose
Cross-platform desktop app to migrate Firebird 2.5, 3.0 or 4.0 to 5.0; compare databases; and optionally synchronize a target with a master/reference. These are separate workflows.

## Operating systems
Windows 10/11 x64, Ubuntu LTS x64 and Fedora x64. C# / .NET 10 / Avalonia UI. GUI smoke tests and platform packaging must be validated separately.

## Migration
- Detect official supported source versions (2.5, 3.0, 4.0), with 5.0 as target. Reject unsupported sources.
- Confirm source and target are different physical databases, not merely different aliases.
- Use the version-specific source `gbak` for backup and Firebird 5.0 `gbak` for restore to a new database.
- Surface progress and sanitized logs; verify ODS, metadata, counts, privileges, drivers and external library compatibility.
- Handle users/security database separately.

## Comparison
- Compare user-visible tables, columns, constraints, indexes, views, triggers, stored procedures and generators/sequences, respecting version-specific differences.
- Compare data via stable primary keys or manually approved unique keys. Avoid guessed row matching.
- Include NULL, BLOBs, time precision, collation/charset, encoding and large-table considerations.
- Comparison must perform **zero writes** to either database.
- Address concurrent writes using appropriate transaction snapshots or maintenance windows.

## Synchronization
- Master is always read-only; all approved changes target a separate database.
- Produce a reviewable dependency-aware change plan with dry run and detailed audit.
- Require backup/recovery plan, explicit authorization and optimistic conflict checks before applying changes.
- Disable deletes by default; require separate confirmation to allow them.
- Respect transaction boundaries and limitations for DDL; revalidate after applying.

## Operator workflow
1. Select Migrate, Compare or Synchronize.
2. Configure/test connection profiles.
3. Choose scope, mappings and rules.
4. Review differences.
5. Explicitly approve target-only changes (if any).
6. Execute, verify and export report.

## Acceptance criteria
- Real migration tests with disposable Firebird 2.5, 3.0 and 4.0 fixtures to Firebird 5.0, including triggers, procedures, BLOBs and generators.
- Compare-only mode never writes.
- Insert/update/delete differences accurately detected on stable keys.
- Synchronization never changes master and requires explicit consent for deletions.
- Failures provide diagnostic logs and recovery guidance.
- Bounded memory behavior for large tables.
- Test Windows, Ubuntu and Fedora GUI runs.
- User-facing UI in both **pt-BR** and **en**, Portuguese by default.

## Planned phases
1. Cross-platform shell, localization, profiles and logs.
2. Real connection checks and migration workflow.
3. Read-only schema/data diff.
4. Controlled sync review and apply.
5. Installers, integration tests and technician manuals.

Visual design changes require user approval before implementation. All capabilities above are planned unless specifically implemented and validated.
