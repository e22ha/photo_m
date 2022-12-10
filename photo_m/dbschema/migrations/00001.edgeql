CREATE MIGRATION m17mw3bfip6dzrl7f4y7u24mkbprjefbl6gvvtz6arwqii7y4injta
    ONTO initial
{
  CREATE FUTURE nonrecursive_access_policies;
  CREATE TYPE default::Camera {
      CREATE REQUIRED PROPERTY brand -> std::str;
      CREATE REQUIRED PROPERTY model -> std::str;
      CREATE REQUIRED PROPERTY name := (((.brand ++ ' ') ++ .model));
  };
  CREATE ABSTRACT TYPE default::Human {
      CREATE REQUIRED PROPERTY name -> std::str;
      CREATE PROPERTY surname -> std::str;
      CREATE PROPERTY full_name := (((.name ++ ' ') ++ .surname));
  };
  CREATE TYPE default::Photographer EXTENDING default::Human {
      CREATE MULTI LINK camera -> default::Camera;
  };
  CREATE TYPE default::Event {
      CREATE PROPERTY date -> std::datetime;
      CREATE REQUIRED PROPERTY title -> std::str;
  };
  CREATE SCALAR TYPE default::Rate EXTENDING enum<None, Bad, NotBad, Normal, Super, Shdevr>;
  CREATE TYPE default::Photo {
      CREATE LINK event -> default::Event;
      CREATE REQUIRED LINK author -> default::Photographer;
      CREATE REQUIRED PROPERTY directory -> std::str;
      CREATE REQUIRED PROPERTY name -> std::str;
      CREATE REQUIRED PROPERTY full_path := (((.directory ++ ' ') ++ .name));
      CREATE PROPERTY rating -> default::Rate {
          SET default := (default::Rate.None);
      };
  };
  CREATE TYPE default::Person EXTENDING default::Human;
  ALTER TYPE default::Photo {
      CREATE MULTI LINK face -> default::Person;
  };
};
