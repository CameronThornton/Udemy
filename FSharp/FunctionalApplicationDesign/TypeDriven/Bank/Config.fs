module Config

let akka =
    """akka.persistence{
        journal {
                plugin = "akka.persistence.journal.sql-server"
            sql-server {
                # qualified type name of the SQL Server persistence journal actor
                class = "Akka.Persistence.SqlServer.Journal.SqlServerJournal, Akka.Persistence.SqlServer"

                # dispatcher used to drive journal actor
                plugin-dispatcher = "akka.actor.default-dispatcher"

                # connection string used for database access
                connection-string = "Data Source={}\\SQLEXPRESS;initial catalog=Bank;Integrated Security=True;Connect Timeout=30;"

                # default SQL commands timeout
                connection-timeout = 30s

                # SQL server schema name to table corresponding with persistent journal
                schema-name = dbo

                # SQL server table corresponding with persistent journal
                table-name = EventJournal

                # should corresponding journal table be initialized automatically
                auto-initialize = on

                # timestamp provider used for generation of journal entries timestamps
                timestamp-provider = "Akka.Persistence.Sql.Common.Journal.DefaultTimestampProvider, Akka.Persistence.Sql.Common"

                # metadata table
                metadata-table-name = Metadata
                
                # Recommended: change default circuit breaker settings
                # By uncommenting below and using Connection Timeout + Command Timeout
                circuit-breaker.call-timeout=65s
            }
        }

        snapshot-store {
                plugin = "akka.persistence.snapshot-store.sql-server"
            sql-server {

                # qualified type name of the SQL Server persistence journal actor
                class = "Akka.Persistence.SqlServer.Snapshot.SqlServerSnapshotStore, Akka.Persistence.SqlServer"

                # dispatcher used to drive journal actor
                plugin-dispatcher = ""akka.actor.default-dispatcher""

                # connection string used for database access
                connection-string = "Data Source={}\\SQLEXPRESS;initial catalog=Bank;Integrated Security=True;Connect Timeout=30;"

                # default SQL commands timeout
                connection-timeout = 30s

                # SQL server schema name to table corresponding with persistent journal
                schema-name = dbo

                # SQL server table corresponding with persistent journal
                table-name = SnapshotStore

                # should corresponding journal table be initialized automatically
                auto-initialize = on
                
                # Recommended: change default circuit breaker settings
                # By uncommenting below and using Connection Timeout + Command Timeout
                circuit-breaker.call-timeout=65s
            }
        }
    }"""
