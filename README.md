# PostgreSQL Exception Test

Repository reproducing an Eventuous/PostgreSQL issue wherein using PostgreSQL as an event store fails to work.

- Set up a PostgreSQL database, schema, and logon user using the information contained in Program.cs.
- Compile and run the project in Visual Studio 2022 or equivalent.
- The result is an InvalidCastException with the message, "Writing values of 'Eventuous.Sql.Base.NewPersistedEvent[]' is not supported for parameters having no NpgsqlDbType or DataTypeName. Try setting one of these values to the expected database type."
