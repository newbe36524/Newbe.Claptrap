-- $StateTableName$
create table '$StateTableName$'
(
    claptrapid       text     not null,
    claptraptypecode text     not null,
    version          long     not null,
    statedata        text     not null,
    updatedtime      datetime not null
);

create unique index '$StateTableName$_id_typecode_uindex'
    on '$StateTableName$' ([claptrapid], [claptraptypecode]);