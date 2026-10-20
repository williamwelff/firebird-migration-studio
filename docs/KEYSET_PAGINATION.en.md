# Keyset pagination — next phase

`KeysetQueryBuilder` produces **SELECT-only** lexicographic cursor queries for simple and composite keys, with parameters `@k0`, `@k1`, etc., without `SKIP`. Unsafe identifiers are rejected.

**Actual status:** the builder and simulated tests were committed; the live `RecordPageReader` **still uses SKIP** pending cursor binding and disposable-database integration tests. Cursor value types, large BLOB handling, consistent snapshots across pages, collations and concurrent edits are not solved. Not ready for production or synchronization.
