# Referential-action comparison

The structural reader now collects foreign-key `RDB$UPDATE_RULE` and `RDB$DELETE_RULE` values from the `RDB$REF_CONSTRAINTS` catalog. The comparer recognizes changes to actions such as `CASCADE`, `SET NULL`, and `NO ACTION` by comparing stored catalog values.

**Status:** simulated tests added, with no write operations. Integration verification against real Firebird servers remains pending, as do CHECK constraints, expression indexes and semantic matching of auto-generated names. These differences must not drive automatic synchronization yet.
