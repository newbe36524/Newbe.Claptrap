-- $SchemaName$ $EventTableName$
CREATE SCHEMA IF NOT EXISTS $SchemaName$;

CREATE TABLE IF NOT EXISTS $SchemaName$.$EventTableName$
(
    claptrap_type_code LONGTEXT NOT NULL,
    claptrap_id        LONGTEXT NOT NULL,
    version            long     NOT NULL,
    event_type_code    LONGTEXT NOT NULL,
    event_data         LONGTEXT null,
    created_time       datetime NOT NULL,
    UNIQUE INDEX (claptrap_type_code(100), claptrap_id(50), version(16))
) ENGINE = 'InnoDB';