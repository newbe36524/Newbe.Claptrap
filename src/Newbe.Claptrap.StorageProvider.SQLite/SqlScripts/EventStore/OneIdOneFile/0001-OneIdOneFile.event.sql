-- $EventTableName$
create table '$EventTableName$'
(
    version       long
            primary key,
    eventtypecode text,
    eventdata     text,
    createdtime   datetime not null
);