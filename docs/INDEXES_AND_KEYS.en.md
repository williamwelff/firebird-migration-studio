# Indexes and constraints — initial implementation

The read-only `StructuralReader` queries Firebird metadata tables `RDB$INDICES`, `RDB$INDEX_SEGMENTS`, `RDB$RELATION_CONSTRAINTS`, `RDB$REF_CONSTRAINTS` and `RDB$RELATIONS`. It collects column-based indexes and primary, unique and foreign keys, then compares names, ordered columns and selected attributes in memory.

**Limitations:** expression indexes without segments, CHECK/NOT NULL constraint metadata, FK ON UPDATE/DELETE actions, inactive indexes and semantic equivalence of auto-generated names are not fully handled. Equivalent schemas may have differently named objects. **Do not use the results to produce automatic changes.**

CI tests use simulated snapshots, including descending indexes and missing primary keys. Real Firebird server integration remains pending. No UI changes or database writes were introduced.
