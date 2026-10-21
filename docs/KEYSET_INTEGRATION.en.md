# Keyset reader integration

`RecordPageReader.ReadKeysetPageAsync` now opens a real Firebird ADO.NET connection, verifies the catalog-declared primary key, and executes a `SELECT FIRST ...` query from `KeysetQueryBuilder`. Continuation values are passed as `@k0`, `@k1`, etc. No `SKIP` or write SQL is executed by this path.

**Status:** integrated source code, **not verified on live Firebird instances**. Cursor values are caller supplied. Cross-page snapshot consistency, cursor parameter typing, collation semantics, concurrency, compound keys and large BLOB memory usage require integration testing. The legacy SKIP reader remains available for experimental purposes.

## Required certification

Run reproducible disposable-server tests across **Firebird 2.5, 3.0, 4.0 and 5.0**, covering authentication, metadata, empty tables, simple/composite keys, continuation, pagination boundaries, large values, charsets and concurrent writes on Windows, Ubuntu and Fedora. Record evidence per version/platform. No live-server version is certified by this change.
