CREATE MIGRATION m1hsbwk3427oo4qfo32jknlwmisllchpe4vstsg5vw3egexhqcnqqq
    ONTO m1pk5zcdwyrfw6bj6huujkprnq5snnk4uz6bhtidboywblf6eyyi7a
{
  ALTER TYPE default::Event {
      ALTER PROPERTY date {
          SET REQUIRED USING (<std::datetime>'1999-03-31T15:17:00Z');
      };
      CREATE CONSTRAINT std::exclusive ON ((.title, .date));
  };
};
