CREATE MIGRATION m1gxx6hpwt32sbbdrhgfzjshnaw2wkqu7vbe2yfvkki6buaetqqzyq
    ONTO initial
{
  CREATE FUNCTION default::c_f_path(dir: std::str, n: std::str) ->  std::str USING (SELECT
      (dir ++ n)
  );
  CREATE TYPE default::Event {
      CREATE REQUIRED PROPERTY date -> std::datetime;
      CREATE REQUIRED PROPERTY title -> std::str;
      CREATE CONSTRAINT std::exclusive ON ((.title, .date));
  };
  CREATE TYPE default::Photo {
      CREATE REQUIRED PROPERTY directory -> std::str;
      CREATE REQUIRED PROPERTY name -> std::str;
      CREATE REQUIRED PROPERTY full_path := (default::c_f_path(.directory, .name));
      CREATE LINK event -> default::Event;
      CREATE CONSTRAINT std::exclusive ON ((.name, .directory));
      CREATE PROPERTY rating -> std::int64 {
          SET default := 0;
      };
  };
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
      CREATE PROPERTY nick -> std::str {
          CREATE CONSTRAINT std::exclusive;
      };
  };
  ALTER TYPE default::Camera {
      CREATE MULTI LINK photographers := (.<camera[IS default::Photographer]);
  };
  ALTER TYPE default::Photo {
      CREATE LINK camera -> default::Camera;
  };
  CREATE TYPE default::Person EXTENDING default::Human {
      CREATE CONSTRAINT std::exclusive ON ((.name, .surname));
  };
  ALTER TYPE default::Photo {
      CREATE MULTI LINK face -> default::Person;
      CREATE LINK author -> default::Photographer;
  };
  ALTER TYPE default::Photographer {
      CREATE MULTI LINK photos := (.<author[IS default::Photo]);
  };
};
