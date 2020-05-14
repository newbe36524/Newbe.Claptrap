-- $ActorTypeCode$ $ActorId$ $EventTableName$ $StateTableName$
create table '$EventTableName$'
(
    version       long
        constraint '$EventTableName$_pk'
            primary key,
    eventtypecode text,
    eventdata     text,
    createdtime   datetime not null
);