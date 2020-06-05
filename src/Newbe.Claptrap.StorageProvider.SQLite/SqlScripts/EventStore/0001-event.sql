-- $EventTableName$
CREATE TABLE IF NOT EXISTS '$EventTableName$' 
(
    claptrap_type_code text not null,
    claptrap_id        text not null,
    version            long not null,
    event_type_code    text not null,
    event_data         text,
    created_time       datetime not null,
    CONSTRAINT uid UNIQUE (claptrap_type_code, claptrap_id, version)
)  ;