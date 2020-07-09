-- $SchemaName$ $StateTableName$
CREATE SCHEMA IF NOT EXISTS $SchemaName$;

CREATE TABLE IF NOT EXISTS $SchemaName$.$StateTableName$
(
    claptrap_type_code LONGTEXT NOT NULL,
    claptrap_id        LONGTEXT NOT NULL,
    version            BIGINT   NOT NULL,
    state_data         LONGTEXT null,
    updated_time       datetime NOT NULL,
    UNIQUE INDEX (claptrap_type_code(100), claptrap_id(50))
) ENGINE = 'InnoDB';