-- $EventTableName$
CREATE TABLE IF NOT EXISTS '$EventTableName$'
(
    version         long
        primary key,
    event_type_code text     not null,
    event_data      text,
    created_time    datetime not null
);