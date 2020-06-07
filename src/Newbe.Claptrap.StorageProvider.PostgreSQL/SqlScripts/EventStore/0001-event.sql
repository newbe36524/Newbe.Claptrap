-- $SchemaName$ $EventTableName$
CREATE SCHEMA IF NOT EXISTS $SchemaName$;

CREATE TABLE IF NOT EXISTS $SchemaName$.$EventTableName$
(
    claptrap_type_code TEXT NOT NULL,
    claptrap_id        TEXT NOT NULL,
    version            INT8     NOT NULL,
    event_type_code    TEXT NOT NULL,
    event_data         TEXT null,
    created_time       date NOT NULL,
    PRIMARY KEY (claptrap_type_code, claptrap_id, version)
) ;
