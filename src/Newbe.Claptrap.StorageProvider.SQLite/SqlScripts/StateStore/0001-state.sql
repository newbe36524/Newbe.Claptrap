-- $StateTableName$
CREATE TABLE IF NOT EXISTS '$StateTableName$'
(
    claptrap_type_code text     not null,
    claptrap_id        text     not null,
    version            long     not null,
    state_data         text     not null,
    updated_time       datetime not null,
    CONSTRAINT $StateTableName$uid UNIQUE (claptrap_type_code, claptrap_id)
);
