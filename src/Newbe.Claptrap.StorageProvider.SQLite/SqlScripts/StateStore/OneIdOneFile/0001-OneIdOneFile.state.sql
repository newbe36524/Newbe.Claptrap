-- $StateTableName$
CREATE TABLE IF NOT EXISTS '$StateTableName$'
(
    claptrap_id        text     not null,
    claptrap_type_code text     not null,
    version            long     not null,
    state_data         text     not null,
    updated_time       datetime not null,
    CONSTRAINT uid UNIQUE (claptrap_type_code, claptrap_id, version)
);