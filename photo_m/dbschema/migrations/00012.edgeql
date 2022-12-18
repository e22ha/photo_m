CREATE MIGRATION m1pk5zcdwyrfw6bj6huujkprnq5snnk4uz6bhtidboywblf6eyyi7a
    ONTO m1skl3wqxuwr3dbopxlciqlu4emkoaxcjleyyngagbgsvbsnducfxq
{
  ALTER TYPE default::Photo {
      ALTER PROPERTY full_path {
          USING (default::c_f_path(.directory, .name));
      };
  };
};
