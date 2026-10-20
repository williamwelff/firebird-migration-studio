# Paged record reading — experimental

`RecordPageReader` issues **SELECT-only** Firebird queries with `FIRST` and `SKIP`, ordered by caller-provided key columns. Each request returns up to 1,000 rows, rejects SQL identifiers outside a strict allowlist, and uses a parameter for row offset.

**Caveats:** high offsets can be slow and separate transactions may observe inconsistent snapshots under concurrent writes. The provided key is **not yet validated against metadata**; quoted/mixed-case identifiers are not supported; `SELECT *` and large BLOBs may consume significant memory. It is not integrated into the desktop UI, and it is not yet a production-ready incremental comparison engine.

Automated tests cover SQL generation and rejection of unsafe identifiers. Real Firebird 2.5, 3.0, 4.0 and 5.0 servers are required to verify SQL syntax and behavior. Database writes remain disabled.
