-- $ActorTypeCode$ $ActorId$
create table '$EventTableName$'
(
    Version long
        constraint '$EventTableName$_pk'
            primary key,
    Uid text,
    EventTypeCode text,
    EventData text,
    CreatedTime long not null
);

create unique index '$EventTableName$_Uid_uindex'
    on '$EventTableName$' (Uid);