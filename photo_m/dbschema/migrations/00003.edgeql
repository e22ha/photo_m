CREATE MIGRATION m12fgvubmfuwympybc3mfg5cnqhtbd6zz2znhmbzsihmmszc3rgc7q
    ONTO m15c6a3lw4jqk4ekao2itv3gtpq2adxg5tatyxn7oa7wlkeojcbvba
{
  ALTER TYPE default::Human {
      ALTER PROPERTY nick {
          CREATE CONSTRAINT std::exclusive;
      };
  };
};
