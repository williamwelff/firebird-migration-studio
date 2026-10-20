# Factory reset of a database — specification

**Status: planned; not implemented.**

## Expected behavior

**Restore factory defaults** creates a **new** Firebird 5.0 database from an approved **application-specific schema template**, retaining the schema and excluding operational/customer records.

Retain, according to the template: tables, columns, primary/foreign keys, indexes, views, triggers, stored procedures, packages, generators/sequences, domains, constraints and required permissions.

This is **not** an empty database without tables, and not an indiscriminate mass DELETE operation.

## Templates and seed data

The Firebird Migration Studio cannot infer each application's intended factory schema. An approved application-specific template is required.

Distinguish:
- Schema definitions to retain.
- Operational data to exclude.
- Essential seed data (configuration, reference tables, required accounts) to retain **only when explicitly designated**.

Validate generator initial values, references and application bootstrap requirements. If no approved template is available, disable the feature.

A permitted, verified template may be bundled with the installer; otherwise the user may import one. Never bundle customer databases, secrets or third-party proprietary artifacts without permission.

## Safety

- Never run on install, update or launch.
- Never change the master.
- Create a **new** database at a separate location by default, leaving the original intact.
- Require template selection, impact preview, verified recoverable backup and explicit confirmation.
- Replacing existing databases is disabled in the MVP.
- Verify resulting schema and required seed data; produce sanitized audit logs.

## UI

Proposed name: **Restore factory defaults** (PT-BR: **Restaurar configurações de fábrica**). Visual implementation requires prior approval.
