-- $SchemaName$ $StateTableName$
CREATE SCHEMA IF NOT EXISTS $SchemaName$;

CREATE TABLE IF NOT EXISTS $SchemaName$.$StateTableName$
(
    claptrap_type_code TEXT NOT NULL,
    claptrap_id        TEXT NOT NULL,
    version            INT8     NOT NULL,
    state_data         TEXT null,
    updated_time       date NOT NULL,
    PRIMARY KEY (claptrap_type_code, claptrap_id, version)
) ;
