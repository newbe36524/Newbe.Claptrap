-- $ActorTypeCode$ $ActorId$ $EventTableName$ $StateTableName$
create table '$EventTableName$'
(
    version       long
        constraint '$EventTableName$_pk'
            primary key,
    uid           text,
    eventtypecode text,
    eventdata     text,
    createdtime   datetime not null
);

create unique index '$EventTableName$_Uid_uindex'
    on '$EventTableName$' ([uid], [eventtypecode]);