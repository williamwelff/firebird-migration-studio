# Safe database recreation (planned feature)

## Goal

Allow operators on Windows, Ubuntu and Fedora to create **a fresh database** without database administration commands. This feature is **not implemented or enabled**; this document defines its safety requirements.

## Modes

1. **Empty database:** create a new Firebird 5.0 database without user tables or records.
2. **Schema-only database:** create a **new** database using a chosen application's schema but without business records. This requires an **application-specific schema/template**; there is no universal Firebird data model for all applications.

A future installer may include a clearly labeled demonstration sample containing no real customer data, subject to its license. Do not bundle customer databases or application-specific templates without permission.

## Required protections

- Never perform resets during installation, update or startup.
- Never modify the master reference database.
- Default to creating a **new database at a different path**, leaving the original unchanged.
- Replacing an existing target requires a tested backup, verification of physical target identity and extra approval; replacement remains disabled in the MVP.
- Do not store real customer `.fdb` files in this repository or installers.
- Clarify that server users/passwords are separate from database contents.
- Test with disposable databases before release.
- Create sanitized audit logs and show clear completion reports.

## Proposed UI — requires visual approval

Settings > Databases > **Create clean database**, with template/mode selection and a new destination path. It is a new-database wizard, **not a universal delete-all-data button**.

The visual change must be reviewed before merging into the UI.
