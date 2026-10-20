# Record synchronization preview

`RecordSyncPlanner` converts computed differences into a **non-executable preview**. It counts proposed inserts and updates, and flags extra target rows for manual review. Extra rows never generate `DELETE` statements.

This is diagnostic metadata only: **no SQL is generated or executed and no database connection is used**. `CanExecute` is always `false`.

Tests use synthetic in-memory differences. Verified key discovery, incremental table reads, conflict handling, backups, approvals and live Firebird integration tests are still missing. **Not suitable for production.**
