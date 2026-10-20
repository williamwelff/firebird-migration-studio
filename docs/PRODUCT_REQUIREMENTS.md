# Product requirements — Firebird Migration Studio

## 1. Product scope

A cross-platform graphical desktop application for technicians to migrate Firebird 2.5 databases to Firebird 5.0, compare independent Firebird databases, and optionally synchronize a target to a designated master. **Comparison and migration are separate workflows.**

## 2. Supported operating systems

Initial release targets:
- Windows 10/11 x64
- Ubuntu LTS x64
- Fedora x64

Implementation: C# and .NET 10 with Avalonia UI, avoiding Windows-only APIs. Build and test each platform independently, including GUI rendering, process launching, library discovery, path handling, authentication and installation. Proposed artifacts: Windows installer, Ubuntu .deb and Fedora .rpm. No executables or packages are available yet.

## 3. Migration mode

1. Select an existing Firebird 2.5 source and a **different** destination database path/instance on Firebird 5.0.
2. Detect source/destination versions and local utility paths; perform permission, connection, backup space and risk checks.
3. Use Firebird 2.5's gbak for a logical backup and Firebird 5.0's gbak to restore into a *new* destination, without asking the technician to type commands.
4. Capture progress, exit status, stdout/stderr, and actionable diagnostics. Protect credentials from command-line process listings/logs where possible.
5. Verify opening the destination, ODS, important object metadata, record counts, and application compatibility checks.
6. Report that users/passwords and external libraries/drivers may require separate migration.

## 4. Comparison mode

Define **Master (reference)** and **Target (candidate)** connections. Compare schema and data separately. Allow comparison without any writes.

### Schema comparison

Compare user tables, fields, types, nullability, defaults, computed fields, keys, constraints, indexes, views, stored procedures, triggers, sequences/generators and dependencies as supported per server version.

- Normalize irrelevant formatting but preserve semantic differences.
- Account for valid version-specific metadata differences: do not blindly synchronize system metadata.
- Group differences by object and risk; display source/target definitions and generated actions.

### Data comparison

- Table matching by name or explicit mapping; row matching by primary key or a user-approved stable unique key.
- For tables without reliable keys: report differences or require explicit mapping rather than guessing row identity.
- Compare inserts, updates and optionally deletes; allow per-table/per-column exclusions and normalization rules.
- Support BLOBs, NULLs, floating point precision rules, dates, timestamps, time zones (where applicable), and character encodings carefully.
- Use streaming/batched reads and stable sorting/keyset pagination; support large databases and cancellation.
- Avoid false results during concurrent writes: require controlled snapshots or a documented maintenance window.

## 5. Target synchronization

- **Master never changes.** Target is writable only after an explicit switch from preview to apply.
- Generate a deterministic change plan with object dependencies and SQL/data parameterization.
- Provide dry-run, affected row counts, risk warnings and downloadable audit report.
- Apply changes in appropriately bounded transactions with error reporting, and recognize that DDL/data operations may require different transaction boundaries.
- Deleting rows is disabled by default and requires separate approval.
- Re-check target state before application to detect concurrent modifications. Do not assume long-running diff results are still current.
- Require a tested backup/recovery path before applying changes and allow post-apply verification.
- Do not suppress trigger side effects or alter generators automatically without a reviewed policy.

## 6. User workflow

1. Select **Migrate**, **Compare**, or **Synchronize**.
2. Configure master/source and target profiles; test connections.
3. Choose scope and comparison settings.
4. Run analysis with progress; inspect differences and risks.
5. If applicable, approve a proposed target-only plan and prerequisites.
6. Execute, validate and export result/logs.

## 7. Acceptance tests

- Firebird 2.5 -> 5.0 migration succeeds on a disposable sample with tables, FK relationships, triggers, procedures, generators and BLOBs.
- Compare-only mode produces **zero writes** to either database.
- Insert/update/delete differences are correctly classified using stable keys.
- Sync changes only the designated target and respects explicit deletion opt-in.
- Failed operations report exact steps, preserve master and provide a recovery path.
- Large tables do not require full in-memory materialization.
- Windows, Ubuntu and Fedora builds run through smoke tests on supported desktop sessions.
- Automated test fixtures include charset differences, NULLs, BLOBs, conflicts, schema drift and concurrent edits.

## 8. Development phases

1. Repo/tooling, cross-platform Avalonia shell, connection wizard, structured logs.
2. Firebird version detection, gbak migration wizard, disposable integration tests.
3. Read-only metadata/schema diff and data diff with key mapping.
4. Target synchronization preview, safe apply workflow, audit and rollback procedures.
5. Platform packages, automated release tests and technician-friendly documentation.

Nothing in this document represents already implemented or validated functionality.
