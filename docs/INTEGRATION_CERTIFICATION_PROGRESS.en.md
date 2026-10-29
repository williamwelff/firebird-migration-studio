# Live integration certification progress

Previously passed on disposable Linux containers: Firebird 3.0.14, 4.0.7 and 5.0.4 — connection, simple/composite primary-key cursor pages, termination and rejection of invalid keys. Evidence: https://github.com/williamwelff/firebird-migration-studio/actions/runs/39696326398

New coverage submitted: VARCHAR UTF8, BLOB SUB_TYPE TEXT, NULL BLOB and cursor termination in the DOCUMENTOS fixture. The current workflow must pass before these cases are marked validated. This does **not** validate huge BLOBs, binary value equality, non-ASCII text or bounded memory use.

Still outstanding: safely sourced isolated Firebird 2.5 runtime with legacy authentication, actual Windows/Fedora runtime tests, consistent snapshots across multiple pages, concurrent edits, memory and connectivity failures. Never use customer databases or credentials. The application remains unapproved for production or synchronization.
