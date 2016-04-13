CREATE TABLE tbl_tag_map (
  TagId    integer NOT NULL,
  QuoteId  integer NOT NULL,
  PRIMARY KEY (TagId, QuoteId),
  CONSTRAINT fk_tag
    FOREIGN KEY (TagId)
    REFERENCES tbl_tags(ID)
    ON DELETE CASCADE, 
  CONSTRAINT fk_quotes
    FOREIGN KEY (QuoteId)
    REFERENCES tbl_quotes(ID)
    ON DELETE CASCADE
);