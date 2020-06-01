-- $EventTableName$
CREATE TABLE IF NOT EXISTS '$EventTableName$' 
(
    claptrap_type_code text,
    claptrap_id        text,
    version            long,
    event_type_code    text,
    event_data         text,
    created_time       datetime not null,
    CONSTRAINT uid UNIQUE (claptrap_type_code, claptrap_id, version)
)  ;