# Schema comparison: initial core implementation

**Status:** source code committed, not yet validated against real Firebird servers.

`SchemaReader` reads user table columns from `RDB$RELATIONS`, `RDB$RELATION_FIELDS` and `RDB$FIELDS` using a snapshot transaction, without DDL/DML. `SchemaComparer` computes missing, extra, or differing columns in memory.

This is **not a complete DDL comparison**. Internal Firebird type codes must still be normalized to SQL types; subtypes, precision, charset, collation, defaults, constraints, indexes, procedures, triggers and views are not covered. Do not use these differences to generate modifications.

The desktop UI does not trigger schema reads yet. Integration verification is deferred for disposable Firebird 2.5, 3.0, 4.0 and 5.0 servers. CI compilation is not evidence of successful real-server access.

**Safety:** no write statements and no synchronization scripts in this phase.

## Extended metadata

The reader now includes character length, subtype, numeric precision, charset ID and effective collation ID alongside earlier fields. This catches previously unseen metadata differences such as equal byte length but unequal character length. The code still does **not** render canonical SQL DDL or establish cross-database semantic charset equivalence. Real-server compatibility remains unverified.
