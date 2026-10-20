# Record comparison — initial core

`RecordComparer` compares already-loaded in-memory record snapshots by an explicitly supplied simple or composite key. It classifies rows missing from the target, extra target rows and differing column values. Nulls are distinct from missing columns; byte-array BLOBs are compared by content. Duplicate or null keys cause an error.

**Safety:** this component neither connects to databases nor executes read/write SQL. Results are diagnostics only.

**Limitations:** no SQL row reader, streaming, pagination, validated primary-key discovery, cross-version type normalization or production data coverage. It must not be used for automatic synchronization or production migrations.

Real Firebird 2.5/3.0/4.0/5.0 integration verification remains a deferred separate milestone.
