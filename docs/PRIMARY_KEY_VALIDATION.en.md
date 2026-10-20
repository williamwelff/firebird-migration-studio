# Declared primary-key validation

The experimental paged record reader now queries `RDB$RELATION_CONSTRAINTS` and `RDB$INDEX_SEGMENTS` within the **same snapshot transaction** to retrieve the declared primary key in column order. Reads are refused when the table has no primary key or when the requested keys do not match it, including ordering of composite keys.

This guards against arbitrary ordering being mistaken for a stable key, but does not solve high SKIP costs, large BLOBs, cross-version live testing or consistent snapshots across separate page requests. **Not production validated.** No write SQL is enabled.
