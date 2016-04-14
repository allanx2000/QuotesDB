CREATE TABLE tbl_quotes (
  ID        integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  AuthorId  integer NOT NULL,
  "Text"    varchar(200) NOT NULL,
  "Count"   integer NOT NULL,
  CONSTRAINT fk_author
    FOREIGN KEY (AuthorId)
    REFERENCES tbl_authors(ID)
    ON DELETE CASCADE
);