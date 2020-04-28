-- $ActorTypeCode$ $ActorId$ $EventTableName$ $StateTableName$
create table '$StateTableName$'
(
    id            text     not null,
    version       long     not null,
    actortypecode text     not null,
    statedata     text     not null,
    updatedtime   datetime not null
);

create unique index '$StateTableName$_id_actorcode_uindex'
    on '$StateTableName$' ([id], [actortypecode]);